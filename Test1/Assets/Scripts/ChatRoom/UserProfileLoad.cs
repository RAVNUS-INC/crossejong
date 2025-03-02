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
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;

// ���� ��/���ӿ� ������ �÷��̾���� �����ʰ� �̸� ǥ���ϴ� ��ũ��Ʈ(PlayerView)
// UI���� RPC
public class UserProfileLoad : MonoBehaviourPun
{
    public GameObject[] InRoomUserList; // ���� �濡 ������ �������� ����Ʈ
    public Image[] InRoomUserImg; // ���� �濡 ������ �������� �����ʻ���
    public TMP_Text[] InRoomUserName; // ���� �濡 ������ �������� �г���
    public Sprite[] profileImages; // 3���� �⺻ ���� �̹���

    private string mydisplayname; //���� ���� �̸� ���� ����
    private int myimgindex; //���� ���� �̹��� ���� ����
    private int myActNum; //���� ���� ���ͳѹ�

    //playerprefs �����(Key)
    private const string DISPLAYNAME_KEY = "DisplayName"; // ������ DisplayName
    private const string IMAGEINDEX_KEY = "ImageIndex"; // ������ �̹��� �ε���

    public List<Player> players = new List<Player>(); // �÷��̾� ����Ʈ
    public string[] userNameList; // ���� �г��� ����Ʈ(���÷��̳���)
    public int[] userImageList; // ���� �̹��� ����Ʈ(�����ʻ���)
    public int[] sortedPlayers; // ���ĵ� �÷��̾� ����Ʈ(���ͳѹ�)

    void Awake() 
    {
        mydisplayname = PlayerPrefs.GetString(DISPLAYNAME_KEY, "Guest"); //�����̸� �ҷ��� mydisplayname ������ ����
        myimgindex = PlayerPrefs.GetInt(IMAGEINDEX_KEY, 0);  //���� �̹����ε��� �ҷ��� myimgindex ������ ����
        myActNum = PhotonNetwork.LocalPlayer.ActorNumber; //���ͳѹ� ����
    }

    void Start()
    {
        // ������ ���� �߰��� ���忡�� ����
        photonView.RPC("RequestAddPlayerInfo", RpcTarget.MasterClient, mydisplayname, myimgindex, myActNum);

    }

    // players ����Ʈ�� �ܺο��� ������ �� �ֵ��� �޼��� ����
    public List<Player> GetPlayers()
    {
        return players;
    }

    [PunRPC]
    public void RequestAddPlayerInfo(string displayName, int imgIndex, int myActNum) // ���常 ����
    {
        if (!PhotonNetwork.IsMasterClient) return; 

        // ������ �÷��̾� ����Ʈ�� �߰�
        players.Add(new Player(displayName, imgIndex, myActNum));
        Debug.Log($"�÷��̾� {myActNum}�� ����Ʈ�� �߰���.");

        // ��� �������� ����ȭ ��û
        SyncPlayerList();
    }

    [PunRPC]
    public void RequestRemoveUserInfo(int userNum) //players ����Ʈ���� �����ϴ� ����
    {
        if (!PhotonNetwork.IsMasterClient) return; // ���常 ����

        //���� ����
        players.RemoveAll(p => p.myActNum == userNum);
        Debug.Log($"�÷��̾� {userNum}�� ����Ʈ���� ���ŵ�.");

        // ��� �������� ����ȭ ��û
        SyncPlayerList();
    }

    void SyncPlayerList() // ���常 ����->��ο��� ����Ʈ ui���� ��û
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // actnum ����Ʈ ������
        OrderedPlayers();

        photonView.RPC("UpdatePlayerList", RpcTarget.AllBuffered,
            players.Select(p => p.displayName).ToArray(),
            players.Select(p => p.imgIndex).ToArray(),
            players.Select(p => p.myActNum).ToArray(),
            sortedPlayers);
    }

    [PunRPC]
    void UpdatePlayerList(string[] names, int[] imgIndexes, int[] actNums, int[] playerList)
    {
        players.Clear(); //ó���� �ʱ�ȭ
        players = names.Select((t, i) => new Player(t, imgIndexes[i], actNums[i])).ToList();

        //actnum �������� ����Ʈ�� ��ΰ� ���Ź���
        sortedPlayers = playerList;

        // ������ �̸� ����Ʈ�� ��ΰ� ���Ź���
        userNameList = names;

        // ������ ���� ����Ʈ�� ��ΰ� ���Ź���
        userImageList = imgIndexes;

        Debug.Log($"�÷��̾� ����Ʈ ����ȭ��.");

        //if (SceneManager.GetActiveScene().name == "MakeRoom")
        //{
        UpdatePlayerViewUI();
        //}
    }

    void UpdatePlayerViewUI() // ������ ������ Ȱ��ȭ
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
                continue;
            }
            else //������ �ƴϸ�
            {
                InRoomUserList[myIndex].SetActive(true); // �÷��̾��� ������ Ȱ��ȭ
                InRoomUserName[myIndex].text = player.displayName;  // �÷��̾��� �̸� �ؽ�Ʈ ǥ��
                InRoomUserImg[myIndex].sprite = profileImages[player.imgIndex]; // �÷��̾��� �̹��� ǥ��
                myIndex++;
            }

        }
    }

    // �÷��̾� ������ �����ϴ� Ŭ����
    [System.Serializable]
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

    public void OrderedPlayers() //actnum�� ������������ ������ int ����Ʈ
    { 
        // players ����Ʈ�� myActNum �������� �������� ����
        sortedPlayers = players.OrderBy(player => player.myActNum)
                                   .Select(player => player.myActNum)
                                   .ToArray();
        Debug.Log($"�������� ���� �Ϸ�: {string.Join(", ", sortedPlayers)}");
    }
}
