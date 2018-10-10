using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lfmt
{
    public static class Utils
    {
        public static bool EqualTo(this IParseTree node, string s)
        {
            var terminalNode = node as ITerminalNode;
            return terminalNode != null && terminalNode.Symbol.Text == s;
        }
    }
}
