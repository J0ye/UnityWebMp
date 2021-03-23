﻿using System;
using UnityEngine;

public class PlayerController : Player
{
    public float speed = 5f;

    void Update()
    {
        if(Input.anyKey)
        {
            var currentPos = transform.position;
            Vector3 target = GetInputAxis();
            transform.position = Vector3.MoveTowards(currentPos,
                currentPos + target, speed * Time.deltaTime);
        }
    }
}
