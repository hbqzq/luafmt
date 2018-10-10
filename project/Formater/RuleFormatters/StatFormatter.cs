using Antlr4.Lua;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace lfmt
{
    [CustomRuleFormatter(LuaParser.RULE_stat, RuleType.FORM_GENERAL)]
    public class StatFormatter : RuleFormatter
    {
        protected override void Format()
        {
            var typeName = target.GetType().Name;
            var match = Regex.Match(typeName, "Stat(?<form>\\d+)Context$");
            var form = int.Parse(match.Groups["form"].Value);

            FormatNode(target, form);
        }

        // 0) ;
        // 2) functioncall
        [CustomRuleFormatter(LuaParser.RULE_stat, 2)]
        // 5) 'goto' NAME
        [CustomRuleFormatter(LuaParser.RULE_stat, 5)]
        public class CommonStatFormatter : RuleFormatter
        {
            protected override void Format()
            {
                base.Format();
            }
        }
        // 1) varlist '=' explist
        [CustomRuleFormatter(LuaParser.RULE_stat, 1)]
        // 14) 'local' namelist ('=' explist)?
        [CustomRuleFormatter(LuaParser.RULE_stat, 14)]
        public class AssignmentStatFormatter : RuleFormatter
        {
            protected override void Format()
            {
                base.Format();
            }
        }
        // 3) label
        [CustomRuleFormatter(LuaParser.RULE_stat, 3)]
        public class LabelStatFormatter : RuleFormatter
        {
            protected override void Format()
            {
                ctx.ResetIndent();
                base.Format();
                ctx.PopIndent();
            }
        }
        // 4) 'break'
        // 6) 'do' block 'end'
        [CustomRuleFormatter(LuaParser.RULE_stat, 6)]
        // 7) 'while' exp 'do' block 'end'
        [CustomRuleFormatter(LuaParser.RULE_stat, 7)]
        // 10) 'for' NAME '=' exp ',' exp (',' exp)? 'do' block 'end'
        [CustomRuleFormatter(LuaParser.RULE_stat, 10)]
        // 11) 'for' namelist 'in' explist 'do' block 'end'
        [CustomRuleFormatter(LuaParser.RULE_stat, 11)]
        public class EndTerminatedStatFormatter : RuleFormatter
        {
            protected override void Format()
            {
                for (int i = 0, len = target.ChildCount - 1; i < len; i++)
                {
                    var child = target.GetChild(i);
                    if (child.EqualTo("do"))
                    {
                        ctx.WriteSymbol(new FormatSymbol(child as ITerminalNode), WriteSymbolFlags.PushIndent);
                    }
                    else
                    {
                        FormatNode(child);
                    }
                }
                ctx.WriteSymbol(new FormatSymbol(target.GetChild(target.ChildCount - 1) as ITerminalNode), WriteSymbolFlags.PopIndentBefore);
            }
        }
        // 8) 'repeat' block 'until' exp
        [CustomRuleFormatter(LuaParser.RULE_stat, 8)]
        public class RepeatStatFormatter : RuleFormatter
        {
            protected override void Format()
            {
                FormatNode(target.GetChild(0));

                ctx.PushIndent();
                for (int i = 1, len = target.ChildCount; i < len; i++)
                {
                    var child = target.GetChild(i);
                    if (child.EqualTo("until"))
                    {
                        ctx.WriteSymbol(new FormatSymbol(child as ITerminalNode), WriteSymbolFlags.PopIndentBefore | WriteSymbolFlags.PushIndent);
                    }
                    else
                    {
                        FormatNode(child);
                    }
                }
                ctx.PopIndent();
            }
        }
        // 9) 'if' exp 'then' block ('elseif' exp 'then' block)* ('else' block)? 'end'
        [CustomRuleFormatter(LuaParser.RULE_stat, 9)]
        public class IfStatFormatter : RuleFormatter
        {
            protected override void Format()
            {
                FormatNode(target.GetChild(0));

                ctx.PushIndent();
                for (int i = 1, len = target.ChildCount; i < len - 1; i++)
                {
                    var child = target.GetChild(i);
                    if (child.EqualTo("elseif") || child.EqualTo("else"))
                    {
                        ctx.WriteSymbol(new FormatSymbol(child as ITerminalNode), WriteSymbolFlags.PopIndentBefore | WriteSymbolFlags.PushIndent);
                    }
                    else
                    {
                        FormatNode(child);
                    }
                }
                ctx.WriteSymbol(new FormatSymbol(target.GetChild(target.ChildCount - 1) as ITerminalNode), WriteSymbolFlags.PopIndentBefore);
            }
        }
        // 12) 'function' funcname funcbody
        [CustomRuleFormatter(LuaParser.RULE_stat, 12)]
        // 13) 'local' 'function' NAME funcbody
        [CustomRuleFormatter(LuaParser.RULE_stat, 13)]
        public class FunctionStatFormatter : RuleFormatter { }
    }
}
