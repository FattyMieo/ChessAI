using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
	[Serializable]
	public class ChessSettings //Turn into scriptableObject/singleton later
    {
        public const int boardSize = 8;
        public const int totalPieces = 32;

        public ChessPieceProfile[] pieceProfiles;
	}
}