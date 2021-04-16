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
    protected bool readyForId = false;
    protected Vector3 lastFramePos = Vector3.zero;

    protected virtual void Start()
    {
        StartCoroutine(SetUpSocket());
        StartCoroutine(UpdateOnlinePlayerPositions());
    }

    protected void Update()
    {
        if(lastFramePos != transform.position)
        {
            SendPlayerPos();
            lastFramePos = transform.position;
        }
    }

    public virtual void SetPingFrequency(float input)
    {
        pingFrequency = input;
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
        }
    }

    protected virtual IEnumerator GetPlayers()
    {
        if(!player.IsReady())
        {
            // Fix waiting for guid
        }
        else if (behaviour != null)
        {
            behaviour.Send("Get:" + player.GetId());
        }
        yield return new WaitForSeconds(pingFrequency);
        StartCoroutine(GetPlayers());
    }

    protected virtual IEnumerator UpdateOnlinePlayerPositions()
    {
        if (debug) Debug.Log("There are " + onlinePlayerObjects.Count + " players online");
        if (onlinePlayers.Count > 0)
        {
            Guid[] keys = new Guid[onlinePlayerObjects.Count];
            onlinePlayerObjects.Keys.CopyTo(keys, 0);
            foreach (Guid key in keys)
            {
                // Update positon
                if (onlinePlayerObjects[key] == null)
                {
                    onlinePlayerObjects[key] = Instantiate(onlinePlayerPrefab);
                }
                Vector3 targetPos = onlinePlayers[key];
                if(targetPos == new Vector3(-9999, -9999, -9999))
                {
                    // The player is at the exit position.
                    Destroy(onlinePlayerObjects[key]);
                    onlinePlayerObjects.Remove(key);
                    onlinePlayers.Remove(key);
                } else
                {
                    onlinePlayerObjects[key].transform.DOMove(targetPos, pingFrequency / 2, false);
                }
            }
        }
        yield return new WaitForSeconds(pingFrequency);
        StartCoroutine(UpdateOnlinePlayerPositions());
    }

    public void GetNewGuid()
    {
        behaviour.Send("Get guid");
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
            }
        }
    }
}
