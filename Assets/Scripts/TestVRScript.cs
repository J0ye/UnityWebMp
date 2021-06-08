using Msg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class TestVRScript : MonoBehaviour
{
    [Header("VR Objects")]
    public Transform head;
    public Transform lHand;
    public Transform rHand;
    [Space]
    public NetworkManager3D testTarget;
    public Vector2 range;
    public bool move = true;

    private Guid connectionID;
    private LastFrameInfo hLF;
    private LastFrameInfo lLF;
    private LastFrameInfo rLF;
    // Start is called before the first frame update
    void Start()
    {
        connectionID = Guid.NewGuid();
        hLF = new LastFrameInfo(head);
        lLF = new LastFrameInfo(lHand);
        rLF = new LastFrameInfo(rHand);
    }

    // Update is called once per frame
    void Update()
    {
        if(move)
        {
            Simulate();
            if(hLF.CompareValues(head, 1) || lLF.CompareValues(lHand, 1) || rLF.CompareValues(rHand, 1))
            {
                SendUpdate();
            }
        }
    }

    private void SendUpdate()
    {
        VRPlayerMessage msg = new VRPlayerMessage(head, lHand, rHand);
        msg.connectionID = connectionID.ToString();
        Debug.Log("Testing Update");
        testTarget.SimulateMessage(msg.ToJson());
        TransformMessage tMsg = new TransformMessage(transform);
        tMsg.connectionID = connectionID.ToString();
        testTarget.SimulateMessage(tMsg.ToJson());
        hLF.UpdateValues(head);
        lLF.UpdateValues(lHand);
        rLF.UpdateValues(rHand);
    }

    private void Simulate()
    {
        if(!AnyTweens())
        {
            Debug.Log("Testing");
            head.DOMove(RandomVector() + head.position, 1f, false);
            lHand.DOMove(RandomVector()+ lHand.position, 1f, false);
            rHand.DOMove(RandomVector()+ rHand.position, 1f, false);
        }
    }

    private bool AnyTweens()
    {
        return DOTween.IsTweening(head) || DOTween.IsTweening(lHand) || DOTween.IsTweening(rHand);
    }

    private Vector3 RandomVector()
    {
        float x = UnityEngine.Random.Range(range.x, range.y);
        float y = UnityEngine.Random.Range(range.x, range.y);
        float z = UnityEngine.Random.Range(range.x, range.y);

        return new Vector3(x, y, z);
    }
}
