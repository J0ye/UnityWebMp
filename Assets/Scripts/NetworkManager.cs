using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using DG.Tweening;


// Use plugin namespace
using HybridWebSocket;

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

    protected virtual void Start()
    {
        StartCoroutine(SetUpSocket());
        StartCoroutine(SendPlayerPos());
        StartCoroutine(GetPlayers());
        StartCoroutine(UpdateOnlinePlayerPositions());
    }

    public virtual void SetPingFrequency(string input)
    {
        float result;
        if(float.TryParse(input, out result))
        {
            pingFrequency = result;
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
    }

    protected virtual IEnumerator SendPlayerPos()
    {
        if(behaviour != null && player.IsReady())
        {
            float x = player.transform.position.x;
            float y = player.transform.position.y;
            float z = player.transform.position.z;
            string msg = "Pos:" + x + "/" + y + "/" + z + "/" + player.GetId();
            //if(debug) Debug.Log("Sending " + msg);
            behaviour.Send(msg);
        }
        yield return new WaitForSeconds(pingFrequency);
        StartCoroutine(SendPlayerPos());
    }

    protected virtual IEnumerator GetPlayers()
    {
        if (behaviour != null)
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
                if (onlinePlayerObjects[key] == null)
                {
                    onlinePlayerObjects[key] = Instantiate(onlinePlayerPrefab);
                }
                Vector3 targetPos = onlinePlayers[key];
                onlinePlayerObjects[key].transform.DOMove(targetPos, pingFrequency/2, false);
            }
        }
        yield return new WaitForSeconds(pingFrequency);
        StartCoroutine(UpdateOnlinePlayerPositions());
    }

    protected virtual void ProcessMessage(string msg)
    {
        // If the message is about the players new ID
        if(readyForId && msg.Contains("ID"))
        {
            var parts = msg.Split(": ".ToCharArray());
            Guid newId = Guid.Parse(parts[2]);
            player.SetId(newId);
        }
        // If the message is about the list of players
        else if(msg.Contains("Players:"))
        {
            // Remove Declaration
            var parts = msg.Split(":".ToCharArray());
            parts = parts[1].Split("%".ToCharArray());

            // Print every recived info
            /*if(debug)
            {
                int temp = parts.Length;
                Debug.Log("Player Count: " + temp);
                int i = 0;
                foreach (string s in parts)
                {
                    Debug.Log("Part " + i + ": " + s);
                    i++;
                }
            }*/

            foreach(string s in parts)
            {
                if(!String.IsNullOrEmpty(s))
                {
                    Guid guid;
                    Vector3 pos = new Vector3();

                    var split = s.Split("!".ToCharArray());

                    guid = Guid.Parse(split[0]);
                    split = split[1].Split("/".ToCharArray());
                    pos.x = float.Parse(split[0]);
                    pos.y = float.Parse(split[1]);
                    pos.z = float.Parse(split[2]);

                    if(onlinePlayers.ContainsKey(guid))
                    {
                        // The player exist, so rewrite his position
                        onlinePlayers[guid] = pos;
                    } else
                    {
                        // The player does not exist, add
                        onlinePlayers.Add(guid, pos);
                        // add new object for new player
                        GameObject obj = null;
                        onlinePlayerObjects.Add(guid, obj);
                        if (debug) Debug.Log("new online player with pos: " + pos);
                    }
                }
            }
        }
    }

    protected virtual void OnApplicationQuit()
    {
        CloseConnection();
    }

    protected virtual void CloseConnection()
    {
        string msg = "Delete:" + player.GetId();
        behaviour.Send(msg);
    }
}
