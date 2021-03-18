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
    [Tooltip ("Determines how often information is sent to the server, in seconds.")]
    public float pingFrequency = 1f;
    public bool debug = false;

    private WebSocket2DMp webSocket;
    private bool readyForId = false;

    void Start()
    {
        StartCoroutine(SetUpSocket());
        StartCoroutine(SendPlayerPos());
        StartCoroutine(GetPlayers());
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
            if(debug) Debug.Log("Sending " + msg);
            webSocket.Send(msg);
        }
        yield return new WaitForSeconds(pingFrequency);
        StartCoroutine(SendPlayerPos());
    }

    private IEnumerator GetPlayers()
    {
        if (webSocket != null)
        {
            webSocket.Send("Get:" + player.GetId());
        }
        yield return new WaitForSeconds(pingFrequency);
        StartCoroutine(GetPlayers());
    }

    private void ProcessMessage(string msg)
    {
        // If the message is about the players new ID
        if(readyForId && msg.Contains("ID"))
        {
            var parts = msg.Split(": ".ToCharArray());
            Guid newId = Guid.Parse(parts[2]);
            player.SetId(newId);
        }
        // If the message is about the list of players
        else if(msg.Contains("Players:"))
        {
            var parts = msg.Split(":".ToCharArray());
            if (debug) Debug.Log("Player information: " + parts[1]);
            parts = parts[1].Split("%".ToCharArray());
            if(debug) Debug.Log("Parts Count: " + parts.Length);
            if(parts.Length > 1)
            {
                if(debug) Debug.Log("player 0: " + parts[0] + " and Player 1: " + parts[1]);
            }
        }
    }

    private void OnApplicationQuit()
    {
        CloseConnection();
    }

    private void CloseConnection()
    {
        string msg = "Delete:" + player.GetId();
        webSocket.Send(msg);
    }
}
