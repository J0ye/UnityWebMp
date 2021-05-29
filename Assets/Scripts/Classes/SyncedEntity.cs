using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Text;
using Msg;

public class SyncedEntity : MonoBehaviour
{
    public bool isDebug = false;
    [Tooltip("This synced entity will only be updated on the server if this member is set to true")]
    public bool isOnline = true;
    [HideInInspector]
    public string guid;

    public virtual void Start()
    {
        StartCoroutine(RegisterOnMessageEvent());
    }

    public void Send(IDMessage msg)
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
            if(isDebug) Debug.Log("Sending via " + WebSocketBehaviour.instance.adress + ": " + msg.ToJson());
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

    protected IEnumerator RegisterOnMessageEvent()
    {
        Func<bool> tempFunc = () => WebSocketBehaviour.WebSocketStatus();
        yield return new WaitUntil(tempFunc);

        WebSocketBehaviour.instance.GetWS().OnMessage += (byte[] msg) =>
        {
            ProcessMessage(Encoding.UTF8.GetString(msg));
        };

        if (isDebug) Debug.Log("Registered OnMessage event");
    }

    protected void ProcessMessage(string msg)
    {
        try
        {
            IDMessage target = IDMessage.FromJson(msg);
            if(target.connectionID == guid)
            {
                TransformMessage message = TransformMessage.FromJson(msg);
                RecieveValues(msg);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error caught " + e + " for: " + msg + " on " + gameObject.name);
            return;
        }
    }

    protected virtual void RecieveValues(string msg)
    {
        Debug.Log("Excuted base function for recieve value");
    }
}
