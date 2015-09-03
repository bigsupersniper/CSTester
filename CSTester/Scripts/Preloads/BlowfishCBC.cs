using System;
using System.Security.Cryptography;
using System.Text;

public class BlowfishCBC : BlowfishECB
{
    // here we hold the CBC IV
    long m_lCBCIV;

    public BlowfishCBC() { }

    /**
     * get the current CBC IV (for cipher resets)
     * @return current CBC IV
     */
    public long GetCBCIV()
    {
        return m_lCBCIV;
    }

    /**
     * get the current CBC IV (for cipher resets)
     * @param dest wher eto put current CBC IV in network byte ordered array
     */
    public void GetCBCIV(byte[] dest)
    {
        LongToByteArray(m_lCBCIV, dest, 0);
    }

    /**
     * set the current CBC IV (for cipher resets)
     * @param lNewCBCIV the new CBC IV
     */
    public void SetCBCIV(long lNewCBCIV)
    {
        m_lCBCIV = lNewCBCIV;
    }

    /**
     * set the current CBC IV (for cipher resets)
     * @param newCBCIV the new CBC IV  in network byte ordered array
     */
    public void SetCBCIV(byte[] newCBCIV)
    {
        m_lCBCIV = ByteArrayToLong(newCBCIV, 0);
    }


    /**
     * constructor, stores a zero CBC IV
     * @param bfkey key material, up to MAXKEYLENGTH bytes
     */
    public BlowfishCBC(byte[] bfkey)
        : base(bfkey)
    {
        // store zero CBCB IV
        SetCBCIV(0);
    }


    /**
     * constructor
     * @param bfkey key material, up to MAXKEYLENGTH bytes
     * @param lInitCBCIV the CBC IV
     */
    public BlowfishCBC(byte[] bfkey, long lInitCBCIV)
        : base(bfkey)
    {
        // store the CBCB IV
        SetCBCIV(lInitCBCIV);
    }


    /**
     * constructor
     * @param bfkey key material, up to MAXKEYLENGTH bytes
     * @param initCBCIV the CBC IV (array with min. BLOCKSIZE bytes)
     */
    public BlowfishCBC(byte[] bfkey, byte[] initCBCIV)
        : base(bfkey)
    {
        // store the CBCB IV
        SetCBCIV(initCBCIV);
    }


    /**
     * cleans up all critical internals,
     * call this if you don't need an instance anymore
     */
    public override void CleanUp()
    {
        m_lCBCIV = 0;
        base.CleanUp();
    }


    // internal routine to encrypt a block in CBC mode
    private long EncryptBlockCBC(long lPlainblock)
    {
        // chain with the CBC IV
        lPlainblock ^= m_lCBCIV;

        // encrypt the block
        lPlainblock = base.EncryptBlock(lPlainblock);

        // the encrypted block is the new CBC IV
        return (m_lCBCIV = lPlainblock);
    }


    // internal routine to decrypt a block in CBC mode
    private long DecryptBlockCBC(long lCipherblock)
    {
        // save the current block
        long lTemp = lCipherblock;

        // decrypt the block
        lCipherblock = base.DecryptBlock(lCipherblock);

        // dechain the block
        lCipherblock ^= m_lCBCIV;

        // set the new CBC IV
        m_lCBCIV = lTemp;

        // return the decrypted block
        return lCipherblock;
    }

    /**
     * encrypts a byte buffer (should be aligned to an 8 byte border)
     * to another buffer (of the same size or bigger)
     * @param inbuffer buffer with plaintext data
     * @param outbuffer buffer to get the ciphertext data
     */
    public override void Encrypt(byte[] inbuffer, byte[] outbuffer)
    {
        int nLen = inbuffer.Length;
        long lTemp;
        for (int nI = 0; nI < nLen; nI += 8)
        {
            // encrypt a temporary 64bit block
            lTemp = ByteArrayToLong(inbuffer, nI);
            lTemp = EncryptBlockCBC(lTemp);
            LongToByteArray(lTemp, outbuffer, nI);
        }
    }

    /**
     * encrypts a byte buffer (should be aligned to an 8 byte border) to itself
     * @param buffer buffer to encrypt
     */
    public override void Encrypt(byte[] buffer)
    {

        int nLen = buffer.Length;
        long lTemp;
        for (int nI = 0; nI < nLen; nI += 8)
        {
            // encrypt a temporary 64bit block
            lTemp = ByteArrayToLong(buffer, nI);
            lTemp = EncryptBlockCBC(lTemp);
            LongToByteArray(lTemp, buffer, nI);
        }
    }

    /**
     * encrypts an int buffer (should be aligned to an
     * two integer border) to another int buffer (of the same
     * size or bigger)
     * @param inbuffer buffer with plaintext data
     * @param outbuffer buffer to get the ciphertext data
     */
    public override void Encrypt(int[] inbuffer, int[] outbuffer)
    {
        int nLen = inbuffer.Length;
        long lTemp;
        for (int nI = 0; nI < nLen; nI += 2)
        {
            // encrypt a temporary 64bit block
            lTemp = IntArrayToLong(inbuffer, nI);
            lTemp = EncryptBlockCBC(lTemp);
            LongToIntArray(lTemp, outbuffer, nI);
        }
    }

