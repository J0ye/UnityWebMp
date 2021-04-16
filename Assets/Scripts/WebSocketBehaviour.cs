using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using HybridWebSocket;

public class WebSocketBehaviour : BasicBehaviour
{
    public static WebSocketBehaviour instance;
    public bool isDebug = false;

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
            Debug.Log("Base state: " + ws.GetState().ToString());
            connected = true;
        };

        // Add OnMessage event listener
        ws.OnMessage += (byte[] msg) =>
        {
            if(isDebug) Debug.Log("Base received message: " + Encoding.UTF8.GetString(msg));
        };
    }

    protected virtual void OnApplicationQuit()
    {
        ws.Close();
    }

    public WebSocket GetWS()
    {
        return ws;
    }
}