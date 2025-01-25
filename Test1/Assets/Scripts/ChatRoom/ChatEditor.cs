using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using UnityEditor.VersionControl;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;
//using static UnityEngine.GraphicsBuffer;
using System;
//using UnityEditor.XR;
//using System.Reflection;


public class ChatEditor : MonoBehaviour, IOnEventCallback
{
    public ChatManager chatManager; //chatmanager��ũ��Ʈ ����

    private const byte SendChatEventCode = 1; // �̺�Ʈ �ڵ� ����(�޽��� ����)
    private const byte SendUserEventCode = 0; // �̺�Ʈ �ڵ� ����(����/����)
    private int myActorNum, myImgIndex; //�� actornumber, �� ���� �ε���
    private string myDisplayName, myMesseages; //�� �̸�, ���� ���� �޽���
    public InputField ChatField; //ä���Է�â

    void Awake()//�ʱ�ȭ
    {
        ChatField.text = ""; //ä���Է�â�� �׻� �������

        //�� ���� �ҷ�����
        var customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        myDisplayName = customProperties.ContainsKey("Displayname") ? (string)customProperties["Displayname"] : "Unknown";
        myActorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        myImgIndex = int.Parse(customProperties.ContainsKey("Imageindex") ? (string)customProperties["Imageindex"] : "Unknown");

        UserEnterState(true); //���� ���� �˸���
    }

    //������ ��/������ �˸��� �Լ�
    public void UserEnterState(bool isbool)
    {
        // ���� ����/���������� �˸��� �޽��� ����
        chatManager.DisplayUserMessage(myDisplayName, isbool);

        // ������ ����(���ͳѹ�, ���̸�)
        object[] data = new object[] { myActorNum, myDisplayName, isbool};

        // �濡 �ִ� ��� �������� �̺�Ʈ ��ε�ĳ��Ʈ
        PhotonNetwork.RaiseEvent(SendUserEventCode, data, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        Debug.Log(isbool ? "���� ������ �˷Ƚ��ϴ�" : "���� ������ �˷Ƚ��ϴ�");
    }

    public void SendMyMessage() //���۹�ư ������ ����
    {
        if (ChatField.text.Trim() != "")
        {
            //ä��â�� �Է��� ������ �޽��� ������ ����
            myMesseages = ChatField.text;
            //ȭ�鿡 �� ��ǳ�� ����(������)
            chatManager.Chat(true, myMesseages, "��", null);

            // �޽����� �Բ� ���� ������ ��Ű��
            object[] data = new object[] { myActorNum, myDisplayName, myImgIndex, myMesseages };
            // �濡 �ִ� ��� �������� �̺�Ʈ ��ε�ĳ��Ʈ
            PhotonNetwork.RaiseEvent(SendChatEventCode, data, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            Debug.Log($"�� �޽����� ������ �����߽��ϴ�");

            //������ ������ ��, ä�� ��ǲ�ʵ� ����
            ChatField.text = "";
        }
        
    }

    public void OnEvent(EventData photonEvent)
    {

        object[] data = (object[])photonEvent.CustomData; //�������Ľ�
        //����
        if (data != null)
        {
            int actornumber = (int)data[0];
            string senderName = (string)data[1];

            switch (photonEvent.Code)
            {
                case SendChatEventCode:
                    int Index = (int)data[2]; //���� ������ �ε��� �ޱ�
                    string message = (string)data[3]; //���� ������ �޽��� ���� �ޱ�
                    Debug.Log($"������ ������ �޾ҽ��ϴ�");

                    if (actornumber != PhotonNetwork.LocalPlayer.ActorNumber) // �ڽ��� ActorNumber�� �ٸ��ٸ�(�ٸ� ������ ���� �̺�Ʈ���)
                    {
                        //ȭ�鿡 ������� ��ǳ�� ����
                        chatManager.Chat(false, message, senderName, Index);
                    }
                    break;

                case SendUserEventCode:
                    bool isEnter = (bool)data[2]; //���� ������ �������� �������� ����

                    if (actornumber != PhotonNetwork.LocalPlayer.ActorNumber) // �ڽ��� ActorNumber�� �ٸ��ٸ�(�ٸ� ������ ���� �̺�Ʈ���)
                    {
                        chatManager.DisplayUserMessage(senderName, isEnter);
                    }
                    break;
            }
        }

    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}


