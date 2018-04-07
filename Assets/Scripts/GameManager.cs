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
    public ChessProfiles pieceProfiles;
    public GameObject[] pieceMeshes;
    public Material[] pieceMat;
    public GameObject piecePrefab;
    public ChessBoardSnapshot defaultBoard;

    [Header("Arrays")]
    public Dictionary<ChessCoordinate, ChessPieceScript> piecesDict = new Dictionary<ChessCoordinate, ChessPieceScript>();
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

        pieceProfiles.Init();
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

        foreach (KeyValuePair<ChessCoordinate, ChessPieceScript> kvp in piecesDict)
            Destroy(kvp.Value.gameObject);

        piecesDict.Clear();

        for (int i = 0; i < board.Length; i++)
        {
            ChessPieceScript newPiece = Instantiate(piecePrefab, this.transform).GetComponent<ChessPieceScript>();

            if (!board[i].IsValid()) continue;
            if (board[i].IsEmpty()) continue;
            
            newPiece.Coord = i.ToChessCoord();
            newPiece.Type = board[i];

            piecesDict.Add(new ChessCoordinate(newPiece.Coord), newPiece);
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
    public void Move(ChessCoordinate from, ChessCoordinate to)
    {
        if (!from.IsWithinRange()) return;
        if (!to.IsWithinRange()) return;

        if (!piecesDict.ContainsKey(from))
        {
            Debug.LogWarning("Move piece failed.\nReason: (" + from.x + ", " + from.y + ") is an INVALID piece.");
            return;
        }

        ChessPieceScript selectedPiece = piecesDict[from];

        if (piecesDict.ContainsKey(to))
        {
            if (selectedPiece.Type.IsSameTeamAs(piecesDict[to].Type))
            {
                Debug.LogWarning("Move piece failed.\nReason: (" + to.x + ", " + to.y + ") is BLOCKED by allied piece.");
                return;
            }
        }

        if(!IsValidMove(selectedPiece.Type, from, to))
        {
            Debug.LogWarning("Move piece failed.\nReason: " + selectedPiece.Type + " (" + from.x + ", " + from.y + ") --> (" + to.x + ", " + to.y + ") is INVALID.");
            return;
        }

        selectedPiece.Coord = to;

        piecesDict.Remove(from);
        piecesDict.Remove(to);
        piecesDict.Add(to, selectedPiece);

        GenNextSnapshot
        (
            new ChessPosition(ChessPieceType.None, from),
            piecesDict[to].position
        );
    }

    public bool IsValidMove(ChessPieceType type, ChessCoordinate from, ChessCoordinate to)
    {
        ChessPieceMove[] possibleMoves = pieceProfiles.dict[type].possibleMoves;
        for (int i = 0; i < possibleMoves.Length; i++)
        {
            if(possibleMoves[i].isSpecialMove)
            {
                if (!IsValidSpecialMove(possibleMoves[i].move, type, from, to))
                    continue;
            }

            if(possibleMoves[i].isRepeatable)
            {
                ChessCoordinate temp = from + possibleMoves[i].move;

                while(temp.IsWithinRange())
                {
                    if (temp == to) return true;
                    temp += possibleMoves[i].move;
                }
            }
            else
            {
                if (from + possibleMoves[i].move == to) return true;
            }
        }

        return false;
    }

    public bool IsValidSpecialMove(ChessCoordinate move, ChessPieceType type, ChessCoordinate from, ChessCoordinate to)
    {
        // Pawn - Move 2 square when starting from initial position
        if (type == ChessPieceType.WhitePawn)
        {
            if(move.x == 0 && move.y == -2)
            {
                if (from.y == 6)
                    return true;
            }
        }
        else if (type == ChessPieceType.BlackPawn)
        {
            if (move.x == 0 && move.y == 2)
            {
                if (from.y == 1)
                    return true;
            }
        }

        return false;
    }
}
