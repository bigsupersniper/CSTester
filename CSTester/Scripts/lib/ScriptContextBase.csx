using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using CSTester.CSEngine;

public class ScriptContextBase : IScriptContext
{
    public string ScriptName { get; set; }

    public IEnumerable<IMethodContext> MethodContexts { get; set; }

    public IScriptBootstrap ScriptBootstrap { get; set; }

    public ScriptContextBase()
    {
        this.ScriptName = this.GetType().Name;
        var contexts = new List<IMethodContext>();

        var methods = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (var m in methods)
        {
            if (m.ReturnType == typeof(IMethodContext) || m.ReturnType == typeof(MethodContext))
            {
                var context = m.Invoke(this, null) as IMethodContext;
                if(contexts.Count(i => i.MethodName == context.MethodName) <= 0)
                {
                    contexts.Add(context);
                }
            }
        }

        MethodContexts = contexts;
    }
}
