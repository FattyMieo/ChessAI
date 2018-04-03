using System;
using UnityEngine;

namespace Chess
{
	[Serializable]
	public class ChessCoordinate
	{
		public int x;
		public int y;

		public ChessCoordinate()
		{
			this.x = 0;
			this.y = 0;
		}

		public ChessCoordinate(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	public static class ChessCoordinateExtension
	{
		public static ChessCoordinate ToChessCoord(this int arrayPos, int boardSize = 8)
		{
			int x = 0;
			int y = 0;
			int _arrayPos = arrayPos;

			while(_arrayPos >= boardSize)
			{
				_arrayPos -= 8;
				y++;
			}

			x = _arrayPos;

			return new ChessCoordinate(x, y);
		}
		
		public static int ToArrayCoord(this ChessCoordinate chessCoord, int boardSize = 8)
		{
			return chessCoord.y * boardSize + chessCoord.x;
		}
	}
}
