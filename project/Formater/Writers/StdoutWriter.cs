using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lfmt
{
    public class StdoutWriter : IFormatWriter
    {
        public void Append(string s)
        {
            Console.Write(s);
        }

        public void Close()
        {
        }
    }
}
