using System;
using System.Security.Cryptography;
using System.Text;

/**
 * A class that provides easy Blowfish encryption.<p>
 * this class is copy from java blowfish implements
 * @author Markus Hahn <markus_hahn@gmx.net>
 * @author Gaston Dombiak
 */
public class Blowfish
{

    private BlowfishCBC m_BlowfishCBC;
    private static Random m_Random = new Random();
    private static RNGCryptoServiceProvider m_RandomProvider = new RNGCryptoServiceProvider();

    public Blowfish() { }

    /**
     * Creates a new Blowfish object using the specified key (oversized
     * password will be cut).
     *
     * @param password the password (treated as a real unicode array)
     */
    public Blowfish(String password)
    {
        // hash down the password to a 160bit key
        SHA1 sha = new SHA1CryptoServiceProvider();
        // setup the encryptor (use a dummy IV)
        m_BlowfishCBC = new BlowfishCBC(sha.ComputeHash(Encoding.ASCII.GetBytes(password)), 0);
    }

    /**
     * Encrypts a string (treated in UNICODE) using the
     * csharp RNGCryptoServiceProvider random generator, which isn't that
     * great for creating IVs
     *
     * @param sPlainText string to encrypt
     * @return encrypted string in binhex format
     */
    public String Encrypt_CBC(String sPlainText)
    {
        long lCBCIV;
        lock (m_Random)
        {
            byte[] buff = new byte[8];
            m_RandomProvider.GetBytes(buff);
            lCBCIV = ByteArrayToLong(buff, 0);
        }

        return Encrypt_CBC(sPlainText, lCBCIV);
    }

    private String Encrypt_CBC(String sPlainText, long lNewCBCIV)
    {
        // allocate the buffer (align to the next 8 byte border plus padding)
        int nStrLen = sPlainText.Length;
        byte[] buf = new byte[((nStrLen << 1) & 0xfffffff8) + 8];

        // copy all bytes of the string into the buffer (use network byte order)
        int nI;
        int nPos = 0;
        for (nI = 0; nI < nStrLen; nI++)
        {
            char cActChar = sPlainText[nI];
            buf[nPos++] = (byte)((cActChar >> 8) & 0x0ff);
            buf[nPos++] = (byte)(cActChar & 0x0ff);
        }

        // pad the rest with the PKCS5 scheme
        byte bPadVal = (byte)(buf.Length - (nStrLen << 1));
        while (nPos < buf.Length)
        {
            buf[nPos++] = bPadVal;
        }

        lock (m_BlowfishCBC)
        {
            // create the encryptor
            m_BlowfishCBC.SetCBCIV(lNewCBCIV);

            // encrypt the buffer
            m_BlowfishCBC.Encrypt(buf);
        }

        // return the binhex string
        byte[] newCBCIV = new byte[BlowfishCBC.BLOCKSIZE];
        LongToByteArray(lNewCBCIV, newCBCIV, 0);

        return BytesToBinHex(newCBCIV, 0, BlowfishCBC.BLOCKSIZE) + BytesToBinHex(buf, 0, buf.Length);
    }

    /**
     * decrypts a hexbin string (handling is case sensitive)
     * @param sCipherText hexbin string to decrypt
     * @return decrypted string (null equals an error)
     */
    public String Decrypt_CBC(String sCipherText)
    {
        // get the number of estimated bytes in the string (cut off broken blocks)
        int nLen = (sCipherText.Length >> 1) & ~7;

        // does the given stuff make sense (at least the CBC IV)?
        if (nLen < BlowfishECB.BLOCKSIZE)
            return null;

        // get the CBC IV
        byte[] cbciv = new byte[BlowfishCBC.BLOCKSIZE];
        int nNumOfBytes = BinHexToBytes(sCipherText, cbciv, 0, 0, BlowfishCBC.BLOCKSIZE);
        if (nNumOfBytes < BlowfishCBC.BLOCKSIZE)
            return null;

        // something left to decrypt?
        nLen -= BlowfishCBC.BLOCKSIZE;
        if (nLen == 0)
        {
            return "";
        }

        // get all data bytes now
        byte[] buf = new byte[nLen];

        nNumOfBytes = BinHexToBytes(sCipherText, buf, BlowfishCBC.BLOCKSIZE * 2, 0, nLen);

        // we cannot accept broken binhex sequences due to padding
        // and decryption
        if (nNumOfBytes < nLen)
        {
            return null;
        }

        lock (m_BlowfishCBC)
        {
            // (got it)
            m_BlowfishCBC.SetCBCIV(cbciv);

            // decrypt the buffer
            m_BlowfishCBC.Decrypt(buf);
        }

        // get the last padding byte
        int nPadByte = (int)buf[buf.Length - 1] & 0x0ff;

        // ( try to get all information if the padding doesn't seem to be correct)
        if ((nPadByte > 8) || (nPadByte < 0))
        {
            nPadByte = 0;
        }

        // calculate the real size of this message
        nNumOfBytes -= nPadByte;
        if (nNumOfBytes < 0)
        {
            return "";
        }

        return ByteArrayToUNCString(buf, 0, nNumOfBytes);
    }

