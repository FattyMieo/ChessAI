using System;
using System.Collections.Generic;
using Chess;

[Serializable]
public class ChessProfiles
{
    public ChessPieceProfile[] profiles;
    public Dictionary<ChessPieceType, ChessPieceProfile> dict = new Dictionary<ChessPieceType, ChessPieceProfile>();

    public void Init()
    {
        dict.Clear();

        for(int i = 0; i < profiles.Length; i++)
        {
            dict.Add(profiles[i].type, profiles[i]);
        }
    }
}
