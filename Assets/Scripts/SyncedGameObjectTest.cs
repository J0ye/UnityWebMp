using Msg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SyncedGameObjectTest : MonoBehaviour
{
    public SyncedGameObject target;

    [Space]
    public Vector2 range;
    [Range(1f, 20f)]
    public float timeBetweenNewValues = 6f;
    public bool move = true;

    private Guid connectionID;
    private Rigidbody rb;
    private LastFrameInfo lastFrame;
    // Start is called before the first frame update
    void Start()
    {
        connectionID = Guid.NewGuid();
        rb = GetComponent<Rigidbody>();
        lastFrame = new LastFrameInfo(transform);
        StartCoroutine(Simulate());
    }

    // Update is called once per frame
    void Update()
    {
        if (!lastFrame.CompareValues(transform, 1))
        {
            SendUpdate();
            lastFrame.UpdateValues(transform);
        }
    }

    private IEnumerator Simulate()
    {
        yield return new WaitForSeconds(timeBetweenNewValues);
        if (move)
        {
            rb.AddForce(RandomVector());
        }
        StartCoroutine(Simulate());
    }

    private void SendUpdate()
    {
        SyncedGameObjectMessage msg = new SyncedGameObjectMessage(connectionID.ToString(), target.id, transform.position, transform.localScale, transform.rotation);
        target.TestProcessMessage(msg.ToJson());
    }

    private Vector3 RandomVector()
    {
        float x = UnityEngine.Random.Range(range.x, range.y);
        float y = UnityEngine.Random.Range(range.x, range.y);
        float z = UnityEngine.Random.Range(range.x, range.y);

        return new Vector3(x, y, z);
    }
}