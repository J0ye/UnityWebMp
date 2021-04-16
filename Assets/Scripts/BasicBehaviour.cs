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

    protected WebSocket ws;
    protected bool connected = false;

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
            Debug.Log("Chat error: " + errMsg);
        };

        // Add OnClose event listener
        ws.OnClose += (WebSocketCloseCode code) =>
        {
            Debug.Log("Chat closed with code: " + code.ToString());
        };

        // Connect to the server
        ws.Connect();

        StartCoroutine(Ping());
    }

    public void Send(string txt)
    {
        if (connected)
        {
            ws.Send(Encoding.UTF8.GetBytes(txt));
        }
    }

    private IEnumerator Ping()
    {
        yield return new WaitForSeconds(1f);
        if (connected) Send("Ping");
        StartCoroutine(Ping());
    }
}
