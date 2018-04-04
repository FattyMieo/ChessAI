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
            if (type < (int)ChessPieceType.Total)
                return iconArray[type];

            return ' ';
        }
    }
}
