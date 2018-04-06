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

    // Use this for initialization
    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this.gameObject);
    }

    [Header("Settings")]
    public ChessSettings settings;
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

    void Start()
    {
        //GenNewPieces();
        GenNextSnapshot(defaultBoard);
        LoadFromSnapshot(LatestSnapshot);

        // Debug test
        //Move(new ChessCoordinate(0, 6), new ChessCoordinate(0, 4));
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
        if (!piecesDict.ContainsKey(from)) return;

        ChessPieceScript selectedPiece = piecesDict[from];

        // Debug: If the destination is not empty, then skip it
        if (piecesDict.ContainsKey(to)) return;

        selectedPiece.Coord = to;

        piecesDict.Remove(from);
        piecesDict.Add(to, selectedPiece);

        GenNextSnapshot
        (
            new ChessPosition(ChessPieceType.None, from),
            piecesDict[to].position
        );
    }
}
