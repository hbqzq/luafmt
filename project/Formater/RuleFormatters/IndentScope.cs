using System.Collections.Generic;

namespace lfmt
{
    public class IndentScope
    {
        private static readonly Stack<IndentScope> pool = new Stack<IndentScope>();

        public static IndentScope Get()
        {
            return pool.Count > 0 ? pool.Pop() : new IndentScope();
        }

        public static void Release(IndentScope scope)
        {
            pool.Push(scope);
        }

        public int level { get; private set; }
        private int start; 

        public void Enter(int level, int start)
        {
            this.level = level;
            this.start = start;
        }

        public void OnWriteSymbol()
        {
            start--;
            if (start < 0)
                start = 0;
        }

        public bool IsValid()
        {
            return start == 0;
        }
    }
}
