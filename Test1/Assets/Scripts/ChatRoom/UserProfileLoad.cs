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

// 현재 방/게임에 접속한 플레이어들의 프로필과 이름 표시하는 스크립트(PlayerView)

public class UserProfileLoad : MonoBehaviour, IOnEventCallback
{
    public GameObject[] InRoomUserList; // 현재 방에 접속한 유저들의 리스트
    public Image[] InRoomUserImg; // 현재 방에 접속한 유저들의 프로필사진
    public Text[] InRoomUserName; // 현재 방에 접속한 유저들의 닉네임
    public Sprite[] profileImages; // 3가지 기본 제공 이미지

    private string mydisplayname; //현재 유저 이름 저장 변수
    private string myimgindex; //현재 유저 이미지 저장 변수
    private int mymaster; //현재 유저 방장 여부 저장 변수

    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // 저장 키

    public List<Player> players = new List<Player>(); // 플레이어 리스트 (방장과 일반 플레이어 구분 및 순서)

    void Awake() // 초기화
    {
        SendPlayerInfoToOthers(); //현재 유저 정보를 불러와서 리스트에 추가, 다른 플레이어들에게 내 정보 전송
        UpdateMyInfo(); //내 정보 업데이트 UI
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
        // 모든 유저리스트는 기본적으로 비활성화(필요할 때 활성화할 것임)
        for (int i = 0; i < InRoomUserList.Length; i++)
        {
            InRoomUserList[i].SetActive(false);
        }
    }


    // -------------------현재 유저-----------------------
    public void SendPlayerInfoToOthers() //시작하자마자 실행
    {
        SetActive(); // 초기에 모든 프로필 오브젝트 비활성화

        LoadCustomProperty("Displayname"); //커스텀프로퍼티에서 유저이름 불러와 변수에 저장
        LoadCustomProperty("Imageindex"); //커스텀프로퍼티에서 유저 이미지인덱스 불러와 변수에 저장
        IsMaster(); //방장인지 아닌지에 대한 0/1 값

        //Debug.Log($"내 이름:  {mydisplayname}");
        //Debug.Log($"내 인덱스:  {myimgindex}");
        //Debug.Log($"방장인가? :  {mymaster}");

        // 전송할 데이터를 Hashtable로 준비
        Hashtable playerInfo = new Hashtable();
        playerInfo["Displayname"] = mydisplayname;
        playerInfo["ImgIndex"] = myimgindex;
        playerInfo["Master"] = mymaster;

        // 내정보를 플레이어 리스트에 추가
        Player newPlayer = new Player(mydisplayname, myimgindex, mymaster);
        players.Add(newPlayer);
        Debug.Log("내 정보를 리스트에 추가하였습니다");

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All  //모든 플레이어가 이벤트를 받음
        };

