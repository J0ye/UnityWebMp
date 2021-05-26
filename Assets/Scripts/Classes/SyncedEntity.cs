using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SyncedEntity : MonoBehaviour
{
    public bool isDebug = false;
    [HideInInspector]
    public string id { get => id; protected set => id = value; }

    public void Send(string msg)
    {
        try
        {
            WebSocketBehaviour.instance.Send(msg);
            if(isDebug) Debug.Log("Sending via " + WebSocketBehaviour.instance.adress + ": " + msg);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not make send message from" + gameObject.name + ". Error: " + e);
        }
    }

    public void SetRandomGuid()
    {
        id = Guid.NewGuid().ToString();
    }

    public void SetGuid(string newID)
    {
        Guid temp;
        if(Guid.TryParse(newID, out temp))
        {
            this.id = newID;
            return;
        }
        Debug.LogError("Failed to parse new ID for synced entity");
    }
}

/*
// Custom Editor 
[CustomEditor(typeof(SyncedEntity), true)]
public class CustomSyncedEntityInfo : Editor
{
    public override void OnInspectorGUI()
    {
        SyncedEntity script = (SyncedEntity)target;
        DrawDefaultInspector();

        EditorGUILayout.LabelField("ID", script.id);
        if (GUILayout.Button("New Id"))
        {
            script.SetRandomGuid();
        }
    }
}*/
