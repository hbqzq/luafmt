using Antlr4.Lua;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lfmt
{
    [CustomRuleFormatter(LuaParser.RULE_tableconstructor, RuleType.FORM_GENERAL)]
    public class TableConstructorFormatter : RuleFormatter
    {
        protected override void Format()
        {
            var hasChild = target.ChildCount > 2;
            ctx.WriteSymbol(new FormatSymbol(target.GetChild(0) as ITerminalNode), WriteSymbolFlags.PushIndent);
            if (hasChild)
            {
                Format(1, target.ChildCount - 2);
            }
            var lastChild = target.GetChild(target.ChildCount - 1) as ITerminalNode;
            ctx.WriteSymbol(new FormatSymbol(lastChild, !hasChild, FormatSymbol.TrimSpaceFollow(lastChild.Symbol.Text)), WriteSymbolFlags.PopIndentBefore);
        }
    }
}
