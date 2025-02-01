using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;


public class ChatEditor : MonoBehaviourPunCallbacks
{
    public ChatManager chatManager; //chatmanager��ũ��Ʈ ����

    private int myActorNum, myImgIndex; //�� actornumber, �� ���� �ε���
    private string myDisplayName, myMesseages; //�� �̸�, ���� ���� �޽���
    private Dictionary<int, bool> playerReadyStates = new Dictionary<int, bool>(); // �÷��̾� �غ� ���� ����

    public InputField ChatField; //ä���Է�â
    public Button ReadyBtn; //�غ��ư

    void Awake()//�ʱ�ȭ
    {
        ChatField.text = ""; //ä���Է�â�� �׻� �������

        ReadyBtn.interactable = true; // ó������ �غ��ư Ȱ��ȭ

        //�� ���� �ҷ�����(�� �̸�, �� ������ȣ, ������ �ε���)
        var customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        myDisplayName = customProperties.ContainsKey("Displayname") ? (string)customProperties["Displayname"] : "Unknown";
        myActorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        myImgIndex = int.Parse(customProperties.ContainsKey("Imageindex") ? (string)customProperties["Imageindex"] : "Unknown");

        //���� ���� �˸���("~���� �����Ͽ����ϴ�.")
        photonView.RPC("EnterState", RpcTarget.All, myDisplayName, true);
        Debug.Log("���� ������ �˷Ƚ��ϴ�");
    }


    //�������� �غ��ϱ� ���� ���� �Լ�
    //public void UserReadyState() //�غ��ϱ� ��ư ������ ����->�ѹ� ������ ��ư ���� ��Ȱ��ȭ
    //{
    //    ReadyBtn.interactable = false; // ��ư �ѹ� �������� �������� ��Ȱ��ȭ(�غ� ��� �Ұ���)

    //    if (!PhotonNetwork.InRoom) return;

    //    // ������ ����(���ͳѹ�, ���̸�)
    //    object[] data = new object[] { myActorNum, myDisplayName };

    //    // ���忡�� �̺�Ʈ ��ε�ĳ��Ʈ
    //    PhotonNetwork.RaiseEvent(SendReadyEventCode, data, new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }, SendOptions.SendReliable);
    //    Debug.Log("���忡�� �غ� �Ϸ� ���¸� �˷Ƚ��ϴ�.");
    //}

    //������ ��/������ �˸��� �Լ�
    //public void UserEnterState(bool isbool)
    //{
    //    // ���� ����/���������� �˸��� �޽��� ����
    //    chatManager.DisplayUserMessage(myDisplayName, isbool);

    //    // ������ ����(���ͳѹ�, ���̸�)
    //    object[] data = new object[] { myActorNum, myDisplayName, isbool};

    //    // �濡 �ִ� ��� �������� �̺�Ʈ ��ε�ĳ��Ʈ
    //    PhotonNetwork.RaiseEvent(SendUserEventCode, data, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    //    Debug.Log(isbool ? "���� ������ �˷Ƚ��ϴ�" : "���� ������ �˷Ƚ��ϴ�");
    //}

    //���۹�ư ������ �޽��� ����
    public void SendMyMessage() 
    {
        if (ChatField.text.Trim() != "")
        {
            //ä��â�� �Է��� ������ �޽��� ������ ����
            myMesseages = ChatField.text;

            //���� ������ �ٸ� �����鿡�� �� ä�� ����
            photonView.RPC("SendChat", RpcTarget.Others, false, myMesseages, myDisplayName, myImgIndex);
            Debug.Log($"�� ä�ð� ������ �ٸ� �������� �����߽��ϴ�");

            //�� ä�ÿ� �� �޽��� ������Ʈ
            chatManager.Chat(true, myMesseages, "��", null);

            //������ ������ ��, ä�� ��ǲ�ʵ� ����
            ChatField.text = "";
        }      
    }

    //public void OnEvent(EventData photonEvent)
    //{
    //    object[] data = (object[])photonEvent.CustomData; //�������Ľ�
    //    //����
    //    if (data != null)
    //    {
    //        int actornumber = (int)data[0];
    //        string senderName = (string)data[1];

    //        switch (photonEvent.Code)
    //        {
    //            //case SendChatEventCode:
    //            //    int Index = (int)data[2]; //���� ������ �ε��� �ޱ�
    //            //    string message = (string)data[3]; //���� ������ �޽��� ���� �ޱ�
    //            //    Debug.Log($"������ ������ �޾ҽ��ϴ�");

    //            //    if (actornumber != PhotonNetwork.LocalPlayer.ActorNumber) // �ڽ��� ActorNumber�� �ٸ��ٸ�(�ٸ� ������ ���� �̺�Ʈ���)
    //            //    {
    //            //        //ȭ�鿡 ������� ��ǳ�� ����
    //            //        chatManager.Chat(false, message, senderName, Index);
    //            //    }
    //            //    break;

    //            //case SendUserEventCode:
    //            //    bool isEnter = (bool)data[2]; //���� ������ �������� �������� ����

    //            //    if (actornumber != PhotonNetwork.LocalPlayer.ActorNumber) // �ڽ��� ActorNumber�� �ٸ��ٸ�(�ٸ� ������ ���� �̺�Ʈ���)
    //            //    {
    //            //        chatManager.DisplayUserMessage(senderName, isEnter);
    //            //    }
    //            //    break;

    //            case SendReadyEventCode: //�غ� ��ư �̺�Ʈ�� �޾��� ��(���常�� ���� �� ����)
    //                playerReadyStates[actornumber] = true; // �غ� ���� ����

    //                if (!PhotonNetwork.IsMasterClient) return; // ���常 ������ �ڵ带 ����

    //                foreach (Player player in PhotonNetwork.PlayerList)
    //                {
    //                    if (!playerReadyStates.ContainsKey(player.ActorNumber) || !playerReadyStates[player.ActorNumber])
    //                        return; // �� ���̶� �غ����� �ʾҴٸ� ����
    //                }
    //                //��� �غ� ���� ���
    //                Debug.Log("��� �÷��̾� �غ�. ���� �̵��մϴ�.");
    //                // �÷��̷� ������ �̵�
    //                //PhotonNetwork.LoadLevel("PlayRoom"); // ���常 ���� �̵��ص�, photon���� ��� �÷��̾���� �� ����ȭ�� ��������
    //                break;
    //        }
    //    }
    //}

    //private void OnEnable()
    //{
    //    PhotonNetwork.AddCallbackTarget(this);
    //}

    //private void OnDisable()
    //{
    //    PhotonNetwork.RemoveCallbackTarget(this);
    //}

    [PunRPC]
    //ä���� ��ο��� ������ ui������Ʈ���� �ѹ��� ����ȭ
    void SendChat(bool who, string chat, string senderName, int index)
    {
        //ȭ�鿡 ��ǳ�� ����(��: true, ����: false), index�� �������̹���
        chatManager.Chat(who, chat, senderName, index);
        Debug.Log("�������� ä���� �����߽��ϴ�");
    }

    [PunRPC]
    //������ ���� ���� �޽��� �˸���
    void EnterState(string enteruserName, bool isbool)
    {
        // ���� ����/���������� �˸��� �޽��� ����
        chatManager.DisplayUserMessage(enteruserName, isbool);
    }



}