        //이벤트 코드 101번으로 다른 플레이어에게 전송
        PhotonNetwork.RaiseEvent(101, playerInfo, RaiseEventOptions.Default, SendOptions.SendReliable);
        Debug.Log("내 정보를 전송하였습니다");

    }



    // 이벤트를 받았을 때
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 101) // 플레이어 정보 이벤트 코드
        {
            // CustomData를 Hashtable로 변환
            Hashtable playerInfo = (Hashtable)photonEvent.CustomData;

            // 정보 추출
            string displayname = (string)playerInfo["Displayname"];
            string imgindex = (string)playerInfo["ImgIndex"];
            int ismaster = (int)playerInfo["Master"];

            // 플레이어 리스트에 추가
            Player newPlayer = new Player(displayname, imgindex, ismaster);
            players.Add(newPlayer); //리스트에 받아온 플레이어의 정보 저장

            Debug.Log("다른유저의 정보를 리스트에 추가하였습니다");

            // 플레이어 정보를 기반으로 UI 업데이트
            UpdatePlayerUI();
        }
    }


    // ------------현재 접속유저에 따른 프로필의 위치 및 UI 업데이트------------
    private void UpdatePlayerUI()
    {
        int index = 0;
        
        // 먼저 방장 정보를 업데이트 (방장만 0번 인덱스에 반영)
        Player owner = players.Find(player => player.isMaster == 1);
        if (owner != null)
        {
            int imgIndexInt = int.Parse(owner.imgIndex); //이미지인덱스 int형으로 변환
            InRoomUserList[index].SetActive(true); // 방장 프로필 활성화
            InRoomUserName[index].text = owner.displayName; // 방장의 이름 텍스트 표시
            InRoomUserImg[index].sprite = profileImages[imgIndexInt]; // 방장의 이미지 표시
            index++;
            Debug.Log("방장 정보가 업데이트되었습니다");
        }

        // 방장이 아니라면, 접속 순서대로 1번부터 차례대로 반영
        foreach (var player in players)
        {
            int imgIndexInt = int.Parse(player.imgIndex); //이미지인덱스 int형으로 변환
            if (player.isMaster == 0)
            {
                InRoomUserList[index].SetActive(true); // 플레이어의 프로필 활성화
                InRoomUserName[index].text = player.displayName;  // 플레이어의 이름 텍스트 표시
                InRoomUserImg[index].sprite = profileImages[imgIndexInt]; // 플레이어의 이미지 표시
                index++;
            }
        }
    }

    // 내 정보가 업데이트되는 메서드 (주로 방장 정보나 자기 정보 업데이트)
    public void UpdateMyInfo()
    {
        int myIndex = -1;

        int myimgindexInt = int.Parse(myimgindex);
        // 내가 방장인 경우, 0번 인덱스에 내 정보를 반영
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            myIndex = 0;
            Debug.Log("현재 유저는 방장입니다");
        }
        else
        {
            // 방장이 아니면 접속 순서대로 1번부터 반영 (players에 추가된 순서대로)
            myIndex = players.FindIndex(player => player.displayName == mydisplayname);
        }

        // 내 정보 반영
        Debug.Log($"내 정보 반영 완료:  {mydisplayname}, {myimgindex}");
        InRoomUserList[myIndex].SetActive(true); // 내 프로필을 활성화
        InRoomUserName[myIndex].text = mydisplayname; // 내 이름 텍스트 표시
        InRoomUserImg[myIndex].sprite = profileImages[myimgindexInt]; // 내 이미지 표시
   
    }

    //방장인지 아닌지
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
                myimgindex = value; //변수에 저장
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

    // 플레이어 정보를 관리하는 클래스
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

    // ---------------------Displayname 불러오기 함수---------------------
    // DisplayName 불러오기 함수
    //public void GetUserDisplayName()
    //{
    //    PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountInfoSuccess, OnGetAccountInfoFailure);
    //}

    //// 성공적으로 DisplayName을 가져온 경우 -> displayname변수에 저장
    //private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    //{
    //    string displayName = result.AccountInfo.TitleInfo.DisplayName;
    //    mydisplayname = displayName; //변수에 저장

    //    if (!string.IsNullOrEmpty(displayName))
    //    {
    //        Debug.Log($"유저의 DisplayName: {displayName}");
    //    }
    //    else
    //    {
    //        Debug.Log("DisplayName이 설정되지 않았습니다.");
    //    }
    //}

    //// DisplayName 가져오기에 실패한 경우
    //private void OnGetAccountInfoFailure(PlayFabError error)
    //{
    //    Debug.LogError($"DisplayName 가져오기 실패: {error.GenerateErrorReport()}");
    //}



    // ---------------------프로필 이미지 인덱스 불러오기 함수---------------------
    //private void LoadProfileImageIndex()
    //{
    //    var request = new GetUserDataRequest();
    //    PlayFabClientAPI.GetUserData(request, result =>
    //    {
    //        // PROFILE_IMAGE_INDEX_KEY가 존재하는지 확인
    //        if (result.Data.ContainsKey(PROFILE_IMAGE_INDEX_KEY))
    //        {
    //            // 저장된 인덱스 값 불러오기
    //            int index = int.Parse(result.Data[PROFILE_IMAGE_INDEX_KEY].Value);
    //            // 인덱스 범위 체크 후 이미지 업데이트
    //            myimgindex = index; // -> imgindex 변수에 인덱스값 저장
    //        }
    //        else
    //        {
    //            Debug.LogWarning("PROFILE_IMAGE_INDEX_KEY가 존재하지 않습니다. 기본 이미지로 설정합니다.");
    //        }
    //    }, error =>
    //    {
    //        Debug.LogError($"유저 데이터 불러오기 실패: {error.GenerateErrorReport()}");
    //    });
    //}


}
