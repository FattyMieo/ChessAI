using UnityEngine;

namespace Chess
{
    /// <summary>
    /// A profile object for creating scores for each piece type
    /// </summary>
	[CreateAssetMenu(fileName = "PieceProfile", menuName = "ChessAI/Piece Profile", order = 2)]
	public class ChessPieceProfile : ScriptableObject
	{
		public ChessPieceType type;
		public int score;
        public ChessPieceMove[] possibleMoves;
	}
}
