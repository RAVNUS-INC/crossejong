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

// 현재 방/게임에 접속한 플레이어들의 프로필과 이름 표시하는 스크립트(PlayerView)
// UI관련 RPC
public class UserProfileLoad : MonoBehaviourPunCallbacks
{
    public GameObject[] InRoomUserList; // 현재 방에 접속한 유저들의 리스트
    public Image[] InRoomUserImg; // 현재 방에 접속한 유저들의 프로필사진
    public Text[] InRoomUserName; // 현재 방에 접속한 유저들의 닉네임
    public Sprite[] profileImages; // 3가지 기본 제공 이미지

    private string mydisplayname; //현재 유저 이름 저장 변수
    private int myimgindex; //현재 유저 이미지 저장 변수
    //private int mymaster; //현재 유저 방장 여부 저장 변수
    private int myActNum; //현재 유저 액터넘버

    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // 저장 키

    public List<Player> players = new List<Player>(); // 플레이어 리스트 (방장과 일반 플레이어 구분 및 순서)

    void Start() 
    { 
   
        //현재 유저 정보를 불러와서 해쉬테이블로 생성(변수에 저장)
        SendPlayerInfoToOthers();

        //리스트에 내 정보 추가(들어온 사람이 모두에게, 이후에도 전달)
        photonView.RPC("AddUserInfo", RpcTarget.AllBuffered, mydisplayname, myimgindex, myActNum);
        
    }


    // -------------------현재 유저-----------------------
    public void SendPlayerInfoToOthers()
    {

        LoadCustomProperty("Displayname"); //커스텀프로퍼티에서 유저이름 불러와 mydisplayname 변수에 저장
        LoadCustomProperty("Imageindex"); //커스텀프로퍼티에서 유저 이미지인덱스 불러와 myimgindex 변수에 저장
        myActNum = PhotonNetwork.LocalPlayer.ActorNumber; //액터넘버 저장

        // 전송할 데이터를 Hashtable로 준비
        Hashtable playerInfo = new Hashtable();
        playerInfo["Displayname"] = mydisplayname;
        playerInfo["ImgIndex"] = myimgindex;
        playerInfo["ActNum"] = myActNum;

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


    [PunRPC]
    // 접속자 프로필 활성화
    void UpdatePlayerViewUI()
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

    [PunRPC]
    //players 리스트에 추가하는 과정(나중 접속자에게도 이 정보 전달)
    void AddUserInfo(string userName, int imgIndex, int userNum)
    {
        // 내 정보를 플레이어 리스트에 추가
        Player newPlayer = new Player(userName, imgIndex, userNum);
        players.Add(newPlayer);
        Debug.Log($"새로운 플레이어 추가: {userName}, ActNum: {userNum}");

        // 방장이 UI를 갱신하고 모든 플레이어에게 업데이트 전달
        photonView.RPC("UpdatePlayerViewUI", RpcTarget.AllBuffered);
    }

    [PunRPC]
    //players 리스트에서 삭제하는 과정(나중 접속자에게도 이 정보 전달)
    public void RemoveUserInfo(int userNum)
    {
        players.RemoveAll(p => p.myActNum == userNum);
        Debug.Log($"플레이어 {userNum}가 리스트에서 제거됨.");

        // 방장이 UI를 갱신하고 모든 플레이어에게 업데이트 전달
        photonView.RPC("UpdatePlayerViewUI", RpcTarget.AllBuffered);
    }


    // 플레이어 정보를 관리하는 클래스
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

    void LoadCustomProperty(string key)
    {
        // 현재 로컬 플레이어의 커스텀 프로퍼티 가져오기
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(key))
        {
            string value = PhotonNetwork.LocalPlayer.CustomProperties[key].ToString();
            if (key == "Displayname")
            {
                mydisplayname = value; //변수에 저장
                Debug.Log($"{key} : {value}");
            }
            if (key == "Imageindex")
            {
                myimgindex = int.Parse(value); //변수에 저장
                Debug.Log($"{key} : {value}");
            }
        }
        else
        {
            Debug.Log($"Key '{key}' not found in custom properties.");
        }
    }