    /**
     * destroys (clears) the encryption engine,
     * after that the instance is not valid anymore
     */
    public void Destroy()
    {
        m_BlowfishCBC.CleanUp();
    }

    /**
     * gets bytes from an array into a long
     * @param buffer where to get the bytes
     * @param nStartIndex index from where to read the data
     * @return the 64bit integer
     */
    private static long ByteArrayToLong(byte[] buffer, int nStartIndex)
    {
        return (((long)buffer[nStartIndex]) << 56) |
                (((long)buffer[nStartIndex + 1] & 0x0ffL) << 48) |
                (((long)buffer[nStartIndex + 2] & 0x0ffL) << 40) |
                (((long)buffer[nStartIndex + 3] & 0x0ffL) << 32) |
                (((long)buffer[nStartIndex + 4] & 0x0ffL) << 24) |
                (((long)buffer[nStartIndex + 5] & 0x0ffL) << 16) |
                (((long)buffer[nStartIndex + 6] & 0x0ffL) << 8) |
                ((long)buffer[nStartIndex + 7] & 0x0ff);
    }

    /**
     * converts a long o bytes which are put into a given array
     * @param lValue the 64bit integer to convert
     * @param buffer the target buffer
     * @param nStartIndex where to place the bytes in the buffer
     */
    private static void LongToByteArray(long lValue, byte[] buffer, int nStartIndex)
    {
        buffer[nStartIndex] = (byte)(lValue >> 56);
        buffer[nStartIndex + 1] = (byte)((lValue >> 48) & 0x0ff);
        buffer[nStartIndex + 2] = (byte)((lValue >> 40) & 0x0ff);
        buffer[nStartIndex + 3] = (byte)((lValue >> 32) & 0x0ff);
        buffer[nStartIndex + 4] = (byte)((lValue >> 24) & 0x0ff);
        buffer[nStartIndex + 5] = (byte)((lValue >> 16) & 0x0ff);
        buffer[nStartIndex + 6] = (byte)((lValue >> 8) & 0x0ff);
        buffer[nStartIndex + 7] = (byte)lValue;
    }

    /**
     * converts values from an integer array to a long
     * @param buffer where to get the bytes
     * @param nStartIndex index from where to read the data
     * @return the 64bit integer
     */
    private static long IntArrayToLong(int[] buffer, int nStartIndex)
    {
        return (((long)buffer[nStartIndex]) << 32) |
                (((long)buffer[nStartIndex + 1]) & 0x0ffffffffL);
    }

    /**
     * converts a long to integers which are put into a given array
     * @param lValue the 64bit integer to convert
     * @param buffer the target buffer
     * @param nStartIndex where to place the bytes in the buffer
     */
    private static void LongToIntArray(long lValue, int[] buffer, int nStartIndex)
    {
        buffer[nStartIndex] = (int)(lValue >> 32);
        buffer[nStartIndex + 1] = (int)lValue;
    }

    /**
     * makes a long from two integers (treated unsigned)
     * @param nLo lower 32bits
     * @param nHi higher 32bits
     * @return the built long
     */
    private static long MakeLong(uint nLo, uint nHi)
    {
        return (((long)nHi << 32) |
                ((long)nLo & 0x00000000ffffffffL));
    }

