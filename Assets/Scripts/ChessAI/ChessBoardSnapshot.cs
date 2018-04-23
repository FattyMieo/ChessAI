using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    /// <summary>
    /// A saved state of the chess board
    /// </summary>
	[Serializable]
    [CreateAssetMenu(fileName = "Board", menuName = "ChessAI/Board", order = 1)]
    public class ChessBoardSnapshot : ScriptableObject
	{
		public ChessPieceType[] board;
        public bool[] hasMoved;
    }

    public static class ChessBoardSnapshotExtention
    {
        public static Dictionary<int, ChessPosition> ToDictionary(this ChessBoardSnapshot boardSnapshot)
        {
            Dictionary<int, ChessPosition> ret = new Dictionary<int, ChessPosition>();

            if (boardSnapshot == null)
                return ret;

            ChessPieceType[] board = boardSnapshot.board;
            bool[] hasMoved = boardSnapshot.hasMoved;

            if (board.Length != ChessSettings.boardSize * ChessSettings.boardSize)
                return ret;

            for (int i = 0; i < board.Length; i++)
            {
                if (!board[i].IsValid()) continue;
                if (board[i].IsEmpty()) continue;

                ChessPosition newPos = new ChessPosition
                {
                    coord = i.ToChessCoord(),
                    type = board[i],
                    hasMoved = hasMoved[i]
                };

                ret.Add(i, newPos);
            }

            return ret;
        }

        public static List<ChessPosition> ToList(this ChessBoardSnapshot boardSnapshot)
        {
            List<ChessPosition> ret = new List<ChessPosition>();

            if (boardSnapshot == null)
                return ret;

            ChessPieceType[] board = boardSnapshot.board;
            bool[] hasMoved = boardSnapshot.hasMoved;

            if (board.Length != ChessSettings.boardSize * ChessSettings.boardSize)
                return ret;

            for (int i = 0; i < board.Length; i++)
            {
                if (!board[i].IsValid()) continue;
                if (board[i].IsEmpty()) continue;

                ChessPosition newPos = new ChessPosition
                {
                    coord = i.ToChessCoord(),
                    type = board[i],
                    hasMoved = hasMoved[i]
                };

                ret.Add(newPos);
            }

            return ret;
        }

        public static int ToHashCode(this ChessPieceType[] board)
        {
            if(board.Length != ChessSettings.boardSize * ChessSettings.boardSize)
                return 0;

            int ret = 0;

            for(int i = 0; i < board.Length; i++)
            {
                ret ^= (int)board[i];
            }

            return ret;
        }

        public static bool IsEndGame(this ChessBoardSnapshot boardSnapshot)
        {
            bool isOneKingAlive = false;

            ChessPieceType[] board = boardSnapshot.board;

            for (int i = 0; i < board.Length; i++)
            {
                if(board[i].IsKing())
                {
                    if(!isOneKingAlive)
                    {
                        isOneKingAlive = true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}