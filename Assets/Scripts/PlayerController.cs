using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    private Guid guid;
    private bool setUp = false;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if(Input.anyKey)
        {
            var currentPos = transform.position;
            transform.position = Vector3.MoveTowards(currentPos,
                currentPos + new Vector3(h, v, 0), speed * Time.deltaTime);
        }
    }

    public void SetId(Guid newId)
    {
        if(!setUp)
        {
            guid = newId;
            setUp = true;
        }
    }

    public Guid GetId()
    {
        if (!setUp)
            Debug.LogError("Guid is not set up and can not be read.");

        return guid;
    }

    public bool IsReady()
    {
        return setUp;
    }
}
