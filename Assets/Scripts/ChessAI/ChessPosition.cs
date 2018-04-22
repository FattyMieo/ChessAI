using System;

namespace Chess
{
    /// <summary>
    /// A class to represent a particular position in the chess board
    /// </summary>
    [Serializable]
    public class ChessPosition
    {
        public ChessPieceType type;
        public ChessCoordinate coord;
        public bool hasMoved;

        public ChessPosition(ChessPieceType type, ChessCoordinate coord, bool hasMoved = false)
        {
            this.type = type;
            this.coord = coord;
            this.hasMoved = hasMoved;
        }

        public ChessPosition() : this(ChessPieceType.None, new ChessCoordinate(0, 0), false) { }

        public ChessPosition(ChessPosition other) : this(other.type, other.coord, other.hasMoved) { }
    }
}
