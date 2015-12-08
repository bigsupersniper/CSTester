using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSTester.CSEngine
{
    public interface IMethodContext
    {
        string MethodName { get; set; }

        JObject Parameters { get; set; }

        Action Execute { get; set; }
    }
}
