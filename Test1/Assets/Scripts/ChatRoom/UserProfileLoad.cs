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

// ���� ��/���ӿ� ������ �÷��̾���� �����ʰ� �̸� ǥ���ϴ� ��ũ��Ʈ(PlayerView)
// UI���� RPC
public class UserProfileLoad : MonoBehaviourPunCallbacks
{
    public static UserProfileLoad Instance;

    public GameObject[] InRoomUserList; // ���� �濡 ������ �������� ����Ʈ
    public Image[] InRoomUserImg; // ���� �濡 ������ �������� �����ʻ���
    public Text[] InRoomUserName; // ���� �濡 ������ �������� �г���
    public Sprite[] profileImages; // 3���� �⺻ ���� �̹���

    private string mydisplayname; //���� ���� �̸� ���� ����
    private int myimgindex; //���� ���� �̹��� ���� ����
    private int myActNum; //���� ���� ���ͳѹ�

    //playerprefs �����(Key)
    private const string DISPLAYNAME_KEY = "DisplayName"; // ������ DisplayName
    private const string IMAGEINDEX_KEY = "ImageIndex"; // ������ �̹��� �ε���
    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // ���� Ű

    private List<Player> players = new List<Player>(); // �÷��̾� ����Ʈ

    void Awake() 
    {
        Instance = this;
        
        mydisplayname = PlayerPrefs.GetString(DISPLAYNAME_KEY, "Guest"); //�����̸� �ҷ��� mydisplayname ������ ����
        myimgindex = PlayerPrefs.GetInt(IMAGEINDEX_KEY, 0);  //���� �̹����ε��� �ҷ��� myimgindex ������ ����
        myActNum = PhotonNetwork.LocalPlayer.ActorNumber; //���ͳѹ� ����
    }

    void Start()
    {
        // ����� ���
        PhotonNetwork.RegisterPhotonView(photonView);

        // ������ ���� �߰��� ���忡�� ����
        //-----------(���� ���� �� �ּ�����), ��ũ��Ʈ�� photonview �߰�------------
         photonView.RPC("RequestAddPlayerInfo", RpcTarget.MasterClient, mydisplayname, myimgindex, myActNum);
    }

    // players ����Ʈ�� �ܺο��� ������ �� �ֵ��� �޼��� ����
    public List<Player> GetPlayers()
    {
        return players;
    }

    [PunRPC]
    public void RequestAddPlayerInfo(string displayName, int imgIndex, int myActNum)
    {
        if (!PhotonNetwork.IsMasterClient) return; // ���常 ����

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

        photonView.RPC("UpdatePlayerList", RpcTarget.AllBuffered,
            players.Select(p => p.displayName).ToArray(),
            players.Select(p => p.imgIndex).ToArray(),
            players.Select(p => p.myActNum).ToArray());
    }

    [PunRPC]
    void UpdatePlayerList(string[] names, int[] imgIndexes, int[] actNums)
    {
        players.Clear(); //ó���� �ʱ�ȭ
        players = names.Select((t, i) => new Player(t, imgIndexes[i], actNums[i])).ToList();
        Debug.Log($"�÷��̾� ����Ʈ ������Ʈ��.");
        UpdatePlayerViewUI();
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

}
