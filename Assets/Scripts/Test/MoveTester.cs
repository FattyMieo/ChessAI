using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;

/// <summary>
/// A test script for applying moves on the chess board
/// </summary>
public class MoveTester : MonoBehaviour
{
    public bool pressToRunMove;
    public ChessCoordinate from;
    public ChessCoordinate to;

    void Start()
    {
        pressToRunMove = false;
    }

    void Update()
    {
        if(pressToRunMove)
        {
            pressToRunMove = false;
            RunMove();
        }
    }

    [ContextMenu("Run Move")]
	void RunMove ()
    {
        if(GameManager.Instance.Move(from, to))
        {
            ChessCoordinate newTo = to - from;
            from.x = to.x;
            from.y = to.y;
            to.x += newTo.x;
            to.y += newTo.y;
        }
    }
}
