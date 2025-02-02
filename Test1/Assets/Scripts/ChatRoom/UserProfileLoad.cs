using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using PlayFab.ClientModels;
using PlayFab;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.Demo.PunBasics;
using System.Globalization;
using System.Threading.Tasks;
using static UserProfileLoad;
using System.Reflection;

// ���� ��/���ӿ� ������ �÷��̾���� �����ʰ� �̸� ǥ���ϴ� ��ũ��Ʈ(PlayerView)
// UI���� RPC
public class UserProfileLoad : MonoBehaviourPunCallbacks
{
    public GameObject[] InRoomUserList; // ���� �濡 ������ �������� ����Ʈ
    public Image[] InRoomUserImg; // ���� �濡 ������ �������� �����ʻ���
    public Text[] InRoomUserName; // ���� �濡 ������ �������� �г���
    public Sprite[] profileImages; // 3���� �⺻ ���� �̹���

    private string mydisplayname; //���� ���� �̸� ���� ����
    private int myimgindex; //���� ���� �̹��� ���� ����
    //private int mymaster; //���� ���� ���� ���� ���� ����
    private int myActNum; //���� ���� ���ͳѹ�

    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // ���� Ű

    public List<Player> players = new List<Player>(); // �÷��̾� ����Ʈ (����� �Ϲ� �÷��̾� ���� �� ����)

    void Start() 
    { 
   
        //���� ���� ������ �ҷ��ͼ� �ؽ����̺�� ����(������ ����)
        SendPlayerInfoToOthers();

        //����Ʈ�� �� ���� �߰�(���� ����� ��ο���, ���Ŀ��� ����)
        photonView.RPC("AddUserInfo", RpcTarget.AllBuffered, mydisplayname, myimgindex, myActNum);
        
    }


    // -------------------���� ����-----------------------
    public void SendPlayerInfoToOthers()
    {

        LoadCustomProperty("Displayname"); //Ŀ����������Ƽ���� �����̸� �ҷ��� mydisplayname ������ ����
        LoadCustomProperty("Imageindex"); //Ŀ����������Ƽ���� ���� �̹����ε��� �ҷ��� myimgindex ������ ����
        myActNum = PhotonNetwork.LocalPlayer.ActorNumber; //���ͳѹ� ����

        // ������ �����͸� Hashtable�� �غ�
        Hashtable playerInfo = new Hashtable();
        playerInfo["Displayname"] = mydisplayname;
        playerInfo["ImgIndex"] = myimgindex;
        playerInfo["ActNum"] = myActNum;

    }

    //�ݺ����� ����Ͽ� ���� ���� ActorNumber ã�� == ����
    public int GetMasterActorNumber()
    {
        int masterActorNumber = int.MaxValue;  // �ʱⰪ�� �ִ밪���� ����

        // players ����Ʈ���� ���� ���� ActorNumber�� ã��
        foreach (var player in players)
        {
            if (player.myActNum < masterActorNumber)
            {
                masterActorNumber = player.myActNum;
            }
        }
        return masterActorNumber;
    }


    [PunRPC]
    // ������ ������ Ȱ��ȭ
    void UpdatePlayerViewUI()
    {
        // �ʱ⿡ ��� ������ ������Ʈ ��Ȱ��ȭ
        SetActive();

        int myIndex = 1;

        // ���� ���� ActorNumber ã�� == ����
        int masterActorNumber = GetMasterActorNumber();

        // ����� ������ �ƴ� ������ ��� ������Ʈ
        foreach (var player in players)
        {
            if (player.myActNum == masterActorNumber) //�����̸�
            {
                InRoomUserList[0].SetActive(true); // �������� Ȱ��ȭ
                InRoomUserName[0].text = player.displayName; //�̸� �ؽ�Ʈ ǥ��
                InRoomUserImg[0].sprite = profileImages[player.imgIndex]; // �̹��� ǥ��
                Debug.Log($"Display Name: {player.displayName}, Img Index: {player.imgIndex}, Actor Number: {player.myActNum}");
                Debug.Log("���� ���� ������Ʈ �Ϸ�");
                continue;
            }
            else //������ �ƴϸ�
            {
                InRoomUserList[myIndex].SetActive(true); // �÷��̾��� ������ Ȱ��ȭ
                InRoomUserName[myIndex].text = player.displayName;  // �÷��̾��� �̸� �ؽ�Ʈ ǥ��
                InRoomUserImg[myIndex].sprite = profileImages[player.imgIndex]; // �÷��̾��� �̹��� ǥ��
                Debug.Log($"Display Name: {player.displayName}, Img Index: {player.imgIndex}, Actor Number: {player.myActNum}");
                Debug.Log("�ٸ� ���� ���� ������Ʈ �Ϸ�");
                myIndex++;
            }
            
        }
    }

    [PunRPC]
    //players ����Ʈ�� �߰��ϴ� ����(���� �����ڿ��Ե� �� ���� ����)
    void AddUserInfo(string userName, int imgIndex, int userNum)
    {
        // �� ������ �÷��̾� ����Ʈ�� �߰�
        Player newPlayer = new Player(userName, imgIndex, userNum);
        players.Add(newPlayer);
        Debug.Log($"���ο� �÷��̾� �߰�: {userName}, ActNum: {userNum}");

        // ������ UI�� �����ϰ� ��� �÷��̾�� ������Ʈ ����
        photonView.RPC("UpdatePlayerViewUI", RpcTarget.AllBuffered);
    }

