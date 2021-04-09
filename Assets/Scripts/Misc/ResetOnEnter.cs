using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetOnEnter : MonoBehaviour
{
    public Vector3 ResetTo = Vector3.zero;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = ResetTo;
    }
}
