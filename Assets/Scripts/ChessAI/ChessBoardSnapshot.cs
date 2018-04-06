using System;
using System.Collections.Generic;
using UnityEngine;
using Chess;

namespace Chess
{
    /// <summary>
    /// A saved state of the chess board
    /// </summary>
	[Serializable]
    [CreateAssetMenu(fileName = "Board", menuName = "ChessAI/Board", order = 1)]
    public class ChessBoardSnapshot : ScriptableObject
	{
		public ChessPieceType[] board;
	}
}