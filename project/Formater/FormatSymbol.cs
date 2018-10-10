using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lfmt
{
    public struct FormatSymbol
    {
        public const string SPACE = " ";
        public const string LINE_BREAK = "\n";

        static readonly HashSet<string> TRIM_SPACE_AHEAD_STRINGS = new HashSet<string>(new string[] { ".", ":", ",", ";", ")", "]", null, "\n", "[" });
        static readonly HashSet<string> TRIM_SPACE_FOLLOW_STRINGS = new HashSet<string>(new string[] { ".", ":", "(", "[", "#", "~", null, "\n" });

        static readonly HashSet<string> BRACKETS = new HashSet<string>(new string[] { "[", "{" });
        static readonly HashSet<string> REVERSE_BRACKETS = new HashSet<string>(new string[] { "]", "}" });

        public static bool IsWhiteSpace(string s)
        {
            return s == null || s.All(c => c == '\t' || c == ' ');
        }

        public static bool TrimSpaceAhead(ITerminalNode t)
        {
            return t == null || TrimSpaceAhead(t.Symbol.Text);
        }

        public static bool TrimSpaceAhead(string s)
        {
            return IsWhiteSpace(s) || TRIM_SPACE_AHEAD_STRINGS.Contains(s);
        }

        public static bool TrimSpaceAhead(FormatSymbol s)
        {
            return IsWhiteSpace(s) || s.trimSpaceAhead;
        }

        public static bool TrimSpaceFollow(ITerminalNode t)
        {
            return t == null || TrimSpaceFollow(t.Symbol.Text);
        }

        public static bool TrimSpaceFollow(string s)
        {
            return IsWhiteSpace(s) || TRIM_SPACE_FOLLOW_STRINGS.Contains(s);
        }

        public static bool TrimSpaceFollow(FormatSymbol s)
        {
            return IsWhiteSpace(s) || s.trimSpaceFollow;
        }

        public static bool IsLineBreak(FormatSymbol s)
        {
            return s == "\n";
        }

        public static bool IsSameLine(FormatSymbol s1, FormatSymbol s2)
        {
            return s1.line == s2.line;
        }

        public string text { get; private set; }
        public bool trimSpaceAhead { get; private set; }
        public bool trimSpaceFollow { get; private set; }
        public int line { get; private set; }
        public int column { get; private set; }
        public int tokenIndex { get; private set; }

        public bool forceSpaceAhead { get; private set; }
        public bool forceSpaceFollow { get; private set; }

        public bool comment { get; private set; }

        public bool noIndent { get; private set; }

        public int numLine
        {
            get
            {
                return text.Count(c => c == '\n') + 1;
            }
        }

        public FormatSymbol(string body)
            : this(body, 0, 0)
        {
        }

        public FormatSymbol(string body, int line, int column)
            : this(body, line, column, TrimSpaceAhead(body), TrimSpaceFollow(body), 0)
        {
        }

        public FormatSymbol(ITerminalNode node)
            : this(node, TrimSpaceAhead(node), TrimSpaceFollow(node))
        {
        }

        public FormatSymbol(ITerminalNode node, bool trimSpaceAhead, bool trimSpaceFollow)
            : this(node.Symbol.Text, node.Symbol.Line, node.Symbol.Column, trimSpaceAhead, trimSpaceFollow, node.Symbol.TokenIndex)
        {
        }

        public FormatSymbol(string text,
            int line, int column, bool trimSpaceAhead, bool trimSpaceFollow, int tokenIndex)
            : this()
        {
            this.text = text;
            this.trimSpaceAhead = trimSpaceAhead;
            this.trimSpaceFollow = trimSpaceFollow;
            this.line = line;
            this.column = column;
            this.tokenIndex = tokenIndex;
        }

        public FormatSymbol ForceSpaceAhead()
        {
            forceSpaceAhead = true;
            trimSpaceAhead = false;
            return this;
        }

        public FormatSymbol ForceSpaceFollow()
        {
            forceSpaceFollow = true;
            trimSpaceFollow = false;
            return this;
        }

        public void SetComment()
        {
            comment = true;
        }

        public void SetNoIndent()
        {
            noIndent = true;
        }

        public void SetLine(int line)
        {
            this.line = line;
        }

        public static implicit operator FormatSymbol(string s)
        {
            return new FormatSymbol(s);
        }

        public static implicit operator string(FormatSymbol s)
        {
            return s.text;
        }
    }
}
