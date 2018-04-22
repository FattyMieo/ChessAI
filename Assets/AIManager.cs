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
    public ChessBoardSnapshot testStartingBoard;
    public MinimaxTree minimaxTree = new MinimaxTree();

    // Use this for initialization
    void Awake ()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this.gameObject);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    
    public List<ChessBoardSnapshot> FindPossibleMoves(ChessBoardSnapshot boardSnapshot, ChessPlayerType playerType)
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

        return ret;
    }

    [ContextMenu("[Test] Generate Possible Moves")]
    public void TestGeneratePossibleMoves()
    {
        minimaxTree.Add(new MinimaxNode(playerType, FindPossibleMoves(GameManager.Instance.LatestSnapshot, playerType)));
        Debug.Log("Generated possible moves for " + playerType + " Player (" + minimaxTree[minimaxTree.Count - 1].outcomes.Count + " outcomes)");
    }

    [ContextMenu("[Test] Generate Possible Moves /w Starting Board")]
    public void TestGeneratePossibleMovesWithStartingBoard()
    {
        minimaxTree.Add(new MinimaxNode(playerType, FindPossibleMoves(testStartingBoard, playerType)));
        Debug.Log("Generated possible moves for " + playerType + " Player (" + minimaxTree[minimaxTree.Count - 1].outcomes.Count + " outcomes)");
    }
}
