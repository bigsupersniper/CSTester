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

public class StringScript : ScriptContextBase
{
    IMethodContext CreateRandomString()
    {
        var context = new MethodContext();
        context.MethodName = MethodBase.GetCurrentMethod().Name;
        context.Parameters = new JObject();
        context.Parameters.Add("length", "");
        context.Parameters.Add("type", 1);
        context.Parameters.Add("summary", "type各个值为 1、字母和数字 2、纯字母 3、纯数字");
        context.Execute = () =>
        {
            try
            {
                int length = context.Parameters.Value<int>("length");
                int type = context.Parameters.Value<int>("type");
                if (length <= 0) throw new Exception("length不正确");
                if (type < 1 || type > 3) throw new Exception("type类型无效");

                string str = "";
                switch (type)
                {
                    case 1:
                        str = RandomStringGenerator.CreateRandomAlphanumeric(length);
                        break;
                    case 2:
                        str = RandomStringGenerator.CreateRandomLetters(length);
                        break;
                    case 3:
                        str = RandomStringGenerator.CreateRandomNumeric(length);
                        break;
                    default:
                        break;
                }
                TextPrinter.WriteLine(context.MethodName + " <output> " + str);
            }
            catch (Exception e)
            {
                TextPrinter.WriteLine(e);
            }
        };

        return context;
    }

    IMethodContext TimeStampParser()
    {
        var context = new MethodContext();
        context.MethodName = MethodBase.GetCurrentMethod().Name;
        context.Parameters = new JObject();
        context.Parameters.Add("input", "");
        context.Parameters.Add("type", 1);
        context.Parameters.Add("summary", "type各个值为 1、获取当前时间戳 2、转换时间戳为日期 3、转换指定日期为时间戳");
        context.Execute = () =>
        {
            try
            {
                string input = context.Parameters.Value<string>("input");
                int type = context.Parameters.Value<int>("type");
                if (type < 1 || type > 3) throw new Exception("type类型无效");

                string str = "";
                switch (type)
                {
                    case 1:
                        str = DateTimeExtension.CurrentTimestamp + "";
                        break;
                    case 2:
                        long tm = 0;
                        if (long.TryParse(input, out tm))
                        {
                            str = DateTimeExtension.ConvertToDateTime(tm).ToString();
                        }
                        break;
                    case 3:
                        DateTime dt;
                        if (DateTime.TryParse(input, out dt))
                        {
                            str = DateTimeExtension.ConvertToTimestamp(dt) + "";
                        }
                        break;
                    default:
                        break;
                }
                TextPrinter.WriteLine(context.MethodName + " <output> " + str);
            }
            catch (Exception e)
            {
                TextPrinter.WriteLine(e);
            }
        };

        return context;
    }

    IMethodContext GetNewGuid()
    {
        var context = new MethodContext();
        context.MethodName = MethodBase.GetCurrentMethod().Name;
        context.Execute = () =>
        {
            try
            {
                TextPrinter.WriteLine(context.MethodName + " <output> " + Guid.NewGuid().ToString("N"));
            }
            catch (Exception e)
            {
                TextPrinter.WriteLine(e);
            }
        };

        return context;
    }

    IMethodContext ComputeHash()
    {
        var context = new MethodContext();
        context.MethodName = MethodBase.GetCurrentMethod().Name;
        context.Parameters = new JObject();
        context.Parameters.Add("str", "");
        context.Parameters.Add("lowerCase", true);
        context.Parameters.Add("algType", "MD5");
        context.Parameters.Add("summary", "algType类型如下： MD5、SHA1、SHA256、SHA384、SHA512");
        context.Execute = () =>
        {
            try
            {
                string str = context.Parameters.Value<String>("str");
                bool lowerCase = context.Parameters.Value<bool>("lowerCase");
                HashAlgorithmType algType;
                string type = context.Parameters.Value<String>("algType");
                if (!Enum.TryParse<HashAlgorithmType>(type, out algType)) throw new Exception("algType值无效");
                if (string.IsNullOrEmpty(str)) throw new Exception("str不能为空");

                TextPrinter.WriteLine("[" + algType + "] " + context.MethodName
                    + " <output> " + HashAlgorithmProvider.ComputeHash(algType, str, lowerCase));
            }
            catch (Exception e)
            {
                TextPrinter.WriteLine(e);
            }
        };

        return context;
    }

    IMethodContext EncryptToBase64()
    {
        var context = new MethodContext();
        context.MethodName = MethodBase.GetCurrentMethod().Name;
        context.Parameters = new JObject();
        context.Parameters.Add("str", "");
        context.Parameters.Add("key", "");
        context.Parameters.Add("cipherMode", "ECB");
        context.Parameters.Add("paddingMode", "PKCS7");
        context.Parameters.Add("summary", @"cipherMode有效值： CBC、ECB、OFB、CFB、CTS  
                            paddingMode有效值：None、PKCS7、Zeros、ANSIX923、ISO10126");

        context.Execute = () =>
        {
            try
            {
                string str = context.Parameters.Value<String>("str");
                string key = context.Parameters.Value<String>("key");
                CipherMode cipherMode;
                PaddingMode paddingMode;
                if (!Enum.TryParse<CipherMode>(context.Parameters.Value<String>("cipherMode"), out cipherMode))
                    throw new Exception("cipherMode值无效");
                if (!Enum.TryParse<PaddingMode>(context.Parameters.Value<String>("paddingMode"), out paddingMode))
                    throw new Exception("paddingMode值无效");
                if (string.IsNullOrEmpty(str)) throw new Exception("str不能为空");

                TextPrinter.WriteLine("[Aes/" + cipherMode + "/" + paddingMode + "] "
                    + context.MethodName + " <output> " + AesAlgorithmProvider.EncryptToBase64(str, key, cipherMode, paddingMode));
            }
            catch (Exception e)
            {
                TextPrinter.WriteLine(e);
            }
        };

        return context;
    }

    IMethodContext DecryptFromBase64()
    {
        var context = new MethodContext();
        context.MethodName = MethodBase.GetCurrentMethod().Name;
        context.Parameters = new JObject();
        context.Parameters.Add("str", "");
        context.Parameters.Add("key", "");
        context.Parameters.Add("cipherMode", "ECB");
        context.Parameters.Add("paddingMode", "PKCS7");
        context.Parameters.Add("summary", @"cipherMode有效值： CBC、ECB、OFB、CFB、CTS  
                            paddingMode有效值：None、PKCS7、Zeros、ANSIX923、ISO10126");

        context.Execute = () =>
        {
            try
            {
                string str = context.Parameters.Value<String>("str");
                string key = context.Parameters.Value<String>("key");
                CipherMode cipherMode;
                PaddingMode paddingMode;
                if (!Enum.TryParse<CipherMode>(context.Parameters.Value<String>("cipherMode"), out cipherMode))
                    throw new Exception("cipherMode值无效");
                if (!Enum.TryParse<PaddingMode>(context.Parameters.Value<String>("paddingMode"), out paddingMode))
                    throw new Exception("paddingMode值无效");
                if (string.IsNullOrEmpty(str)) throw new Exception("str不能为空");

                TextPrinter.WriteLine("[Aes/" + cipherMode + "/" + paddingMode + "] "
                    + context.MethodName + " <output> " + AesAlgorithmProvider.DecryptFromBase64(Encoding.UTF8, str, key, cipherMode, paddingMode));
            }
            catch (Exception e)
            {
                TextPrinter.WriteLine(e);
            }
        };

        return context;
    }
}
