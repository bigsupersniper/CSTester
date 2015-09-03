using LF.Toolkit.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StringDebug : ScriptBase
{
    void RandomString()
    {
        var func = new Function();
        func.Name = MethodBase.GetCurrentMethod().Name;
        var json = new JObject();
        func.Json = json;
        json.Add("length", "");
        json.Add("type", 1);
        json.Add("summary", "type各个值为 1、字母和数字 2、纯字母 3、纯数字");

        if (Functions.Count(i => i.Name == func.Name) <= 0)
        {
            func.Invoker = () =>
            {
                try
                {
                    int length = func.Json.Value<int>("length");
                    int type = func.Json.Value<int>("type");
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

                    Console.Out.WriteLine(DateTime.Now.ToString() + " -->  " + func.Name + " <output> " + str);
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
        json.Add("input", "");
        json.Add("type", 1);
        json.Add("summary", "type各个值为 1、获取当前时间戳 2、转换时间戳为日期 3、转换指定日期为时间戳");

        if (Functions.Count(i => i.Name == func.Name) <= 0)
        {
            func.Invoker = () =>
            {
                try
                {
                    string input = func.Json.Value<string>("input");
                    int type = func.Json.Value<int>("type");
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
                    Console.Out.WriteLine(DateTime.Now.ToString() + " -->  " + func.Name + " <output> " + str);
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine(e);
                }
            };

            Functions.Add(func);
        }
    }

    void NewGuid()
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
                    Console.Out.WriteLine(DateTime.Now.ToString() + " -->  " + func.Name + " <output> " + Guid.NewGuid().ToString("N"));
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
