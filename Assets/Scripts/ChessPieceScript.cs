using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;

public class ChessPieceScript : MonoBehaviour
{
    private GameObject mesh;
    private ChessPieceType _type;
    public ChessPieceType type
    {
        get { return _type; }
        set
        {
            _type = value;
            gameObject.name = _type.ToString();
            UpdateMesh();
        }
    }
    private ChessCoordinate _coord;
	public ChessCoordinate coord
    {
        get { return _coord; }
        set
        {
            _coord = value;
            UpdatePosition();
        }
    }

    public void UpdatePosition()
    {
        transform.position = new Vector3(coord.x * 2.0f, 0.0f, coord.y * 2.0f);
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

        mesh = Instantiate(GameManager.instance.pieceMeshes[meshType], this.transform);
        MaterialPainter meshMatPainter = mesh.GetComponent<MaterialPainter>();
		meshMatPainter.mat = GameManager.instance.pieceMat[colorType];
		meshMatPainter.GetComponent<MaterialPainter>().UpdateMaterial();
	}

	int FindMeshType()
    {
        if (type.IsPawn())   return 0;
        if (type.IsKnight()) return 1;
        if (type.IsBishop()) return 2;
        if (type.IsRook())   return 3;
        if (type.IsQueen())  return 4;
        if (type.IsKing())   return 5;
		return -1;
	}

	int FindColorType()
    {
        if (type.IsWhite()) return 0;
        if (type.IsBlack()) return 1;
        return -1;
	}
}
