using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
    public Vector3 target = new Vector3();
    public float speed = 2f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(target, speed * Time.deltaTime);
    }
}
