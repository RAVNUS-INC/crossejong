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

// 현재 방/게임에 접속한 플레이어들의 프로필과 이름 표시하는 스크립트(PlayerView)
// UI관련 RPC
public class UserProfileLoad : MonoBehaviourPunCallbacks
{
    public static UserProfileLoad instance;

    // 인스펙터에서 PhotonView를 할당
    public PhotonView PV;

    public Countdown countDown; // 카운트다운 실행을 위해 사용
    public GameObject[] InRoomUserList; // 현재 방에 접속한 유저들의 리스트
    public Image[] InRoomUserImg; // 현재 방에 접속한 유저들의 프로필사진
    public TMP_Text[] InRoomUserName; // 현재 방에 접속한 유저들의 닉네임
    public Sprite[] profileImages; // 3가지 기본 제공 이미지

    public List<int> ActPlayerIntList = new List<int>(); // 액터넘버 리스트

    void Awake() 
    {
        SetActive(); // 리스트 표시 비활성화

        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        AddMyProfileToRoomProperties(UserInfoManager.instance.MyName, UserInfoManager.instance.MyImageIndex);
    }

    public void AddMyProfileToRoomProperties(string name, int profileIndex)
    {
        object existing;
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("UserProfiles", out existing);

        List<Hashtable> userList = new List<Hashtable>();

        // 기존 데이터가 있다면 가져오기
        if (existing != null)
        {
            var rawArray = (object[])existing;
            foreach (var obj in rawArray)
            {
                userList.Add((Hashtable)obj);
            }
        }

        int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        string myKey = "Player_" + myActorNumber;

        // 중복 유저 체크
        bool alreadyExists = userList.Any(u => (string)u["Key"] == myKey);

        if (alreadyExists)
        {
            // 이미 존재하는 유저라면 UI 업데이트만 수행 ---- playroom에서 수행
            Debug.Log("중복 유저 - 프로퍼티는 수정하지 않음, UI만 갱신");
            UpdateRoomUserUIFromProperties(); // UI 수동 동기화

            Debug.Log($"{ActPlayerIntList.Count}");
            PV.RPC("RequestStartGame", RpcTarget.MasterClient);
            return;
        }

        // 신규 유저 데이터 생성
        Hashtable myData = new Hashtable
        {
            ["ActorNumber"] = myActorNumber,
            ["Key"] = myKey,
            ["Name"] = name,
            ["Index"] = profileIndex
        };

        // 리스트에 추가
        userList.Add(myData);

        // Custom Properties에 저장
        Hashtable updated = new Hashtable
        {
            ["UserProfiles"] = userList.ToArray() // object[]로 저장
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(updated);
    }


    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) //누군가 들어오면 이를 UI 업데이트
    {
        if (propertiesThatChanged.ContainsKey("UserProfiles"))
        {
            UpdateRoomUserUIFromProperties();
        }
    }

    public void RemoveUserProfile(int actorNumber) //누군가 나가면 제거, UI 재업데이트
    {
        // 나간 유저 퇴장 메시지 띄우기
        string name = GetUserNameByActorNumber(actorNumber);
        ChatRoomSet.instance.EnterState(name, false);

        object existing;
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("UserProfiles", out existing);

        if (existing == null) return;

        var rawArray = (object[])existing;
        List<Hashtable> userList = new List<Hashtable>();

        foreach (var obj in rawArray)
        {
            var user = (Hashtable)obj;
            if (user.ContainsKey("ActorNumber") && (int)user["ActorNumber"] != actorNumber)
            {
                userList.Add(user);
            }
        }

        // 방 프로퍼티 갱신
        Hashtable updated = new Hashtable();
        updated["UserProfiles"] = userList.ToArray();
        PhotonNetwork.CurrentRoom.SetCustomProperties(updated);

        PrintAllUserProfiles();

    }

    public void UpdateRoomUserUIFromProperties() // UI 업데이트
    {
        object existing;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("UserProfiles", out existing))
        {
            var rawList = (object[])existing;

            ActPlayerIntList.Clear(); // 액터넘버 리스트 비우기
            SetActive(); //객체 초기화 비활성화

            for (int i = 0; i < rawList.Length && i < InRoomUserList.Length; i++)
            {
                Hashtable userData = (Hashtable)rawList[i];
                string name = (string)userData["Name"];
                int profileIndex = (int)userData["Index"];
                int actnum = (int)userData["ActorNumber"];

                InRoomUserList[i].SetActive(true);
                InRoomUserName[i].text = name;
                InRoomUserImg[i].sprite = profileImages[profileIndex];

                ActPlayerIntList.Add(actnum); // 액터넘버리스트 추가
            }

            Debug.Log("ActPlayerIntList: " + string.Join(", ", ActPlayerIntList));
        }
    }


    void StartCountDownAll()
    {
        countDown.photonView.RPC("StartCountDown", RpcTarget.All);
    }

    public void SetActive()
    {
        // 모든 유저리스트는 기본적으로 비활성화(필요할 때 활성화할 것임)
        for (int i = 0; i < InRoomUserList.Length; i++)
        {
            InRoomUserList[i].SetActive(false);
        }
    }

    [PunRPC]
    public void RequestStartGame() //방장이 요청받아 수행
    {
        if ((PhotonNetwork.CurrentRoom.PlayerCount == ActPlayerIntList.Count))
        {
            Debug.Log("모든 플레이어 입장 완료!");

            // 모두가 입장했으므로 자신을 포함한 모두에게 카운트다운 실행 요청
            // 1초 뒤에 RPC 호출
            Invoke("StartCountDownAll", 1f);
        }
    }

    public int? GetProfileIndexByActorNumber(int targetActorNumber)
    {
        object existingProfilesObj;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("UserProfiles", out existingProfilesObj))
        {
            object[] userProfiles = (object[])existingProfilesObj;

            foreach (object profileObj in userProfiles)
            {
                Hashtable profile = (Hashtable)profileObj;
                if ((int)profile["ActorNumber"] == targetActorNumber)
                {
                    return (int)profile["Index"]; // profileIndex 반환
                }
            }
        }

        return null; // 못 찾은 경우
    }
    public string GetUserNameByActorNumber(int targetActorNumber)
    {
        object existingProfilesObj;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("UserProfiles", out existingProfilesObj))
        {
            object[] userProfiles = (object[])existingProfilesObj;

            foreach (object profileObj in userProfiles)
            {
                Hashtable profile = (Hashtable)profileObj;
                if ((int)profile["ActorNumber"] == targetActorNumber)
                {
                    return (string)profile["Name"]; // 이름 반환
                }
            }
        }

        return null; // 못 찾은 경우
    }

    public void PrintAllUserProfiles()
    {
        object existingProfilesObj;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("UserProfiles", out existingProfilesObj))
        {
            object[] userProfiles = (object[])existingProfilesObj;

            Debug.Log($"[UserProfiles] 현재 총 {userProfiles.Length}명의 데이터가 있습니다:");

            foreach (object profileObj in userProfiles)
            {
                Hashtable profile = (Hashtable)profileObj;

                int actorNum = (int)profile["ActorNumber"];
                string name = (string)profile["Name"];
                int index = (int)profile["Index"];
                string key = (string)profile["Key"];

                //Debug.Log($" - ActorNumber: {actorNum}, Name: {name}, Index: {index}, Key: {key}");
            }
        }
        else
        {
            Debug.Log("UserProfiles가 존재하지 않습니다.");
        }
    }
}
