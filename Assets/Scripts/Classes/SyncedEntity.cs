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

    public string id = 0.ToString();

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
                if (isDebug) Debug.Log("Sending via " + WebSocketBehaviour.instance.adress + ": " + msg.ToJson());
                WebSocketBehaviour.instance.Send(msg);
            } else
            {
                if (isDebug) Debug.Log(gameObject.name + " is trying to send a message, but is currently offline");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Could not make send message from" + gameObject.name + ". Error: " + e);
        }
    }

    public void TestProcessMessage(string msg)
    {
        ProcessMessage(msg);
    }

    protected IEnumerator RegisterOnMessageEvent()
    {
        while (WebSocketBehaviour.instance == null)
        {
            yield return new WaitForSeconds(0.2f);
        }

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
            if(target.type == WebsocketMessageType.SyncedGameObject && target.connectionID != WebSocketBehaviour.instance.ConnectionID.ToString())
            {
                SyncedGameObjectMessage objectMessage = SyncedGameObjectMessage.FromJson(msg);
                if (objectMessage.messageGuid == id)
                {
                    StartCoroutine(RecieveValues(msg));
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error caught " + e + " for: " + msg + " on " + gameObject.name);
            return;
        }
    }

    protected virtual IEnumerator RecieveValues(string msg)
    {
        yield return 0;
        Debug.Log("Excuted base function for recieve value");
    }
}
