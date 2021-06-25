using System;
using System.Collections;
using System.Text;
using UnityEngine;
using Msg;

public class NetworkManager : MonoBehaviour
{
    [Tooltip ("Determines how often information is sent to the server, in seconds.")]
    public float pingFrequency = 1f;
    public bool debug = false;   

    public virtual void SetPingFrequency(float input)
    {
        pingFrequency = input;
    }

    public void SimulateMessage(string msg)
    {
        ProcessMessage(msg);
    }

    protected virtual void Start()
    {
        StartCoroutine(SetUpSocket());
    }

    protected virtual IEnumerator SetUpSocket()
    {
        while (WebSocketBehaviour.instance == null)
        {
            Debug.Log("Waiting for instance");
            yield return new WaitForSeconds(0.2f);
        }

        WebSocketBehaviour.instance.GetWS().OnMessage += (byte[] msg) =>
        {
            ProcessMessage(Encoding.UTF8.GetString(msg));
        };
        if(debug) Debug.Log("Completed set up for network manager");
    }

    protected virtual void ProcessMessage(string msg)
    {
        try
        {
            IDMessage target = IDMessage.FromJson(msg);
            // Ignore, if the message is about this game
            if (target.Guid != WebSocketBehaviour.instance.ConnectionID && target.type != WebsocketMessageType.SyncedGameObject)
            {
                if (debug) Debug.Log("From Json: guid " + target.connectionID + " and type " + target.type);
                ExecuteOnJson(target, msg);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("This msg: " + msg + " led to this error:" + e);
            return;
        }
    }

    protected virtual void ExecuteOnJson(IDMessage iDMessage, string msg)
    {
        Debug.LogWarning("The base function of ExecuteOnJson has been called.");
    }
}