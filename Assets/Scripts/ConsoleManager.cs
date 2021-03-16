using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleManager : MonoBehaviour
{
    public Text output;
    public Text input;

    private WebSocketDemo webSocket;
    private string txt;

    private void Awake()
    {
        StartCoroutine(SetUpSocket());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            AddText(input.text);
            WebSocketDemo.instance.Send(input.text);
        }

        if(txt != output.text)
        {
            UpdateOutput();
        }
    }

    public void UpdateOutput()
    {
        output.text = txt;
    }

    private IEnumerator SetUpSocket()
    {
        while(WebSocketDemo.instance == null)
        {
            Debug.Log("Waiting for instance");
            yield return new WaitForSeconds(1f);
        }

        webSocket = WebSocketDemo.instance;
        webSocket.GetWS().OnMessage += (byte[] msg) =>
        {
            RecieveMessage(Encoding.UTF8.GetString(msg));
        };
    }

    private void AddText(string newTxt)
    {
        if(!string.IsNullOrWhiteSpace(newTxt))
        {
            txt += newTxt + " \n";
        }
    }

    private void RecieveMessage(string msg)
    {
        Debug.Log("Working on Message: " + msg);
        if (!string.IsNullOrWhiteSpace(msg))
        {
            txt += "Server: " + msg + " \n";
        }
    }
}
