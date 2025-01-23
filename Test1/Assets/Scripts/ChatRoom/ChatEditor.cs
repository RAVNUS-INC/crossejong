using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.GraphicsBuffer;


public class ChatEditor : MonoBehaviour, IOnEventCallback
{
    public ChatManager chatManager;

    private const byte SendChatEventCode = 1; // �̺�Ʈ �ڵ� ����
    public InputField ChatField; //ä���Է�â


    void Awake()
    {
        ChatField.text = ""; //ä���Է�â�� �׻� �������

        var customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        string displayname = customProperties.ContainsKey("Displayname") ? (string)customProperties["Displayname"] : "Unknown";
        // ������ ���������� �˸��� �޽��� ����
        chatManager.DisplayUserMessage(displayname, true);
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void SendMyMessage() //���۹�ư ������ ����
    {
        if (ChatField.text.Trim() != "")
        {
            // Ŀ���� ������Ƽ���� ���� �̸�, �ε��� ���� ������
            var customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            string displayname = customProperties.ContainsKey("Displayname") ? (string)customProperties["Displayname"] : "Unknown";
            string imgindex = customProperties.ContainsKey("Imageindex") ? (string)customProperties["Imageindex"] : "Unknown";

            //ä��â�� �Է��� ������ �޽��� ������ ����
            string message = ChatField.text;
            //ȭ�鿡 �� ��ǳ�� ����(������)
            chatManager.Chat(true, message, "��", null);

            //int�������� ��ȯ�ϱ�
            int index = int.Parse(imgindex);
            //������ actornumber(�ڵ��Ҵ�) ������ ����
            int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber; 

            // �޽����� �Բ� ���� ������ ��Ű��
            object[] data = new object[] { myActorNumber, displayname, index, message };

            // �濡 �ִ� ��� �������� �̺�Ʈ ��ε�ĳ��Ʈ
            PhotonNetwork.RaiseEvent(SendChatEventCode, data, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            Debug.Log($"�� �޽����� ������ �����߽��ϴ�");

            //������ ������ ��, ä�� ��ǲ�ʵ� ����
            ChatField.text = "";
        }
        
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == SendChatEventCode)
        {
            // ���ŵ� ������ �Ľ�
            object[] data = (object[])photonEvent.CustomData;
            int actornumber = (int)data[0]; //���� ������ actornumber �ޱ�
            string senderName = (string)data[1]; //���� ������ �̸� �ޱ�
            int Index = (int)data[2]; //���� ������ �ε��� �ޱ�
            string message = (string)data[3]; //���� ������ �޽��� ���� �ޱ�
            Debug.Log($"������ ������ �޾ҽ��ϴ�");

            if (actornumber != PhotonNetwork.LocalPlayer.ActorNumber) // �ڽ��� ActorNumber�� �ٸ��ٸ�(�ٸ� ������ ���� �̺�Ʈ���)
            {
                //ȭ�鿡 ������� ��ǳ�� ����
                chatManager.Chat(false, message, senderName, Index);
            }
                
        }
    }

}


