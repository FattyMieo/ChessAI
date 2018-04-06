using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;

/// <summary>
/// Script for chess piece prefab
/// Can be found in Prefabs/Pieces/Piece.prefab
/// </summary>
public class ChessPieceScript : MonoBehaviour
{
    private GameObject mesh;
    public ChessPosition position;
    public ChessPieceType Type
    {
        get { return position.type; }
        set
        {
            position.type = value;
            gameObject.name = position.type.ToString();
            UpdateMesh();
        }
    }
	public ChessCoordinate Coord
    {
        get { return position.coord; }
        set
        {
            position.coord = value;
            UpdatePosition();
        }
    }

    public void UpdatePosition()
    {
        transform.position = new Vector3(Coord.x * 2.0f, 0.0f, -Coord.y * 2.0f);
    }

	[ContextMenu("Update Mesh")]
	public void UpdateMesh()
    {
        if (mesh != null)
            Destroy(mesh);

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

        mesh = Instantiate(GameManager.Instance.pieceMeshes[meshType], this.transform);
        MaterialPainter meshMatPainter = mesh.GetComponent<MaterialPainter>();
		meshMatPainter.mat = GameManager.Instance.pieceMat[colorType];
		meshMatPainter.GetComponent<MaterialPainter>().UpdateMaterial();
	}

	int FindMeshType()
    {
        if (Type.IsPawn())   return 0;
        if (Type.IsKnight()) return 1;
        if (Type.IsBishop()) return 2;
        if (Type.IsRook())   return 3;
        if (Type.IsQueen())  return 4;
        if (Type.IsKing())   return 5;
		return -1;
	}

	int FindColorType()
    {
        if (Type.IsWhite()) return 0;
        if (Type.IsBlack()) return 1;
        return -1;
	}
}
