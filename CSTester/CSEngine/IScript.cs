using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSTester.CSEngine
{
    public interface IScript
    {
        /// <summary>
        /// 脚本名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 函数集合
        /// </summary>
        IList<IFunction> Functions { get; }
    }
}
