using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    public TMP_InputField ChatInput;
    public TextMeshProUGUI ChatContent;
    private PhotonView _photon;
    private List<string> messages = new List<string>();
    private float _buildDelay = 0f;
    private int _maximumMessages = 14;


 
    void Start()
    {
        _photon = GetComponent<PhotonView>();
    }

    [PunRPC]

    void RPC_AddNewMessage(string msg)
    {
        messages.Add(msg);
    }

    public void SendChat(string msg)
    {
        string sender = "";
        string chatColor = "";
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            sender = "Elon Musk";
            chatColor = "red";
        }
        else
        {
            sender = "Jeff Bezos";
            chatColor = "blue";
        }

        string NewMessage = "<color=" + chatColor + ">" + sender + ": " + msg + "</color>";
        _photon.RPC("RPC_AddNewMessage", RpcTarget.All, NewMessage);
    }

    public void SubmitChat()
    {
        string blankCheck = ChatInput.text;
        blankCheck = Regex.Replace(blankCheck, @"\s", "");
        if (blankCheck == "")
        {
            ChatInput.ActivateInputField();
            ChatInput.text = "";
            return;
        }
        SendChat(ChatInput.text);
        ChatInput.ActivateInputField();
        ChatInput.text = "";
    }

    void BuildChatContents()
    {
        string NewContents = "";
        foreach(string s in messages)
        {
            NewContents += s + "\n";
        }
        ChatContent.text = NewContents;
    }

    void Update()
    {
        if (PhotonNetwork.InRoom)
        {

            if (messages.Count >= _maximumMessages)
            {
                messages.RemoveAt(0);
            }
            if (_buildDelay < Time.time)
            {
                BuildChatContents();
                _buildDelay = Time.time + 0.25f;
            }
        }
        else if(messages.Count>0)
        {
            messages.Clear();
            ChatContent.text = "";
        }
    }
}
