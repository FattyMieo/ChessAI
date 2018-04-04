using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;

public class ChessPieceScript : MonoBehaviour
{
	private MaterialPainter meshMatPainter;
	public ChessPieceType type;
	public ChessCoordinate coord;

	// Use this for initialization
	void Start ()
	{
		UpdateMesh();
	}

	[ContextMenu("Update Mesh")]
	void UpdateMesh()
	{
		if(meshMatPainter != null)
			Destroy(meshMatPainter.gameObject);

		int meshType = FindMeshType();

		if(meshType < 0)
		{
			Debug.LogError("Invalid ChessPieceType for meshType");
			return;
		}

		int colorType = FindColorType();

		if(colorType < 0)
		{
			Debug.LogError("Invalid ChessPieceType for colorType");
			return;
		}

		meshMatPainter = Instantiate(GameManager.instance.pieceMeshes[meshType], this.transform).GetComponent<MaterialPainter>();
		meshMatPainter.mat = GameManager.instance.pieceMat[colorType];
		meshMatPainter.GetComponent<MaterialPainter>().UpdateMaterial();
	}

	int FindMeshType()
	{
		switch(type)
		{
		case ChessPieceType.WhitePawn:
		case ChessPieceType.BlackPawn:
			return 0;
		case ChessPieceType.WhiteKnight:
		case ChessPieceType.BlackKnight:
			return 1;
		case ChessPieceType.WhiteBishop:
		case ChessPieceType.BlackBishop:
			return 2;
		case ChessPieceType.WhiteRook:
		case ChessPieceType.BlackRook:
			return 3;
		case ChessPieceType.WhiteQueen:
		case ChessPieceType.BlackQueen:
			return 4;
		case ChessPieceType.WhiteKing:
		case ChessPieceType.BlackKing:
			return 5;
		}
		return -1;
	}

	int FindColorType()
	{
		switch(type)
		{
		case ChessPieceType.WhitePawn:
		case ChessPieceType.WhiteKnight:
		case ChessPieceType.WhiteBishop:
		case ChessPieceType.WhiteRook:
		case ChessPieceType.WhiteQueen:
		case ChessPieceType.WhiteKing:
			return 0;
		case ChessPieceType.BlackPawn:
		case ChessPieceType.BlackKnight:
		case ChessPieceType.BlackBishop:
		case ChessPieceType.BlackRook:
		case ChessPieceType.BlackQueen:
		case ChessPieceType.BlackKing:
			return 1;
		}
		return -1;
	}

	// Update is called once per frame
	void Update ()
	{
		
	}
}
