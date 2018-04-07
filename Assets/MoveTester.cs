using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;

public class MoveTester : MonoBehaviour
{
    public bool pressToRunMove;
    public ChessCoordinate from;
    public ChessCoordinate to;

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
        GameManager.Instance.Move(from, to);
	}
}
