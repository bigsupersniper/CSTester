using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSTester.CSEngine
{
    public class TextPrinter
    {
        static string CurrentTimeFormat
        {
            get
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public static void WriteLine(object value)
        {
            Console.WriteLine(CurrentTimeFormat + " --> " + value);
        }

        public static void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(CurrentTimeFormat + " --> " + string.Format(format, args));
        }
    }
}