    [PunRPC]
    //players ����Ʈ���� �����ϴ� ����(���� �����ڿ��Ե� �� ���� ����)
    public void RemoveUserInfo(int userNum)
    {
        players.RemoveAll(p => p.myActNum == userNum);
        Debug.Log($"�÷��̾� {userNum}�� ����Ʈ���� ���ŵ�.");

        // ������ UI�� �����ϰ� ��� �÷��̾�� ������Ʈ ����
        photonView.RPC("UpdatePlayerViewUI", RpcTarget.AllBuffered);
    }


    // �÷��̾� ������ �����ϴ� Ŭ����
    public class Player
    {
        public string displayName;
        public int imgIndex;
        public int myActNum;

        public Player(string displayName, int imgIndex, int myActNum)
        {
            this.displayName = displayName;
            this.imgIndex = imgIndex;
            this.myActNum = myActNum;
        }
    }

    public void SetActive()
    {
        // ��� ��������Ʈ�� �⺻������ ��Ȱ��ȭ(�ʿ��� �� Ȱ��ȭ�� ����)
        for (int i = 0; i < InRoomUserList.Length; i++)
        {
            InRoomUserList[i].SetActive(false);
        }
    }

    void LoadCustomProperty(string key)
    {
        // ���� ���� �÷��̾��� Ŀ���� ������Ƽ ��������
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(key))
        {
            string value = PhotonNetwork.LocalPlayer.CustomProperties[key].ToString();
            if (key == "Displayname")
            {
                mydisplayname = value; //������ ����
                Debug.Log($"{key} : {value}");
            }
            if (key == "Imageindex")
            {
                myimgindex = int.Parse(value); //������ ����
                Debug.Log($"{key} : {value}");
            }
        }
        else
        {
            Debug.Log($"Key '{key}' not found in custom properties.");
        }
    }


    // �̺�Ʈ�� �޾��� ��
    //public void OnEvent(EventData photonEvent)
    //{
    //    //if (photonEvent.Code == 101) // �÷��̾� ���� �̺�Ʈ �ڵ�
    //    //{
    //    //    // CustomData�� Hashtable�� ��ȯ
    //    //    Hashtable playerInfo = (Hashtable)photonEvent.CustomData;

    //    //    // ���� ����
    //    //    string displayname = (string)playerInfo["Displayname"];
    //    //    string imgindex = (string)playerInfo["ImgIndex"];
    //    //    int ismaster = (int)playerInfo["Master"];

    //    //    // �÷��̾� ����Ʈ�� �߰�
    //    //    Player newPlayer = new Player(displayname, imgindex, ismaster);
    //    //    players.Add(newPlayer); //����Ʈ�� �޾ƿ� �÷��̾��� ���� ����

    //    //    Debug.Log("�ٸ������� ������ ����Ʈ�� �߰��Ͽ����ϴ�");

    //    //    // �÷��̾� ������ ������� UI ������Ʈ
    //    //    UpdatePlayerUI();
    //    //}
    //}


    // ------------���� ���������� ���� �������� ��ġ �� UI ������Ʈ------------
    //private void UpdatePlayerUI()
    //{
    //    int index = 0;

    //    // ���� ���� ������ ������Ʈ (���常 0�� �ε����� �ݿ�)
    //    Player owner = players.Find(player => player.isMaster == 1);
    //    if (owner != null)
    //    {
    //        int imgIndexInt = int.Parse(owner.imgIndex); //�̹����ε��� int������ ��ȯ
    //        InRoomUserList[index].SetActive(true); // ���� ������ Ȱ��ȭ
    //        InRoomUserName[index].text = owner.displayName; // ������ �̸� �ؽ�Ʈ ǥ��
    //        InRoomUserImg[index].sprite = profileImages[imgIndexInt]; // ������ �̹��� ǥ��
    //        index++;
    //        Debug.Log("���� ������ ������Ʈ�Ǿ����ϴ�");
    //    }

    //    // ������ �ƴ϶��, ���� ������� 1������ ���ʴ�� �ݿ�
    //    foreach (var player in players)
    //    {
    //        int imgIndexInt = int.Parse(player.imgIndex); //�̹����ε��� int������ ��ȯ
    //        if (player.isMaster == 0)
    //        {
    //            InRoomUserList[index].SetActive(true); // �÷��̾��� ������ Ȱ��ȭ
    //            InRoomUserName[index].text = player.displayName;  // �÷��̾��� �̸� �ؽ�Ʈ ǥ��
    //            InRoomUserImg[index].sprite = profileImages[imgIndexInt]; // �÷��̾��� �̹��� ǥ��
    //            index++;
    //        }
    //    }
    //}

    // �� ������ ������Ʈ�Ǵ� �޼��� (�ַ� ���� ������ �ڱ� ���� ������Ʈ)
    //public void UpdateMyInfo()
    //{
    //    int myIndex = -1;

    //    int myimgindexInt = int.Parse(myimgindex);

    //    // ���� ������ ���, 0�� �ε����� �� ������ �ݿ�
    //    if (PhotonNetwork.LocalPlayer.IsMasterClient)
    //    {
    //        myIndex = 0;
    //        Debug.Log("���� ������ �����Դϴ�");
    //    }
    //    else
    //    {
    //        // ������ �ƴϸ� ���� ������� 1������ �ݿ� (players�� �߰��� �������)
    //        myIndex = players.FindIndex(player => player.displayName == mydisplayname);
    //    }

    //    // �� ���� �ݿ�
    //    Debug.Log($"�� ���� �ݿ� �Ϸ�:  {mydisplayname}, {myimgindex}");
    //    InRoomUserList[myIndex].SetActive(true); // �� �������� Ȱ��ȭ
    //    InRoomUserName[myIndex].text = mydisplayname; // �� �̸� �ؽ�Ʈ ǥ��
    //    InRoomUserImg[myIndex].sprite = profileImages[myimgindexInt]; // �� �̹��� ǥ��

    //}






}
