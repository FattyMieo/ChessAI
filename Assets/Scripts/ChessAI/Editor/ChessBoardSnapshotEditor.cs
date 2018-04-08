using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Chess;

[CustomEditor(typeof(ChessBoardSnapshot))]
public class ChessBoardSnapshotEditor : Editor
{
	SerializedProperty board;
    SerializedProperty hasMoved;
    bool showBoard = true;
    bool isWhite = true;

	Color defaultBColor = Color.white;
	Color invertedColor = Color.gray;
    Color tintBColor = Color.cyan + Color.red * 0.9f;

    public override void OnInspectorGUI()
    {
		serializedObject.Update();

		board = serializedObject.FindProperty("board");
		board.arraySize = ChessSettings.boardSize * ChessSettings.boardSize;

        hasMoved = serializedObject.FindProperty("hasMoved");
        hasMoved.arraySize = ChessSettings.boardSize * ChessSettings.boardSize;

        if (ShowBoardVisualization())
        {
			TogglePieceColor();
			DrawBoard();
        }

        serializedObject.ApplyModifiedProperties();

        DrawDefaultInspector();
    }

	bool ShowBoardVisualization()
	{
		EditorGUILayout.BeginHorizontal();
		showBoard = EditorGUILayout.Toggle(showBoard, GUILayout.Width(20));
		EditorGUILayout.LabelField("Show Board Visualization");
		EditorGUILayout.EndHorizontal();

		return showBoard;
	}

	void TogglePieceColor()
	{
		if (isWhite)
			GUI.color = defaultBColor;
		else
			GUI.color = invertedColor;

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("", GUILayout.Width(13), GUILayout.Height(13)))
			isWhite = !isWhite;

		GUI.color = defaultBColor;

		GUILayout.Space(7.5f);
		EditorGUILayout.LabelField("Toggle Piece Color");
		EditorGUILayout.EndHorizontal();
	}

	void DrawBoard()
	{
		EditorGUI.indentLevel++;

		for (int i = 0; i < board.arraySize;)
		{
			if(i % ChessSettings.boardSize == 0)
			{
				EditorGUILayout.BeginHorizontal();
			}

			// Button
			GUIStyle gs = GUI.skin.button;
			gs.fontSize = 30;

            if (hasMoved.GetArrayElementAtIndex(i).boolValue)
                GUI.color = tintBColor;

            if (GUILayout.Button
			(
				board.GetArrayElementAtIndex(i).enumValueIndex
											   .ToChessPieceIcon()
											   .ToString(),
				gs,
				GUILayout.Width(40), GUILayout.Height(40)
			))
			{
				DoConversion(i);
            }

            GUI.color = defaultBColor;

            i++;

			if (i % ChessSettings.boardSize == 0)
			{
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
			}
		}

		EditorGUILayout.Space();
		EditorGUI.indentLevel--;
	}

	void DoConversion(int i)
	{
		if(board.GetArrayElementAtIndex(i).enumValueIndex == 0)
		{
			if (isWhite)
				board.GetArrayElementAtIndex(i).enumValueIndex = 1;
			else
				board.GetArrayElementAtIndex(i).enumValueIndex = 7;
		}
		else if (board.GetArrayElementAtIndex(i).enumValueIndex >= 1 && board.GetArrayElementAtIndex(i).enumValueIndex <= 6)
		{
			if (isWhite)
			{
				board.GetArrayElementAtIndex(i).enumValueIndex++;

				if (board.GetArrayElementAtIndex(i).enumValueIndex >= (int)ChessPieceType.BlackPawn)
					board.GetArrayElementAtIndex(i).enumValueIndex = 0;
			}
			else
				board.GetArrayElementAtIndex(i).enumValueIndex += 6;
		}
		else if (board.GetArrayElementAtIndex(i).enumValueIndex >= 7 && board.GetArrayElementAtIndex(i).enumValueIndex <= 12)
		{
			if(!isWhite)
			{
				board.GetArrayElementAtIndex(i).enumValueIndex++;

				if (board.GetArrayElementAtIndex(i).enumValueIndex >= (int)ChessPieceType.Total)
					board.GetArrayElementAtIndex(i).enumValueIndex = 0;
			}
			else
				board.GetArrayElementAtIndex(i).enumValueIndex -= 6;
		}
		else
		{
			board.GetArrayElementAtIndex(i).enumValueIndex = 0;
		}
	}
}
