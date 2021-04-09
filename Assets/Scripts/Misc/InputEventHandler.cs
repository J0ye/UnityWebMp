using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputEventHandler : MonoBehaviour {

    [Tooltip("These functions will be called, when the the key R is pressed")]
    public UnityEvent onPressR;
    [Tooltip("These functions will be called, when the the key E is pressed")]
    public UnityEvent onPressE;
    [Tooltip("These functions will be called, when the the key I is pressed")]
    public UnityEvent onPressI;
    [Tooltip("These functions will be called, when the the key U is pressed")]
    public UnityEvent onPressU;
    [Tooltip("These functions will be called, when the the key J is pressed")]
    public UnityEvent onPressJ;
    [Tooltip("These functions will be called, when the the return key is pressed")]
    public UnityEvent onPressReturn;
    [Tooltip("These functions will be called, when the the space bar is pressed")]
    public UnityEvent onPressSpace;
    // Use this for initialization
    void Start ()
    {
        StartEvent(onPressR);
        StartEvent(onPressE);
        StartEvent(onPressI);
        StartEvent(onPressU);
        StartEvent(onPressJ);
        StartEvent(onPressReturn);
        StartEvent(onPressSpace);
    }

	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.R))
        {
            onPressR.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            onPressE.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            onPressI.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            onPressU.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            onPressJ.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            onPressReturn.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            onPressSpace.Invoke();
        }
    }

    private void StartEvent(UnityEvent eve)
    {
        if (eve == null)
        {
            eve = new UnityEvent();
        }
    }
}
