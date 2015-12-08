using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSTester.CSEngine
{
    public interface IScriptBootstrap
    {
        IEnumerable<IScriptContext> ScriptContexts { get; }
    }
}
