using System.Collections.Generic;
using UnityEngine;
using Chess;

namespace Chess
{
	public class ChessBoardSnapshot : ScriptableObject
	{
		public ChessPieceType[] board = new ChessPieceType[ChessSettings.boardSize];
	}
}