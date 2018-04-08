using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;

/// <summary>
/// Main Game Manager
/// Contain all functions to play the chess
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

            return _instance;
        }
    }

    [Header("Settings")]
    public ChessSettings settings;
    public ChessPieceProfile[] profiles;
    private ChessPieceProfileDictionary profilesDict = new ChessPieceProfileDictionary();
    public GameObject[] pieceMeshes;
    public Material[] pieceMat;
    public GameObject piecePrefab;
    public ChessBoardSnapshot defaultBoard;

    [Header("Arrays")]
    public Dictionary<int, ChessPieceScript> piecesDict = new Dictionary<int, ChessPieceScript>();
    public List<ChessBoardSnapshot> snapshots;

    /// <summary>
    /// Return the latest snapshot in the list
    /// </summary>
    public ChessBoardSnapshot LatestSnapshot
    {
        get
        {
            if (snapshots.Count <= 0) return null;
            return snapshots[snapshots.Count - 1];
        }
    }
    
    // Use this for initialization
    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this.gameObject);

        profilesDict.Init(profiles);
    }

    void Start()
    {
        GenNextSnapshot(defaultBoard);
        LoadFromSnapshot(LatestSnapshot);
    }

    /// <summary>
    /// Use a board snapshot to create the board
    /// </summary>
    /// <param name="boardSnapshot"></param>
    void LoadFromSnapshot(ChessBoardSnapshot boardSnapshot)
    {
        if (boardSnapshot == null) return;

        ChessPieceType[] board = boardSnapshot.board;
        if (board.Length != ChessSettings.boardSize * ChessSettings.boardSize) return;

        foreach (KeyValuePair<int, ChessPieceScript> kvp in piecesDict)
            Destroy(kvp.Value.gameObject);

        piecesDict.Clear();

        for (int i = 0; i < board.Length; i++)
        {
            ChessPieceScript newPiece = Instantiate(piecePrefab, this.transform).GetComponent<ChessPieceScript>();

            if (!board[i].IsValid()) continue;
            if (board[i].IsEmpty()) continue;
            
            newPiece.Coord = i.ToChessCoord();
            newPiece.Type = board[i];

            piecesDict.Add(newPiece.Coord.ToArrayCoord(), newPiece);
        }
    }

    /// <summary>
    /// Adjust the board snapshot and return it back
    /// </summary>
    /// <param name="boardSnapshot"></param>
    /// <param name="newName"></param>
    /// <param name="changed">Changes to the positions</param>
    /// <returns></returns>
    ChessBoardSnapshot AdjustBoard(ChessBoardSnapshot boardSnapshot, string newName = "Board", params ChessPosition[] changed)
    {
        //ChessBoardSnapshot newBoard = ScriptableObject.CreateInstance<ChessBoardSnapshot>();
        ChessBoardSnapshot newBoard = ScriptableObject.Instantiate<ChessBoardSnapshot>(boardSnapshot);
        newBoard.name = newName;

        for(int i = 0; i < changed.Length; i++)
        {
            int aCoord = changed[i].coord.ToArrayCoord();
            newBoard.board[aCoord] = changed[i].type;
        }

        return newBoard;
    }

    /// <summary>
    /// Generate the next board snapshot
    /// </summary>
    /// <param name="boardSnapshot"></param>
    /// <param name="changed">Changes to the positions</param>
    /// <returns></returns>
    ChessBoardSnapshot GenNextSnapshot(ChessBoardSnapshot boardSnapshot, params ChessPosition[] changed)
    {
        ChessBoardSnapshot newBoard = AdjustBoard(boardSnapshot, "Board #" + snapshots.Count.ToString("0000"), changed);
        snapshots.Add(newBoard);
        return newBoard;
    }

    /// <summary>
    /// Generate the next board snapshot
    /// </summary>
    /// <param name="changed">Changes to the positions</param>
    /// <returns></returns>
    ChessBoardSnapshot GenNextSnapshot(params ChessPosition[] changed)
    {
        return GenNextSnapshot(LatestSnapshot, changed);
    }

    /// <summary>
    /// Make a move from the latest snapshot
    /// </summary>
    /// <param name="from">Current position's coordinate</param>
    /// <param name="to">Destination position's coordinate</param>
    public bool Move(ChessCoordinate from, ChessCoordinate to)
    {
        if (!from.IsWithinRange())
        {
            Debug.LogWarning("Move piece failed.\nReason: (" + from.x + ", " + from.y + ") is an INVALID coordinate.");
            return false;
        }

        if (!to.IsWithinRange())
        {
            Debug.LogWarning("Move piece failed.\nReason: (" + to.x + ", " + to.y + ") is an INVALID coordinate.");
            return false;
        }

        if (!piecesDict.ContainsKey(from.ToArrayCoord()))
        {
            Debug.LogWarning("Move piece failed.\nReason: (" + from.x + ", " + from.y + ") is EMPTY.");
            return false;
        }

        ChessPieceScript selectedPiece = piecesDict[from.ToArrayCoord()];

        if(!IsValidMove(selectedPiece.Type, from, to))
        {
            Debug.LogWarning("Move piece failed.\nReason: " + selectedPiece.Type + " (" + from.x + ", " + from.y + ") --> (" + to.x + ", " + to.y + ") is INVALID.");
            return false;
        }

        selectedPiece.Coord = to;

        if (piecesDict.ContainsKey(to.ToArrayCoord()))
            Destroy(piecesDict[to.ToArrayCoord()].gameObject);

        piecesDict.Remove(from.ToArrayCoord());
        piecesDict.Remove(to.ToArrayCoord());
        piecesDict.Add(to.ToArrayCoord(), selectedPiece);

        GenNextSnapshot
        (
            new ChessPosition(ChessPieceType.None, from),
            new ChessPosition(selectedPiece.Type, to)
        );

        return true;
    }

    /// <summary>
    /// Check whether it's a valid move for the specified piece
    /// </summary>
    /// <param name="type">The specified piece's type</param>
    /// <param name="from">The specified piece's coordinate</param>
    /// <param name="to">The destination's coordinate</param>
    /// <returns></returns>
    public bool IsValidMove(ChessPieceType type, ChessCoordinate from, ChessCoordinate to)
    {
        ChessPieceMove[] possibleMoves = profilesDict[type].possibleMoves;
        for (int i = 0; i < possibleMoves.Length; i++)
        {
            if (!IsValidSpecialRule(possibleMoves[i].specialRule, type, from, to))
                continue;

            ChessCoordinate temp = from + possibleMoves[i].move;
            int j = 0;

            while
            (
                temp.IsWithinRange() &&
                (possibleMoves[i].repeatTimes < 0 || j < possibleMoves[i].repeatTimes)
            )
            {
                if (temp == to)
                {
                    if (possibleMoves[i].pattern == ChessPieceMovePattern.Normal)
                    {
                        return true;
                    }
                    if (possibleMoves[i].pattern == ChessPieceMovePattern.MoveOnly)
                    {
                        if (!piecesDict.ContainsKey(temp.ToArrayCoord()))
                            return true;
                    }
                    else if (possibleMoves[i].pattern == ChessPieceMovePattern.CaptureOnly)
                    {
                        if (!piecesDict.ContainsKey(temp.ToArrayCoord()))
                            break;

                        if (piecesDict[temp.ToArrayCoord()].Type.IsDifferentTeamAs(type))
                            return true;
                    }
                }
                else
                {
                    if (piecesDict.ContainsKey(temp.ToArrayCoord()))
                        break;
                }

                temp += possibleMoves[i].move;
                j++;
            }
        }

        return false;
    }

    /// <summary>
    /// Check whether the special rule is matching for the specific piece
    /// </summary>
    /// <param name="specialRule">The special rule used</param>
    /// <param name="type">The specified piece's type</param>
    /// <param name="from">The specified piece's coordinate</param>
    /// <param name="to">The destination's coordinate</param>
    /// <returns></returns>
    public bool IsValidSpecialRule(ChessPieceSpecialRule specialRule, ChessPieceType type, ChessCoordinate from, ChessCoordinate to)
    {
        if (specialRule == ChessPieceSpecialRule.None)
            return true;

        // Pawn - Move 2 square when starting from initial position
        if (specialRule == ChessPieceSpecialRule.Pawn2Squares)
        {
            if
            (
                (type.IsWhite() && from.y == 6) ||
                (type.IsBlack() && from.y == 1)
            )
                return true;
        }

        return false;
    }
}
