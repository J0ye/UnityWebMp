using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


// Use plugin namespace
using HybridWebSocket;

public class NetworkManager : MonoBehaviour
{
    public PlayerController player;

    private WebSocket2DMp webSocket;
    private bool readyForId = false;

    void Start()
    {
        StartCoroutine(SetUpSocket());
        StartCoroutine(SendPlayerPos());
    }

    private IEnumerator SetUpSocket()
    {
        while (WebSocket2DMp.instance == null)
        {
            Debug.Log("Waiting for instance");
            yield return new WaitForSeconds(0.5f);
        }

        webSocket = WebSocket2DMp.instance;
        webSocket.GetWS().OnOpen += () =>
        {
            readyForId = true;
        };
        webSocket.GetWS().OnMessage += (byte[] msg) =>
        {
            ProcessMessage(Encoding.UTF8.GetString(msg));
        };
    }

    private IEnumerator SendPlayerPos()
    {
        if(webSocket != null && player.IsReady())
        {
            float x = player.transform.position.x;
            float y = player.transform.position.y;
            string msg = "Pos:" + x + "/" + y + "/" + player.GetId();
            Debug.Log("Sending " + msg);
            webSocket.Send(msg);
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(SendPlayerPos());
    }

    private void ProcessMessage(string msg)
    {
        if(readyForId && msg.Contains("ID"))
        {
            var parts = msg.Split(": ".ToCharArray());
            Guid newId = Guid.Parse(parts[2]);
            player.SetId(newId);
        }
    }
}
