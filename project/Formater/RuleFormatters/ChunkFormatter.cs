using Antlr4.Lua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lfmt
{
    [CustomRuleFormatter(LuaParser.RULE_chunk)]
    public class ChunkFormatter : RuleFormatter
    {
        protected override void Format()
        {
            FormatNode(target.GetChild(0));
        }
    }
}
