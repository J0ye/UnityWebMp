using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(BasicBehaviour), true)]
public class CustomWebsocketInfo : Editor
{
    public override void OnInspectorGUI()
    {
        BasicBehaviour script = (BasicBehaviour)target;
        DrawDefaultInspector();

        DisplayID();
        EditorGUILayout.LabelField("State", script.GetState());        
    }

    private void DisplayID()
    {
        try
        {
            WebSocketBehaviour wb = (WebSocketBehaviour)target;
            EditorGUILayout.LabelField("ID", wb.ConnectionID.ToString());
        }
        catch (Exception e)
        {
            
        }
    }
}
