using System;

namespace Chess
{
    /// <summary>
    /// A class to represent a set of coordinates in the chess board
    /// </summary>
	[Serializable]
    public class ChessCoordinate
	{
		public int x;
		public int y;

        /// <summary>
        /// Default constructor
        /// </summary>
		public ChessCoordinate()
		{
			this.x = 0;
			this.y = 0;
        }

        /// <summary>
        /// Constructor with specific coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ChessCoordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        
        /// <summary>
        /// Constructor by reference
        /// </summary>
        /// <param name="other"></param>
        public ChessCoordinate(ChessCoordinate other)
        {
            this.x = other.x;
            this.y = other.y;
        }

        //==================================================
        // Overrides
        //==================================================
        public override bool Equals(object o)
        {
            if (o is ChessCoordinate)
                return Equals((ChessCoordinate)o);
            else
                return base.Equals(o);
        }

        public bool Equals(ChessCoordinate c)
        {
            if (object.Equals(c, null)) return false;

            return this.x == c.x && this.y == c.y;
        }

        public static bool operator==(ChessCoordinate c1, ChessCoordinate c2)
        {
            if (object.ReferenceEquals(c1, c2))
            {
                return true;
            }

            if (object.Equals(c1, null)) return false;
            if (object.Equals(c2, null)) return false;

            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(ChessCoordinate c1, ChessCoordinate c2)
        {
            if (object.ReferenceEquals(c1, c2))
            {
                return false;
            }

            if (object.Equals(c1, null)) return true;
            if (object.Equals(c2, null)) return true;

            return c1.x != c2.x || c1.y != c2.y;
        }

        // Required as operator == and != is overriden
        public override int GetHashCode()
        {
            int hash = 17;
            // Suitable nullity checks etc, of course :)
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            return hash;
        }

        public static ChessCoordinate operator +(ChessCoordinate c1, ChessCoordinate c2)
        {
            return new ChessCoordinate(c1.x + c2.x, c1.y + c2.y);
        }

        public static ChessCoordinate operator -(ChessCoordinate c1, ChessCoordinate c2)
        {
            return new ChessCoordinate(c1.x - c2.x, c1.y - c2.y);
        }

        public static ChessCoordinate operator *(ChessCoordinate c1, ChessCoordinate c2)
        {
            return new ChessCoordinate(c1.x * c2.x, c1.y * c2.y);
        }

        public static ChessCoordinate operator /(ChessCoordinate c1, ChessCoordinate c2)
        {
            return new ChessCoordinate(c1.x / c2.x, c1.y / c2.y);
        }
    }

	public static class ChessCoordinateExtension
	{
        /// <summary>
        /// Convert array position into chess coordinate
        /// </summary>
        /// <param name="arrayPos"></param>
        /// <param name="boardSize">Specify the chess board size</param>
        /// <returns></returns>
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

        /// <summary>
        /// Convert chess coordinate into array position
        /// </summary>
        /// <param name="chessCoord"></param>
        /// <param name="boardSize">Specify the chess board size</param>
        /// <returns></returns>
        public static int ToArrayCoord(this ChessCoordinate chessCoord, int boardSize = 8)
		{
			return chessCoord.y * boardSize + chessCoord.x;
		}

        public static bool IsWithinRange(this ChessCoordinate chessCoord, int boardSize = ChessSettings.boardSize)
        {
            if (chessCoord.x < 0) return false;
            if (chessCoord.x >= boardSize) return false;
            if (chessCoord.y < 0) return false;
            if (chessCoord.y >= boardSize) return false;
            return true;
        }
	}
}
