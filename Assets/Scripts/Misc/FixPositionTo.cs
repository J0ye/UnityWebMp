using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPositionTo : MonoBehaviour
{
    public GameObject target;

    // Update is called once per frame
    void LateUpdate()
    {
        if(target != null)
            transform.position = target.transform.position;
    }
}
