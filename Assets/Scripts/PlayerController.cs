using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerController : Player
{
    [Header("Movment Options")]
    public float speed = 5f;
    public float fallSpeed = 5f;
    public float hoverHeight = 1f;
    public float jumpStrength = 5f;
    public LayerMask groundLayer;

    private float startingFallSpeed;
    private float startingHoverHeight;
    private bool grounded = true;

    private void Start()
    {
        startingFallSpeed = fallSpeed;
        startingHoverHeight = hoverHeight;
    }

    void Update()
    {
        // Walking
        if (ShouldMove())
        {
            Vector3 target = new Vector3(GetInputAxis().x, 0, GetInputAxis().z); // ignore the y axis
            Move(target);
        }

        // Jumping
        if (Input.GetKey(KeyCode.Space) && grounded)
        {
            Jump();
        }

        Hover();
    }

    protected void Move(Vector3 target)
    {
        var currentPos = transform.position;
        transform.position = Vector3.MoveTowards(currentPos,
            currentPos + target, speed * Time.deltaTime);
    }

    protected void Jump()
    {
        fallSpeed *= -jumpStrength;
        hoverHeight -= 1;
        DOTween.To(() => hoverHeight, x => hoverHeight = x, startingHoverHeight, 1);
        DOTween.To(() => fallSpeed, x => fallSpeed = x, startingFallSpeed, 1);
    }

    protected void ResetJump()
    {
        hoverHeight = startingHoverHeight;
        fallSpeed = startingFallSpeed;
        DOTween.Clear();
    }

    protected void Hover()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, -Vector3.up, Color.green, hoverHeight);
        if (!Physics.Raycast(transform.position, -Vector3.up, out hit, hoverHeight + 0.1f, groundLayer))
        {
            grounded = false;
            // The player is too high, move the player down by deltaTime * fallSpeed
            transform.position = Vector3.MoveTowards(transform.position, transform.position - Vector3.up, fallSpeed * Time.deltaTime);
        }
        else
        {
            float dis = Vector3.Distance(transform.position, hit.point);
            if (dis < hoverHeight)
            {
                // The player is too low, move the player up by deltaTime * speed
                transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up, speed * Time.deltaTime);
            }
            grounded = true;
            ResetJump();
        }

    }
}
