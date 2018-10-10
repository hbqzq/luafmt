using Antlr4.Lua;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lfmt
{
    [CustomRuleFormatter(LuaParser.RULE_args, RuleType.FORM_GENERAL)]
    public class ArgsFormatter : RuleFormatter
    {
        protected override void Format()
        {
            var firstChild = target.GetChild(0);

            if (firstChild is LuaParser.TableconstructorContext ||  firstChild is LuaParser.StringContext)
            {
                base.Format();
                return;
            }

            var firstAsTerminal = firstChild as ITerminalNode;
            if (firstAsTerminal != null)
            {
                if (firstAsTerminal.Symbol.Text == "(")
                {
                    var symbol = new FormatSymbol(firstAsTerminal, true, true); // remove space before '('
                    ctx.WriteSymbol(symbol);
                    Format(1);
                    return;
                }
            }

            base.Format();
        }
    }
}
