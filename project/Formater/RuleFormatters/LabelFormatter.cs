using Antlr4.Lua;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lfmt
{
    [CustomRuleFormatter(LuaParser.RULE_label, RuleType.FORM_GENERAL)]
    public class LabelFormatter : RuleFormatter
    {
        protected override void Format()
        {
            ctx.ResetIndent();
            ctx.WriteSymbol(new FormatSymbol(target.GetChild(0) as ITerminalNode, true, true));

            ctx.PushIndent();
            for (int i = 1, len = target.ChildCount; i < len - 1; i++)
            {
                var child = target.GetChild(i);
                FormatNode(child);
            }
            ctx.PopIndent();
            ctx.WriteSymbol(new FormatSymbol(target.GetChild(target.ChildCount - 1) as ITerminalNode, true, true));
            ctx.PopIndent();
        }
    }
}
