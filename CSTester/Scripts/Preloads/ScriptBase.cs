using System;
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
}