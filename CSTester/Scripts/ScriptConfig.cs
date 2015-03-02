using System;
using CSTester.CSEngine;

public class ScriptConfig : IScriptConfig
{
    /// <summary>
    /// 引用的dll文件名集合
    /// </summary>
    public string[] Assemblies { get; private set; }

    /// <summary>
    /// 预加载文件名集合
    /// </summary>
    public string[] Preloads { get; private set; }

    /// <summary>
    /// 可执行脚本集合
    /// </summary>
    public string[] Scripts { get; private set; }

    public ScriptConfig()
    {
        Assemblies = new string[]
        {
            "System.Net.Http", 
            "Newtonsoft.Json",
            "LF.Toolkit.Util"
        };

        Preloads = new string[]
        {
            "Preloads/Function.cs",
            "Preloads/ScriptBase.cs",
        };

        Scripts = new string[]
        {
            "StringUtils.cs",
        };
    }
}
