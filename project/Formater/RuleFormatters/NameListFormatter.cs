using Antlr4.Lua;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lfmt
{
    [CustomRuleFormatter(LuaParser.RULE_namelist)]
    public class NameListFormatter : RuleFormatter
    {
        protected override void Format()
        {
            for (int i = 0, len = target.ChildCount; i < len; i++)
            {
                var child = target.GetChild(i) as ITerminalNode;
                if (child.EqualTo(","))
                {
                    ctx.WriteSymbol(new FormatSymbol(child));
                }
                else if (FormatSymbol.IsLineBreak(ctx.prevSymbol) || ctx.prevSymbol.line < child.Symbol.Line)
                {
                    ctx.PushIndent();
                    ctx.WriteSymbol(new FormatSymbol(child));
                    ctx.PopIndent();
                }
                else
                {
                    ctx.WriteSymbol(new FormatSymbol(child));
                }
            }
        }
    }
}
