using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSTester.CSEngine
{
    public interface ISqlQuery
    {
        IEnumerable<T> Query<T>(string sql);

        int Execute(string sql);
    }
}
