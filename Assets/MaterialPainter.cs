using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MaterialPainter : MonoBehaviour
{
	public Material mat;
	public MeshRenderer[] rends;

	[ContextMenu("Update Material")]
	public void UpdateMaterial ()
	{
		for(int i = 0; i < rends.Length; i++)
		{
			rends[i].material = mat;
		}
	}
}
