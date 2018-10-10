using Antlr4.Lua;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lfmt
{
    [CustomRuleFormatter(LuaParser.RULE_funcbody, RuleType.FORM_GENERAL)]
    public class FunctionBodyFormatter : RuleFormatter
    {
        protected override void Format()
        {
            ctx.PushIndent(1);
            base.Format(0, target.ChildCount - 1);
            ctx.WriteSymbol(new FormatSymbol(target.GetChild(target.ChildCount - 1) as ITerminalNode), WriteSymbolFlags.PopIndentBefore);
        }
    }
}
