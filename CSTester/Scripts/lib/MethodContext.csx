using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using CSTester.CSEngine;

public class MethodContext : IMethodContext
{
    public string MethodName { get; set; }

    public JObject Parameters { get; set; }

    public Action Execute { get; set; }
}
