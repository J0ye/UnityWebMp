using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WebXR;

public class XRInputEventHandler : InputEventHandler
{
    [Space]
    [Tooltip("These functions will be called, when the application runs in VR. This event will only be fired once.")]
    public UnityEvent onOpenInVR;

    protected bool onOpenInVRDone = false;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        SetUpEvent(onOpenInVR);
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        onOpenInVR.Invoke();
        onOpenInVRDone = true;
    }
}
