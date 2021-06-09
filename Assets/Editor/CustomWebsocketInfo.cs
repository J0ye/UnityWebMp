using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(BasicBehaviour), true)]
public class CustomWebsocketInfo : Editor
{
    BasicBehaviour script;
    public override void OnInspectorGUI()
    {
        script = (BasicBehaviour)target;

        PaintDefault();

        if(script.displayStateInText)
        {
            PaintDisplayTextField();
        }
    }

    private void PaintDefault()
    {
        DisplayID();
        EditorGUILayout.LabelField("State", script.GetState());
        DrawDefaultInspector();
    }

    private void PaintDisplayTextField()
    {
        SerializedProperty displayTextProp = serializedObject.FindProperty("displayText");
        EditorGUILayout.PropertyField(displayTextProp, new GUIContent("Display Text"));
        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayID()
    {
        try
        {
            WebSocketBehaviour wb = (WebSocketBehaviour)target;
            EditorGUILayout.LabelField("Connection ID", wb.ConnectionID.ToString());
        }
        catch (Exception e)
        {
            
        }
    }
}
