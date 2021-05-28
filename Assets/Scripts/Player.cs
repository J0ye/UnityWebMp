using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity PlayerController base class
/// </summary>
public class Player : MonoBehaviour
{
    [Header("Input Settings")]
    public DPadInput visualInput;
    public List<KeyCode> movementKeys = new List<KeyCode>();

    protected virtual Vector3 GetInputAxis()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // calculate direction vectors based on orientation
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
}
