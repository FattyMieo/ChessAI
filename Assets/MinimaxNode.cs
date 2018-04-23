using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimaxNode
{
    public ulong parentHash;
    public List<ulong> childrenHash = new List<ulong>();

    public ulong hash;
    public int score;

    public MinimaxNode(ulong hash, int score)
    {
        this.hash = hash;
        this.score = score;
    }
}
