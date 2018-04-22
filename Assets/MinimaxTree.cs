using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;

[Serializable]
public class MinimaxTree
{
    public List<MinimaxNode> nodes;

    public MinimaxNode this[int index]
    {
        get
        {
            return nodes[index];
        }
        set
        {
            nodes[index] = value;
        }
    }

    public int Count { get { return nodes.Count; } }

    public void Add(MinimaxNode item)
    {
        nodes.Add(item);
    }

    public void Clear()
    {
        nodes.Clear();
    }

    public bool Contains(MinimaxNode item)
    {
        return nodes.Contains(item);
    }

    public int IndexOf(MinimaxNode item)
    {
        return nodes.IndexOf(item);
    }

    public void Insert(int index, MinimaxNode item)
    {
        nodes.Insert(index, item);
    }

    public bool Remove(MinimaxNode item)
    {
        return nodes.Remove(item);
    }

    public void RemoveAt(int index)
    {
        nodes.RemoveAt(index);
    }
}

public static class MinimaxTreeExtention
{
    public static MinimaxNode[] ToArray(this MinimaxTree tree)
    {
        return tree.nodes.ToArray();
    }
}
