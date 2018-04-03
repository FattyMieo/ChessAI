using UnityEngine;

namespace Chess
{
	[CreateAssetMenu(fileName = "PieceProfile", menuName = "ChessAI/Piece Profile", order = 1)]
	public class ChessPieceProfile : ScriptableObject
	{
		public ChessPieceType type;
		public int score;
	}
}
