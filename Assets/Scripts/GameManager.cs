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

	public ChessSettings settings;
	public ChessPieceScript[] pieces;
	public GameObject[] pieceMeshes;
	public Material[] pieceMat;

	void Start()
	{
		
	}
}
