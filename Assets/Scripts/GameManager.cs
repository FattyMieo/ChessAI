using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;

public class GameManager : MonoBehaviour
{
	private static GameManager _instance;
	public static GameManager instance
	{
		get
		{
			if(_instance == null)
				_instance = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
			
			return _instance;
		}
	}

	// Use this for initialization
	void Awake () 
	{
		if(_instance == null)
			_instance = this;
		else if(_instance != this)
			Destroy(this.gameObject);
	}

    [Header("Settings")]
	public ChessSettings settings;
	public GameObject[] pieceMeshes;
	public Material[] pieceMat;
    public GameObject piecePrefab;
    
    [Header("Arrays")]
    public ChessPieceScript[] pieces;
    public List<ChessBoardSnapshot> snapshots;

    void Start()
	{
        GenNewPieces();
        GenNewBoard();
    }

    [ContextMenu("Generate New Pieces")]
    void GenNewPieces()
    {
        pieces = new ChessPieceScript[ChessSettings.totalPieces];

        for(int i = 0; i < ChessSettings.totalPieces; i++)
        {
            pieces[i] = Instantiate(piecePrefab, this.transform).GetComponent<ChessPieceScript>();
            
            if(i < 16)
            {
                if (i < 8)
                {
                    pieces[i].coord = new ChessCoordinate(i, 1);
                    pieces[i].type = ChessPieceType.WhitePawn;
                }
                else
                {
                    pieces[i].coord = new ChessCoordinate(i - 8, 6);
                    pieces[i].type = ChessPieceType.BlackPawn;
                }
            }
            else
            {
                if ((i - 16) < 8)
                {
                    pieces[i].coord = new ChessCoordinate(i - 16, 0);
                    switch (pieces[i].coord.x)
                    {
                        case 0:
                        case 7:
                            pieces[i].type = ChessPieceType.WhiteRook;
                            break;
                        case 1:
                        case 6:
                            pieces[i].type = ChessPieceType.WhiteKnight;
                            break;
                        case 2:
                        case 5:
                            pieces[i].type = ChessPieceType.WhiteBishop;
                            break;
                        case 3:
                            pieces[i].type = ChessPieceType.WhiteQueen;
                            break;
                        case 4:
                            pieces[i].type = ChessPieceType.WhiteKing;
                            break;
                    }
                }
                else
                {
                    pieces[i].coord = new ChessCoordinate(i - 16 - 8, 7);
                    switch (pieces[i].coord.x)
                    {
                        case 0:
                        case 7:
                            pieces[i].type = ChessPieceType.BlackRook;
                            break;
                        case 1:
                        case 6:
                            pieces[i].type = ChessPieceType.BlackKnight;
                            break;
                        case 2:
                        case 5:
                            pieces[i].type = ChessPieceType.BlackBishop;
                            break;
                        case 3:
                            pieces[i].type = ChessPieceType.BlackQueen;
                            break;
                        case 4:
                            pieces[i].type = ChessPieceType.BlackKing;
                            break;
                    }
                }
            }
        }
    }

    [ContextMenu("Generate New Board")]
    void GenNewBoard()
    {
        ChessBoardSnapshot newBoard = ScriptableObject.CreateInstance<ChessBoardSnapshot>();
        newBoard.name = "Board #" + snapshots.Count.ToString("0000");
        snapshots.Add(newBoard);
    }
}
