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

    [Header("Visuals")]
    public GameObject gear;

    [Header("Settings")]
    public ChessPlayerType playerType;
    //public int fixedDepthLimit = 5;
    public float maxTime = 20.0f;
    public int minDepth = 2;
    public int maxDepth = 40;
    public int levelUpEvery = 10;

    [Header("Minimax")]
    public int growth = 0;
    public ChessBoardSnapshot[] outcomes;
    public bool isRunningItDeep = false;
    public bool hasRunItDeep = false;
    public bool isRunningMinimax = false;
    public bool hasRunMinimax = false;

    public int iterativeDepth;

    public int lastScore;
    public int lastIteration;
    public int totalIterations;

    public int highestScore = int.MinValue;
    public int highestScoreIndex = -1;

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

    void Start()
    {
        growth = 0;
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
                            bool isLastMove = false;

                            if (possibleMoves[j].pattern == ChessPieceMovePattern.Normal)
                            {
                                if (boardDict.ContainsKey(to.ToArrayCoord()))
                                {
                                    if (boardDict[to.ToArrayCoord()].type.IsSameTeamAs(position.type))
                                        break;

                                    isLastMove = true;
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

                            if (isLastMove)
                                break;
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
    
    int AlphaBeta(ChessBoardSnapshot boardPosition, int depth, int alpha, int beta, bool maximizingPlayer)
    {
        int origAlpha = alpha;
        int origBeta = beta;
        MinimaxNode node = new MinimaxNode();

        // Transposition Table Lookup; node is the lookup key for ttEntry
        ulong hash = boardPosition.board.ToZobristHash();
        if (tTable.ContainsKey(hash))
        {
            node = tTable[hash];

            if(node.depth >= depth)
            {
                switch(node.flag)
                {
                    case MinimaxNodeFlag.Exact:
                        return node.eval;
                    case MinimaxNodeFlag.LowerBound:
                        if (node.eval > alpha)
                            alpha = node.eval;
                        break;
                    case MinimaxNodeFlag.UpperBound:
                        if (node.eval < beta)
                            beta = node.eval;
                        break;
                }

                if (beta <= alpha)
                    return node.eval;
            }
        }

        // Minimax + Alpha Beta Pruning
        if (depth <= 0 || boardPosition.IsEndGame())
        {
            return GameManager.Instance.CalculateScore(boardPosition, playerType);
        }
        
        int val = 0;

        if (maximizingPlayer)
        {
            val = int.MinValue;
            ChessBoardSnapshot[] nextBoardPositions = FindPossibleMoves(boardPosition, playerType);

            for (int i = 0; i < nextBoardPositions.Length; i++)
            {
                int newValue = AlphaBeta(nextBoardPositions[i], depth - 1, alpha, beta, false);

                if (newValue > val)
                    val = newValue;
                if (val > alpha)
                    alpha = val;
                if (beta <= alpha)
                    break;
            }
        }
        else
        {
            val = int.MaxValue;
            ChessBoardSnapshot[] nextBoardPositions = FindPossibleMoves(boardPosition, playerType.ToOpposite());

            for (int i = 0; i < nextBoardPositions.Length; i++)
            {
                int newValue = AlphaBeta(nextBoardPositions[i], depth - 1, alpha, beta, true);

                if (newValue < val)
                    val = newValue;
                if (val < beta)
                    beta = val;
                if (beta <= alpha)
                    break;
            }
        }
        
        // Transposition Table Store; node is the lookup key for ttEntry
        node.hash = hash;
        node.eval = val;

        if(val <= origAlpha)
            node.flag = MinimaxNodeFlag.UpperBound;
        else if(val >= origBeta)
            node.flag = MinimaxNodeFlag.LowerBound;
        else
            node.flag = MinimaxNodeFlag.Exact;

        node.depth = depth;

        if(tTable.ContainsKey(hash))
            tTable[hash] = node;
        else
            tTable.Add(hash, node);

        return val;
    }

    IEnumerator IterativeDeepeningSearch(ChessBoardSnapshot boardPosition)
    {
        isRunningItDeep = true;

        float startTime = Time.unscaledTime;
        bool isOutTime = false;

        for (int depth = 1; (depth <= minDepth || depth <= maxDepth && !isOutTime); depth++)
        {
            StartCoroutine(MinimaxAB(boardPosition, depth));
            hasRunMinimax = true;
            isRunningMinimax = true;
            iterativeDepth = depth;
            yield return new WaitUntil(() => isRunningMinimax == false);
            hasRunMinimax = false;
            isOutTime = Time.unscaledTime - startTime >= maxTime;
        }

        isRunningItDeep = false;
    }

    IEnumerator MinimaxAB(ChessBoardSnapshot boardPosition, int depth)
    {
        isRunningMinimax = true;

        //int highestScore = int.MinValue;
        //int highestScoreIndex = -1;
        highestScore = int.MinValue;
        highestScoreIndex = -1;
        
        ChessBoardSnapshot[] nextBoardPositions = FindPossibleMoves(boardPosition, playerType);
        totalIterations = nextBoardPositions.Length;

        for (int i = 0; i < nextBoardPositions.Length; i++)
        {
            int score = AlphaBeta(nextBoardPositions[i], depth, int.MinValue, int.MaxValue, false);
            if (score > highestScore)
            {
                highestScore = score;
                highestScoreIndex = i;
            }

            lastScore = score;
            lastIteration = i;
            minimaxResult = nextBoardPositions[highestScoreIndex];
            yield return null;
        }

        isRunningMinimax = false;
    }

    IEnumerator Minimax(ChessBoardSnapshot boardPosition, int depth)
    {
        isRunningMinimax = true;

        //int highestScore = int.MinValue;
        //int highestScoreIndex = -1;
        highestScore = int.MinValue;
        highestScoreIndex = -1;

        //getAllBoardPositions returns a list of next possible board positions, the boolean flag is to tell whether the current move is Max or Min
        ChessBoardSnapshot[] nextBoardPositions = FindPossibleMoves(boardPosition, playerType);
        totalIterations = nextBoardPositions.Length;

        for (int i = 0; i < nextBoardPositions.Length; i++)
        {
            ChessBoardSnapshot board = nextBoardPositions[i];
            int score = Min(board, depth);
            if (score > highestScore)
            {
                highestScore = score;
                highestScoreIndex = i;
            }

            lastScore = score;
            lastIteration = i;
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
    
    [ContextMenu("[Debug] Generate Possible Moves")]
    public void GeneratePossibleMoves()
    {
        outcomes = FindPossibleMoves(GameManager.Instance.LatestSnapshot, playerType);
        Debug.Log("Generated possible moves for " + playerType + " Player (" + outcomes.Length + " outcomes)");
    }

    [ContextMenu("[Debug] Make Move now")]
    public void MakeMove()
    {
        growth++;
        if (growth % levelUpEvery == 0)
            minDepth++;

        StartCoroutine(IterativeDeepeningSearch(GameManager.Instance.LatestSnapshot));
        hasRunItDeep = true;
        isRunningItDeep = true;
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
            GeneratePossibleMoves();
        }

        if(hasRunItDeep)
        {
            // If it's not running anymore
            if(!isRunningItDeep)
            {
                PostGenerateMinimax();
                hasRunItDeep = false;
            }
        }
    }

    private void OnGUI()
    {
        gear.SetActive(isRunningItDeep);
    }
}
