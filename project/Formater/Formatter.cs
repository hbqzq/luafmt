using Antlr4.Lua;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace lfmt
{
    public class Formatter
    {
        public static void Format(ICharStream stream, IFormatWriter writer, FormatOptions options)
        {
            var lexer = new LuaLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new LuaParser(tokens);

            tokens.Fill();

            var comments = tokens.GetTokens().Where(t => t.Channel == LuaLexer.Hidden);
            var spaces = tokens.GetTokens().Where(t => t.Channel == 2);

            parser.BuildParseTree = true;
            parser.TrimParseTree = false;

            IRuleNode root = parser.chunk();

            var ctx = new FormatContext(root, comments, spaces, writer, options);
            RuleFormatter.Format(root, ctx);

            ctx.WriteComments(int.MaxValue);

            var allTokens = tokens.GetTokens();
            if (allTokens.Count > 0)
            {
                var lastToken = allTokens[allTokens.Count - 1];
                while (ctx.line <= lastToken.Line)
                {
                    ctx.WriteLineBreak();
                }
            }

            tokens.Release(0);
        }
    }
}