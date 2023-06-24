using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webprotocol
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Log log = new Log();
            log.Write(args);
        }

    }
}
