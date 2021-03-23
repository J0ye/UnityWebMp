using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : Player
{
    public float speed = 5f;

    void Update()
    {
        if (Input.anyKey)
        {
            var currentPos = transform.position;
            Vector3 target = new Vector3(GetInputAxis().x, GetInputAxis().z, 0); // Parse GetInputAxis to 2D format
            transform.position = Vector3.MoveTowards(currentPos,
                currentPos + target, speed * Time.deltaTime);
        }
    }
}
