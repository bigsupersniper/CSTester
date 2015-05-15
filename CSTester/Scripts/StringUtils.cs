using LF.Toolkit.Util;
using LF.Toolkit.Util.Crypto;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StringUtils : ScriptBase
{
    void CreateRandom()
    {
        var func = new Function();
        func.Name = MethodBase.GetCurrentMethod().Name;
        var json = new JObject();
        func.Json = json;
        json.Add("length", "");
        json.Add("type", "");
        json.Add("summary", "type各个值为 1、字母和数字 2、纯字母 3、纯数字");

        if (Functions.Count(i => i.Name == func.Name) <= 0)
        {
            func.Invoker = () =>
            {
                try
                {
                    int length = (int)func.Json["length"];
                    int type = (int)func.Json["type"];
                    if (length <= 0) throw new Exception("length不正确");
                    if (type < 1 || type > 3) throw new Exception("type不正确");

                    string str = "";
                    switch (type)
                    {
                        case 1:
                            str = StringProvider.CreateRandomAlphanumeric(length);
                            break;
                        case 2:
                            str = StringProvider.CreateRandomLetters(length);
                            break;
                        case 3:
                            str = StringProvider.CreateRandomNumeric(length);
                            break;
                        default:
                            break;
                    }

                    Console.Out.WriteLine(func.Name + "输出结果如下：" + str);
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine(e);
                }
            };

            Functions.Add(func);
        }
    }

    void ComputeHash()
    {
        var func = new Function();
        func.Name = MethodBase.GetCurrentMethod().Name;
        var json = new JObject();
        func.Json = json;
        json.Add("source", "");
        json.Add("format", "x2");
        json.Add("type", "");
        json.Add("summary", "type各个值为 1、MD5 2、SHA1");

        if (Functions.Count(i => i.Name == func.Name) <= 0)
        {
            func.Invoker = () =>
            {
                try
                {
                    string source = (string)func.Json["source"];
                    string format = (string)func.Json["format"];
                    int type = (int)func.Json["type"];
                    if (string.IsNullOrEmpty(source)) throw new Exception("source不能为空");
                    if (string.IsNullOrEmpty(format)) throw new Exception("format不能为空");
                    if (type < 1 || type > 2) throw new Exception("type不正确");

                    string str = "";
                    switch (type)
                    {
                        case 1:
                            str = MD5Provider.ComputeHash(source, format);
                            break;
                        case 2:
                            str = SHA1Provider.ComputeHash(source, format);
                            break;
                        default:
                            break;
                    }

                    Console.Out.WriteLine(func.Name + "输出结果如下：" + str);
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine(e);
                }
            };

            Functions.Add(func);
        }
    }

    void Crypto()
    {
        var func = new Function();
        func.Name = MethodBase.GetCurrentMethod().Name;
        var json = new JObject();
        func.Json = json;
        json.Add("source", "");
        json.Add("key", "");
        json.Add("type", "");
        json.Add("decrypt", false);
        json.Add("summary", "type各个值为 1、AES(ECB/PKCS7) 2、Blowfish(CBC/SHA1(key)) \r decrypt各个值为 true、解密 false、加密");

        if (Functions.Count(i => i.Name == func.Name) <= 0)
        {
            func.Invoker = () =>
            {
                try
                {
                    string source = (string)func.Json["source"];
                    string key = (string)func.Json["key"];
                    bool decrypt = (bool)func.Json["decrypt"];
                    int type = (int)func.Json["type"];
                    if (string.IsNullOrEmpty(source)) throw new Exception("source不能为空");
                    if (string.IsNullOrEmpty(key)) throw new Exception("key不能为空");
                    if (type < 1 || type > 2) throw new Exception("type不正确");

                    string str = "";
                    switch (type)
                    {
                        case 1:
                            if (decrypt)
                            {
                                str = AESProvider.DecryptFromBase64(key, source);
                            }
                            else
                            {
                                str = AESProvider.EncryptToBase64(key, source);
                            }
                            break;
                        case 2:
                            var bf = new BlowfishProvider(key);
                            if (decrypt)
                            {
                                str = bf.Decrypt_CBC(source);
                            }
                            else
                            {
                                str = bf.Encrypt_CBC(source);
                            }
                            break;
                        default:
                            break;
                    }

                    Console.Out.WriteLine(func.Name + "输出结果如下：" + str);
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine(e);
                }
            };

            Functions.Add(func);
        }
    }

    void TimeStamp()
    {
        var func = new Function();
        func.Name = MethodBase.GetCurrentMethod().Name;
        var json = new JObject();
        func.Json = json;
        json.Add("source", "");
        json.Add("type", "1");
        json.Add("summary", "type各个值为 1、获取当前时间戳 2、转换时间戳为日期 3、转换指定日期为时间戳");

        if (Functions.Count(i => i.Name == func.Name) <= 0)
        {
            func.Invoker = () =>
            {
                try
                {
                    string source = (string)func.Json["source"];
                    int type = (int)func.Json["type"];
                    if (type < 1 || type > 3) throw new Exception("type不正确");

                    string str = "";
                    switch (type)
                    {
                        case 1:
                            str = Timestamp.GetCurrentTimeMillis() + "";
                            break;
                        case 2:
                            long tm = 0;
                            if (long.TryParse(source, out tm))
                            {
                                str = Timestamp.ParseTimeMillis(tm).ToString();
                            }
                            break;
                        case 3:
                            DateTime dt;
                            if (DateTime.TryParse(source, out dt))
                            {
                                str = Timestamp.GetTimeMillis(dt) + "";
                            }
                            break;
                        default:
                            break;
                    }

                    Console.Out.WriteLine(func.Name + "输出结果如下：" + str);
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine(e);
                }
            };

            Functions.Add(func);
        }
    }

    void DateTimeUtil()
    {
        var func = new Function();
        func.Name = MethodBase.GetCurrentMethod().Name;
        var json = new JObject();
        func.Json = json;

        if (Functions.Count(i => i.Name == func.Name) <= 0)
        {
            func.Invoker = () =>
            {
                try
                {
					var now = DateTime.Now;
                    Console.Out.WriteLine("today ：" + now.Date + " -- " + now.Date.Add(new TimeSpan(23, 59, 59)));					
					var startOfWeek = now.Date.AddDays(((int)now.DayOfWeek - 1) * -1);
					var endOfWeek = startOfWeek.Date.Add(new TimeSpan(6, 23, 59, 59));
                    Console.Out.WriteLine("weekend ：" + startOfWeek + " -- " + endOfWeek);
					var startOfMonth = now.Date.AddDays(((int)now.Day - 1) * -1);
					var endOfMonth = startOfMonth.Date.AddMonths(1).AddSeconds(-1);                    
					Console.Out.WriteLine("month ：" + startOfMonth + " -- " + endOfMonth);
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine(e);
                }
            };

            Functions.Add(func);
        }
    }

    void GuidUtil()
    {
        var func = new Function();
        func.Name = MethodBase.GetCurrentMethod().Name;
        var json = new JObject();
        func.Json = json;

        if (Functions.Count(i => i.Name == func.Name) <= 0)
        {
            func.Invoker = () =>
            {
                try
                {               
					Console.Out.WriteLine("guid ：" + Guid.NewGuid().ToString("N"));
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine(e);
                }
            };

            Functions.Add(func);
        }
    }	
	
}
