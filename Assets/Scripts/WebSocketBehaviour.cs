﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Use plugin namespace
using HybridWebSocket;

public class WebSocketBehaviour : MonoBehaviour
{
    public string adress = "ws://joye.dev:9000/2dmp";
    public static WebSocketBehaviour instance;
    public bool isDebug = false;

    protected WebSocket ws;
    protected bool connected = false;

    // Use this for initialization
    protected virtual void Start()
    {
        // Singelton
        if (instance != null)
            Destroy(this);

        if (instance == null)
            instance = this;

        // Create WebSocket instance
        Debug.Log("Connecting to " + adress);
        ws = WebSocketFactory.CreateInstance(adress);

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
            if(isDebug) Debug.Log("WS received message: " + Encoding.UTF8.GetString(msg));
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

        StartCoroutine(Ping());
    }

    protected virtual void OnApplicationQuit()
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
    private IEnumerator Ping()
    {
        yield return new WaitForSeconds(1f);
        if (connected) Send("Ping");
        StartCoroutine(Ping());
    }
}