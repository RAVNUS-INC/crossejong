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

// ���� ��/���ӿ� ������ �÷��̾���� �����ʰ� �̸� ǥ���ϴ� ��ũ��Ʈ

public class UserProfileLoad : MonoBehaviour, IOnEventCallback
{
    public GameObject[] InRoomUserList; // ���� �濡 ������ �������� ����Ʈ
    public Image[] InRoomUserImg; // ���� �濡 ������ �������� �����ʻ���
    public Text[] InRoomUserName; // ���� �濡 ������ �������� �г���
    public Sprite[] profileImages; // 3���� �⺻ ���� �̹���

    private string mydisplayname;
    private int myimgindex;
    private int mymaster;

    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // ���� Ű
    private bool isMyInfoUpdated = false;

    private List<Player> players = new List<Player>(); // �÷��̾� ����Ʈ (����� �Ϲ� �÷��̾� ���� �� ����)

    void Start()
    {
        // ��� ��������Ʈ�� �⺻������ ��Ȱ��ȭ(�ʿ��� �� Ȱ��ȭ�� ����)
        for (int i = 0; i < InRoomUserList.Length; i++)
        {
            InRoomUserList[i].SetActive(false);
        }

        SendPlayerInfoToOthers(); //�� ���� ����Ʈ�� �߰�, ���� �̺�Ʈ ������

    }

    // �÷��̾� ������ �����ϴ� Ŭ����
    public class Player
    {
        public string displayName;
        public int imgIndex;
        public int isMaster;

        public Player(string displayName, int imgIndex, int isMaster)
        {
            this.displayName = displayName;
            this.imgIndex = imgIndex;
            this.isMaster = isMaster;
        }
    }

    // -------------------���� ����-----------------------
    public async void SendPlayerInfoToOthers() //�������ڸ��� ����
    {
        //GetUserDisplayName(); //displayname�� ���� ���� �̸� �����
        //LoadProfileImageIndex(); //imgindex�� ���� �������� �ε��� �����
        //IsMaster(); //�������� �ƴ����� ���� 0/1 ��

        // �񵿱� �۾� ���� �� �Ϸ� ���
        await GetUserDisplayNameAsync(); // displayname�� ���� ���� �̸� ����
        await LoadProfileImageIndexAsync(); // imgindex�� ���� ���� ���� �ε��� ����
        await IsMasterAsync(); // �������� �ƴ��� Ȯ�� �� 0/1 �� ����

        // ������ �����͸� Hashtable�� �غ�
        Hashtable playerInfo = new Hashtable();
        playerInfo["Displayname"] = mydisplayname;
        playerInfo["ImgIndex"] = myimgindex;
        playerInfo["Master"] = mymaster;

        // �������� �÷��̾� ����Ʈ�� �߰�
        Player newPlayer = new Player(mydisplayname, myimgindex, mymaster);
        players.Add(newPlayer);
        Debug.Log("�� ������ ����Ʈ�� �߰��Ͽ����ϴ�");

        // �̺�Ʈ �ڵ� 101������ �ٸ� �÷��̾�� ����
        PhotonNetwork.RaiseEvent(101, playerInfo, RaiseEventOptions.Default, SendOptions.SendReliable);
        Debug.Log("�� ������ �����Ͽ����ϴ�");

    }

    private async Task GetUserDisplayNameAsync()
    {
        // �񵿱� �۾� �ùķ��̼� (��: API ȣ��)
        await Task.Delay(100); // 100ms ���
        GetUserDisplayName();
    }

    private async Task LoadProfileImageIndexAsync()
    {
        // �񵿱� �۾� �ùķ��̼�
        await Task.Delay(100);
        LoadProfileImageIndex();
    }

    private async Task IsMasterAsync()
    {
        // �񵿱� �۾� �ùķ��̼�
        await Task.Delay(100);
        IsMaster();
    }

