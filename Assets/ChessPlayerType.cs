using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public enum ChessPlayerType
    {
        White = 0,
        Black = 1
    }

    public static class ChessPlayerTypeExtension
    {
        public static bool IsSameTeamAs(this ChessPlayerType playerType, ChessPieceType pieceType)
        {
            return pieceType.IsSameTeamAs(playerType);
        }

        public static bool IsDifferentTeamAs(this ChessPlayerType playerType, ChessPieceType pieceType)
        {
            return pieceType.IsDifferentTeamAs(playerType);
        }
    }
}
