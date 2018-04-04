using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;

public class TestBoardSnapshot : MonoBehaviour
{
	public int step = 0;
	public List<ChessBoardSnapshot> board = new List<ChessBoardSnapshot>();

	[ContextMenu("Generate New Board")]
	void GenNew()
	{
		ChessBoardSnapshot newBoard = ScriptableObject.CreateInstance<ChessBoardSnapshot>();
		newBoard.name = "New Board #" + step.ToString("0000");
		board.Add(newBoard);
		step++;
	}
}
