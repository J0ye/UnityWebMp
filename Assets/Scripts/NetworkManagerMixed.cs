using Msg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebXR;

public class NetworkManagerMixed : NetworkManager3D
{
    [Tooltip("Use the WebXRCameraSet object for this")]
    public WebXRManager vrPlayer;

    protected Dictionary<Guid, GameObject> onlineVRPlayerObjects = new Dictionary<Guid, GameObject>();
    protected Transform head;
    protected Transform leftHand;
    protected Transform rightHand;
    protected LastFrameInfo headInfo = new LastFrameInfo();
    protected LastFrameInfo leftHandInfo = new LastFrameInfo();
    protected LastFrameInfo rightHandInfo = new LastFrameInfo();

    public bool IsVR()
    {
        return vrPlayer.gameObject.activeSelf;
    }

    protected override void Start()
    {
        base.Start();
        leftHand = vrPlayer.transform.GetChild(0); // First child of the WebXRCameraSet should always be the left hand
        rightHand = vrPlayer.transform.GetChild(1); //The right hand should always be second
        head = vrPlayer.transform.GetChild(2).GetChild(0);
        leftHandInfo.UpdateValues(leftHand);
        rightHandInfo.UpdateValues(rightHand);
        headInfo.UpdateValues(head);
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        //if (debug) Debug.Log("Comparing " + lastFrame.position.Round(1) + " with " + player.position.Round(1));
        if ((!headInfo.CompareValues(head, 1) 
            || !leftHandInfo.CompareValues(leftHand, 1) 
            || !rightHandInfo.CompareValues(rightHand, 1)) && IsVR())
        {
            SendTransform();
        }
    }

    protected override void SendTransform()
    {
        base.SendTransform();
        if (IsVR())
        {
            VRPlayerMessage msg = new VRPlayerMessage(vrPlayer);
            WebSocketBehaviour.instance.Send(msg);
            headInfo.UpdateValues(head);
            leftHandInfo.UpdateValues(leftHand);
            rightHandInfo.UpdateValues(rightHand);
            if (debug) Debug.Log("Finisehed vr transform informations");
        }
    }
}
