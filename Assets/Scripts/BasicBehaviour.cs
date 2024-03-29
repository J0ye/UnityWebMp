﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Use plugin namespace
using HybridWebSocket;

public class BasicBehaviour : MonoBehaviour
{
    public string target = "base";
    [Tooltip ("This will be altered for use in editor and desktop apps.")]
    public string adress = "wss://joye.dev:9000/";
    public float pingFrequency = 0.5f;

    protected WebSocket ws;
    protected bool connected = false;
    protected string websocketState = "not active";

    protected virtual void Start()
    {
        if(Application.platform != RuntimePlatform.WebGLPlayer)
        {
            adress = "ws://joye.dev:9001/";
        }
        adress = adress + target;

        // Create WebSocket instance
        Debug.Log("Connecting to " + adress);
        ws = WebSocketFactory.CreateInstance(adress);

        // Add OnError event listener
        ws.OnError += (string errMsg) =>
        {
            Debug.Log("Connection error: " + errMsg);
        };

        // Add OnClose event listener
        ws.OnClose += (WebSocketCloseCode code) =>
        {
            Debug.Log("Connection closed with code: " + code.ToString());
        };

        // Connect to the server
        ws.Connect();

        StartCoroutine(Ping());
    }

    private void Update()
    {
        string state = ws.GetState().ToString();
        if (websocketState != state) websocketState = state;
    }

    public void Send(string txt)
    {
        // To Do Add connection id here to every message 
        if (connected)
        {
            ws.Send(Encoding.UTF8.GetBytes(txt));
        }
    }

    public string GetState()
    {
        return websocketState.ToString();
    }

    private IEnumerator Ping()
    {
        yield return new WaitForSeconds(pingFrequency);
        if (connected) Send("Ping");
        StartCoroutine(Ping());
    }
}
