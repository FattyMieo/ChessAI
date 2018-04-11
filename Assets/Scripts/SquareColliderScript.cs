using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareColliderScript : MonoBehaviour
{
	public SpriteRenderer rend;
	public Color currentColor;

	// Use this for initialization
	void Awake ()
	{
		currentColor = Color.white;
		currentColor.a = 0.0f;

		rend = GetComponentInChildren<SpriteRenderer>();
		rend.color = currentColor;
	}

	public void SetVisibility(bool b)
	{
		if(b)
		{
			currentColor.a = 1.0f;
		}
		else
		{
			currentColor.a = 0.0f;
		}

		rend.color = currentColor;
	}

	public void SetColor(Color col)
	{
		currentColor.r = col.r;
		currentColor.g = col.g;
		currentColor.b = col.b;

		rend.color = currentColor;
	}
}
