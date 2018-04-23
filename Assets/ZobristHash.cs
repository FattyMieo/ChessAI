using System;
using System.Collections;
using System.Collections.Generic;
using Chess;

public static class ZobristHash
{
    private static Random rand;
    private static UInt64[,] hashtable;
    
    public static UInt64 GetRandomHash(Random rand)
    {
        byte[] buf = new byte[8];
        rand.NextBytes(buf);
        UInt64 uint64_rand = BitConverter.ToUInt64(buf, 0);

        return uint64_rand;
    }

    static ZobristHash()
    {
        rand = new Random();
        hashtable = new UInt64[ChessSettings.boardSize * ChessSettings.boardSize, (int)ChessPieceTypeExtension.Total];

        for (int i = 0; i < ChessSettings.boardSize * ChessSettings.boardSize; i++)
        {
            for (int j = 0; j < (int)ChessPieceTypeExtension.Total; j++)
            {
                hashtable[i,j] = GetRandomHash(rand);
            }
        }
    }

    public static UInt64 ToZobristHash(this ChessPieceType[] board)
    {
        UInt64 h = 0;
        for (int i = 0; i < ChessSettings.boardSize * ChessSettings.boardSize; i++)
        {
            if (!board[i].IsValid())
                continue;
            if (board[i].IsEmpty())
                continue;

            int j = (int)board[i] - 1;

            h ^= hashtable[i, j];
        }

        return h;
    }
}
