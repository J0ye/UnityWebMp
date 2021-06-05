using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Msg;

public class NetworkManager3D : NetworkManager
{
    public PlayerController player;
    public GameObject onlinePlayerPrefab;

    public Dictionary<Guid, BasicProcedureEntity> basicProcedureEntities = new Dictionary<Guid, BasicProcedureEntity>();

    protected Dictionary<Guid, GameObject> onlinePlayerObjects = new Dictionary<Guid, GameObject>();
    protected SyncFloat score;
    protected LastFrameInfo lastFrame;

    public void UpScore()
    {
        Debug.Log("Upping score");
        score.Value++;
    }

    public void ScoreInConsole()
    {
        Debug.Log("Score is: " + score.Value.ToString());
    }

    protected override void Start()
    {
        base.Start();
        DOTween.Init();
        lastFrame = new LastFrameInfo(player.transform);
    }

    protected void LateUpdate()
    {
        //if (debug) Debug.Log("Comparing " + lastFrame.position.Round(1) + " with " + player.transform.position.Round(1));
        if (!lastFrame.CompareValues(player.transform, 1))
        {
            SendTransform();
        }
    }

    protected override IEnumerator SetUpSocket()
    {
        StartCoroutine(base.SetUpSocket());
        yield return 0;

        new SyncedStrings();
        new SyncedFloats();
        score = new SyncFloat("score", 0);
    }

    protected virtual void SendTransform()
    {
        try
        {
            if (WebSocketBehaviour.instance != null)
            {
                //if (debug) Debug.Log("Sending transform informations");
                TransformMessage msg = new TransformMessage(WebSocketBehaviour.instance.ConnectionID, player.transform);
                WebSocketBehaviour.instance.Send(msg);
                lastFrame.UpdateValues(player.transform);
                if (debug) Debug.Log("Finisehed transform informations");
            }
        }
        catch (Exception e)
        {
            Debug.Log("System failed to send player transform data to server, because of: " + e);
        }
    }

    /// <summary>
    /// Updates the existence of an online players local avatar.
    /// This has to be an IEnumerator, so it can be processed in another thread.
    /// Same thread would crash the OnMessageRecieved event of the websocket.
    /// </summary>
    /// <param name="update">New values</param>
    /// <returns></returns>
    protected virtual IEnumerator UpdateOnlinePlayer(TransformMessage update)
    {
        try
        {
            Debug.Log("also Here");
            if (!onlinePlayerObjects.ContainsKey(update.Guid))
            {
                onlinePlayerObjects.Add(update.Guid, Instantiate(onlinePlayerPrefab));
            }

            if (update.position == new Vector3(-9999, -9999, -9999))
            {
                // The player is at the exit position.
                RemoveOnlinePlayer(update.Guid);
            }
            else
            {
                if (debug) Debug.Log("Updating online player " + update.connectionID + " with " + update.position);
                onlinePlayerObjects[update.Guid].transform.position = update.position;
                onlinePlayerObjects[update.Guid].transform.DOMove(update.position, pingFrequency / 2, false);
                onlinePlayerObjects[update.Guid].transform.DOLocalRotateQuaternion(update.rotation, pingFrequency / 2);
                onlinePlayerObjects[update.Guid].transform.DOScale(update.scale, pingFrequency / 2);

                Debug.Log("also even Here");
            }
        } catch(Exception e)
        {
            Debug.LogError("Could not set valeus for online player: " + e);
        }
        yield return 0;
    }

    protected virtual void RemoveOnlinePlayer(Guid gameID)
    {
        try
        {
            Destroy(onlinePlayerObjects[gameID]);
            onlinePlayerObjects.Remove(gameID);
        }
        catch (Exception e)
        {
            Debug.LogError("Something went wrong while deleting an online player: " + e);
        }
    }

    protected void FinishRPC(RPCMessage msg)
    {
        Debug.Log("Recieved RPC call with: " + msg);
        Guid targetId = Guid.Parse(msg.procedureGuid);
        foreach (KeyValuePair<Guid, BasicProcedureEntity> bpe in basicProcedureEntities)
        {
            if (bpe.Key == targetId)
            {
                bpe.Value.Invoke(msg.procedureName, 0f);
            }
        }
    }

    protected override void ExecuteOnJson(IDMessage iDMessage, string msg)
    {
        try
        {
            switch (iDMessage.type)
            {
                case WebsocketMessageType.ID:
                    WebSocketBehaviour.instance.ConnectionID = iDMessage.Guid;
                    if (debug) Debug.Log("New Id");
                    break;
                case WebsocketMessageType.Position:
                    PositionMessage positionMessage = PositionMessage.FromJson(msg);
                    StartCoroutine(UpdateOnlinePlayer((TransformMessage)positionMessage));
                    break;
                case WebsocketMessageType.Transform:
                    TransformMessage transformMessage = TransformMessage.FromJson(msg);
                    Debug.Log("Here: " +transformMessage.ToJson());
                    StartCoroutine(UpdateOnlinePlayer(transformMessage));
                    break;
                case WebsocketMessageType.Request:
                    WebsocketRequest req = WebsocketRequest.FromJson(msg);
                    if (req.requestType == WebsocketMessageType.Position) SendTransform();
                    break;
                case WebsocketMessageType.SyncString:
                    SyncString.FromJson(msg);
                    break;
                case WebsocketMessageType.SyncFloat:
                    SyncFloat.FromJson(msg);
                    break;
                case WebsocketMessageType.RPC:
                    FinishRPC(RPCMessage.FromJson(msg));
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("A message with type " + iDMessage.type + " and ID " + iDMessage.connectionID + " led to an error: " + e);
        }
    }
}
