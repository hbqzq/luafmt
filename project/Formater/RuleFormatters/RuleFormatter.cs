using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
namespace lfmt
{
    public class RuleFormatter
    {
        static Dictionary<RuleType, Stack<RuleFormatter>> dict = new Dictionary<RuleType, Stack<RuleFormatter>>();
        static Dictionary<RuleType, Type> typeDict;

        public static void Format(IRuleNode node, FormatContext ctx, int form = RuleType.FORM_GENERAL, RuleFormatter parent = null)
        {
            RuleType nodeRuleType = new RuleType(node.RuleContext.RuleIndex, form);

            RuleFormatter formatter = null;

            Stack<RuleFormatter> stack = null;
            if (!dict.TryGetValue(nodeRuleType, out stack))
                dict.Add(nodeRuleType, stack = new Stack<RuleFormatter>());

            if (stack.Count > 0)
            {
                formatter = stack.Pop();
            }
            else
            {
                Type formatterType = null;

                if (typeDict == null)
                {
                    typeDict = new Dictionary<RuleType, Type>();

                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var assembly in assemblies)
                    {
                        var types = assembly.GetTypes();
                        foreach (var t in types)
                        {
                            if (t.IsSubclassOf(typeof(RuleFormatter)))
                            {
                                var attributes = t.GetCustomAttributes(typeof(CustomRuleFormatterAttribute), false);
                                foreach (var attr in attributes)
                                {
                                    var ruleFormatterAttr = attr as CustomRuleFormatterAttribute;
                                    typeDict[new RuleType(ruleFormatterAttr.ruleIndex, ruleFormatterAttr.ruleForm)] = t;
                                }
                            }
                        }
                    }
                }

                if (!typeDict.TryGetValue(nodeRuleType, out formatterType))
                {
                    //foreach (var pair in typeDict)
                    //{
                    //    if (pair.Key.Test(nodeRuleType))
                    //    {
                    //        formatterType = pair.Value;
                    //        break;
                    //    }
                    //}
                    //typeDict.TryGetValue(new RuleType(nodeRuleType.ruleIndex, RuleType.FORM_GENERAL), out formatterType);

                    if (formatterType == null)
                    {
                        formatterType = typeof(RuleFormatter);
                    }

                    typeDict.Add(nodeRuleType, formatterType);
                }

                formatter = Activator.CreateInstance(formatterType) as RuleFormatter;
            }

            formatter.target = node;
            formatter.ctx = ctx;
            formatter.parent = parent;
            formatter.ruleType = nodeRuleType;

            formatter.Format();
            formatter.Reset();

            stack.Push(formatter);
        }

        public IRuleNode target { get; private set; }
        public FormatContext ctx { get; private set; }
        public FormatOptions options { get { return ctx.options; } }
        public RuleType ruleType { get; private set; }
        public RuleFormatter parent { get; private set; }

        protected virtual void Format()
        {
            Format(0, -1);
        }

        protected void Format(int start, int count = -1)
        {
            var childCount = target.ChildCount;

            if (start < 0)
                start = 0;
            else if (start >= childCount)
                start = childCount - 1;

            if (count < 0)
                count = childCount - start;
            else if (count > childCount - start)
                count = childCount - start;

            var ruleCtx = target.RuleContext;
            for (int i = 0; i < count; i++)
            {
                FormatNode(ruleCtx.GetChild(i + start));
            }
        }

        protected virtual void FormatNode(IParseTree node, int form = RuleType.FORM_GENERAL)
        {
            var ruleNode = node as IRuleNode;
            if (ruleNode != null)
            {
                Format(ruleNode, ctx, form, this);
                return;
            }

            var terminalNode = node as ITerminalNode;
            if (terminalNode != null)
            {
                ctx.WriteSymbol(new FormatSymbol(terminalNode));
            }
        }

        protected virtual void Reset()
        {
            ctx = null;
            target = null;
            parent = null;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CustomRuleFormatterAttribute : Attribute
    {
        public readonly int ruleIndex;
        public readonly int ruleForm;
        public CustomRuleFormatterAttribute(int ruleIndex, int ruleForm = RuleType.FORM_GENERAL)
        {
            this.ruleIndex = ruleIndex;
            this.ruleForm = ruleForm;
        }
    }
}
