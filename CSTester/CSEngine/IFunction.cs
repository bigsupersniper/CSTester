using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSTester.CSEngine
{
    public interface IFunction
    {
        /// <summary>
        /// 函数名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 函数所需JSON格式参数
        /// </summary>
        JObject Json { get; set; }

        /// <summary>
        /// 函数调用入口委托
        /// </summary>
        Action<JObject> Invoker { get; set; }
    }
}
