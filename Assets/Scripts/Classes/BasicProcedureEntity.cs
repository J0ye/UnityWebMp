using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Msg;

public class BasicProcedureEntity : MonoBehaviour
{
    public NetworkManager manager;
    public GameObject testItem;

    [HideInInspector]
    public WebSocketBehaviour behaviour;
    [HideInInspector]
    public string id;

    // Start is called before the first frame update
    void Start()
    {
        manager.basicProcedureEntities.Add(Guid.Parse(id), this);
    }

    public void MakeRPC(string procedureName)
    {
        Invoke(procedureName, 0f);
        RPCMessage msg = new RPCMessage(manager.player.GetId(), Guid.Parse(id), procedureName);
        if(behaviour != null) behaviour.Send(msg.ToJson());
    }

    public void SpawnTest()
    {
        if (testItem != null)
        {
            GameObject canvas = GameObject.Find("Canvas");
            Instantiate(testItem, canvas.transform);
        }
    }

    public void SetNewGuid()
    {
        id = Guid.NewGuid().ToString();
    }
}
