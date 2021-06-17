using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerMixed : PlayerController
{
    public Transform headAlternative;
    protected override Vector3 GetInputAxisRaw()
    {
        if(headAlternative == null || !headAlternative.parent.parent.gameObject.activeSelf)
        {
            return base.GetInputAxisRaw();
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // calculate direction vectors based on orientation
        Vector3 forward = headAlternative.forward * v;
        Vector3 sideways = headAlternative.right * h;

        Vector3 re = forward + sideways;
        return re;
    }
}
