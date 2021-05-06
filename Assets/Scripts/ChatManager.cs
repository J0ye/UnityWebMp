using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ChatWebSocketBehaviour))]
public class ChatManager : MonoBehaviour
{
    public GameObject messagePrefab;
    public float displayedMessageTimer = 3f;
    public Text inputText;

    protected ChatWebSocketBehaviour behaviour;
    private List<GameObject> displayedMessages = new List<GameObject>();
    private void Start()
    {
        StartCoroutine(SetUpSocket());
    }

    public void SendToChat(string msg)
    {
        ClearInput();
        ChatWebSocketBehaviour.instance.Send(msg);
    }

    protected virtual IEnumerator SetUpSocket()
    {
        while (ChatWebSocketBehaviour.instance == null)
        {
            Debug.Log("Waiting for instance");
            yield return new WaitForSeconds(0.5f);
        }

        behaviour = ChatWebSocketBehaviour.instance;
        behaviour.GetWS().OnMessage += (byte[] msg) =>
        {
            ProcessMessage(Encoding.UTF8.GetString(msg));
        };
    }

    public void ProcessMessage(string msg)
    {
        foreach(GameObject go in displayedMessages)
        {
            Vector3 pos = go.GetComponent<RectTransform>().localPosition;
            // Move up every displayed message box by the heigth of a new box
            go.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, pos.y + 30,  pos.z);
        }
        GameObject newMsgObj = Instantiate(messagePrefab, transform);
        displayedMessages.Add(newMsgObj);
        newMsgObj.transform.GetChild(0).GetComponent<Text>().text = msg;
        StartCoroutine(RemoveDisplayedMessage(newMsgObj, displayedMessageTimer));
    }

    private IEnumerator RemoveDisplayedMessage(GameObject target, float timer)
    {
        yield return new WaitForSeconds(timer);
        if(displayedMessages.Find(p => p == target)) displayedMessages.Remove(target);
        Destroy(target);
    }

    private void ClearInput()
    {
        inputText.text = "";
    }
}
