using System;

namespace Chess
{
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