    // 이벤트를 받았을 때
    //public void OnEvent(EventData photonEvent)
    //{
    //    //if (photonEvent.Code == 101) // 플레이어 정보 이벤트 코드
    //    //{
    //    //    // CustomData를 Hashtable로 변환
    //    //    Hashtable playerInfo = (Hashtable)photonEvent.CustomData;

    //    //    // 정보 추출
    //    //    string displayname = (string)playerInfo["Displayname"];
    //    //    string imgindex = (string)playerInfo["ImgIndex"];
    //    //    int ismaster = (int)playerInfo["Master"];

    //    //    // 플레이어 리스트에 추가
    //    //    Player newPlayer = new Player(displayname, imgindex, ismaster);
    //    //    players.Add(newPlayer); //리스트에 받아온 플레이어의 정보 저장

    //    //    Debug.Log("다른유저의 정보를 리스트에 추가하였습니다");

    //    //    // 플레이어 정보를 기반으로 UI 업데이트
    //    //    UpdatePlayerUI();
    //    //}
    //}


    // ------------현재 접속유저에 따른 프로필의 위치 및 UI 업데이트------------
    //private void UpdatePlayerUI()
    //{
    //    int index = 0;

    //    // 먼저 방장 정보를 업데이트 (방장만 0번 인덱스에 반영)
    //    Player owner = players.Find(player => player.isMaster == 1);
    //    if (owner != null)
    //    {
    //        int imgIndexInt = int.Parse(owner.imgIndex); //이미지인덱스 int형으로 변환
    //        InRoomUserList[index].SetActive(true); // 방장 프로필 활성화
    //        InRoomUserName[index].text = owner.displayName; // 방장의 이름 텍스트 표시
    //        InRoomUserImg[index].sprite = profileImages[imgIndexInt]; // 방장의 이미지 표시
    //        index++;
    //        Debug.Log("방장 정보가 업데이트되었습니다");
    //    }

    //    // 방장이 아니라면, 접속 순서대로 1번부터 차례대로 반영
    //    foreach (var player in players)
    //    {
    //        int imgIndexInt = int.Parse(player.imgIndex); //이미지인덱스 int형으로 변환
    //        if (player.isMaster == 0)
    //        {
    //            InRoomUserList[index].SetActive(true); // 플레이어의 프로필 활성화
    //            InRoomUserName[index].text = player.displayName;  // 플레이어의 이름 텍스트 표시
    //            InRoomUserImg[index].sprite = profileImages[imgIndexInt]; // 플레이어의 이미지 표시
    //            index++;
    //        }
    //    }
    //}

    // 내 정보가 업데이트되는 메서드 (주로 방장 정보나 자기 정보 업데이트)
    //public void UpdateMyInfo()
    //{
    //    int myIndex = -1;

    //    int myimgindexInt = int.Parse(myimgindex);

    //    // 내가 방장인 경우, 0번 인덱스에 내 정보를 반영
    //    if (PhotonNetwork.LocalPlayer.IsMasterClient)
    //    {
    //        myIndex = 0;
    //        Debug.Log("현재 유저는 방장입니다");
    //    }
    //    else
    //    {
    //        // 방장이 아니면 접속 순서대로 1번부터 반영 (players에 추가된 순서대로)
    //        myIndex = players.FindIndex(player => player.displayName == mydisplayname);
    //    }

    //    // 내 정보 반영
    //    Debug.Log($"내 정보 반영 완료:  {mydisplayname}, {myimgindex}");
    //    InRoomUserList[myIndex].SetActive(true); // 내 프로필을 활성화
    //    InRoomUserName[myIndex].text = mydisplayname; // 내 이름 텍스트 표시
    //    InRoomUserImg[myIndex].sprite = profileImages[myimgindexInt]; // 내 이미지 표시

    //}






}
