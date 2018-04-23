using System;

namespace Chess
{
    /// <summary>
    /// Type of a chess piece
    /// </summary>
    [Serializable]
    public enum ChessPieceType
    {
        None = 0,
        WhitePawn,
        WhiteKnight,
        WhiteBishop,
        WhiteRook,
        WhiteQueen,
        WhiteKing,
        BlackPawn,
        BlackKnight,
        BlackBishop,
        BlackRook,
        BlackQueen,
        BlackKing,

        Total
    }
    
    /// <summary>
    /// Extension for ChessPieceType
    /// </summary>
    public static class ChessPieceTypeExtension
    {
        private static char[] iconArray =
        {
            ' ',
            '♙',
            '♘',
            '♗',
            '♖',
            '♕',
            '♔',
            '♟',
            '♞',
            '♝',
            '♜',
            '♛',
            '♚'
        };

        public const int Total = 12;

        public static char ToIcon(this ChessPieceType type)
        {
            return ToChessPieceIcon((int)type);
        }

        public static char ToChessPieceIcon(this int type)
        {
            if (type >= 0 && type < iconArray.Length)
                return iconArray[type];

            return ' ';
        }

        public static bool IsValid(this ChessPieceType type)
        {
            return type != ChessPieceType.Total;
        }

        public static bool IsEmpty(this ChessPieceType type)
        {
            return type == ChessPieceType.None;
        }

        public static bool IsWhite(this ChessPieceType type)
        {
            switch (type)
            {
                case ChessPieceType.WhitePawn:
                case ChessPieceType.WhiteKnight:
                case ChessPieceType.WhiteBishop:
                case ChessPieceType.WhiteRook:
                case ChessPieceType.WhiteQueen:
                case ChessPieceType.WhiteKing:
                    return true;
            }
            return false;
        }

        public static bool IsBlack(this ChessPieceType type)
        {
            switch (type)
            {
                case ChessPieceType.BlackPawn:
                case ChessPieceType.BlackKnight:
                case ChessPieceType.BlackBishop:
                case ChessPieceType.BlackRook:
                case ChessPieceType.BlackQueen:
                case ChessPieceType.BlackKing:
                    return true;
            }
            return false;
        }

        public static bool IsSameTeamAs(this ChessPieceType type1, ChessPieceType type2)
        {
            if (!type1.IsValid()) return false;
            if (!type2.IsValid()) return false;
            if (type1.IsEmpty()) return false;
            if (type2.IsEmpty()) return false;
            if (type1.IsWhite() && type2.IsWhite()) return true;
            if (type1.IsBlack() && type2.IsBlack()) return true;
            return false;
        }

        public static bool IsSameTeamAs(this ChessPieceType pieceType, ChessPlayerType playerType)
        {
            if (!pieceType.IsValid()) return false;
            if (pieceType.IsEmpty()) return false;
            if (pieceType.IsWhite() && playerType == ChessPlayerType.White) return true;
            if (pieceType.IsBlack() && playerType == ChessPlayerType.Black) return true;
            return false;
        }

        public static bool IsDifferentTeamAs(this ChessPieceType type1, ChessPieceType type2)
        {
            if (!type1.IsValid()) return false;
            if (!type2.IsValid()) return false;
            if (type1.IsEmpty()) return false;
            if (type2.IsEmpty()) return false;
            if (type1.IsWhite() && type2.IsBlack()) return true;
            if (type1.IsBlack() && type2.IsWhite()) return true;
            return false;
        }

        public static bool IsDifferentTeamAs(this ChessPieceType pieceType, ChessPlayerType playerType)
        {
            if (!pieceType.IsValid()) return false;
            if (pieceType.IsEmpty()) return false;
            if (pieceType.IsWhite() && playerType == ChessPlayerType.Black) return true;
            if (pieceType.IsBlack() && playerType == ChessPlayerType.White) return true;
            return false;
        }

        public static bool IsPawn(this ChessPieceType type)
        {
            switch (type)
            {
                case ChessPieceType.WhitePawn:
                case ChessPieceType.BlackPawn:
                    return true;
            }
            return false;
        }

        public static bool IsKnight(this ChessPieceType type)
        {
            switch (type)
            {
                case ChessPieceType.WhiteKnight:
                case ChessPieceType.BlackKnight:
                    return true;
            }
            return false;
        }

        public static bool IsBishop(this ChessPieceType type)
        {
            switch (type)
            {
                case ChessPieceType.WhiteBishop:
                case ChessPieceType.BlackBishop:
                    return true;
            }
            return false;
        }

        public static bool IsRook(this ChessPieceType type)
        {
            switch (type)
            {
                case ChessPieceType.WhiteRook:
                case ChessPieceType.BlackRook:
                    return true;
            }
            return false;
        }

        public static bool IsQueen(this ChessPieceType type)
        {
            switch (type)
            {
                case ChessPieceType.WhiteQueen:
                case ChessPieceType.BlackQueen:
                    return true;
            }
            return false;
        }

        public static bool IsKing(this ChessPieceType type)
        {
            switch (type)
            {
                case ChessPieceType.WhiteKing:
                case ChessPieceType.BlackKing:
                    return true;
            }
            return false;
        }
    }
}
