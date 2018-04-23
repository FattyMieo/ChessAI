using System.Collections;
using System.Collections.Generic;
using Chess;

public class MinimaxNode
{
    public ulong hash;
    public int depth;
    public MinimaxNodeFlag flag;
    public int eval;
}

public enum MinimaxNodeFlag
{
    LowerBound = -1,
    Exact = 0,
    UpperBound = 1
}
