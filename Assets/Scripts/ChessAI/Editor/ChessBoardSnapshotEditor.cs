using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Chess;

[CustomEditor(typeof(ChessBoardSnapshot))]
public class ChessBoardSnapshotEditor : Editor
{
	SerializedProperty board;
    bool showBoard = false;
    bool isWhite = true;

    public override void OnInspectorGUI()
    {
        Color defaulBColor = GUI.color;
        Color invertedColor = defaulBColor * 0.5f;
        invertedColor.a = 1.0f;

        serializedObject.Update();

        board = serializedObject.FindProperty("board");
        board.arraySize = ChessSettings.boardSize * ChessSettings.boardSize;

        EditorGUILayout.BeginHorizontal();
        showBoard = EditorGUILayout.Toggle(showBoard, GUILayout.Width(20));
        EditorGUILayout.LabelField("Show Board Visualization");
        EditorGUILayout.EndHorizontal();

        if (showBoard)
        {
            if (isWhite)
                GUI.color = defaulBColor;
            else
                GUI.color = invertedColor;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("", GUILayout.Width(13), GUILayout.Height(13)))
                isWhite = !isWhite;

            GUI.color = defaulBColor;

            GUILayout.Space(7.5f);
            EditorGUILayout.LabelField("Piece Color");
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;

            for (int i = 0; i < board.arraySize;)
			{
                if(i % ChessSettings.boardSize == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                }

                GUIStyle gs = GUI.skin.button;
                gs.fontSize = 40;
                if (GUILayout.Button(board.GetArrayElementAtIndex(i).enumValueIndex.ToChessPieceIcon().ToString(), gs, GUILayout.Width(40), GUILayout.Height(40)))
                {
                    if (board.GetArrayElementAtIndex(i).enumValueIndex >= 1 && board.GetArrayElementAtIndex(i).enumValueIndex <= 6)
                    {
                        if (isWhite)
                        {
                            board.GetArrayElementAtIndex(i).enumValueIndex++;

                            if (board.GetArrayElementAtIndex(i).enumValueIndex >= (int)ChessPieceType.BlackPawn)
                            {
                                board.GetArrayElementAtIndex(i).enumValueIndex = 0;
                            }
                        }
                        else
                        {
                            board.GetArrayElementAtIndex(i).enumValueIndex += 6;
                        }
                    }
                    else if (board.GetArrayElementAtIndex(i).enumValueIndex >= 7 && board.GetArrayElementAtIndex(i).enumValueIndex <= 12)
                    {
                        if(!isWhite)
                        {
                            board.GetArrayElementAtIndex(i).enumValueIndex++;

                            if (board.GetArrayElementAtIndex(i).enumValueIndex >= (int)ChessPieceType.Total)
                            {
                                board.GetArrayElementAtIndex(i).enumValueIndex = 0;
                            }
                            else if (board.GetArrayElementAtIndex(i).enumValueIndex >= (int)ChessPieceType.WhitePawn)
                            {
                                board.GetArrayElementAtIndex(i).enumValueIndex = 7;
                            }
                        }
                        else
                        {
                            board.GetArrayElementAtIndex(i).enumValueIndex -= 6;
                        }
                    }

                    //board.GetArrayElementAtIndex(i).enumValueIndex++;

                    if(board.GetArrayElementAtIndex(i).enumValueIndex >= (int)ChessPieceType.Total)
                    {
                        board.GetArrayElementAtIndex(i).enumValueIndex = 0;
                    }
                }

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

        serializedObject.ApplyModifiedProperties();

        DrawDefaultInspector();
    }
}
