using CSScriptLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSTester.CSEngine
{
    public interface IScriptContext
    {
        string ScriptName { get; set; }

        IEnumerable<IMethodContext> MethodContexts { get; set; }

        IScriptBootstrap ScriptBootstrap { get; set; }
    }
}
