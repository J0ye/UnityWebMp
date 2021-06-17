using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharePosition : MonoBehaviour
{
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        if(target == null && transform.childCount > 0)
        {
            target = transform.GetChild(0);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(target != null) transform.position = target.position;
    }
}
