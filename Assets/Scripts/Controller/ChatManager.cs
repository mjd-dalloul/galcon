using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct MyMessage
{
    public int ID;
    public string name, msg;
}

public class ChatManager : MonoBehaviour
{
    public GameObject content;
    public GameObject MsgPref;
    public InputField input;
    public GameObject Pause;

    private InputField inputField;
    private Queue<MyMessage> msgQueue;

    

    // Start is called before the first frame update
    void Awake()
    {
        msgQueue = new Queue<MyMessage>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendOnClick()
    {
        string msg = input.text;
        if (msg == "")
        {
            return;
        }
        input.text = "";
        Server.Instance().messageNotifyServer(GameSettings.getMainPlayerId(), msg);
    }

    public void MessageCB(int ID, string msg)
    {

        MyMessage msgStruct = new MyMessage();
        msgStruct.ID = ID;
        msgStruct.name = GameSettings.getPlayerName(ID);
        msgStruct.msg = msg;
        if (!Pause.gameObject.activeSelf)
        {
            msgQueue.Enqueue(msgStruct);
        }
        else
        {
            addMessage(msgStruct);
        }
    }

    public void addMessage(MyMessage msg)
    {
        GameObject msgGO = Instantiate(MsgPref);
        msgGO.GetComponent<TextMeshProUGUI>().text = msg.msg;
        msgGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = msg.name;
        msgGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = GameSettings.getColor(msg.ID);
        msgGO.transform.SetParent(content.transform, false);
    }

    public void addPendingMessages()
    {
        while (msgQueue.Count != 0)
            addMessage(msgQueue.Dequeue());
    }
}
