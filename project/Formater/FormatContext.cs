using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace lfmt
{
    public enum WriteSymbolFlags
    {
        None = 0,
        PushIndent = 0x1,
        PopIndent = 0x2,
        PopIndentBefore = 0x4,
    }

    public class FormatContext
    {
        private static readonly string[] COMMENT_SEPARATOR = new string[] { 
            "\n", 
            "\r\n",
            "\n\r"
        };

        private static readonly char[] COMMENT_SUFFIX = new char[] {
            '\n',
            '\r'
        };

        private readonly IFormatWriter writer;
        private readonly IParseTree root;
        private readonly Queue<IToken> comments;
        private readonly Queue<IToken> spaces;
        private readonly Stack<IndentScope> scopes = new Stack<IndentScope>();

        private readonly string INDENT_UNIT;

        public int line { get; private set; }

        public FormatOptions options { get; private set; }
        public int validIndentLevel
        {
            get
            {
                if (scopes.Count > 0)
                {
                    var valid = scopes.FirstOrDefault(s => s.IsValid());
                    if (valid != null)
                    {
                        return valid.level;
                    }
                }
                return 0;
            }
        }
        public int realIndentLevel
        {
            get
            {
                if (scopes.Count > 0)
                {
                    return scopes.Peek().level;
                }
                return 0;
            }
        }
        public FormatSymbol prevSymbol { get; private set; }

        public FormatContext(IParseTree root, IEnumerable<IToken> comments, IEnumerable<IToken> spaces, IFormatWriter writer, FormatOptions options)
        {
            this.root = root;
            this.comments = new Queue<IToken>(comments);
            this.spaces = new Queue<IToken>(spaces);
            this.writer = writer;
            this.options = options;
            this.line = 1;

            var indentChar = options.useTabs ? "\t" : " ";
            var sb = new StringBuilder();
            for (int i = 0; i < options.indentCount; i++)
                sb.Append(indentChar);
            INDENT_UNIT = sb.ToString();
        }

        public void WriteSymbol(FormatSymbol symbol, WriteSymbolFlags flags = WriteSymbolFlags.None)
        {
            var text = symbol.text;

            var newline = false;
            var insertSpace = false;

            if (!symbol.comment)
            {
                WriteComments(symbol.tokenIndex);
            }

            if (options.keepWraps)
            {
                while (symbol.line > line)
                {
                    WriteLineBreak();
                }
            }

            var prev = prevSymbol;

            if ((flags & WriteSymbolFlags.PopIndentBefore) != WriteSymbolFlags.None)
            {
                PopIndent();
            }

            newline = FormatSymbol.IsLineBreak(prev);
            if (newline)
            {
                if (!symbol.noIndent)
                    WriteIndent();
                insertSpace = false;
            }
            else if ((FormatSymbol.TrimSpaceFollow(prev) || FormatSymbol.TrimSpaceAhead(symbol)) && !prev.forceSpaceFollow && !symbol.forceSpaceAhead)
            {
                insertSpace = false;
            }
            else
            {
                insertSpace = true;
            }

            if (insertSpace)
            {
                Write(FormatSymbol.SPACE);
            }

            var l = validIndentLevel;
            Write(symbol);

            if (!symbol.comment && scopes.Count > 0)
            {
                scopes.Peek().OnWriteSymbol();
            }

            if ((flags & WriteSymbolFlags.PushIndent) != WriteSymbolFlags.None)
            {
                PushIndent();
            }
            if ((flags & WriteSymbolFlags.PopIndentBefore) == WriteSymbolFlags.None && (flags & WriteSymbolFlags.PopIndent) != WriteSymbolFlags.None)
            {
                PopIndent();
            }

            line += symbol.numLine - 1;
        }

        public void WriteComments(int tokenIndex)
        {
            if (comments.Count == 0)
            {
                return;
            }

            var next = comments.Peek();
            if (next.TokenIndex > tokenIndex)
            {
                return;
            }

            comments.Dequeue();

            if (options.keepWraps)
            {
                while (next.Line > line)
                {
                    WriteLineBreak();
                }
            }

            var commentLines = next.Text.TrimEnd(COMMENT_SUFFIX).Split(COMMENT_SEPARATOR, StringSplitOptions.None);
            if (commentLines.Length == 1)
            {
                var symbol = new FormatSymbol(commentLines[0].Trim());
                if (!FormatSymbol.IsWhiteSpace(prevSymbol))
                {
                    symbol.ForceSpaceAhead();
                }
                symbol.SetComment();
                WriteSymbol(symbol);
            }
            else
            {
                if (options.formatComments)
                {
                    var symbol = new FormatSymbol(commentLines[0].Trim());
                    if (!FormatSymbol.IsWhiteSpace(prevSymbol))
                    {
                        symbol.ForceSpaceAhead();
                    }
                    symbol.SetComment();
                    WriteSymbol(symbol);
                    WriteLineBreak();

                    bool unindented = false;
                    PushIndent();

                    for (int i = 1, len = commentLines.Length; i < len; i++)
                    {
                        var item = commentLines[i].Trim();
                        if (i == len - 1)
                        {
                            if (Regex.Match(item, "^\\]=*\\]$").Success)
                            {
                                unindented = true;
                                PopIndent();
                            }
                        }
                        symbol = new FormatSymbol(item.Trim());
                        symbol.SetComment();
                        WriteSymbol(symbol);
                        if (i < len - 1)
                        {
                            WriteLineBreak();
                        }
                    }

                    if (!unindented)
                    {
                        PopIndent();
                    }
                }
                else
                {
                    if (FormatSymbol.IsLineBreak(prevSymbol) || prevSymbol.line < next.Line)
                    {
                        while (next.Line > line)
                        {
                            WriteLineBreak();
                        }

                        while (spaces.Count > 0)
                        {
                            var item = spaces.Peek();
                            if (item.Line < line)
                            {
                                spaces.Dequeue();
                                continue;
                            }
                            if (item.Line > line)
                            {
                                break;
                            }
                            if (item != null && item.Line == line)
                            {
                                spaces.Dequeue();
                                Write(item.Text);
                            }
                        }
                    }

                    for (int i = 0, len = commentLines.Length; i < len; i++)
                    {
                        var item = commentLines[i];
                        var symbol = new FormatSymbol(item);

                        symbol.SetComment();
                        symbol.SetNoIndent();

                        WriteSymbol(symbol);

                        if (i < len - 1)
                        {
                            WriteLineBreak();
                        }
                    }
                }
            }

            WriteComments(tokenIndex);
        }

        public void WriteIndent()
        {
            int level = validIndentLevel;
            for (int i = 0; i < level; i++)
            {
                Write(INDENT_UNIT);
            }
        }

        public void PushIndent(int delay = 0)
        {
            var scope = IndentScope.Get();
            scope.Enter(realIndentLevel + 1, delay);
            scopes.Push(scope);
        }

        public void PopIndent()
        {
            if (scopes.Count > 0)
            {
                IndentScope.Release(scopes.Pop());
            }
        }

        public void PopIndent(IndentScope scope)
        {
            if (scopes.Peek() == scope)
            {
                PopIndent();
            }
        }

        public void ResetIndent()
        {
            var scope = IndentScope.Get();
            scope.Enter(0, 0);
            scopes.Push(scope);
        }

        private void Write(FormatSymbol symbol)
        {
            if (symbol.line <= 0)
                symbol.SetLine(line);
            prevSymbol = symbol;
            writer.Append(symbol);
        }

        public void WriteLineBreak()
        {
            line++;
            Write(FormatSymbol.LINE_BREAK);
        }
    }
}
