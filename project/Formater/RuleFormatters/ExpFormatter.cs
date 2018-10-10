using Antlr4.Lua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace lfmt
{
    [CustomRuleFormatter(LuaParser.RULE_exp)]
    public class ExpFormatter : RuleFormatter
    {
        protected override void Format()
        {
            if (parent.ruleType.ruleIndex == LuaParser.RULE_explist)
            {
                var typeName = target.GetType().Name;
                var match = Regex.Match(typeName, "Exp(?<form>\\d+)Context$");
                var form = int.Parse(match.Groups["form"].Value);

                if ((FormatSymbol.IsLineBreak(ctx.prevSymbol) || ctx.prevSymbol.line < (target as LuaParser.ExpContext).Start.Line) &&
                    (form != 8))
                {
                    ctx.PushIndent();
                    base.Format();
                    ctx.PopIndent();
                    return;
                }
            }
            base.Format();
        }
    }
}
