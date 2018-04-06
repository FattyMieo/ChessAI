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

        public ChessPosition()
        {
            this.type = ChessPieceType.None;
            this.coord = new ChessCoordinate(0, 0);
        }

        public ChessPosition(ChessPieceType type, ChessCoordinate coord)
        {
            this.type = type;
            this.coord = coord;
        }
    }
}
