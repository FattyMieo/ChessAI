using UnityEngine;

namespace Chess
{
    /// <summary>
    /// A class that stores information about a chess piece
    /// </summary>
	[CreateAssetMenu(fileName = "PieceProfile", menuName = "ChessAI/Piece Profile", order = 2)]
	public class ChessPieceProfile : ScriptableObject
	{
		public ChessPieceType type;
		public int score;
        public ChessPieceMove[] possibleMoves;
	}
}
