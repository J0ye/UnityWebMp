using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Msg;

public class SyncedGameObject : SyncedEntity
{
    public int comparisonrange = 2;

    protected LastFrameInfo lastFrame;

    void Start()
    {
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
}
