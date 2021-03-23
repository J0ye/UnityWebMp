using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using DG.Tweening;

public class NetworkManager2D : NetworkManager
{
    private WebSocket2DMp webSocket;

    protected override IEnumerator SetUpSocket()
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
}
