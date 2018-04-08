using System;

namespace Chess
{
    /// <summary>
    /// A class that stores moving information of a chess piece
    /// </summary>
    [Serializable]
    public class ChessPieceMove
    {
        public ChessCoordinate move;
        public int repeatTimes = 1;
        public ChessPieceMovePattern pattern;
        public ChessPieceSpecialRule specialRule;
    }

    /// <summary>
    /// Move patterns of a chess piece
    /// </summary>
    public enum ChessPieceMovePattern
    {
        Normal = 0,
        MoveOnly,
        CaptureOnly
    }

    /// <summary>
    /// Special rules for moving a chess piece
    /// </summary>
    public enum ChessPieceSpecialRule
    {
        None,
        Pawn2Squares,
        CastlingLeft,
        CastlingRight,
    }
}
