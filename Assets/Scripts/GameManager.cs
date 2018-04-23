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
    [HideInInspector] public ChessPieceProfileDictionary profilesDict = new ChessPieceProfileDictionary();
	private Transform chessPiecesParent;
    public GameObject[] pieceMeshes;
    public Material[] pieceMat;
    public GameObject piecePrefab;
	public GameObject squareColPrefab;
	private Transform squareColliderParent;
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

		chessPiecesParent = (new GameObject()).transform;
		chessPiecesParent.gameObject.name = "Chess Pieces";
		chessPiecesParent.SetParent(this.transform);

		squareColliderParent = (new GameObject()).transform;
		squareColliderParent.gameObject.name = "Square Colliders";
		squareColliderParent.SetParent(this.transform);

        profilesDict.Init(profiles);
    }

    void Start()
    {
		SpawnSquareColliders();
		GenNextSnapshot(defaultBoard);
		LoadFromSnapshot(LatestSnapshot);
    }

	void SpawnSquareColliders()
	{
		for(int i = 0; i < ChessSettings.boardSize * ChessSettings.boardSize; i++)
		{
			float x = (i % ChessSettings.boardSize);
			float y = Mathf.FloorToInt(i / ChessSettings.boardSize);
			GameObject newSC = Instantiate
			(
				squareColPrefab, new Vector3(x * 2.0f, 0.0f, y * -2.0f), Quaternion.identity, squareColliderParent
			);
			newSC.name = squareColPrefab.name + " (" + x + "," + y + ")";
		}
	}

    /// <summary>
    /// Use a board snapshot to create the board
    /// </summary>
    /// <param name="boardSnapshot"></param>
    public void LoadFromSnapshot(ChessBoardSnapshot boardSnapshot)
    {
        List<ChessPosition> boardDict = boardSnapshot.ToList();

        if (boardDict.Count <= 0)
            return;

        foreach (KeyValuePair<int, ChessPieceScript> kvp in piecesDict)
            Destroy(kvp.Value.gameObject);

        piecesDict.Clear();
        
        for (int i = 0; i < boardDict.Count; i++)
		{
			ChessPieceScript newPiece = Instantiate(piecePrefab, chessPiecesParent)
											.GetComponent<ChessPieceScript>();

            newPiece.Position = boardDict[i];

            piecesDict.Add(newPiece.Coord.ToArrayCoord(), newPiece);
        }
    }

    /// <summary>
    /// Adjust the board snapshot and return it back
    /// </summary>
    /// <param name="boardSnapshot"></param>
    /// <param name="newName"></param>
    /// <param name="changed">Changes to the positions</param>
    /// <returns>
    /// A new modified snapshot
    /// </returns>
    public ChessBoardSnapshot AdjustBoard(ChessBoardSnapshot boardSnapshot, string newName = "Board", params ChessPosition[] changed)
    {
        //ChessBoardSnapshot newBoard = ScriptableObject.CreateInstance<ChessBoardSnapshot>();
        ChessBoardSnapshot newBoard = ScriptableObject.Instantiate<ChessBoardSnapshot>(boardSnapshot);
        newBoard.name = newName;

        for(int i = 0; i < changed.Length; i++)
        {
            int aCoord = changed[i].coord.ToArrayCoord();
            newBoard.board[aCoord] = changed[i].type;
            newBoard.hasMoved[aCoord] = true;
        }

        return newBoard;
    }

    /// <summary>
    /// Generate the next board snapshot
    /// </summary>
    /// <param name="boardSnapshot"></param>
    /// <param name="changed">Changes to the positions</param>
    /// <returns>
    /// A new modified snapshot
    /// </returns>
    public ChessBoardSnapshot GenNextSnapshot(ChessBoardSnapshot boardSnapshot, params ChessPosition[] changed)
    {
        ChessBoardSnapshot newBoard = AdjustBoard(boardSnapshot, "Board #" + snapshots.Count.ToString("0000"), changed);
        snapshots.Add(newBoard);
        return newBoard;
    }

    /// <summary>
    /// Generate the next board snapshot
    /// </summary>
    /// <param name="changed">Changes to the positions</param>
    /// <returns>
    /// A new modified snapshot
    /// </returns>
    public ChessBoardSnapshot GenNextSnapshot(params ChessPosition[] changed)
    {
        return GenNextSnapshot(LatestSnapshot, changed);
    }

    /// <summary>
    /// Make a move from the latest snapshot
    /// </summary>
    /// <param name="from">Current position's coordinate</param>
    /// <param name="to">Destination position's coordinate</param>
    /// <returns>
    /// TRUE if move has successfully been executed.
    /// </returns>
    public bool Move(ChessCoordinate from, ChessCoordinate to)
    {
        ChessPosition[] resultPositions;

        if(Move(from, to, out resultPositions))
        {
            GenNextSnapshot(resultPositions);
            AIManager.Instance.MakeMove();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Make a move from the latest snapshot (Without generating the next snapshot)
    /// </summary>
    /// <param name="from">Current position's coordinate</param>
    /// <param name="to">Destination position's coordinate</param>
    /// <param name="resultPositions">The returning result of positions</param>
    /// <returns>
    /// TRUE if move is valid.
    /// </returns>
    private bool Move(ChessCoordinate from, ChessCoordinate to, out ChessPosition[] resultPositions)
    {
        resultPositions = null;

        if (!from.IsWithinRange())
        {
            Debug.LogWarning("Failed to execute Move.\nReason: (" + from.x + ", " + from.y + ") is not within RANGE.");
            return false;
        }

        if (!to.IsWithinRange())
        {
            Debug.LogWarning("Failed to execute Move.\nReason: (" + to.x + ", " + to.y + ") is not within RANGE.");
            return false;
        }

        if (!piecesDict.ContainsKey(from.ToArrayCoord()))
        {
            Debug.LogWarning("Failed to execute Move.\nReason: (" + from.x + ", " + from.y + ") is EMPTY.");
            return false;
        }

        ChessPieceScript selectedPiece = piecesDict[from.ToArrayCoord()];
        ChessPieceSpecialRule specialRule = ChessPieceSpecialRule.None;
        ChessPosition[] castlingPositions;

        if
        (
            !IsValidMove(selectedPiece.Position, from, to, out specialRule) ||
            !AdditionalMove(specialRule, selectedPiece.Position, out castlingPositions)
        )
        {
            Debug.LogWarning("Failed to execute Move.\nReason: " + selectedPiece.Type + " (" + from.x + ", " + from.y + ") --> (" + to.x + ", " + to.y + ") is INVALID.");
            return false;
        }

        selectedPiece.Coord = to;
        selectedPiece.HasMoved = true;

        // Pawn Promotion
        if (selectedPiece.Type.IsPawn())
        {
            if
            (
                selectedPiece.Type == ChessPieceType.WhitePawn &&
                selectedPiece.Coord.y == 0
            )
            {
                selectedPiece.Type = ChessPieceType.WhiteQueen;
            }
            else if
            (
                selectedPiece.Type == ChessPieceType.BlackPawn &&
                selectedPiece.Coord.y == 7
            )
            {
                selectedPiece.Type = ChessPieceType.BlackQueen;
            }
        }

        if (piecesDict.ContainsKey(to.ToArrayCoord()))
            Destroy(piecesDict[to.ToArrayCoord()].gameObject);

        piecesDict.Remove(from.ToArrayCoord());
        piecesDict.Remove(to.ToArrayCoord());
        piecesDict.Add(to.ToArrayCoord(), selectedPiece);

        List<ChessPosition> resultPositionsList = new List<ChessPosition>()
        {
            new ChessPosition(ChessPieceType.None, from),
            new ChessPosition(selectedPiece.Type, to)
        };

        if (castlingPositions != null)
        {
            for (int i = 0; i < castlingPositions.Length; i++)
                resultPositionsList.Add(castlingPositions[i]);
        }

        resultPositions = resultPositionsList.ToArray();

        Debug.Log("Succeeded to execute Move.\n" + selectedPiece.Type + " (" + from.x + ", " + from.y + ") --> (" + to.x + ", " + to.y + ").");

        return true;
    }

    /// <summary>
    /// Make an additional special move from the latest snapshot (Without generating the next snapshot)
    /// </summary>
    /// <param name="specialRule">The special rule used</param>
    /// <param name="position">The specified piece's data</param>
    /// <param name="resultPositions">The returning result of positions</param>
    /// <returns>
    /// TRUE if move is valid.
    /// </returns>
    private bool AdditionalMove(ChessPieceSpecialRule specialRule, ChessPosition position, out ChessPosition[] resultPositions)
    {
        resultPositions = null;

        if (specialRule == ChessPieceSpecialRule.CastlingLeft || specialRule == ChessPieceSpecialRule.CastlingRight)
        {
            int xFromCoord = position.coord.x;
            int xToCoord = position.coord.x;

            if (specialRule == ChessPieceSpecialRule.CastlingLeft)
            {
                xFromCoord = 0;
                xToCoord -= 1;
            }
            else if (specialRule == ChessPieceSpecialRule.CastlingRight)
            {
                xFromCoord = ChessSettings.boardSize - 1;
                xToCoord += 1;
            }

            ChessCoordinate rookFromCoord = new ChessCoordinate(xFromCoord, position.coord.y);
            ChessCoordinate rookToCoord = new ChessCoordinate(xToCoord, position.coord.y);
                
            return Move(rookFromCoord, rookToCoord, out resultPositions);
        }

        return true;
    }

    /// <summary>
    /// Check whether it's a valid move for the specified piece
    /// </summary>
    /// <param name="position">The specified piece's data</param>
    /// <param name="from">The specified piece's coordinate</param>
    /// <param name="to">The destination's coordinate</param>
    /// <param name="specialRule">The special rule used</param>
    /// <returns>
    /// TRUE if move is valid.
    /// </returns>
    public bool IsValidMove(ChessPosition position, ChessCoordinate from, ChessCoordinate to, out ChessPieceSpecialRule specialRule)
    {
        specialRule = ChessPieceSpecialRule.None;
        ChessPieceMove[] possibleMoves = profilesDict[position.type].possibleMoves;
        for (int i = 0; i < possibleMoves.Length; i++)
        {
            specialRule = possibleMoves[i].specialRule;
            if (!IsValidSpecialRule(possibleMoves[i].specialRule, position, from, to))
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
                        if (piecesDict.ContainsKey(temp.ToArrayCoord()))
                        {
                            if (piecesDict[temp.ToArrayCoord()].Type.IsSameTeamAs(position.type))
                                break;
                        }

                        return true;
                    }
                    else if (possibleMoves[i].pattern == ChessPieceMovePattern.MoveOnly)
                    {
                        if (piecesDict.ContainsKey(temp.ToArrayCoord()))
                            break;

                        return true;
                    }
                    else if (possibleMoves[i].pattern == ChessPieceMovePattern.CaptureOnly)
                    {
                        if (!piecesDict.ContainsKey(temp.ToArrayCoord()))
                            break;

                        if (piecesDict[temp.ToArrayCoord()].Type.IsSameTeamAs(position.type))
                            break;

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
    /// <param name="position">The specified piece's data</param>
    /// <param name="from">The specified piece's coordinate</param>
    /// <param name="to">The destination's coordinate</param>
    /// <returns>
    /// TRUE if move is valid regarding special rule.
    /// </returns>
    public bool IsValidSpecialRule(ChessPieceSpecialRule specialRule, ChessPosition position, ChessCoordinate from, ChessCoordinate to)
    {
        if (specialRule == ChessPieceSpecialRule.None)
            return true;

        // Pawn - Move 2 square when starting from initial position
        if (specialRule == ChessPieceSpecialRule.Pawn2Squares)
        {
            if (position.hasMoved)
                return false;

            return true;
        }

        if (specialRule == ChessPieceSpecialRule.CastlingLeft || specialRule == ChessPieceSpecialRule.CastlingRight)
        {
            if (position.hasMoved)
                return false;

            int xCoord = position.coord.x;

            if (specialRule == ChessPieceSpecialRule.CastlingLeft)
                xCoord = 0;
            else if (specialRule == ChessPieceSpecialRule.CastlingRight)
                xCoord = ChessSettings.boardSize - 1;

            int rookCoord = new ChessCoordinate(xCoord, position.coord.y).ToArrayCoord();

            if (!piecesDict.ContainsKey(rookCoord))
                return false;

            if (!piecesDict[rookCoord].Type.IsRook())
                return false;

            if (piecesDict[rookCoord].HasMoved)
                return false;
            
            return true;
        }

        return false;
    }

    public int CalculateScore(ChessBoardSnapshot boardSnapshot, ChessPlayerType playerType)
    {
        int ret = 0;
        ChessPieceType[] board = boardSnapshot.board;

        for (int i = 0; i < board.Length; i++)
        {
            if (!board[i].IsValid())
                continue;
            if (board[i].IsEmpty())
                continue;

            if (board[i].IsSameTeamAs(playerType))
            {
                ret += profilesDict[board[i]].score + 1;
            }
            else if (board[i].IsDifferentTeamAs(playerType))
            {
                ret -= profilesDict[board[i]].score + 1;
            }
        }

        return ret;
    }
}
