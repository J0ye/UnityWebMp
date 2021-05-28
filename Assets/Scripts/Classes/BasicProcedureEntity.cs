using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Msg;

public class BasicProcedureEntity : SyncedEntity
{
    public NetworkManager manager;
    public GameObject testItem;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        manager.basicProcedureEntities.Add(System.Guid.Parse(guid), this);
    }

    public void MakeRPC(string procedureName)
    {
        Invoke(procedureName, 0f);
        RPCMessage msg = new RPCMessage(WebSocketBehaviour.instance.ConnectionID, System.Guid.Parse(guid), procedureName);
        Sync(msg);        
    }

    private void Sync(RPCMessage msg)
    {
        try
        {
            Send(msg.ToJson());
        }
        catch (Exception e)
        {
            Debug.LogError("Could not make RPC called " + msg.procedureName + ". Error: " + e);
        }
    }

    public void SpawnTest()
    {
        if (testItem != null)
        {
            GameObject canvas = GameObject.Find("Canvas");
            Instantiate(testItem, canvas.transform);
        }
    }
}
