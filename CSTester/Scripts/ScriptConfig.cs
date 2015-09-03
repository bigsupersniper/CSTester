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
			"PresentationFramework",
            "System.Net.Http",
			"System.Net.Http.Formatting",
            "Newtonsoft.Json",
			"LF.Toolkit.Common",
			"System.Drawing"
		};

		Preloads = new string[]
		{
            "Preloads/Function.cs",
            "Preloads/ScriptBase.cs",
            "Preloads/BlowfishECB.cs",
            "Preloads/BlowfishCBC.cs",
            "Preloads/Blowfish.cs",
		};

		Scripts = new string[]
		{
            "StringDebug.cs",
			"CryptoDebug.cs"
        };
    }
}
