using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnEnter : MonoBehaviour
{
    public string target = "Target";
    public void OnTriggerEnter(Collider coll)
    {
        if(coll.CompareTag(target))
        {
            Destroy(coll.gameObject);
        }
    }

    public void OnTriggerStay(Collider coll)
    {
        if(coll.CompareTag(target))
        {
            Destroy(coll.gameObject);
        }
    }
}
