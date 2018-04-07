using System;

namespace Chess
{
    [Serializable]
    public class ChessPieceMove
    {
        public ChessCoordinate move;
        public bool isRepeatable;
        public bool isSpecialMove;

        public ChessPieceMove()
        {
            this.move = new ChessCoordinate(0, 0);
            this.isRepeatable = false;
        }

        public ChessPieceMove(ChessCoordinate move, bool isRepeating = false)
        {
            this.move = move;
            this.isRepeatable = isRepeating;
        }
    }
}
