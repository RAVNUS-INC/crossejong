using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using ExitGames.Client.Photon;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;
using Button = UnityEngine.UI.Button;
using PlayFab.ClientModels;
using PlayFab;
using System.Reflection;
using Photon.Pun.Demo.PunBasics;

public class ChatRoomSet : MonoBehaviourPunCallbacks
{
    public UserProfileLoad UserProfileLoad;
    public ChatEditor ChatEditor;
    public ChatManager chatManager; 

    // 방 이름, 현재인원/최대인원, 난이도, 제한시간, 저장완료메시지
    public Text txtRoomName, txtPlayerCount, txtDifficulty, txtTimelimit, Savetext; 
    // 난이도, 제한시간 버튼 배열
    public Button[] DifButton, TimeButton;
    // 방장만 사용할 수 있는 방 속성 변경 버튼, 저장버튼
    public Button RoomSetBtn, SaveBtn;
    //방장만 사용할 수 있는 방 속성 패널
    public GameObject RoomSetPanel;
    // 갱신된 난이도(초급, 중급, 고급)(변경 전)
    private string selectedDifficulty; 
    // 난이도, 제한시간 선택 인덱스, 갱신된 제한시간(15초, 30초, 45초)(변경 전)
    private int selectedDifficultyIndex, selectedTimeLimitIndex, selectedTimeLimit;

    private int myActorNum, myImgIndex, myMaster; //내 actornumber, 내 사진 인덱스, 방장인지 아닌지
    private string myDisplayName, myMesseages; //내 이름, 내가 보낸 메시지
    private Dictionary<int, bool> playerReadyStates = new Dictionary<int, bool>(); // 플레이어 준비 상태 저장

    public InputField ChatField; //채팅입력창
    public Button ReadyBtn; //준비버튼



    void Awake()
    {
        // property에 있는 방 정보 불러와 변수에 저장(방이름도 저장)
        LoadRoomInfo();

        // 현재 인원 업데이트
        PlayersUpdate();

        // 난이도, 제한시간 text 업데이트
        txtDifficulty.text = selectedDifficulty; //ex. 초급
        txtTimelimit.text = selectedTimeLimit + "초"; //ex. 15초

        ChatField.text = ""; //채팅입력창은 항상 비워놓기

        ReadyBtn.interactable = true; // 처음에는 준비버튼 활성화

        //내 정보 불러오기(내 이름, 내 고유번호, 프로필 인덱스)
        var customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        myDisplayName = customProperties.ContainsKey("Displayname") ? (string)customProperties["Displayname"] : "Unknown";
        myActorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        myImgIndex = int.Parse(customProperties.ContainsKey("Imageindex") ? (string)customProperties["Imageindex"] : "Unknown");

        //나의 입장 알리기("~님이 입장하였습니다.")
        photonView.RPC("EnterState", RpcTarget.All, myDisplayName, true);
        Debug.Log("나의 입장을 알렸습니다");
    }

    private void Start()
    {
        // 방장 여부에 따른 버튼 처리
        RoomSetBtn.interactable = PhotonNetwork.IsMasterClient;

        // 각 버튼 배열에 리스너 추가(클릭시 색상 변경)
        DifficultySet(DifButton);
        TimeLimitSet(TimeButton);

        //방 정보 변경 후, 저장버튼 누르면 실행할 함수
        SaveBtn.onClick.AddListener(() =>
        {
            if (PhotonNetwork.InRoom)
            {
                Room room = PhotonNetwork.CurrentRoom;
                // 실제로 표시될 정보들을 key값에 따라 value를 업데이트
                var properties = new (string key, object value)[]
                {
                    ("difficulty", selectedDifficulty),
                    ("timeLimit", selectedTimeLimit),
                    ("DifficultyIndex", selectedDifficultyIndex),
                    ("TimeLimitIndex", selectedTimeLimitIndex)
                };
                foreach (var prop in properties)
                {
                    if (room.CustomProperties.ContainsKey(prop.key))
                    {
                        //선택한 속성으로 값 업데이트 진행
                        SaveRoomProperties(prop.key, prop.value);
                    }
                }
            }
            // 바뀐 정보를 토대로 색상 변경
            UpdateButtonColors(DifButton, selectedDifficultyIndex);
            UpdateButtonColors(TimeButton, selectedTimeLimitIndex);

            // UI 갱신 (저장 메시지 출력)
            Savetext.text = "저장되었습니다.";
        });
    }

    public void LoadRoomInfo()
    {
        //현재 방 정보 불러오기(customProperties로부터)
        if (PhotonNetwork.InRoom)
        {
            Room room = PhotonNetwork.CurrentRoom;
            selectedDifficultyIndex = (int)room.CustomProperties["DifficultyIndex"]; //난이도 인덱스를 불러오기
            selectedTimeLimitIndex = (int)room.CustomProperties["TimeLimitIndex"]; //제한시간 인덱스를 불러오기
            selectedDifficulty = (string)room.CustomProperties["difficulty"]; //난이도를 불러오기
            selectedTimeLimit = (int)room.CustomProperties["timeLimit"]; //제한시간을 불러오기

            // 방 이름
            txtRoomName.text = $"{room.Name}";
        }
    }

