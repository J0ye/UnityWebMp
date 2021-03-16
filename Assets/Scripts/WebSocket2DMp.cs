using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Use plugin namespace
using HybridWebSocket;


public class WebSocket2DMp : MonoBehaviour
{
    public static WebSocket2DMp instance;

    private WebSocket ws;
    private bool connected = false;

    // Use this for initialization
    void Start()
    {
        // Singelton
        if (instance != null)
            Destroy(this);

        if (instance == null)
            instance = this;

        // Create WebSocket instance
        ws = WebSocketFactory.CreateInstance("ws://localhost:9000/2dmp");

        // Add OnOpen event listener
        ws.OnOpen += () =>
        {
            Debug.Log("WS connected!");
            Debug.Log("WS state: " + ws.GetState().ToString());
            connected = true;
        };

        // Add OnMessage event listener
        ws.OnMessage += (byte[] msg) =>
        {
            Debug.Log("WS received message: " + Encoding.UTF8.GetString(msg));
        };

        // Add OnError event listener
        ws.OnError += (string errMsg) =>
        {
            Debug.Log("WS error: " + errMsg);
        };

        // Add OnClose event listener
        ws.OnClose += (WebSocketCloseCode code) =>
        {
            Debug.Log("WS closed with code: " + code.ToString());
        };

        // Connect to the server
        ws.Connect();
    }

    private void OnApplicationQuit()
    {
        ws.Close();
    }

    public WebSocket GetWS()
    {
        return ws;
    }

    public void Send(string txt)
    {
        if (connected)
        {
            ws.Send(Encoding.UTF8.GetBytes(txt));
        }
    }
}
