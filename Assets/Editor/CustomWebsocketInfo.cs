using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BasicBehaviour), true)]
public class CustomWebsocketInfo : Editor
{
    public override void OnInspectorGUI()
    {
        BasicBehaviour script = (BasicBehaviour)target;
        DrawDefaultInspector();
        
        EditorGUILayout.LabelField("State", script.GetState());
    }
}
