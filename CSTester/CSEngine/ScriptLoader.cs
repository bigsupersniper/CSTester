using CSScriptLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSTester.CSEngine
{
    internal class ScriptLoader
    {
        public static IScriptBootstrap CreateBootstrap()
        {
            return CSScript.Evaluator.LoadFile<IScriptBootstrap>("./Scripts/ScriptBootstrap.csx");
        }
    }
}
