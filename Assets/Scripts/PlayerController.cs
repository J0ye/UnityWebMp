using System;
using UnityEngine;

public class PlayerController : Player
{
    public float speed = 5f;
    public float fallSpeed = 5f;
    public float hoverHeight = 1f;
    public LayerMask groundLayer;

    void Update()
    {
        if (Input.anyKey)
        {
            var currentPos = transform.position;
            Vector3 target = new Vector3 (GetInputAxis().x, 0, GetInputAxis().z); // ignore the y axis
            transform.position = Vector3.MoveTowards(currentPos,
                currentPos + target, speed * Time.deltaTime);
        }

        RaycastHit hit;
        Debug.DrawRay(transform.position, -Vector3.up, Color.green, hoverHeight);
        if(!Physics.Raycast(transform.position, -Vector3.up, out hit, hoverHeight +0.1f, groundLayer))
        {
            // The player is too high, move the player down by deltaTime * fallSpeed
            transform.position = Vector3.MoveTowards(transform.position, transform.position - transform.up, fallSpeed * Time.deltaTime);
        }else
        {
            float dis = Vector3.Distance(transform.position, hit.point);
            if(dis < hoverHeight)
            {
                // The player is too low, move the player up by deltaTime * speed
                transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.up, speed * Time.deltaTime);
            }
        }
    }
}
