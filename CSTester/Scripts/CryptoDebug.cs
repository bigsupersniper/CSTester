using LF.Toolkit.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using CSTester.CSEngine;

public class CryptoDebug : ScriptBase
{
    void ComputeHash()
    {
        var func = new Function();
        func.Name = MethodBase.GetCurrentMethod().Name;
        var json = new JObject();
        func.Json = json;
        json.Add("str", "");
        json.Add("lowerCase", true);
        json.Add("algType", "MD5");
        json.Add("summary", "algType类型如下： MD5、SHA1、SHA256、SHA384、SHA512");

        if (Functions.Count(i => i.Name == func.Name) <= 0)
        {
            func.Invoker = () =>
            {
                try
                {
                    string str = func.Json.Value<String>("str");
                    bool lowerCase = func.Json.Value<bool>("lowerCase");
                    HashAlgorithmType algType;
                    string type = func.Json.Value<String>("algType");
                    if (!Enum.TryParse<HashAlgorithmType>(type, out algType)) throw new Exception("algType值无效");
                    if (string.IsNullOrEmpty(str)) throw new Exception("str不能为空");

                    TextPrinter.WriteLine("[" + algType + "] " + func.Name
                        + " <output> " + HashAlgorithmProvider.ComputeHash(algType, str, lowerCase));
                }
                catch (Exception e)
                {
                    TextPrinter.WriteLine(e);
                }
            };

            Functions.Add(func);
        }
    }

    void EncryptToBase64()
    {
        var func = new Function();
        func.Name = MethodBase.GetCurrentMethod().Name;
        var json = new JObject();
        func.Json = json;
        json.Add("str", "");
        json.Add("key", "");
        json.Add("cipherMode", "ECB");
        json.Add("paddingMode", "PKCS7");
        json.Add("summary", @"cipherMode有效值： CBC、ECB、OFB、CFB、CTS  
                            paddingMode有效值：None、PKCS7、Zeros、ANSIX923、ISO10126");

        if (Functions.Count(i => i.Name == func.Name) <= 0)
        {
            func.Invoker = () =>
            {
                try
                {
                    string str = func.Json.Value<String>("str");
                    string key = func.Json.Value<String>("key");
                    CipherMode cipherMode;
                    PaddingMode paddingMode;
                    if (!Enum.TryParse<CipherMode>(func.Json.Value<String>("cipherMode"), out cipherMode))
                        throw new Exception("cipherMode值无效");
                    if (!Enum.TryParse<PaddingMode>(func.Json.Value<String>("paddingMode"), out paddingMode))
                        throw new Exception("paddingMode值无效");
                    if (string.IsNullOrEmpty(str)) throw new Exception("str不能为空");

                    TextPrinter.WriteLine("[Aes/" + cipherMode + "/" + paddingMode + "] "
                        + func.Name + " <output> " + SymmetricAlgorithmProvider.EncryptToBase64(SymmetricAlgorithmType.Aes
                        , str, key, cipherMode, paddingMode));
                }
                catch (Exception e)
                {
                    TextPrinter.WriteLine(e);
                }
            };

            Functions.Add(func);
        }
    }

    void DecryptFromBase64()
    {
        var func = new Function();
        func.Name = MethodBase.GetCurrentMethod().Name;
        var json = new JObject();
        func.Json = json;
        json.Add("str", "");
        json.Add("key", "");
        json.Add("cipherMode", "ECB");
        json.Add("paddingMode", "PKCS7");
        json.Add("summary", @"cipherMode有效值： CBC、ECB、OFB、CFB、CTS  
                            paddingMode有效值：None、PKCS7、Zeros、ANSIX923、ISO10126");

        if (Functions.Count(i => i.Name == func.Name) <= 0)
        {
            func.Invoker = () =>
            {
                try
                {
                    string str = func.Json.Value<String>("str");
                    string key = func.Json.Value<String>("key");
                    CipherMode cipherMode;
                    PaddingMode paddingMode;
                    if (!Enum.TryParse<CipherMode>(func.Json.Value<String>("cipherMode"), out cipherMode))
                        throw new Exception("cipherMode值无效");
                    if (!Enum.TryParse<PaddingMode>(func.Json.Value<String>("paddingMode"), out paddingMode))
                        throw new Exception("paddingMode值无效");
                    if (string.IsNullOrEmpty(str)) throw new Exception("str不能为空");

                    TextPrinter.WriteLine("[Aes/" + cipherMode + "/" + paddingMode + "] "
                        + func.Name + " <output> " + SymmetricAlgorithmProvider.DecryptFromBase64(SymmetricAlgorithmType.Aes
                        , Encoding.UTF8, str, key, cipherMode, paddingMode));
                }
                catch (Exception e)
                {
                    TextPrinter.WriteLine(e);
                }
            };

            Functions.Add(func);
        }

    }

    void BlowfishEncrypt_CBC()
    {
        var func = new Function();
        func.Name = MethodBase.GetCurrentMethod().Name;
        var json = new JObject();
        func.Json = json;
        json.Add("str", "");
        json.Add("key", "");

        if (Functions.Count(i => i.Name == func.Name) <= 0)
        {
            func.Invoker = () =>
            {
                try
                {
                    string str = func.Json.Value<String>("str");
                    string key = func.Json.Value<String>("key");
                    var bf = new Blowfish(key);
                    bf.Encrypt_CBC(str);

                    TextPrinter.WriteLine(func.Name + " <output> " + bf.Encrypt_CBC(str));
                }
                catch (Exception e)
                {
                    TextPrinter.WriteLine(e);
                }
            };

            Functions.Add(func);
        }
    }

    void BlowfishDecrypt_CBC()
    {
        var func = new Function();
        func.Name = MethodBase.GetCurrentMethod().Name;
        var json = new JObject();
        func.Json = json;
        json.Add("str", "");
        json.Add("key", "");

        if (Functions.Count(i => i.Name == func.Name) <= 0)
        {
            func.Invoker = () =>
            {
                try
                {
                    string str = func.Json.Value<String>("str");
                    string key = func.Json.Value<String>("key");
                    var bf = new Blowfish(key);
                    bf.Encrypt_CBC(str);

                    TextPrinter.WriteLine(func.Name + " <output> " + bf.Decrypt_CBC(str));
                }
                catch (Exception e)
                {
                    TextPrinter.WriteLine(e);
                }
            };

            Functions.Add(func);
        }
    }
}
