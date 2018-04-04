using System.Collections.Generic;
using UnityEngine;
using Chess;

namespace Chess
{
    [CreateAssetMenu(fileName = "Board", menuName = "ChessAI/Board", order = 1)]
    public class ChessBoardSnapshot : ScriptableObject
	{
		public ChessPieceType[] board = new ChessPieceType[ChessSettings.boardSize * ChessSettings.boardSize];
	}
}