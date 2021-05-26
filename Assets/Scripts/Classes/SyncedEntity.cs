using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SyncedEntity : MonoBehaviour
{
    public bool isDebug = false;
    [Tooltip("This synced entity will only be updated on the server if this member is set to true")]
    public bool isOnline = true;
    [HideInInspector]
    public string guid;

    public void Send(string msg)
    {
        try
        {
            if(isOnline)
            {
                WebSocketBehaviour.instance.Send(msg);
            } else
            {
                if (isDebug) Debug.Log(gameObject.name + " is trying to send a message, but is currently offline");
            }
            if(isDebug) Debug.Log("Sending via " + WebSocketBehaviour.instance.adress + ": " + msg);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not make send message from" + gameObject.name + ". Error: " + e);
        }
    }

    public void SetRandomGuid()
    {
        guid = System.Guid.NewGuid().ToString();
    }

    public void SetGuid(string newID)
    {
        Guid temp;
        if(System.Guid.TryParse(newID, out temp))
        {
            guid = newID;
            return;
        }
        Debug.LogError("Failed to parse new ID for " + gameObject.name);
    }
}


// Custom Editor 
[CustomEditor(typeof(SyncedEntity), true)]
public class CustomSyncedEntityInfo : Editor
{
    public override void OnInspectorGUI()
    {
        SyncedEntity script = (SyncedEntity)target;
        DrawDefaultInspector();

        EditorGUILayout.LabelField("ID", script.guid);
        if (GUILayout.Button("New Id"))
        {
            script.SetRandomGuid();
        }
    }
}
