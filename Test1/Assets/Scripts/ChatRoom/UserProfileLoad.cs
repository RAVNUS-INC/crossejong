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

// ���� ��/���ӿ� ������ �÷��̾���� �����ʰ� �̸� ǥ���ϴ� ��ũ��Ʈ(PlayerView)

public class UserProfileLoad : MonoBehaviour, IOnEventCallback
{
    public GameObject[] InRoomUserList; // ���� �濡 ������ �������� ����Ʈ
    public Image[] InRoomUserImg; // ���� �濡 ������ �������� �����ʻ���
    public Text[] InRoomUserName; // ���� �濡 ������ �������� �г���
    public Sprite[] profileImages; // 3���� �⺻ ���� �̹���

    private string mydisplayname; //���� ���� �̸� ���� ����
    private string myimgindex; //���� ���� �̹��� ���� ����
    private int mymaster; //���� ���� ���� ���� ���� ����

    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // ���� Ű

    public List<Player> players = new List<Player>(); // �÷��̾� ����Ʈ (����� �Ϲ� �÷��̾� ���� �� ����)

    void Awake() // �ʱ�ȭ
    {
        SendPlayerInfoToOthers(); //���� ���� ������ �ҷ��ͼ� ����Ʈ�� �߰�, �ٸ� �÷��̾�鿡�� �� ���� ����
        UpdateMyInfo(); //�� ���� ������Ʈ UI
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void SetActive()
    {
        // ��� ��������Ʈ�� �⺻������ ��Ȱ��ȭ(�ʿ��� �� Ȱ��ȭ�� ����)
        for (int i = 0; i < InRoomUserList.Length; i++)
        {
            InRoomUserList[i].SetActive(false);
        }
    }


    // -------------------���� ����-----------------------
    public void SendPlayerInfoToOthers() //�������ڸ��� ����
    {
        SetActive(); // �ʱ⿡ ��� ������ ������Ʈ ��Ȱ��ȭ

        LoadCustomProperty("Displayname"); //Ŀ����������Ƽ���� �����̸� �ҷ��� ������ ����
        LoadCustomProperty("Imageindex"); //Ŀ����������Ƽ���� ���� �̹����ε��� �ҷ��� ������ ����
        IsMaster(); //�������� �ƴ����� ���� 0/1 ��

        //Debug.Log($"�� �̸�:  {mydisplayname}");
        //Debug.Log($"�� �ε���:  {myimgindex}");
        //Debug.Log($"�����ΰ�? :  {mymaster}");

        // ������ �����͸� Hashtable�� �غ�
        Hashtable playerInfo = new Hashtable();
        playerInfo["Displayname"] = mydisplayname;
        playerInfo["ImgIndex"] = myimgindex;
        playerInfo["Master"] = mymaster;

        // �������� �÷��̾� ����Ʈ�� �߰�
        Player newPlayer = new Player(mydisplayname, myimgindex, mymaster);
        players.Add(newPlayer);
        Debug.Log("�� ������ ����Ʈ�� �߰��Ͽ����ϴ�");

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All  //��� �÷��̾ �̺�Ʈ�� ����
        };

        //�̺�Ʈ �ڵ� 101������ �ٸ� �÷��̾�� ����
        PhotonNetwork.RaiseEvent(101, playerInfo, RaiseEventOptions.Default, SendOptions.SendReliable);
        Debug.Log("�� ������ �����Ͽ����ϴ�");

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
            string imgindex = (string)playerInfo["ImgIndex"];
            int ismaster = (int)playerInfo["Master"];

            // �÷��̾� ����Ʈ�� �߰�
            Player newPlayer = new Player(displayname, imgindex, ismaster);
            players.Add(newPlayer); //����Ʈ�� �޾ƿ� �÷��̾��� ���� ����

            Debug.Log("�ٸ������� ������ ����Ʈ�� �߰��Ͽ����ϴ�");

            // �÷��̾� ������ ������� UI ������Ʈ
            UpdatePlayerUI();
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
            int imgIndexInt = int.Parse(owner.imgIndex); //�̹����ε��� int������ ��ȯ
            InRoomUserList[index].SetActive(true); // ���� ������ Ȱ��ȭ
            InRoomUserName[index].text = owner.displayName; // ������ �̸� �ؽ�Ʈ ǥ��
            InRoomUserImg[index].sprite = profileImages[imgIndexInt]; // ������ �̹��� ǥ��
            index++;
            Debug.Log("���� ������ ������Ʈ�Ǿ����ϴ�");
        }

