using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Msg;
using System;

public class NetworkManagerVR : NetworkManager3D
{
    public VRPlayerController playerVR;

    private LastFrameInfo headLastFrame;
    private LastFrameInfo lHandLastFrame;
    private LastFrameInfo rHandLastFrame;

    protected override void Start()
    {
        base.Start();
        headLastFrame = new LastFrameInfo(playerVR.head);
        lHandLastFrame = new LastFrameInfo(playerVR.lHand);
        rHandLastFrame = new LastFrameInfo(playerVR.rHand);
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();
        if(!headLastFrame.CompareValues(playerVR.head, 1) || !lHandLastFrame.CompareValues(playerVR.lHand, 1) || 
            !rHandLastFrame.CompareValues(playerVR.rHand, 1))
        {
            SendTransform();
        }
    }

    protected override void SendTransform()
    {
        try
        {
            if (WebSocketBehaviour.instance != null)
            {
                //if (debug) Debug.Log("Sending transform informations");
                VRPlayerMessage msg = new VRPlayerMessage(playerVR);
                WebSocketBehaviour.instance.Send(msg);
                lastFrame.UpdateValues(playerVR.head);
                lHandLastFrame.UpdateValues(playerVR.lHand);
                rHandLastFrame.UpdateValues(playerVR.rHand);
                if (debug) Debug.Log("Finisehed transform informations");
            }
        }
        catch (Exception e)
        {
            Debug.Log("System failed to send vr player transform data to server, because of: " + e);
        }
    }
}
