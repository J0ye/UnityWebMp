using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Msg;
using DG.Tweening;
using System;

public class SyncedGameObject : SyncedEntity
{
    public int comparisonRange = 2;

    protected LastFrameInfo lastFrame;

    public override void Start()
    {
        base.Start();
        lastFrame = new LastFrameInfo(transform);
    }

    void LateUpdate()
    {
        if (isDebug) Debug.Log("Comparing " + lastFrame.position + " with " + transform.position + ", "
            + lastFrame.scale + " with " + transform.localScale + " and " 
            + lastFrame.rotation + " with " + transform.rotation);

        if(!lastFrame.CompareValues(transform, 1))
        {
            //Will send an update message to the server, if any of the values are different from last frame
            UpdateValuesOnServer();
        }
    }

    protected void UpdateValuesOnServer()
    {
        // Dont have to check id. Has to be a valid guid, because of parent structure of this class
        SyncedGameObjectMessage msg = new SyncedGameObjectMessage(guid, WebSocketBehaviour.instance.ConnectionID.ToString(), transform);
        Send(msg);
        if (isDebug) Debug.Log("Updated values on server");
    }

    protected override void RecieveValues(string msg)
    {
        try
        {
            SyncedGameObjectMessage newData = SyncedGameObjectMessage.FromJson(msg);

            // Exit if the message is not of type WebsocketMessageType.SyncedGameObject, or if the message was created by this game instance.
            if (newData.type != WebsocketMessageType.SyncedGameObject || Guid.Parse(newData.messageGuid) == WebSocketBehaviour.instance.ConnectionID) return;


            LastFrameInfo temp = new LastFrameInfo();
            temp.position = newData.transform.position;
            temp.scale = newData.transform.scale;
            temp.rotation = newData.transform.rotation;

            if (!temp.CompareValues(transform, 1))
            {
                //Only update the values if they are different
                transform.DOMove(newData.transform.position, 0.01f);
                transform.DOScale(newData.transform.scale, 0.01f);
                transform.DORotateQuaternion(newData.transform.rotation, 0.01f);
                lastFrame.UpdateValues(temp);
            }
        }
        catch(Exception e)
        {
            Debug.LogError(gameObject.name + " recieved a faulty message from the server. Message: " + msg + " It led to this error: " + e);
        }
    }
}
