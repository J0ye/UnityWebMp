using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BasicProcedureEntity), true)]
public class CustomRPCInfo : Editor
{
    public override void OnInspectorGUI()
    {
        BasicProcedureEntity script = (BasicProcedureEntity)target;
        DrawDefaultInspector();

        EditorGUILayout.LabelField("ID", script.id);
        if (GUILayout.Button("New Id"))
        {
            script.SetNewGuid();
        }
    }
}
