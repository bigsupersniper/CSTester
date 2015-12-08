using System;
using System.IO;
using System.Collections.Generic;
using CSScriptLibrary;
using CSTester.CSEngine;

public class ScriptBootstrap : IScriptBootstrap
{
    public IEnumerable<IScriptContext> ScriptContexts { get; private set; }

    public ScriptBootstrap()
    {
        LoadAssemblyByName();
        LoadLibraryScript();
        LoadExecutableScript();
    }

    void LoadAssemblyByName()
    {
        string[] referenceAssemblyNames = new string[]
        {
            "PresentationFramework",
            "System.Net.Http",
            "System.Net.Http.Formatting",
            "Newtonsoft.Json",
            "LF.Toolkit.Common",
            "System.Drawing",
            "System.Data",
            "Dapper"
        };
        foreach (var name in referenceAssemblyNames)
        {
            CSScript.Evaluator.ReferenceAssemblyByName(name);
        }
    }

    void LoadLibraryScript()
    {
        string[] scripts = new string[]
        {
            "MethodContext.csx",
            "ScriptContextBase.csx",
            "SqlServerQuery.csx",
        };

        foreach (var script in scripts)
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", "lib", script);

            try
            {
                CSScript.Evaluator.LoadFile(file);
            }
            catch (Exception e)
            {
                throw new Exception("Import Reference Script \"" + file + "\" Catch " + e);
            }
        }
    }

    void LoadExecutableScript()
    {
        var contexts = new List<IScriptContext>();
        string[] scripts = new string[]
        {
            "CSDialogScript.csx",
            "StringScript.csx"
        };

        foreach (var script in scripts)
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", script);

            try
            {
                var context = CSScript.Evaluator.LoadFile<IScriptContext>(file);
                context.ScriptBootstrap = this;
                contexts.Add(context);
            }
            catch (Exception e)
            {
                throw new Exception("Load Executable Script \"" + file + "\" Catch " + e);
            }
        }

        ScriptContexts = contexts;
    }
}
