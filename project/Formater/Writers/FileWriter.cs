using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lfmt
{
    public class FileWriter : IFormatWriter
    {
        private FileStream stream;
        private StreamWriter writer;

        public FileWriter(string path)
        {
            stream = new FileStream(path, FileMode.Create);
            writer = new StreamWriter(stream);
        }

        public void Append(string s)
        {
            writer.Write(s);
        }

        public void Close()
        {
            writer.Close();
            stream.Close();
        }
    }
}
