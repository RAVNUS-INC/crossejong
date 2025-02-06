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

// 현재 방/게임에 접속한 플레이어들의 프로필과 이름 표시하는 스크립트(PlayerView)
// UI관련 RPC
public class UserProfileLoad : MonoBehaviourPunCallbacks
{
    public static UserProfileLoad Instance;

    public GameObject[] InRoomUserList; // 현재 방에 접속한 유저들의 리스트
    public Image[] InRoomUserImg; // 현재 방에 접속한 유저들의 프로필사진
    public Text[] InRoomUserName; // 현재 방에 접속한 유저들의 닉네임
    public Sprite[] profileImages; // 3가지 기본 제공 이미지

    private string mydisplayname; //현재 유저 이름 저장 변수
    private int myimgindex; //현재 유저 이미지 저장 변수
    private int myActNum; //현재 유저 액터넘버

    //playerprefs 내용들(Key)
    private const string DISPLAYNAME_KEY = "DisplayName"; // 유저의 DisplayName
    private const string IMAGEINDEX_KEY = "ImageIndex"; // 유저의 이미지 인덱스
    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // 저장 키

    private List<Player> players = new List<Player>(); // 플레이어 리스트

    void Awake() 
    {
        Instance = this;
        
        mydisplayname = PlayerPrefs.GetString(DISPLAYNAME_KEY, "Guest"); //유저이름 불러와 mydisplayname 변수에 저장
        myimgindex = PlayerPrefs.GetInt(IMAGEINDEX_KEY, 0);  //유저 이미지인덱스 불러와 myimgindex 변수에 저장
        myActNum = PhotonNetwork.LocalPlayer.ActorNumber; //액터넘버 저장
    }

    void Start()
    {
        // 포톤뷰 등록
        PhotonNetwork.RegisterPhotonView(photonView);

        // 본인의 정보 추가를 방장에게 전달
        //-----------(서버 연결 시 주석해제), 스크립트에 photonview 추가------------
         photonView.RPC("RequestAddPlayerInfo", RpcTarget.MasterClient, mydisplayname, myimgindex, myActNum);
    }

    // players 리스트를 외부에서 접근할 수 있도록 메서드 제공
    public List<Player> GetPlayers()
    {
        return players;
    }

    [PunRPC]
    public void RequestAddPlayerInfo(string displayName, int imgIndex, int myActNum)
    {
        if (!PhotonNetwork.IsMasterClient) return; // 방장만 실행

        // 방장이 플레이어 리스트에 추가
        players.Add(new Player(displayName, imgIndex, myActNum));
        Debug.Log($"플레이어 {myActNum}가 리스트에 추가됨.");

        // 모든 유저에게 동기화 요청
        SyncPlayerList();
    }

    [PunRPC]
    public void RequestRemoveUserInfo(int userNum) //players 리스트에서 삭제하는 과정
    {
        if (!PhotonNetwork.IsMasterClient) return; // 방장만 실행

        //직접 삭제
        players.RemoveAll(p => p.myActNum == userNum);
        Debug.Log($"플레이어 {userNum}가 리스트에서 제거됨.");

        // 모든 유저에게 동기화 요청
        SyncPlayerList();
    }

    void SyncPlayerList() // 방장만 실행->모두에게 리스트 ui업뎃 요청
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
        players.Clear(); //처음엔 초기화
        players = names.Select((t, i) => new Player(t, imgIndexes[i], actNums[i])).ToList();
        Debug.Log($"플레이어 리스트 업데이트됨.");
        UpdatePlayerViewUI();
    }

    void UpdatePlayerViewUI() // 접속자 프로필 활성화
    {
        // 초기에 모든 프로필 오브젝트 비활성화
        SetActive();

        int myIndex = 1;
        // 가장 작은 ActorNumber 찾기 == 방장
        int masterActorNumber = GetMasterActorNumber();

        // 방장과 방장이 아닌 유저들 모두 업데이트
        foreach (var player in players)
        {
            if (player.myActNum == masterActorNumber) //방장이면
            {
                InRoomUserList[0].SetActive(true); // 프로필을 활성화
                InRoomUserName[0].text = player.displayName; //이름 텍스트 표시
                InRoomUserImg[0].sprite = profileImages[player.imgIndex]; // 이미지 표시
                Debug.Log($"Display Name: {player.displayName}, Img Index: {player.imgIndex}, Actor Number: {player.myActNum}");
                Debug.Log("방장 정보 업데이트 완료");
                continue;
            }
            else //방장이 아니면
            {
                InRoomUserList[myIndex].SetActive(true); // 플레이어의 프로필 활성화
                InRoomUserName[myIndex].text = player.displayName;  // 플레이어의 이름 텍스트 표시
                InRoomUserImg[myIndex].sprite = profileImages[player.imgIndex]; // 플레이어의 이미지 표시
                Debug.Log($"Display Name: {player.displayName}, Img Index: {player.imgIndex}, Actor Number: {player.myActNum}");
                Debug.Log("다른 유저 정보 업데이트 완료");
                myIndex++;
            }

        }
    }


    // 플레이어 정보를 관리하는 클래스
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
        // 모든 유저리스트는 기본적으로 비활성화(필요할 때 활성화할 것임)
        for (int i = 0; i < InRoomUserList.Length; i++)
        {
            InRoomUserList[i].SetActive(false);
        }
    }

    //반복문을 사용하여 가장 작은 ActorNumber 찾기 == 방장
    public int GetMasterActorNumber()
    {
        int masterActorNumber = int.MaxValue;  // 초기값을 최대값으로 설정

        // players 리스트에서 가장 작은 ActorNumber를 찾음
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
