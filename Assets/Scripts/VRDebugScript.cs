using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebXR;

public class VRDebugScript : MonoBehaviour
{
    public WebXRManager xRManager;
    public PlayerControllerMixed player;
    protected SimpleUIManager ui;
    protected bool displayed = false;

    // Update is called once per frame
    void Update()
    {
        if(ui != null)
        {
            ui.stringList.Clear();
            DisplaInfo();
            ui.stringList.Add("Oritations:");
            ui.stringList.Add("Player forward: " + player.transform.forward);
            ui.stringList.Add("Cameras forward: " + xRManager.transform.GetChild(2).forward);
            ui.stringList.Add("Camera Main forward: " + xRManager.transform.GetChild(2).GetChild(0).forward);
            ui.stringList.Add("Camera L forward: " + xRManager.transform.GetChild(2).GetChild(1).forward);
            ui.stringList.Add("Camera R forward: " + xRManager.transform.GetChild(2).GetChild(2).forward);
            ui.stringList.Add("Camera ARL forward: " + xRManager.transform.GetChild(2).GetChild(3).forward);
            ui.stringList.Add("Camera ARR forward: " + xRManager.transform.GetChild(2).GetChild(4).forward);
            ui.stringList.Add("Camera Follower forward: " + xRManager.transform.GetChild(2).GetChild(5).forward);
            ui.stringList.Add("Positions: ");
            ui.stringList.Add("Left hand: " + xRManager.transform.GetChild(0).position.ToString());
            ui.stringList.Add("Right hand: " + xRManager.transform.GetChild(1).position.ToString());
            ui.stringList.Add("Head: " + xRManager.transform.GetChild(2).position.ToString());
        } 
        else if(SimpleUIManager.Instance != null)
        {
            ui = SimpleUIManager.Instance;
        }
    }

    protected void DisplaInfo()
    {
        if(xRManager.gameObject.activeSelf)
        {
            try
            {
                ui.stringList.Add("VR Support: " + xRManager.isSupportedVR);
                ui.stringList.Add("XR State: " + xRManager.XRState.ToString());
                ui.stringList.Add("Subsystem available: " + xRManager.isSubsystemAvailable.ToString());
            }
            catch (Exception e)
            {
                ui.stringList.Add("Error: " + e);
            }
        }
    }
}