    // 방장이 방 속성 변경 패널 열기 버튼을 눌렀을때 -> 버튼의 위치를 현재 속성에 맞게 초기화
    public void RoomSetPanelOpenBtn()
    {
        LoadRoomInfo();
        UnityEngine.Debug.Log("난이도: " + selectedDifficulty);
        UnityEngine.Debug.Log("시간: " + selectedTimeLimit); 

        // 처음 선택했던 버튼들(난이도, 제한시간)은 색상 다르게(바뀐 정보에만 노란색) 
        UpdateButtonColors(DifButton, selectedDifficultyIndex);
        UpdateButtonColors(TimeButton, selectedTimeLimitIndex);
    }

    //현재인원과 최대인원 텍스트 정보 업데이트
    private void PlayersUpdate()
    {
        if (PhotonNetwork.InRoom)
        {
            Room room = PhotonNetwork.CurrentRoom;
            // 현재 인원 / 최대 인원
            txtPlayerCount.text = $"{room.PlayerCount}/{room.MaxPlayers}";
        }
    }

    // --------------------------- 방 속성 버튼 클릭 시 ------------------------
    public void DifficultySet(Button[] buttons)
    {
        // Difficulty 버튼에 리스너 추가
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnDifficultyButtonClicked(index, buttons));
        }
    }

    public void TimeLimitSet(Button[] buttons)
    {
        // TimeLimit 버튼에 리스너 추가
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnTimeLimitButtonClicked(index, buttons));
        }
    }

    public void OnDifficultyButtonClicked(int index, Button[] difBtn)
    {
        switch (index) // 0: 초급, 1: 중급, 2: 고급
        {
            case 0:
                selectedDifficulty = "초급";
                break;
            case 1:
                selectedDifficulty = "중급";
                break;
            case 2:
                selectedDifficulty = "고급";
                break;
        }
        selectedDifficultyIndex = index;
        UpdateButtonColors(difBtn, index); //색상 업데이트
        UnityEngine.Debug.Log("Selected Difficulty: " + selectedDifficulty); //메시지 출력
    }

    public void OnTimeLimitButtonClicked(int index, Button[] TimBtn)
    {
        switch (index) // 0: 15초, 1: 30초, 2: 45초
        {
            case 0:
                selectedTimeLimit = 15;
                break;
            case 1:
                selectedTimeLimit = 30;
                break;
            case 2:
                selectedTimeLimit = 45;
                break;

        }
        selectedTimeLimitIndex = index;
        UpdateButtonColors(TimBtn, index); //색상 업데이트
        UnityEngine.Debug.Log("Selected Time Limit: " + selectedTimeLimit);
    }


    // 선택한 버튼을 실제로 색칠하는 함수
    private void UpdateButtonColors(Button[] buttons, int selectedIndex)
    {
        // 초기화 작업 때 버튼 색상 표시하기 위해 쓰이는 반복문
        for (int i = 0; i < buttons.Length; i++)
        {
            ColorBlock colorBlockbg = buttons[i].colors; //colorBlock에 색상 정보 넘겨주기
            colorBlockbg.normalColor = Color.white; // 기본 색상 화이트
            colorBlockbg.normalColor = (i == selectedIndex) ? Color.yellow : Color.white; //현재 인덱스와 같으면 노란색
            colorBlockbg.selectedColor = Color.yellow;
            buttons[i].colors = colorBlockbg; //버튼에 색상 업데이트
        }
    }

    // --------------------------- 방 속성 업데이트 시 ------------------------
    public void UpdateRoomUI(string key, object value) // UI 업데이트 함수(난이도,시간)
    {
        switch (key)
        {
            case "difficulty": //난이도 업데이트
                txtDifficulty.text = (string)value;
                break;

            case "timeLimit": //제한시간 업데이트
                txtTimelimit.text = $"{value}초";
                break;

            case "DifficultyIndex": //인덱스 업데이트
            case "TimeLimitIndex":
                Debug.Log($"인덱스 속성 반영됨: {key} = {value}");
                break;
        }
    }

    // 방 속성만 서버에 업데이트하는 함수 (UI 갱신은 하지 않음)
    private void SaveRoomProperties(string key, object value)
    {
        // 변경할 속성 생성
        Hashtable propertiesToUpdate = new Hashtable
        {
            { key, value }
        };

        // Photon을 통해 방 속성 업데이트
        PhotonNetwork.CurrentRoom.SetCustomProperties(propertiesToUpdate);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        foreach (DictionaryEntry entry in propertiesThatChanged)
        {
            string key = entry.Key.ToString();
            object value = entry.Value;

            Debug.Log($"속성 업데이트 반영됨: {key} = {value}");

            // 변경된 UI 갱신
            UpdateRoomUI(key, value);
        }
    }


    // 내가 아닌 새로운 플레이어가 입장한 경우
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // 방장이 아닌 플레이어는 버튼 비활성화
        RoomSetBtn.interactable = PhotonNetwork.IsMasterClient;

        //현재 접속 인원 업데이트
        PlayersUpdate();
        UnityEngine.Debug.Log("새로운 플레이어 입장");
    }

    // 플레이어가 방을 나갔을 때 호출되는 콜백
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 방장이 아닌 플레이어는 버튼 비활성화
        RoomSetBtn.interactable = PhotonNetwork.IsMasterClient;

        //현재 접속 인원 업데이트
        PlayersUpdate();
        UnityEngine.Debug.Log("다른 플레이어 방 나감");
    }


    // 방을 나갈때
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            //나의 퇴장을 모두에게 알리기
            photonView.RPC("EnterState", RpcTarget.All, myDisplayName, false);

            //나 자신을 players리스트에서 제거(후 접속자에게도 정보 전달)
            UserProfileLoad.photonView.RPC("RemoveUserInfo", RpcTarget.AllBuffered, myActorNum);

            //나가기
            PhotonNetwork.LeaveRoom();

            //로딩바 ui 애니메이션 보여주기
            LoadingSceneController.Instance.LoadScene("Main");
        }
    }

    // 방을 성공적으로 나갔을 때 호출되는 콜백
    public override void OnLeftRoom()
    {
        Debug.Log("방을 성공적으로 퇴장했습니다.");

        // 로비 씬 이름으로 이동
        //SceneManager.LoadScene("Main");
    }

    // 채팅 전송버튼에 직접 연결해 사용(메시지 전송 역할)
    public void SendMyMessage()
    {
        if (ChatField.text.Trim() != "")
        {
            //채팅창에 입력한 내용을 메시지 변수에 저장
            myMesseages = ChatField.text;

            //나를 제외한 다른 유저들에게 내 채팅 전달
            photonView.RPC("SendChat", RpcTarget.Others, false, myMesseages, myDisplayName, myImgIndex);
            Debug.Log($"내 채팅과 정보를 다른 유저에게 전달했습니다");

            //내 채팅에 내 메시지 업데이트
            chatManager.Chat(true, myMesseages, "나", null);

            //내용을 전달한 뒤, 채팅 인풋필드 비우기
            ChatField.text = "";
        }
    }

    //준비 버튼에 직접 연결(준비 상태 알리는 역할, 방장은 이동까지 수행)
    public void UserReadyState()
    {
        ReadyBtn.interactable = false; // 버튼 한번 눌렀으면 다음부턴 비활성화(준비 취소 불가능)

        //방장에게만 나의 준비 상태 전달
        photonView.RPC("IsReady", RpcTarget.MasterClient, myDisplayName, myActorNum);
        Debug.Log("방장에게 준비 완료 상태를 알렸습니다.");

        //방장인 경우에만 아래의 코드를 수행
        if (!PhotonNetwork.IsMasterClient) return;

        // 플레이어들 중 한 명이라도 준비하지 않았다면 종료
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!playerReadyStates.ContainsKey(player.ActorNumber) || !playerReadyStates[player.ActorNumber])
                return; 
        }
        //모두 준비 했을 경우
        Debug.Log("모든 플레이어 준비. 플레이방으로 이동합니다.");
        // 플레이룸 씬으로 이동(현재 방에서 준비버튼을 누른 모든 플레이어에 한하여)
        photonView.RPC("ChangeScene", RpcTarget.All, "PlayRoom");
    }


    [PunRPC]
    //채팅을 모두에게 보내고 ui업데이트까지 한번에 동기화
    void SendChat(bool who, string chat, string senderName, int index)
    {
        //화면에 말풍선 띄우기(나: true, 상대방: false), index는 프로필이미지
        chatManager.Chat(who, chat, senderName, index);
        Debug.Log("누군가의 채팅이 도착했습니다");
    }

    [PunRPC]
    //유저의 입장 퇴장 메시지 알리미
    void EnterState(string enteruserName, bool isbool)
    {
        // 내가 입장/퇴장했음을 알리는 메시지 띄우기
        chatManager.DisplayUserMessage(enteruserName, isbool);
        Debug.Log(isbool ? $"{enteruserName}이 입장했습니다" : $"{enteruserName}이 퇴장했습니다");
    }

    [PunRPC]
    //모든 유저들의 준비 유무 알리미
    void IsReady(string userName, int userNum)
    {
        playerReadyStates[userNum] = true; // 유저의 준비 상태 저장
    }

    [PunRPC]
    //모든 유저가 준비하면 플레이룸으로 이동
    void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
