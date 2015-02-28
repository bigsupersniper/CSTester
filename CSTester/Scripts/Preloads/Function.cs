
using System;
using System.Collections.Generic;
using CSTester.CSEngine;
using Newtonsoft.Json.Linq;

public class Function : IFunction
{
    public string Name { get; set; }

    public JObject Json { get; set; }

    public Action<JObject> Invoker { get; set; }
}
