using System;
using UnityEngine;

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
        public bool[] hasMoved;
    }
}