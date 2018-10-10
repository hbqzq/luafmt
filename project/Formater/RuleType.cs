using Antlr4.Lua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace lfmt
{
    public struct RuleType
    {
        public const int FORM_GENERAL = -1;

        public int ruleIndex { get; private set; } // RuleIndex
        public int ruleForm { get; private set; }

        public RuleType(int index, int form)
            : this()
        {
            ruleIndex = index;
            ruleForm = form;
        }
    }
}
