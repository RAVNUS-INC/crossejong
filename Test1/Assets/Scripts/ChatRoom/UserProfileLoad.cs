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
    private int myActNum; //���� ���� ���ͳѹ�

    //playerprefs �����(Key)
    private const string DISPLAYNAME_KEY = "DisplayName"; // ������ DisplayName
    private const string IMAGEINDEX_KEY = "ImageIndex"; // ������ �̹��� �ε���
    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // ���� Ű

    private List<Player> players = new List<Player>(); // �÷��̾� ����Ʈ (����� �Ϲ� �÷��̾� ���� �� ����)

    void Awake() 
    {
        mydisplayname = PlayerPrefs.GetString(DISPLAYNAME_KEY, "Guest"); //�����̸� �ҷ��� mydisplayname ������ ����
        myimgindex = PlayerPrefs.GetInt(IMAGEINDEX_KEY, 0);  //���� �̹����ε��� �ҷ��� myimgindex ������ ����
        myActNum = PhotonNetwork.LocalPlayer.ActorNumber; //���ͳѹ� ����
    }

    void Start()
    {
        // ����� ���
        PhotonNetwork.RegisterPhotonView(photonView);

        //����Ʈ�� ���� �߰� ��û
        photonView.RPC("AddUserInfo", RpcTarget.All, mydisplayname, myimgindex, myActNum);

        // ����Ʈ ������Ʈ�� ��� Ŭ���̾�Ʈ�� ����
        photonView.RPC("UpdatePlayerList", RpcTarget.All, players);

        // ������ UI�� �����ϰ� ��� �÷��̾�� ������Ʈ ����
        photonView.RPC("UpdatePlayerViewUI", RpcTarget.All);

    }

    //players ����Ʈ�� �߰��ϴ� ����(���� �����ڿ��Ե� �� ���� ����)-���常 ���Ѱ���
    [PunRPC]
    void AddUserInfo(string userName, int imgIndex, int userNum)
    {
        // ������ �÷��̾� ����Ʈ�� �߰�
        Player newPlayer = new Player(userName, imgIndex, userNum);
        players.Add(newPlayer);
        Debug.Log($"���ο� �÷��̾� �߰�: {userName}, ActNum: {userNum}");
    }

    // ������ ������ Ȱ��ȭ
    [PunRPC]
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


    // �÷��̾� ����Ʈ ������Ʈ�� ��� Ŭ���̾�Ʈ���� �����ϴ� RPC �Լ�
    [PunRPC]
    void UpdatePlayerList(List<Player> updatedPlayerList)
    {
        // ��� Ŭ���̾�Ʈ���� �÷��̾� ����Ʈ�� ������Ʈ
        players = updatedPlayerList;
        foreach (Player player in players)
        {
            Debug.Log($"Display Name: {player.displayName}, Img Index: {player.imgIndex}, Act Num: {player.myActNum}");
        }
    }



    //players ����Ʈ���� �����ϴ� ����(���� �����ڿ��Ե� �� ���� ����)
    [PunRPC]
    public void RemoveUserInfo(int userNum)
    {
        //���� ����
        players.RemoveAll(p => p.myActNum == userNum);
        Debug.Log($"�÷��̾� {userNum}�� ����Ʈ���� ���ŵ�.");

        // ����Ʈ ������Ʈ�� ��� Ŭ���̾�Ʈ�� ����
        photonView.RPC("UpdatePlayerList", RpcTarget.AllBuffered, players);

        // ������ UI�� �����ϰ� ��� �÷��̾�� ������Ʈ ����
        photonView.RPC("UpdatePlayerViewUI", RpcTarget.AllBuffered);
    }



    // �÷��̾� ������ �����ϴ� Ŭ����
    [System.Serializable]  // Photon�� ����ȭ�� �� �ֵ��� ����
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
