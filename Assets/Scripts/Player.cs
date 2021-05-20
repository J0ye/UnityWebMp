using System;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    [Header("Input Settings")]
    public DPadInput visualInput;
    public List<KeyCode> movementKeys = new List<KeyCode>();
    private Guid guid;
    private bool setUp = false;

    protected virtual Vector3 GetInputAxis()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        /*if( visualInput != null)
        {
            h = visualInput.vector.x;
            v = visualInput.vector.y;
        }*/

        Vector3 forward = transform.forward * v;
        Vector3 sideways = transform.right * h;
        
        Vector3 re = forward + sideways;
        return re;
    }

    protected virtual bool ShouldMove()
    {
        foreach (KeyCode kc in movementKeys)
        {
            if (Input.GetKey(kc))
            {
                return true;
            }
        }
        if (visualInput != null)
        {
            if (visualInput.x != 0 || visualInput.y != 0) return true;
        }

        return false;
    }

    public void SetId(Guid newId)
    {
        if (!setUp)
        {
            guid = newId;
            setUp = true;
        }
    }

    public Guid GetId()
    {
        if (!setUp)
            return Guid.Empty;

        return guid;
    }

    public bool IsReady()
    {
        return setUp;
    }
}