    // �̺�Ʈ�� �޾��� ��
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 101) // �÷��̾� ���� �̺�Ʈ �ڵ�
        {
            // CustomData�� Hashtable�� ��ȯ
            Hashtable playerInfo = (Hashtable)photonEvent.CustomData;

            // ���� ����
            string displayname = (string)playerInfo["Displayname"];
            int imgindex = (int)playerInfo["ImgIndex"];
            int ismaster = (int)playerInfo["Master"];

            // �÷��̾� ����Ʈ�� �߰�
            Player newPlayer = new Player(displayname, imgindex, ismaster);
            players.Add(newPlayer);
            Debug.Log("�ٸ������� ������ �߰��Ͽ����ϴ�");

            // �÷��̾� ������ ������Ʈ�Ǿ����� UI ������Ʈ
            UpdatePlayerUI();

            // UI ������Ʈ �۾� ��
            //Debug.Log($"DisplayName: {displayname}, ImgIndex: {imgindex}, master: {master}");
        }
    }

    

    // ------------���� ���������� ���� �������� ��ġ �� UI ������Ʈ------------
    private void UpdatePlayerUI()
    {
        int index = 0;

        // ���� ���� ������ ������Ʈ (���常 0�� �ε����� �ݿ�)
        Player owner = players.Find(player => player.isMaster == 1);
        if (owner != null)
        {
            InRoomUserName[index].text = owner.displayName;
            InRoomUserList[index].SetActive(true); // ���� ������ Ȱ��ȭ
            index++;
            Debug.Log("���� ������ ������Ʈ�Ǿ����ϴ�");
        }

        // ������ �ƴ϶��, ���� ������� 1������ ���ʴ�� �ݿ�
        foreach (var player in players)
        {
            if (player.isMaster == 0)
            {
                InRoomUserName[index].text = player.displayName;
                InRoomUserList[index].SetActive(true); // �ش� �÷��̾��� ������ Ȱ��ȭ
                index++;
            }
        }
    }

    // �� ������ ������Ʈ�Ǵ� �޼��� (�ַ� ���� ������ �ڱ� ���� ������Ʈ)
    public void UpdateMyInfo()
    {
        int myIndex = -1;

        // ���� ������ ���, 0�� �ε����� �� ������ �ݿ�
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            myIndex = 0;
            Debug.Log("���� ������ �����Դϴ�");
        }
        else
        {
            // ������ �ƴϸ� ���� ������� 1������ �ݿ� (players�� �߰��� �������)
            myIndex = players.FindIndex(player => player.displayName == mydisplayname);
        }

        // �� ���� �ݿ�
        InRoomUserName[myIndex].text = mydisplayname;
        InRoomUserList[myIndex].SetActive(true); // �� �������� Ȱ��ȭ
    }

    // ���� ������ ���� �� ������ ������Ʈ�ϴ� �޼���
    private void Update()
    {
        // �� ������ ���� ������Ʈ���� �ʾҴٸ�, �� ������ �ݿ�
        if (string.IsNullOrEmpty(InRoomUserName[0].text) && !isMyInfoUpdated) //0�� �ε��� �̸����� ����ְ� �������� �ݿ��ȵ� ���¶��
        {
            UpdateMyInfo(); // �� ������ �ݿ�
            isMyInfoUpdated = true;
        }
    }

    //�������� �ƴ���
    private void IsMaster()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            mymaster = 1;
        }
        else
        {
            mymaster = 0;
        }
    }


    // ---------------------Displayname �ҷ����� �Լ�---------------------
    // DisplayName �ҷ����� �Լ�
    public void GetUserDisplayName()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountInfoSuccess, OnGetAccountInfoFailure);
    }

    // ���������� DisplayName�� ������ ��� -> displayname������ ����
    private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        string displayName = result.AccountInfo.TitleInfo.DisplayName;
        mydisplayname = displayName; //������ ����

        if (!string.IsNullOrEmpty(displayName))
        {
            Debug.Log($"������ DisplayName: {displayName}");
        }
        else
        {
            Debug.Log("DisplayName�� �������� �ʾҽ��ϴ�.");
        }
    }

    // DisplayName �������⿡ ������ ���
    private void OnGetAccountInfoFailure(PlayFabError error)
    {
        Debug.LogError($"DisplayName �������� ����: {error.GenerateErrorReport()}");
    }



    // ---------------------������ �̹��� �ε��� �ҷ����� �Լ�---------------------
    private void LoadProfileImageIndex()
    {
        var request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request, result =>
        {
            // PROFILE_IMAGE_INDEX_KEY�� �����ϴ��� Ȯ��
            if (result.Data.ContainsKey(PROFILE_IMAGE_INDEX_KEY))
            {
                // ����� �ε��� �� �ҷ�����
                int index = int.Parse(result.Data[PROFILE_IMAGE_INDEX_KEY].Value);
                // �ε��� ���� üũ �� �̹��� ������Ʈ
                myimgindex = index; // -> imgindex ������ �ε����� ����
            }
            else
            {
                Debug.LogWarning("PROFILE_IMAGE_INDEX_KEY�� �������� �ʽ��ϴ�. �⺻ �̹����� �����մϴ�.");
            }
        }, error =>
        {
            Debug.LogError($"���� ������ �ҷ����� ����: {error.GenerateErrorReport()}");
        });
    }


}