    /**
     * gets the lower 32 bits of a long
     * @param lVal the long integer
     * @return lower 32 bits
     */
    private static uint LongLower32(long lVal)
    {
        return (uint)lVal;
    }

    /**
     * gets the higher 32 bits of a long
     * @param lVal the long integer
     * @return higher 32 bits
     */
    private static uint LongHight32(long lVal)
    {
        return (uint)((lVal >> 32));
    }

    // our table for binhex conversion
    static readonly char[] HEXTAB = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

    /**
     * converts a byte array to a binhex string
     * @param data the byte array
     * @param nStartPos start index where to get the bytes
     * @param nNumOfBytes number of bytes to convert
     * @return the binhex string
     */
    private static String BytesToBinHex(byte[] data, int nStartPos, int nNumOfBytes)
    {
        StringBuilder sbuf = new StringBuilder();
        sbuf.EnsureCapacity(nNumOfBytes << 1);

        int nPos = 0;
        for (int nI = 0; nI < nNumOfBytes; nI++)
        {
            sbuf.Insert(nPos++, HEXTAB[(data[nI + nStartPos] >> 4) & 0x0f]);
            sbuf.Insert(nPos++, HEXTAB[data[nI + nStartPos] & 0x0f]);
        }
        return sbuf.ToString();
    }

    /**
     * converts a binhex string back into a byte array (invalid codes will be skipped)
     * @param sBinHex binhex string
     * @param data the target array
     * @param nSrcPos from which character in the string the conversion should begin,
     *                remember that (nSrcPos modulo 2) should equals 0 normally
     * @param nDstPos to store the bytes from which position in the array
     * @param nNumOfBytes number of bytes to extract
     * @return number of extracted bytes
     */
    private static int BinHexToBytes(String sBinHex, byte[] data, int nSrcPos, int nDstPos, int nNumOfBytes)
    {
        // check for correct ranges
        int nStrLen = sBinHex.Length;

        int nAvailBytes = (nStrLen - nSrcPos) >> 1;
        if (nAvailBytes < nNumOfBytes)
        {
            nNumOfBytes = nAvailBytes;
        }

        int nOutputCapacity = data.Length - nDstPos;
        if (nNumOfBytes > nOutputCapacity)
        {
            nNumOfBytes = nOutputCapacity;
        }

        // convert now
        int nResult = 0;
        for (int nI = 0; nI < nNumOfBytes; nI++)
        {
            byte bActByte = 0;
            bool blConvertOK = true;
            for (int nJ = 0; nJ < 2; nJ++)
            {
                bActByte <<= 4;
                char cActChar = sBinHex[nSrcPos++];

                if ((cActChar >= 'a') && (cActChar <= 'f'))
                {
                    bActByte |= (byte)((cActChar - 'a') + 10);
                }
                else
                {
                    if ((cActChar >= '0') && (cActChar <= '9'))
                    {
                        bActByte |= (byte)(cActChar - '0');
                    }
                    else
                    {
                        blConvertOK = false;
                    }
                }
            }
            if (blConvertOK)
            {
                data[nDstPos++] = bActByte;
                nResult++;
            }
        }

        return nResult;
    }

    /**
     * converts a byte array into an UNICODE string
     * @param data the byte array
     * @param nStartPos where to begin the conversion
     * @param nNumOfBytes number of bytes to handle
     * @return the string
     */
    private static String ByteArrayToUNCString(byte[] data, int nStartPos, int nNumOfBytes)
    {
        // we need two bytes for every character
        nNumOfBytes &= ~1;

        // enough bytes in the buffer?
        int nAvailCapacity = data.Length - nStartPos;

        if (nAvailCapacity < nNumOfBytes)
        {
            nNumOfBytes = nAvailCapacity;
        }

        StringBuilder sbuf = new StringBuilder();
        sbuf.EnsureCapacity(nNumOfBytes >> 1);

        int nSBufPos = 0;

        while (nNumOfBytes > 0)
        {
            sbuf.Insert(nSBufPos++, (char)(((int)data[nStartPos] << 8) | ((int)data[nStartPos + 1] & 0x0ff)));
            nStartPos += 2;
            nNumOfBytes -= 2;
        }

        return sbuf.ToString();
    }
}

