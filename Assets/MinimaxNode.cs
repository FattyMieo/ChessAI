using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;

[Serializable]
public class MinimaxNode
{
    public ChessPlayerType playerTurn;
    public List<ChessBoardSnapshot> outcomes;

    public MinimaxNode(ChessPlayerType playerTurn)
    {
        outcomes = new List<ChessBoardSnapshot>();
    }

    public MinimaxNode(ChessPlayerType playerTurn, List<ChessBoardSnapshot> list)
    {
        this.playerTurn = playerTurn;
        outcomes = list;
    }
}
