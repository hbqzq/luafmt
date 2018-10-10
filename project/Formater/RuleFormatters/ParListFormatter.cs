using Antlr4.Lua;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lfmt
{
    [CustomRuleFormatter(LuaParser.RULE_parlist)]
    public class ParListFormatter : RuleFormatter
    {
        protected override void Format()
        {
            for (int i = 0, len = target.ChildCount; i < len; i++)
            {
                var child = target.GetChild(i);
                if (child.EqualTo("...") && (FormatSymbol.IsLineBreak(ctx.prevSymbol) || ctx.prevSymbol.line < (child as ITerminalNode).Symbol.Line))
                {
                    ctx.PushIndent();
                    ctx.WriteSymbol(new FormatSymbol(child as ITerminalNode));
                    ctx.PopIndent();
                }
                else
                {
                    FormatNode(child);
                }
            }
        }
    }
}
