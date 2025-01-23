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
//using Unity.VisualScripting.Dependencies.Sqlite;


public class ChatRoomSet : MonoBehaviourPunCallbacks
{
    //[SerializeField] private UserProfileLoad UserProfileLoad; // Inspector에서 드래그하여 연결
    public UserProfileLoad UserProfileLoad;
    
    private string selectedDifficulty; // 갱신된 난이도(초급, 중급, 고급)(변경 전)
    private int selectedTimeLimit; //  갱신된 제한시간(15초, 30초, 45초)(변경 전)

    private int selectedDifficultyIndex; // 난이도 선택 인덱스
    private int selectedTimeLimitIndex; // 제한시간 인덱스


    // UI 텍스트 연결
    public Text txtRoomName; // 방 이름
    public Text txtPlayerCount; // 현재인원/최대인원

    public Text txtDifficulty; // 난이도
    public Text txtTimelimit; // 제한시간

    public Button[] DifButton; // 난이도 버튼 배열
    public Button[] TimeButton; // 제한시간 버튼 배열

    public Button RoomSetBtn; // 방장만 사용할 수 있는 방 속성 변경 버튼
    public GameObject RoomSetPanel; //방장만 사용할 수 있는 방 속성 패널

    public Button SaveBtn; //저장버튼
    public Text Savetext; //저장완료메시지

    
    void Awake()
    {
        
    }

    private void Start()
    {
        UserProfileLoad = GameObject.FindObjectOfType<UserProfileLoad>();  // 씬에 있는 PlayerManager를 찾기

        UpdateRoomInfo(); // 방 들어서자마자 방 정보 업데이트

        //저장버튼 누르면 실행할 함수
        SaveBtn.onClick.AddListener(() =>
        {
            if (PhotonNetwork.InRoom)
            {
                Room room = PhotonNetwork.CurrentRoom;
                if (room.CustomProperties.ContainsKey("difficulty"))
                {
                    //string difficultyText = GetDifficultyText(selectedDifficulty); //난이도 문자열 변환
                    UpdateRoomUIBtn("DifficultyIndex", selectedDifficultyIndex); //인덱스도 저장
                    UpdateRoomUIBtn("difficulty", selectedDifficulty); //난이도 저장(실제 반영될 텍스트)

                    UnityEngine.Debug.Log("difficulty: " + selectedDifficulty);
                }
                if (room.CustomProperties.ContainsKey("timeLimit"))
                {
                    UpdateRoomUIBtn("TimeLimitIndex", selectedTimeLimitIndex); //인덱스도 저장
                    UpdateRoomUIBtn("timeLimit", selectedTimeLimit); //시간 저장(실제 반영될 값)

                    UnityEngine.Debug.Log("timeLimit: " + selectedTimeLimit);
                }

            }
            // UI 갱신 (저장 메시지 출력)
            Savetext.text = "저장되었습니다.";
        });

        // 방장 여부에 따른 버튼 처리
        if (PhotonNetwork.IsMasterClient)
        {
            RoomSetBtn.interactable = true;  // 방장이면 버튼 활성화
        }
        else
        {
            RoomSetBtn.interactable = false;  // 방장이 아니면 버튼 비활성화
        }


    }


    // 방장이 방 속성 변경 패널 열기 버튼을 눌렀을때 -> 버튼의 위치 현재 속성에 맞게 초기화
    public void RoomSetPanelOpenBtn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            RoomSetPanel.SetActive(true);  // 방장이면 패널 열기

