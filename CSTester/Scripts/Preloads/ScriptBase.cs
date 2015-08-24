﻿using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

public class ScriptBase : IScript
{
    public string Name { get; private set; }

    public IList<IFunction> Functions { get; private set; }

    public ScriptBase()
    {
        this.Name = this.GetType().Name;
        this.Functions = new List<IFunction>();

        var methods = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (var m in methods)
        {
            if (m.IsPrivate && m.GetParameters().Length <= 0)
            {
                m.Invoke(this, null);
            }
        }
    }

    public IScript GetScript(string scriptName)
    {
        IScript script = null;

        if (ScriptBootstrap.Scripts != null)
        {
            script = ScriptBootstrap.Scripts.FirstOrDefault(i => i.Name.Equals(scriptName));
        }

        if (script == null)
        {
            Console.Out.WriteLine("\r" + DateTime.Now.ToString() + " --> script " + scriptName + " not found !");
        }

        return script;
    }

    public IFunction GetFunction(string scriptName, string funcName)
    {
        var script = GetScript(scriptName);
        IFunction func = null;
        if (script != null)
        {
            func = script.Functions.FirstOrDefault(i => i.Name.Equals(funcName));
        }

        if (func == null)
        {
            Console.Out.WriteLine("\r" + DateTime.Now.ToString() + " --> function " + scriptName + " not found !");
        }

        return func;
    }

    public void RefreshFunctionJson(string scriptName, string funcName, Action<JObject> callback)
    {
        var func = GetFunction(scriptName, funcName);
        if (func != null && callback != null)
        {
            callback(func.Json);
        }
    }

}