        // ������ �ƴ϶��, ���� ������� 1������ ���ʴ�� �ݿ�
        foreach (var player in players)
        {
            int imgIndexInt = int.Parse(player.imgIndex); //�̹����ε��� int������ ��ȯ
            if (player.isMaster == 0)
            {
                InRoomUserList[index].SetActive(true); // �÷��̾��� ������ Ȱ��ȭ
                InRoomUserName[index].text = player.displayName;  // �÷��̾��� �̸� �ؽ�Ʈ ǥ��
                InRoomUserImg[index].sprite = profileImages[imgIndexInt]; // �÷��̾��� �̹��� ǥ��
                index++;
            }
        }
    }

    // �� ������ ������Ʈ�Ǵ� �޼��� (�ַ� ���� ������ �ڱ� ���� ������Ʈ)
    public void UpdateMyInfo()
    {
        int myIndex = -1;

        int myimgindexInt = int.Parse(myimgindex);
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
        Debug.Log($"�� ���� �ݿ� �Ϸ�:  {mydisplayname}, {myimgindex}");
        InRoomUserList[myIndex].SetActive(true); // �� �������� Ȱ��ȭ
        InRoomUserName[myIndex].text = mydisplayname; // �� �̸� �ؽ�Ʈ ǥ��
        InRoomUserImg[myIndex].sprite = profileImages[myimgindexInt]; // �� �̹��� ǥ��
   
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
                myimgindex = value; //������ ����
                Debug.Log($"{key} : {value}");
            }  
        }
        else
        {
            Debug.Log($"Key '{key}' not found in custom properties.");
        }
    }

    internal void PlayersRemove(Photon.Realtime.Player otherPlayer)
    {
        throw new NotImplementedException();
    }

    // �÷��̾� ������ �����ϴ� Ŭ����
    public class Player
    {
        public string displayName;
        public string imgIndex;
        public int isMaster;

        public Player(string displayName, string imgIndex, int isMaster)
        {
            this.displayName = displayName;
            this.imgIndex = imgIndex;
            this.isMaster = isMaster;
        }
    }

    // ---------------------Displayname �ҷ����� �Լ�---------------------
    // DisplayName �ҷ����� �Լ�
    //public void GetUserDisplayName()
    //{
    //    PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountInfoSuccess, OnGetAccountInfoFailure);
    //}

    //// ���������� DisplayName�� ������ ��� -> displayname������ ����
    //private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    //{
    //    string displayName = result.AccountInfo.TitleInfo.DisplayName;
    //    mydisplayname = displayName; //������ ����

    //    if (!string.IsNullOrEmpty(displayName))
    //    {
    //        Debug.Log($"������ DisplayName: {displayName}");
    //    }
    //    else
    //    {
    //        Debug.Log("DisplayName�� �������� �ʾҽ��ϴ�.");
    //    }
    //}

    //// DisplayName �������⿡ ������ ���
    //private void OnGetAccountInfoFailure(PlayFabError error)
    //{
    //    Debug.LogError($"DisplayName �������� ����: {error.GenerateErrorReport()}");
    //}



    // ---------------------������ �̹��� �ε��� �ҷ����� �Լ�---------------------
    //private void LoadProfileImageIndex()
    //{
    //    var request = new GetUserDataRequest();
    //    PlayFabClientAPI.GetUserData(request, result =>
    //    {
    //        // PROFILE_IMAGE_INDEX_KEY�� �����ϴ��� Ȯ��
    //        if (result.Data.ContainsKey(PROFILE_IMAGE_INDEX_KEY))
    //        {
    //            // ����� �ε��� �� �ҷ�����
    //            int index = int.Parse(result.Data[PROFILE_IMAGE_INDEX_KEY].Value);
    //            // �ε��� ���� üũ �� �̹��� ������Ʈ
    //            myimgindex = index; // -> imgindex ������ �ε����� ����
    //        }
    //        else
    //        {
    //            Debug.LogWarning("PROFILE_IMAGE_INDEX_KEY�� �������� �ʽ��ϴ�. �⺻ �̹����� �����մϴ�.");
    //        }
    //    }, error =>
    //    {
    //        Debug.LogError($"���� ������ �ҷ����� ����: {error.GenerateErrorReport()}");
    //    });
    //}


}
