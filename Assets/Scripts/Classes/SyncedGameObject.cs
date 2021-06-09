using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Msg;
using DG.Tweening;
using System;

[RequireComponent(typeof(Rigidbody))]
public class SyncedGameObject : SyncedEntity
{
    public int comparisonRange = 2;

    protected LastFrameInfo lastFrame;
    protected Rigidbody rb;

    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        lastFrame = new LastFrameInfo(transform);
    }

    void LateUpdate()
    {
        if (isDebug) Debug.Log("Comparing " + lastFrame.position + " with " + transform.position + ", "
            + lastFrame.scale + " with " + transform.localScale + " and " 
            + lastFrame.rotation + " with " + transform.rotation);

        if(DidValuesChange() && !rb.isKinematic)
        {
            //Will send an update message to the server, if any of the values are different from last frame
            UpdateValuesOnServer();
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            rb.isKinematic = false;
        }
    }

    public bool DidValuesChange()
    {
        return !lastFrame.CompareValues(transform, 1);
    }

    protected void UpdateValuesOnServer()
    {
        // Dont have to check id. Has to be a valid guid, because of parent structure of this class
        SyncedGameObjectMessage msg = new SyncedGameObjectMessage(this);
        Send(msg);
    }

    protected override IEnumerator RecieveValues(string msg)
    {
        yield return 0;
        try
        {
            SyncedGameObjectMessage newData = SyncedGameObjectMessage.FromJson(msg);

            LastFrameInfo temp = new LastFrameInfo();
            temp.position = newData.transform.position;
            temp.scale = newData.transform.scale;
            temp.rotation = newData.transform.rotation;

            if (!temp.CompareValues(transform, 1))
            {
                rb.isKinematic = true;
                //Only update the values if they are different
                transform.DOMove(temp.position, 0.01f);
                transform.DOScale(temp.scale, 0.01f);
                transform.DORotateQuaternion(temp.rotation, 0.01f);
                lastFrame.UpdateValues(transform);
            }
        }
        catch(Exception e)
        {
            Debug.LogError(gameObject.name + " recieved a faulty message from the server. Message: " + msg + " It led to this error: " + e);
        }
    }
}
