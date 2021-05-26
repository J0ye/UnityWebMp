using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Msg;
using DG.Tweening;

public class SyncedGameObject : SyncedEntity
{
    public int comparisonrange = 2;

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
        TransformMessage msg = new TransformMessage(guid, transform);
        Send(msg.ToJson());
        if (isDebug) Debug.Log("Updated values on server");
    }

    protected override void RecieveValues(TransformMessage msg)
    {
        LastFrameInfo temp = new LastFrameInfo();
        temp.position = msg.position;
        temp.scale = msg.scale;
        temp.rotation = msg.rotation;

        if(!temp.CompareValues(transform, 1))
        {
            //Only update the values if they are different
            transform.DOMove(msg.position, 0.01f);
            transform.DOScale(msg.scale, 0.01f);
            transform.DORotateQuaternion(msg.rotation, 0.01f);
            lastFrame.UpdateValues(temp);
        }
    }
}
