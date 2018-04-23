using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess;

/// <summary>
/// A test script for applying moves on the chess board
/// </summary>
public class MoveTester : MonoBehaviour
{
    public bool pressToRunMove;
    public ChessCoordinate from;
	public ChessCoordinate to;
	public bool hasSelected;
	public SquareColliderScript lastSquare;
	public SquareColliderScript selectedSquare;

    void Start()
    {
        pressToRunMove = false;
    }

    void Update()
    {
        if(pressToRunMove)
        {
            pressToRunMove = false;
            RunMove();
        }

		CheckMouseClick();
    }

	void CheckMouseClick()
	{
		RaycastHit hit;
		if(Physics.Raycast
		(
			Camera.main.ScreenPointToRay(Input.mousePosition),
			out hit, 100.0f
		))
		{
			if(!hasSelected || lastSquare != selectedSquare)
			{
				if(lastSquare != null)
					lastSquare.SetVisibility(false);
			}
			lastSquare = hit.collider.GetComponent<SquareColliderScript>();
			if(!hasSelected || lastSquare != selectedSquare)
			{
				lastSquare.SetColor(Color.white);
				lastSquare.SetVisibility(true);
			}

            if(AIManager.Instance.isRunningMinimax)
                return;

			if(Input.GetMouseButtonUp(0))
			{
				if(hit.collider.gameObject.CompareTag("SquareCollider"))
				{
					Vector3 pos = hit.collider.transform.position;

					if(!hasSelected)
					{
						from.x = (int)(pos.x / 2.0f);
						from.y = (int)(-pos.z / 2.0f);

						if(GameManager.Instance.piecesDict.ContainsKey(from.ToArrayCoord()))
						{
							hasSelected = true;
							selectedSquare = hit.collider.GetComponent<SquareColliderScript>();
							selectedSquare.SetColor(Color.green);
							selectedSquare.SetVisibility(true);
							return;
						}
					}
					else
					{
						to.x = (int)(pos.x / 2.0f);
						to.y = (int)(-pos.z / 2.0f);

						if(from == to)
						{
							hasSelected = false;
							if(selectedSquare != null && lastSquare != selectedSquare)
							{
								selectedSquare.SetVisibility(false);
							}
							return;
						}

						if(!GameManager.Instance.Move(from, to))
						{
							return;
						}

						hasSelected = false;
						if(selectedSquare != null && lastSquare != selectedSquare)
						{
							selectedSquare.SetVisibility(false);
						}
					}
				}
			}
		}
	}

    [ContextMenu("Run Move")]
	void RunMove ()
    {
        if(GameManager.Instance.Move(from, to))
        {
            ChessCoordinate newTo = to - from;
            from.x = to.x;
            from.y = to.y;
            to.x += newTo.x;
            to.y += newTo.y;
        }
    }
}
