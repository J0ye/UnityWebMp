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

    protected WebSocketBehaviour behaviour;
    protected Dictionary<Guid, Vector3> onlinePlayers = new Dictionary<Guid, Vector3>();
    protected Dictionary<Guid, GameObject> onlinePlayerObjects = new Dictionary<Guid, GameObject>();
    protected SyncFloat score;
    protected Vector3 lastFramePos = Vector3.zero;
    protected bool readyForId = false;

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
        Debug.Log("String as json: " + testString.ToJson(player.GetId()));
        Debug.Log("And back to SyncVar " + SyncString.FromJson(testString.ToJson(player.GetId())).CallName 
            + " " + SyncString.FromJson(testString.ToJson(player.GetId())).Value);
        Debug.Log("Float as json: " + testFloat.ToJson(player.GetId()));
        Debug.Log("And back to SyncVar " + SyncFloat.FromJson(testFloat.ToJson(player.GetId())).CallName
            + " " + SyncFloat.FromJson(testFloat.ToJson(player.GetId())).Value);
    }

    public void TestNewJsonClass()
    {
        //PositionMessage msg = new PositionMessage(player.GetId().ToString(), player.transform.position);
        TransformMessage msg = new TransformMessage(player.GetId(), player.transform);
        Debug.Log("ID:" + player.GetId());
        Debug.Log("As json: " + msg.ToJson());
        TransformMessage b = TransformMessage.FromJson(msg.ToJson());
        Debug.Log("Parsed back from Json: Type: " + b.type +  " and Id" + b.guid 
            + " " + b.position);
        Debug.Log("Converted to base class: " + WebsocketMessage.FromJson(msg.ToJson()).type);
    }
#endregion

    protected virtual void Start()
    {
        StartCoroutine(SetUpSocket());
        lastFramePos = player.transform.position;
        TestNewJsonClass();
    }

    protected void Update()
    {
        if (debug) Debug.Log("Comparing " + lastFramePos.Round(1) + " with " + player.transform.position.Round(1));
        if(lastFramePos.Round(1) != player.transform.position.Round(1))
        {
            SendPlayerPos();
        }
    }

    protected virtual IEnumerator SetUpSocket()
    {
        while (WebSocketBehaviour.instance == null)
        {
            Debug.Log("Waiting for instance");
            yield return new WaitForSeconds(0.5f);
        }

        behaviour = WebSocketBehaviour.instance;
        behaviour.GetWS().OnOpen += () =>
        {
            readyForId = true;
        };
        behaviour.GetWS().OnMessage += (byte[] msg) =>
        {
            ProcessMessage(Encoding.UTF8.GetString(msg));
        };
        new SyncedStrings(behaviour, Guid.NewGuid());
        new SyncedFloats(behaviour, Guid.NewGuid());
        score = new SyncFloat("score", 0);
        Debug.Log("Finished Set Up");
    }

    protected virtual void SendPlayerPos()
    {
        if (behaviour != null && player.IsReady())
        {
            if (debug) Debug.Log("Sending Position");
            PositionMessage temp = new PositionMessage(player.GetId(), player.transform.position);
            string msg = JsonUtility.ToJson(temp);
            behaviour.Send(msg);
            lastFramePos = player.transform.position;
            if (debug) Debug.Log("Finisehed sending Player Pos");
        }
    }

    protected virtual void UpdateOnlinePlayerPosition(Guid key)
    {
        if(debug) Debug.Log("Updating online player position");
        Vector3 targetPos = onlinePlayers[key];
        GameObject targetObject;
        // Update positon
        if (onlinePlayerObjects[key] == null)
        {
            onlinePlayerObjects[key] = Instantiate(onlinePlayerPrefab);
        }

        targetObject = onlinePlayerObjects[key];

        if (targetPos == new Vector3(-9999, -9999, -9999))
        {
            // The player is at the exit position.
            Destroy(onlinePlayerObjects[key]);
            onlinePlayerObjects.Remove(key);
            onlinePlayers.Remove(key);
        }
        else if (targetObject.transform.position.Round(3) != targetPos.Round(3))
        {
            targetObject.transform.position = targetPos;
        }
    }

    protected virtual void ProcessMessage(string msg)
    {
        try
        {
            IDMessage temp = IDMessage.FromJson(msg);
            if (debug) Debug.Log("From Json: guid " + temp.guid + " and type " + temp.type);
            // Ignore, if the message is about this player
            if (player.IsReady())
            {
                if (temp.Guid == player.GetId())
                {
                    if (debug) Debug.Log("Recieved message about me");
                    return;
                }
            }
            switch (temp.type)
            {
                case WebsocketMessageType.ID:
                    if (readyForId)
                    {
                        IDMessage iDMessage = IDMessage.FromJson(msg);
                        player.SetId(iDMessage.Guid);
                        SyncedStrings.Instance.playerID = iDMessage.Guid;
                        SyncedFloats.Instance.playerID = iDMessage.Guid;
                        if (debug) Debug.Log("New Id");
                    }
                    break;
                case WebsocketMessageType.Position:
                    PositionMessage positionMessage = PositionMessage.FromJson(msg);
                    if (onlinePlayers.ContainsKey(positionMessage.Guid))
                    {
                        onlinePlayers[positionMessage.Guid] = positionMessage.position;
                    }
                    else
                    {
                        // Add new player
                        onlinePlayers.Add(positionMessage.Guid, positionMessage.position);
                        // add new object for new player
                        GameObject obj = null;
                        onlinePlayerObjects.Add(positionMessage.Guid, obj);
                        if (debug) Debug.Log("new online player with pos: " + positionMessage.position);
                    }
                    UpdateOnlinePlayerPosition(positionMessage.Guid);
                    break;
                case WebsocketMessageType.SyncString:
                    SyncString.FromJson(msg);
                    break;
                case WebsocketMessageType.SyncFloat:
                    SyncFloat.FromJson(msg);
                    break;               
            }
        } catch (Exception e)
        {
            try
            {
                WebsocketRequest request = WebsocketRequest.FromJson(msg);
                if (request.requestType == WebsocketMessageType.Position) SendPlayerPos();
            } catch(Exception f)
            {
                Debug.Log("Error caught " + f + " for: " + msg);
            }
            //Fix. Very unclean
        }
    }
}

// Adds .Round() to any Vector3
static class ExtensionMethods
{
    /// <summary>
    /// Rounds Vector3.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="decimalPlaces"></param>
    /// <returns></returns>
    public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }
}




/*
 * 
        else if(msg.Contains("Pos:"))
        {
            var stringArray = msg.Split(":".ToCharArray());
            stringArray = stringArray[1].Split("/".ToCharArray());
            string x = stringArray[0];
            string y = stringArray[1];
            string z = stringArray[2];
            string id = stringArray[3];

            Guid guid = Guid.Parse(id);
            // Only work on the data if it isnt this players data, that was send back
            if(guid != player.GetId())
            {
                float xPos = float.Parse(x);
                float yPos = float.Parse(y);
                float zPos = float.Parse(z);
                Vector3 newPos = new Vector3(xPos, yPos, zPos);
                if (onlinePlayers.ContainsKey(guid))
                {
                    onlinePlayers[guid] = newPos;
                }
                else
                {
                    // Add new player
                    onlinePlayers.Add(guid, newPos);
                    // add new object for new player
                    GameObject obj = null;
                    onlinePlayerObjects.Add(guid, obj);
                    if (debug) Debug.Log("new online player with pos: " + newPos);
                }
                UpdateOnlinePlayerPosition(guid);
            }
        }*/