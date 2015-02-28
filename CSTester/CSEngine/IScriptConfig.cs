
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSTester.CSEngine
{
    public interface IScriptConfig
    {
        /// <summary>
        /// 引用的dll文件名集合
        /// </summary>
        string[] Assemblies { get; }

        /// <summary>
        /// 预加载文件名集合
        /// </summary>
        string[] Preloads { get; }

        /// <summary>
        /// 可执行脚本集合
        /// </summary>
        string[] Scripts { get; }
    }
}
