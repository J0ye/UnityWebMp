using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebXR;

public class VRPlayerController : PlayerController
{
    [Header("WebXR camera set objects")]
    public Transform head;
    public WebXRController lController;
    public WebXRController rController;
    [HideInInspector]
    public Transform lHand { get => lController.transform; }
    [HideInInspector]
    public Transform rHand { get => rController.transform; }

    protected override bool ShouldMove()
    {
        Debug.Log(Input.GetAxisRaw("Horizontal"));
        return base.ShouldMove();
    }
}
