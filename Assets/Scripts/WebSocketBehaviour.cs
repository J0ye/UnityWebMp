using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using Msg;
using HybridWebSocket;

public class WebSocketBehaviour : BasicBehaviour
{
    public static WebSocketBehaviour instance;
    public bool isDebug = false;

    protected Guid connectionID;
    public Guid ConnectionID { get => connectionID; set => connectionID = value; }

    public UnityEvent OnConnect = new UnityEvent();

    // Use this for initialization
    protected override void Start()
    {
        base.Start(); // Sets up adress to websocketserver 

        // Singelton
        if (instance != null)
            Destroy(this);

        if (instance == null)
            instance = this;

        // Add OnOpen event listener
        ws.OnOpen += () =>
        {
            Debug.Log("Connected to base!");
            if(isDebug) Debug.Log("Base state: " + ws.GetState().ToString());
            connected = true;
        };

        // Add OnMessage event listener
        ws.OnMessage += (byte[] msg) =>
        {
            if(isDebug) Debug.Log("Base received message: " + Encoding.UTF8.GetString(msg));
        };
    }

    protected void LateUpdate()
    {
        if (ws.GetState() == WebSocketState.Open) OnConnect.Invoke();
    }

    protected virtual void OnApplicationQuit()
    {
        ws.Close();
    }

    public void Send(IDMessage msg)
    {
        msg.connectionID = connectionID.ToString(); // Signing message with unique connection id 
        base.Send(msg.ToJson());
    }

    public WebSocket GetWS()
    {
        return ws;
    }

    public static bool WebSocketStatus()
    {
        return instance != null;
    }
}