    /**
     * encrypts an integer buffer (should be aligned to an
     * @param buffer buffer to encrypt
     */

    public override void Encrypt(int[] buffer)
    {
        int nLen = buffer.Length;
        long lTemp;
        for (int nI = 0; nI < nLen; nI += 2)
        {
            // encrypt a temporary 64bit block
            lTemp = IntArrayToLong(buffer, nI);
            lTemp = EncryptBlockCBC(lTemp);
            LongToIntArray(lTemp, buffer, nI);
        }
    }

    /**
     * encrypts a long buffer to another long buffer (of the same size or bigger)
     * @param inbuffer buffer with plaintext data
     * @param outbuffer buffer to get the ciphertext data
     */
    public override void Encrypt(long[] inbuffer, long[] outbuffer)
    {
        int nLen = inbuffer.Length;
        for (int nI = 0; nI < nLen; nI++)
        {
            outbuffer[nI] = EncryptBlockCBC(inbuffer[nI]);
        }
    }

    /**
     * encrypts a long buffer to itself
     * @param buffer buffer to encrypt
     */
    public override void Encrypt(long[] buffer)
    {
        int nLen = buffer.Length;
        for (int nI = 0; nI < nLen; nI++)
        {
            buffer[nI] = EncryptBlockCBC(buffer[nI]);
        }
    }

    /**
     * decrypts a byte buffer (should be aligned to an 8 byte border)
     * to another buffer (of the same size or bigger)
     * @param inbuffer buffer with ciphertext data
     * @param outbuffer buffer to get the plaintext data
     */
    public override void Decrypt(byte[] inbuffer, byte[] outbuffer)
    {
        int nLen = inbuffer.Length;
        long lTemp;
        for (int nI = 0; nI < nLen; nI += 8)
        {
            // decrypt a temporary 64bit block
            lTemp = ByteArrayToLong(inbuffer, nI);
            lTemp = DecryptBlockCBC(lTemp);
            LongToByteArray(lTemp, outbuffer, nI);
        }
    }

    /**
     * decrypts a byte buffer (should be aligned to an 8 byte border) to itself
     * @param buffer buffer to decrypt
     */
    public override void Decrypt(byte[] buffer)
    {
        int nLen = buffer.Length;
        long lTemp;
        for (int nI = 0; nI < nLen; nI += 8)
        {
            // decrypt over a temporary 64bit block
            lTemp = ByteArrayToLong(buffer, nI);
            lTemp = DecryptBlockCBC(lTemp);
            LongToByteArray(lTemp, buffer, nI);
        }
    }

    /**
     * decrypts an integer buffer (should be aligned to an
     * two integer border) to another int buffer (of the same size or bigger)
     * @param inbuffer buffer with ciphertext data
     * @param outbuffer buffer to get the plaintext data
     */
    public override void Decrypt(int[] inbuffer, int[] outbuffer)
    {

        int nLen = inbuffer.Length;
        long lTemp;
        for (int nI = 0; nI < nLen; nI += 2)
        {
            // decrypt a temporary 64bit block
            lTemp = IntArrayToLong(inbuffer, nI);
            lTemp = DecryptBlockCBC(lTemp);
            LongToIntArray(lTemp, outbuffer, nI);
        }
    }

    /**
     * decrypts an int buffer (should be aligned to a
     * two integer border)
     * @param buffer buffer to decrypt
     */
    public override void Decrypt(int[] buffer)
    {
        int nLen = buffer.Length;
        long lTemp;
        for (int nI = 0; nI < nLen; nI += 2)
        {
            // decrypt a temporary 64bit block
            lTemp = IntArrayToLong(buffer, nI);
            lTemp = DecryptBlockCBC(lTemp);
            LongToIntArray(lTemp, buffer, nI);
        }
    }

    /**
     * decrypts a long buffer to another long buffer (of the same size or bigger)
     * @param inbuffer buffer with ciphertext data
     * @param outbuffer buffer to get the plaintext data
     */
    public override void Decrypt(long[] inbuffer, long[] outbuffer)
    {
        int nLen = inbuffer.Length;
        for (int nI = 0; nI < nLen; nI++)
        {
            outbuffer[nI] = DecryptBlockCBC(inbuffer[nI]);
        }
    }

    /**
     * decrypts a long buffer to itself
     * @param buffer buffer to decrypt
     */
    public override void Decrypt(long[] buffer)
    {
        int nLen = buffer.Length;
        for (int nI = 0; nI < nLen; nI++)
        {
            buffer[nI] = DecryptBlockCBC(buffer[nI]);
        }
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


}

