using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
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
        score.Value++;
    }

    public virtual void SetPingFrequency(float input)
    {
        pingFrequency = input;
    }

    public void GetNewGuid()
    {
        behaviour.Send("Get guid");
    }

    protected virtual void Start()
    {
        StartCoroutine(SetUpSocket());
    }

    protected void Update()
    {
        if(lastFramePos.Round(2) != player.transform.position.Round(2))
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
        new SyncedStrings(behaviour);
        new SyncedFloats(behaviour);
        TestSyncVar();
        score = new SyncFloat("score", 0);
        Debug.Log("Created syncfloat with name " + score.CallName);
    }

    protected virtual void SendPlayerPos()
    {
        if (behaviour != null && player.IsReady())
        {
            float x = player.transform.position.x;
            float y = player.transform.position.y;
            float z = player.transform.position.z;
            string msg = "Pos:" + x + "/" + y + "/" + z + "/" + player.GetId();
            //if(debug) Debug.Log("Sending " + msg);
            behaviour.Send(msg);
            lastFramePos = player.transform.position;
        }
    }

    protected virtual IEnumerator UpdateOnlinePlayerPosition(Guid key)
    {
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
            targetObject.transform.DOMove(targetPos, pingFrequency / 2, false);
        }
        yield return new WaitForSeconds(pingFrequency);
    }

    protected virtual void ProcessMessage(string msg)
    {
        // If the message is about the players new ID
        if(readyForId && msg.Contains("ID"))
        {
            var parts = msg.Split(":".ToCharArray());
            for(int i = parts.Length; i <= 0; i--)
            {
                Debug.Log("part " + i + ": " + parts[i-1]);
            }
            Guid newId = Guid.Parse(parts[1]);
            player.SetId(newId);
            SendPlayerPos(); // notify the server abut our position
        }
        else if(msg.Contains("Update")) // The server is aksing for an update on this players position
        {
            SendPlayerPos();
        }
        else if (msg.Contains("Sync")) // The server is aksing for an update on this players position
        {
            Debug.Log("Got syncvar message:" + msg);
            if (msg.Contains("SyncString")) SyncString.Parse(msg);
            else if (msg.Contains("SyncFloat")) SyncFloat.Parse(msg);
        }
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
                StartCoroutine(UpdateOnlinePlayerPosition(guid));
            }
        }
    }

    private void TestSyncVar()
    {
        SyncString testString = new SyncString("MyTestString", "MyTestValue");
        SyncFloat testFloat = new SyncFloat("MyTestFloat", 7.4f);
        string tempS = testString.ToString();
        string tempF = testFloat.ToString();
        SyncString parsedS = SyncString.Parse(tempS);
        SyncFloat parsedF = SyncFloat.Parse(tempF);
        Debug.Log(SyncedStrings.Instance.GetCount() + " strings and " + SyncedFloats.Instance.GetCount() + " floats.");
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
