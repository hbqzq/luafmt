using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace lfmt
{
    public class FormatOptions
    {
        [Option("line-width", Default = 0, Required = false, HelpText = "Maximum length of a line before it will be wrapped")]
        public int lineWidth { get; set; }
        [Option("indent-count", Default = 4, Required = false, HelpText = "Number of characters to indent")]
        public int indentCount { get; set; }
        [Option("use-tabs", Default = false, Required = false, HelpText = "Use tabs instead of spaces for indentation")]
        public bool useTabs { get; set; }
        [Option("keep-wraps", Default = true, Required = false, HelpText = "Keep wraps unchanged")]
        public bool keepWraps { get; set; }
        [Option("input-mode", Default = "stdin", Required = false, HelpText = "Input source mode, should be one of text/file/stdin, using 'stdin' if absent")]
        public string inputMode { get; set; }
        [Option('i', "input", Required = false, HelpText = "Input source")]
        public string input { get; set; }
        [Option("output-mode", Default = "stdout", Required = false, HelpText = "Output mode, should be one of stdout/file, using 'stdout' if absent")]
        public string outputMode { get; set; }
        [Option('o', "output", Default = null, Required = false, HelpText = "Output file path, use std-out if absent")]
        public string output { get; set; }
        [Option("format-comments", Default = false, Required = false, HelpText = "Format multi-line comments.")]
        public bool formatComments { get; set; }

        public static FormatOptions Parse(string[] args)
        {
            FormatOptions ret = null;
            Parser.Default.ParseArguments<FormatOptions>(args)
                .WithParsed(options => ret = options)
                .WithNotParsed(errors =>
                {
                    foreach (var e in errors)
                    {
                        if (e.Tag == CommandLine.ErrorType.HelpRequestedError)
                            continue;
                        throw new System.Exception(e.ToString());
                    }
                });
            return ret;
        }
    }
}