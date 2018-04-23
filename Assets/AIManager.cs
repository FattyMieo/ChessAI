using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;

public class AIManager : MonoBehaviour
{
    private static AIManager _instance;
    public static AIManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindGameObjectWithTag("AIManager").GetComponent<AIManager>();

            return _instance;
        }
    }

    [Header("Settings")]
    public ChessPlayerType playerType;

    [Header("Minimax")]
    public ChessBoardSnapshot[] outcomes;
    public bool isRunningMinimax = false;
    public bool hasRunMinimax = false;
    public int iteration;
    public ChessBoardSnapshot minimaxResult;
    public Dictionary<ulong, MinimaxNode> tTable = new Dictionary<ulong, MinimaxNode>();

    // Use this for initialization
    void Awake ()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this.gameObject);
    }
    
    public ChessBoardSnapshot[] FindPossibleMoves(ChessBoardSnapshot boardSnapshot, ChessPlayerType playerType)
    {
        List<ChessBoardSnapshot> ret = new List<ChessBoardSnapshot>();
        Dictionary<int, ChessPosition> boardDict = boardSnapshot.ToDictionary();

        foreach(KeyValuePair<int, ChessPosition> kvp in boardDict)
        {
            ChessPosition position = kvp.Value;

            if(position.type.IsDifferentTeamAs(playerType))
                continue;

            ChessCoordinate from = position.coord;

            ChessPieceMove[] possibleMoves = GameManager.Instance.profilesDict[position.type].possibleMoves;

            for (int j = 0; j < possibleMoves.Length; j++)
            {
                ChessCoordinate to = from + possibleMoves[j].move;
                int k = 1;

                while
                (
                    to.IsWithinRange() &&
                    (possibleMoves[j].repeatTimes < 0 || k <= possibleMoves[j].repeatTimes)
                )
                {
                    if (GameManager.Instance.IsValidSpecialRule(possibleMoves[j].specialRule, position, from, to))
                    {
                        // When there are only finite amount of moves, set only when reached destination
                        if (possibleMoves[j].repeatTimes < 0 || k == possibleMoves[j].repeatTimes)
                        {
                            if (possibleMoves[j].pattern == ChessPieceMovePattern.Normal)
                            {
                                if (boardDict.ContainsKey(to.ToArrayCoord()))
                                {
                                    if (boardDict[to.ToArrayCoord()].type.IsSameTeamAs(position.type))
                                        break;
                                }
                            }
                            else if (possibleMoves[j].pattern == ChessPieceMovePattern.MoveOnly)
                            {
                                if (boardDict.ContainsKey(to.ToArrayCoord()))
                                    break;
                            }
                            else if (possibleMoves[j].pattern == ChessPieceMovePattern.CaptureOnly)
                            {
                                if (!boardDict.ContainsKey(to.ToArrayCoord()))
                                    break;

                                if (boardDict[to.ToArrayCoord()].type.IsSameTeamAs(position.type))
                                    break;
                            }

                            // Pawn Promotion
                            if (position.type.IsPawn())
                            {
                                if
                                (
                                    position.type == ChessPieceType.WhitePawn &&
                                    position.coord.y == 0
                                )
                                {
                                    position.type = ChessPieceType.WhiteQueen;
                                }
                                else if
                                (
                                    position.type == ChessPieceType.BlackPawn &&
                                    position.coord.y == 7
                                )
                                {
                                    position.type = ChessPieceType.BlackQueen;
                                }
                            }

                            ret.Add(GameManager.Instance.AdjustBoard
                            (
                                boardSnapshot,
                                position.type.ToIcon() + " " + from.x + "," + from.y + "-->" + to.x + "," + to.y,
                                new ChessPosition(ChessPieceType.None, from, true),
                                new ChessPosition(position.type, to, true)
                            ));
                        }
                        else
                        {
                            if (boardDict.ContainsKey(to.ToArrayCoord()))
                                break;
                        }
                    }

                    to += possibleMoves[j].move;
                    k++;
                }
            }
        }

        return ret.ToArray();
    }

    public void AddTransposition(ulong hash, int score, ulong parentHash)
    {
        MinimaxNode newNode = new MinimaxNode(hash, score);

        newNode.parentHash = parentHash;
        if (tTable.ContainsKey(parentHash))
        {
            tTable[parentHash].childrenHash.Add(hash);
        }

        tTable.Add(hash, newNode);
    }
    
    int AlphaBeta(ChessBoardSnapshot boardPosition, int depth, int alpha, int beta, bool maximizingPlayer, ulong parentHash)
    {
        if (depth <= 0 || boardPosition.IsEndGame())
        {
            return GameManager.Instance.CalculateScore(boardPosition, playerType);
        }

        int retValue = maximizingPlayer ? int.MinValue : int.MaxValue;
        ChessBoardSnapshot[] nextBoardPositions = FindPossibleMoves(boardPosition, playerType);

        for(int i = 0; i < nextBoardPositions.Length; i++)
        {
            if (maximizingPlayer)
            {
                ulong hash = nextBoardPositions[i].board.ToZobristHash();
                int newValue = AlphaBeta(nextBoardPositions[i], depth - 1, alpha, beta, false, hash);

                if (!tTable.ContainsKey(hash))
                {
                    AddTransposition(hash, newValue, parentHash);
                }

                if (newValue > retValue)
                    retValue = newValue;
                if (retValue > alpha)
                    alpha = retValue;
                if (beta <= alpha)
                    break;
            }
            else
            {
                ulong hash = nextBoardPositions[i].board.ToZobristHash();
                int newValue = AlphaBeta(nextBoardPositions[i], depth - 1, alpha, beta, true, hash);

                if (!tTable.ContainsKey(hash))
                {
                    AddTransposition(hash, newValue, parentHash);
                }

                if (newValue < retValue)
                    retValue = newValue;
                if (retValue < beta)
                    beta = retValue;
                if (beta <= alpha)
                    break;
            }
        }

        return retValue;
    }

    IEnumerator MinimaxAB(ChessBoardSnapshot boardPosition, int depth)
    {
        isRunningMinimax = true;

        int highestScore = int.MinValue;
        int highestScoreIndex = -1;

        ulong parentHash = boardPosition.board.ToZobristHash();
        ChessBoardSnapshot[] nextBoardPositions = FindPossibleMoves(boardPosition, playerType);

        for (int i = 0; i < nextBoardPositions.Length; i++)
        {
            int score = AlphaBeta(nextBoardPositions[i], depth, int.MinValue, int.MaxValue, true, 0);
            ulong hash = nextBoardPositions[i].board.ToZobristHash();

            if (!tTable.ContainsKey(hash))
            {
                AddTransposition(hash, score, parentHash);
            }

            if (score > highestScore)
            {
                highestScore = score;
                highestScoreIndex = i;
            }

            iteration = i;
            minimaxResult = nextBoardPositions[highestScoreIndex];
            yield return null;
        }

        isRunningMinimax = false;
    }

    IEnumerator Minimax(ChessBoardSnapshot boardPosition, int depth)
    {
        int highestScore = int.MinValue;
        int highestScoreIndex = -1;

        //getAllBoardPositions returns a list of next possible board positions, the boolean flag is to tell whether the current move is Max or Min
        ChessBoardSnapshot[] nextBoardPositions = FindPossibleMoves(boardPosition, playerType);

        for (int i = 0; i < nextBoardPositions.Length; i++)
        {
            ChessBoardSnapshot board = nextBoardPositions[i];
            int score = Min(board, depth);
            if (score > highestScore)
            {
                highestScore = score;
                highestScoreIndex = i;
            }

            iteration = i;
            minimaxResult = nextBoardPositions[highestScoreIndex];
            yield return null;
        }

        isRunningMinimax = false;
    }
    
    int Min(ChessBoardSnapshot boardPosition, int depth)
    {
        if (depth <= 0 || boardPosition.IsEndGame())
        {
            return GameManager.Instance.CalculateScore(boardPosition, playerType);
        }

        int lowestScore = int.MaxValue;

        ChessBoardSnapshot[] nextBoardPositions = FindPossibleMoves(boardPosition, playerType.ToOpposite());
        for (int i = 0; i < nextBoardPositions.Length; i++)
        {
            ChessBoardSnapshot board = nextBoardPositions[i];
            int score = Max(board, depth - 1);
            if (score < lowestScore)
                lowestScore = score;
        }
        return lowestScore;
    }

    int Max(ChessBoardSnapshot boardPosition, int depth)
    {
        if (depth <= 0 || boardPosition.IsEndGame())
        {
            return GameManager.Instance.CalculateScore(boardPosition, playerType);
        }

        int highestScore = int.MinValue;

        ChessBoardSnapshot[] nextBoardPositions = FindPossibleMoves(boardPosition, playerType);
        for (int i = 0; i < nextBoardPositions.Length; i++)
        {
            ChessBoardSnapshot board = nextBoardPositions[i];
            int score = Min(board, depth - 1);
            if (score > highestScore)
                highestScore = score;
        }

        return highestScore;
    }
    
    [ContextMenu("[Test] Generate Possible Moves")]
    public void TestGeneratePossibleMoves()
    {
        outcomes = FindPossibleMoves(GameManager.Instance.LatestSnapshot, playerType);
        Debug.Log("Generated possible moves for " + playerType + " Player (" + outcomes.Length + " outcomes)");
    }

    [ContextMenu("[Test] Generate Minimax")]
    public void TestGenerateMinimax()
    {
        StartCoroutine(MinimaxAB(GameManager.Instance.LatestSnapshot, 5));
        hasRunMinimax = true;
        isRunningMinimax = true;
    }

    public void PostGenerateMinimax()
    {
        GameManager.Instance.GenNextSnapshot(minimaxResult);
        GameManager.Instance.LoadFromSnapshot(GameManager.Instance.LatestSnapshot);
        //playerType = playerType.ToOpposite();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TestGenerateMinimax();
        }

        if(hasRunMinimax)
        {
            // If it's not running anymore
            if(!isRunningMinimax)
            {
                PostGenerateMinimax();
                hasRunMinimax = false;
            }
        }
    }
}
