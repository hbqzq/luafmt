using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Antlr4.Lua;
using lfmt;
using System.IO;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        var options = lfmt.FormatOptions.Parse(args);
        if (options == null)
            return;

        ICharStream charStream = null;
        switch (options.inputMode)
        {
            case "text":
                charStream = CharStreams.fromstring(options.input);
                break;
            case "file":
                charStream = CharStreams.fromPath(options.input);
                break;
            case "stdin":
                charStream = CharStreams.fromTextReader(Console.In);
                break;
            default:
                // show error messages
                return;
        }

        IFormatWriter writer = null;
        switch (options.outputMode)
        {
            case "stdout":
                writer = new StdoutWriter();
                break;
            case "file":
                writer = new FileWriter(options.output);
                break;
            default:
                // show error messages
                return;
        }

        lfmt.Formatter.Format(charStream, writer, options);

        charStream.Release(0);
        writer.Close();
    }
}