            if (PhotonNetwork.InRoom)
            {
                Room room = PhotonNetwork.CurrentRoom;

                object ReDifficulty = room.CustomProperties["DifficultyIndex"]; //난이도 인덱스를 불러오기
                object ReselectedTimeLimit = room.CustomProperties["TimeLimitIndex"]; //제한시간 인덱스를 불러오기
                object Difficulty = room.CustomProperties["difficulty"]; //난이도를 불러오기
                object TimeLimit = room.CustomProperties["timeLimit"]; //제한시간을 불러오기

                // 변수명을 로비매니저 변수와 똑같게(함수 사용을 위해)
                selectedDifficultyIndex = (int)ReDifficulty; //int형변환
                selectedTimeLimitIndex = (int)ReselectedTimeLimit; //int형변환
                selectedDifficulty = (string)Difficulty; //문자열 변환
                selectedTimeLimit= (int)TimeLimit; //int형변환

                // 처음 선택했던 버튼들(난이도, 제한시간)은 색상 다르게(바뀐 정보라면 바뀐 정보에만 노란색) 
                SetDefaultSelection(DifButton, selectedDifficultyIndex);
                SetDefaultSelection(TimeButton, selectedTimeLimitIndex);
            }
        }
        else
        {
            Debug.LogWarning("방장만 설정 패널을 열 수 있습니다.");
        }
    }


    // 플레이어가 방에 들어올때, 누군가 나갈때(실시간 인원수 업데이트)
    public void UpdateRoomInfo()

    {   
        if (PhotonNetwork.InRoom)
        {
            Room room = PhotonNetwork.CurrentRoom;

            // -------------프로퍼티로부터 초기버튼 색상 표시-------------
            object ReDifficulty = room.CustomProperties["DifficultyIndex"];
            object ReselectedTimeLimit = room.CustomProperties["TimeLimitIndex"];

            // 변수명을 로비매니저 변수와 똑같게(함수 사용을 위해)
            selectedDifficultyIndex = (int)ReDifficulty; //int형변환
            selectedTimeLimitIndex = (int)ReselectedTimeLimit; //int형변환

            // 처음 선택했던 버튼들(난이도, 제한시간)은 색상 다르게(초기설정 1번만) 
            SetDefaultSelection(DifButton, selectedDifficultyIndex);
            SetDefaultSelection(TimeButton, selectedTimeLimitIndex);


            // -------------버튼 클릭 시 색상 변경(이거는 프로퍼티 영향X, 값이 실제로 저장되지는 않는다)-------------
            // 각 버튼 배열에 리스너 추가(클릭시 색상 변경) -> OK
            DifficultySet(DifButton);
            TimeLimitSet(TimeButton);


            // -------------프로퍼티로부터 접속한 방의 Text에 정보 표시/업데이트-------------
            // 방 이름
            txtRoomName.text = $"{room.Name}";
            
            // 현재 인원 / 최대 인원
            txtPlayerCount.text = $"{room.PlayerCount}/{room.MaxPlayers}";

            // 난이도(Custom Properties에서 가져오기)
            string difficulty = room.CustomProperties.ContainsKey("difficulty")
                                ? room.CustomProperties["difficulty"].ToString(): "없음";

            // 제한 시간(Custom Properties에서 가져오기)
            string timeLimit = room.CustomProperties.ContainsKey("timeLimit")
                                ? room.CustomProperties["timeLimit"].ToString(): "없음";

            // 난이도, 제한시간 text 업데이트
            txtDifficulty.text = $"{difficulty}";
            txtTimelimit.text = $"{timeLimit}초";
        }
        else
        {
            Debug.LogWarning("현재 방에 접속되어 있지 않음!");
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
        //string difficultyText = GetDifficultyText(selectedDifficulty); // selectedDifficulty 값을 기반으로 실제 문자열로 반환
        UpdateButtonColors(difBtn, index); //색상 업데이트
        UnityEngine.Debug.Log("Selected Difficulty: " + selectedDifficulty); //메시지 출력
    }

    // selectedDifficulty의 값이 2, 3, 4일 때 각각 "초급", "중급", "고급"이라는 문자열을 출력
    //public string GetDifficultyText(int difficulty)
    //{
    //    switch (difficulty)
    //    {
    //        case 2:
    //            return "초급";
    //        case 3:
    //            return "중급";
    //        case 4:
    //            return "고급";
    //        default:
    //            return "알 수 없음"; // 다른 값일 경우 기본 값 반환
    //    }
    //}

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
        UpdateButtonColors(TimBtn, index);
        UnityEngine.Debug.Log("Selected Time Limit: " + selectedTimeLimit);

    }
    private void SetDefaultSelection(Button[] buttons, int defaultIndex)
    {

        for (int i = 0; i < buttons.Length; i++) //세번 반복(각 버튼 배열 길이)
        {
            int index = i;

            ColorBlock colorBlock = buttons[i].colors;
            colorBlock.normalColor = Color.white; // 기본 색상 화이트
            colorBlock.selectedColor = Color.yellow; //선택된 색상 노란색
            buttons[i].colors = colorBlock;

        }
        UpdateButtonColors(buttons, defaultIndex);  //기본값 버튼 색상을 노란색으로
    }

    // 버튼을 실제로 색칠하는 함수
    private void UpdateButtonColors(Button[] buttons, int selectedIndex)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            ColorBlock colorBlock = buttons[i].colors; //colorBlock에 색상 정보 넘겨주기

            if (i == selectedIndex) //현재 선택한 인덱스와 i값이 같을때
            {
                colorBlock.normalColor = Color.yellow; //노란색
            }
            else //현재 선택한 인덱스와 i값이 다를때
            {
                colorBlock.normalColor = Color.white; //하얀색
            }
            buttons[i].colors = colorBlock; //버튼에 색상 업데이트
        }
    }


    // --------------------------- 방 속성 업데이트 시 ------------------------
    public void UpdateRoomUI(string key, object value) // UI 업데이트 함수(난이도,시간)
    {
        if (key == "difficulty")
        {
            txtDifficulty.text = (string)value;
        }
        if (key == "timeLimit")
        {
            txtTimelimit.text = $"{value}초";
        }
        if (key == "DifficultyIndex")
        {
            Debug.Log($"속성 업데이트 반영됨: {key} = {value}");
        }
        if (key == "TimeLimitIndex")
        {
            Debug.Log($"속성 업데이트 반영됨: {key} = {value}");
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

    // 저장 버튼 클릭 시 호출되는 함수 (UI 메시지 및 텍스트 갱신)
    public void UpdateRoomUIBtn(string key, object value)
    {
        // 방 속성 서버에 업데이트
        SaveRoomProperties(key, value);
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



    // 내가 아닌 새로운 플레이어가 입장한 경우
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // 방장인지 아닌지 구분을 할 필요가 없다(인덱스0이 항상 방장 아니겟는가?)
        PlayersUpdate(); //현재 방 접속 인원 업데이트

        // 방장이 아닌 플레이어는 UI 패널 비활성화
        if (!PhotonNetwork.IsMasterClient)
        {
            RoomSetBtn.interactable = false;
            RoomSetPanel.SetActive(false);
        }
        UnityEngine.Debug.Log("내가 아닌 새로운 플레이어 입장");
        UserProfileLoad.players.Clear();  // 기존 리스트의 모든 항목 제거
        UserProfileLoad.SendPlayerInfoToOthers();
        UserProfileLoad.UpdateMyInfo();
    }

    // 플레이어가 방을 나갔을 때 호출되는 콜백
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);  // 부모 클래스 메서드 호출(선택 사항)


        // 방장인지 아닌지 구분할 필요가 있다

        PlayersUpdate(); //현재 방 접속 인원 업데이트

        // 방장이 된 플레이어는 UI 패널 비활성화
        if (PhotonNetwork.IsMasterClient)
        {
            RoomSetBtn.interactable = true;
            RoomSetPanel.SetActive(false);
        }

        UnityEngine.Debug.Log("다른 플레이어 방 나감");
        UserProfileLoad.players.Clear();  // 기존 리스트의 모든 항목 제거
        UserProfileLoad.SendPlayerInfoToOthers();
        UserProfileLoad.UpdateMyInfo();
       
    }


    // 방을 나갈때
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        
    }

    // 방을 성공적으로 나갔을 때 호출되는 콜백
    public override void OnLeftRoom()
    {
        // 로비 씬 이름으로 이동
        SceneManager.LoadScene("Main");
    }

    // 네트워크 에러가 발생하거나 방을 나가지 못했을 때
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError($"Disconnected from server: {cause}");
        // 필요한 경우 재접속 로직 추가
    }

    
}
