using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Msg;
using DG.Tweening;

public class NetworkManager : MonoBehaviour
{
    public PlayerController player;
    public GameObject onlinePlayerPrefab;
    [Tooltip ("Determines how often information is sent to the server, in seconds.")]
    public float pingFrequency = 1f;
    public bool debug = false;

    [HideInInspector]
    public WebSocketBehaviour behaviour;
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

    public virtual void SetPingFrequency(float input)
    {
        pingFrequency = input;
    }

#region Test Functions 
    public void TestSyncVar()
    {
        SyncString testString = new SyncString("MyTestString", "MyTestValue");
        SyncFloat testFloat = new SyncFloat("MyTestFloat", 7.4f);
        string tempS = testString.ToString();
        string tempF = testFloat.ToString();
        SyncString parsedS = SyncString.Parse(tempS);
        SyncFloat parsedF = SyncFloat.Parse(tempF);
        Debug.Log("String as json: " + testString.ToJson(behaviour.ConnectionID));
        Debug.Log("And back to SyncVar " + SyncString.FromJson(testString.ToJson(behaviour.ConnectionID)).CallName 
            + " " + SyncString.FromJson(testString.ToJson(behaviour.ConnectionID)).Value);
        Debug.Log("Float as json: " + testFloat.ToJson(behaviour.ConnectionID));
        Debug.Log("And back to SyncVar " + SyncFloat.FromJson(testFloat.ToJson(behaviour.ConnectionID)).CallName
            + " " + SyncFloat.FromJson(testFloat.ToJson(behaviour.ConnectionID)).Value);
    }

    public void TestNewJsonClass()
    {
        //PositionMessage msg = new PositionMessage(behaviour.GameID.ToString(), player.transform.position);
        TransformMessage msg = new TransformMessage(behaviour.ConnectionID, player.transform);
        Debug.Log("As json: " + msg.ToJson());
        TransformMessage b = TransformMessage.FromJson(msg.ToJson());
        Debug.Log("Parsed back from Json: Type: " + b.type +  " and rotation" + b.rotation
            + " " + b.position);
        PositionMessage pm = PositionMessage.FromJson(msg.ToJson());
        Debug.Log("Converted to position: " + pm.position);
        TransformMessage temp = (TransformMessage)pm;
        Debug.Log("Cast as from position" + temp.position);
    }
#endregion

    protected virtual void Start()
    {
        StartCoroutine(SetUpSocket());
        lastFrame = new LastFrameInfo(transform);
    }

    protected void LateUpdate()
    {
        if (debug) Debug.Log("Comparing " + lastFrame.position.Round(1) + " with " + player.transform.position.Round(1));
        if(lastFrame.position.Round(1) != player.transform.position.Round(1))
        {
            SendTransform();
        }
    }

    protected virtual IEnumerator SetUpSocket()
    {
        Func<bool> tempFunc = () => WebSocketBehaviour.WebSocketStatus();
        yield return new WaitWhile(tempFunc);

        behaviour = WebSocketBehaviour.instance;
        behaviour.GetWS().OnMessage += (byte[] msg) =>
        {
            ProcessMessage(Encoding.UTF8.GetString(msg));
        };

        new SyncedStrings(behaviour, Guid.NewGuid());
        new SyncedFloats(behaviour, Guid.NewGuid());
        score = new SyncFloat("score", 0);
    }

    protected virtual void SendTransform()
    {
        if (behaviour == null)
        {
            return;
        }

        if (debug) Debug.Log("Sending Position");
        TransformMessage temp = new TransformMessage(behaviour.ConnectionID, player.transform);
        string msg = JsonUtility.ToJson(temp);
        behaviour.Send(msg);
        lastFrame.UpdateValues(player.transform);
        if (debug) Debug.Log("Finisehed sending Player Pos");        
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
        if(!onlinePlayerObjects.ContainsKey(update.Guid))
        {
            onlinePlayerObjects.Add(update.Guid, Instantiate(onlinePlayerPrefab));
        }

        if (update.position == new Vector3(-9999, -9999, -9999))
        {
            // The player is at the exit position.
            RemoveOnlinePlayer(update.Guid);
        } else
        {
            onlinePlayerObjects[update.Guid].transform.DOMove(update.position, pingFrequency / 2, false);
            onlinePlayerObjects[update.Guid].transform.DOLocalRotateQuaternion(update.rotation, pingFrequency / 2);
            onlinePlayerObjects[update.Guid].transform.DOScale(update.scale, pingFrequency / 2);
        }
        yield return 0;
    }

    protected virtual void RemoveOnlinePlayer(Guid gameID)
    {
        try
        {
            Destroy(onlinePlayerObjects[gameID]);
            onlinePlayerObjects.Remove(gameID);
        } catch (Exception e)
        {
            Debug.LogError("Something went wrong while deleting an online player: " + e);
        }
    }

    protected virtual void ProcessMessage(string msg)
    {
        try
        {
            IDMessage target = IDMessage.FromJson(msg);
            if (debug) Debug.Log("From Json: guid " + target.guid + " and type " + target.type);
            // Ignore, if the message is about this game
            if (target.Guid != behaviour.ConnectionID && target.type != WebsocketMessageType.SyncedGameObject)
            {
                ExecuteOnJson(target, msg);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("This msg: " + msg + " led to this error:" + e);
            return;
        }
    }

    protected virtual void ExecuteOnJson(IDMessage iDMessage, string msg)
    {
        try
        {
            switch (iDMessage.type)
            {
                case WebsocketMessageType.ID:
                    behaviour.ConnectionID = iDMessage.Guid;
                    SyncedStrings.Instance.gameID = iDMessage.Guid;
                    SyncedFloats.Instance.gameID = iDMessage.Guid;
                    if (debug) Debug.Log("New Id");
                    break;
                case WebsocketMessageType.Position:
                    PositionMessage positionMessage = PositionMessage.FromJson(msg);
                    StartCoroutine(UpdateOnlinePlayer((TransformMessage)positionMessage));
                    break;
                case WebsocketMessageType.Transform:
                    TransformMessage transformMessage = TransformMessage.FromJson(msg);
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
        } catch (Exception e)
        {
            Debug.LogError("A message with type " + iDMessage.type +" and ID " + iDMessage.guid +" led to an error: " + e);
        }
    }

    private void FinishRPC(RPCMessage msg)
    {
        Debug.Log("Recieved RPC call with: " + msg);
        Guid targetId = Guid.Parse(msg.procedureGuid);
        foreach(KeyValuePair<Guid, BasicProcedureEntity> bpe in basicProcedureEntities)
        {
            if(bpe.Key == targetId)
            {
                bpe.Value.Invoke(msg.procedureName, 0f);
            }
        }
    }
}