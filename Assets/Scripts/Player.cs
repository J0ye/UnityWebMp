using System;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public List<KeyCode> movementKeys = new List<KeyCode>();
    private Guid guid;
    private bool setUp = false;

    protected virtual Vector3 GetInputAxis()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 forward = transform.forward * v;
        Vector3 sideways = transform.right * h;
        
        Vector3 re = forward + sideways;
        return re;
    }

    protected virtual bool ShouldMove()
    {
        foreach(KeyCode kc in movementKeys)
        {
            if(Input.GetKey(kc))
            {
                return true;
            }
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
            Debug.LogError("Guid is not set up and can not be read.");


        return guid;
    }

    public bool IsReady()
    {
        return setUp;
    }
}
