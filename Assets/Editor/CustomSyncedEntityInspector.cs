using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Custom Editor 
[CustomEditor(typeof(SyncedEntity), true)]
public class CustomSyncedEntityInfo : Editor
{
    public override void OnInspectorGUI()
    {
        SyncedEntity script = (SyncedEntity)target;
        DrawDefaultInspector();
    }
}