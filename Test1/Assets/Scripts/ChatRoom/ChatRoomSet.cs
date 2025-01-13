using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using ExitGames.Client.Photon;
using System;


public class ChatRoomSet : MonoBehaviourPunCallbacks
{
    private int selectedDifficulty; // 갱신된 난이도(초급, 중급, 고급)(변경 전)
    private int selectedTimeLimit; //  갱신된 제한시간(15초, 30초, 45초)(변경 전)
    private int selectedDifficultyIndex; // 난이도 선택 인덱스
    private int selectedTimeLimitIndex; // 제한시간 인덱스

    // UI 텍스트 연결
    public Text txtRoomName; // 방 이름
    public Text txtPlayerCount; // 현재인원/최대인원
    public Text txtDandT; // 난이도와 제한시간

    public Button[] DifButton; // 난이도 버튼 배열
    public Button[] TimeButton; // 제한시간 버튼 배열

    //GameObject LobbyManager;
    

    void Awake()
    {

        UpdateRoomInfo(); // 방 들어서자마자 방 정보 업데이트

    }



    // 플레이어가 방에 들어올때, 누군가 나갈때(실시간 인원수 업데이트)
    // 
    public void UpdateRoomInfo()

    { 
        if (PhotonNetwork.InRoom)
        {
            Room room = PhotonNetwork.CurrentRoom;

            // LobbyManager 오브젝트를 찾고, 그 오브젝트에서 LobbyManager 스크립트를 가져오기
            //LobbyManager lobbyManager = GameObject.Find("LobbyManager_Clone").GetComponent<LobbyManager>(); //씬이 전환되어도 객체정보는 유지

            object ReDifficulty = room.CustomProperties["DifficultyIndex"];
            object ReselectedTimeLimit = room.CustomProperties["TimeLimitIndex"];

            // 변수명을 로비매니저 변수와 똑같게(함수 사용을 위해)
            selectedDifficulty = (int)ReDifficulty; //int형변환
            selectedTimeLimit = (int)ReselectedTimeLimit; //int형변환

            // 처음 선택했던 버튼들(난이도, 제한시간)은 색상 다르게(초기설정 1번만) 
            SetDefaultSelection(DifButton, selectedDifficulty);
            SetDefaultSelection(TimeButton, selectedTimeLimit); 

            // 각 버튼 배열에 리스너 추가(클릭시 색상 변경) -> OK
            DifficultySet(DifButton);
            TimeLimitSet(TimeButton);


            // OK (1/13)
            // 방 이름
            txtRoomName.text = $"{room.Name}";
            
            // 현재 인원 / 최대 인원
            txtPlayerCount.text = $"{room.PlayerCount}/{room.MaxPlayers}";

            // 난이도(Custom Properties에서 가져오기)
            string difficulty = room.CustomProperties.ContainsKey("difficulty")
                                ? room.CustomProperties["difficulty"].ToString()
                                : "없음";
            // 제한 시간(Custom Properties에서 가져오기)
            string timeLimit = room.CustomProperties.ContainsKey("timeLimit")
                                ? room.CustomProperties["timeLimit"].ToString()
                                : "없음";
            txtDandT.text = $"{difficulty} / {timeLimit}초";
        }
        else
        {
            Debug.LogWarning("현재 방에 접속되어 있지 않음!");
        }
    }

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
                selectedDifficulty = 2;
                break;
            case 1:
                selectedDifficulty = 3;
                break;
            case 2:
                selectedDifficulty = 4;
                break;
        }
        selectedDifficultyIndex = index;
        string difficultyText = GetDifficultyText(selectedDifficulty); // selectedDifficulty 값을 기반으로 실제 문자열로 반환
        UpdateButtonColors(difBtn, index); //색상 업데이트
        UnityEngine.Debug.Log("Selected Difficulty: " + difficultyText); //메시지 출력
    }

    // selectedDifficulty의 값이 2, 3, 4일 때 각각 "초급", "중급", "고급"이라는 문자열을 출력
    public string GetDifficultyText(int difficulty)
    {
        switch (difficulty)
        {
            case 2:
                return "초급";
            case 3:
                return "중급";
            case 4:
                return "고급";
            default:
                return "알 수 없음"; // 다른 값일 경우 기본 값 반환
        }
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

            //buttons[i].onClick.AddListener(() =>
            //{
            //    UpdateButtonColors(buttons, index); //버튼 색상 갱신
            //});
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

    private void PlayersUpdate() //유저 수 업데이트
    {
        if (PhotonNetwork.InRoom)
        {
            Room room = PhotonNetwork.CurrentRoom;
            // 현재 인원 / 최대 인원
            txtPlayerCount.text = $"{room.PlayerCount}/{room.MaxPlayers}";
        }
    }

    // 플레이어가 방에 들어왔을 때 호출
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayersUpdate(); //유저 수 업데이트
    }

    // 플레이어가 방을 나갔을 때 호출
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayersUpdate(); //유저 수 업데이트
    }


    // 방을 나갈때
    // LobbyManager스크립트의 모든 변수 값 초기화
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

        // LobbyManager 오브젝트를 찾고, 그 오브젝트에서 LobbyManager 스크립트를 가져오기
        //LobbyManager lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>(); //씬이 전환되어도 객체정보는 유지
        // 메인으로 다시 돌아가기에, 메인 접속 시 최초 실행하는 함수와 동일하게 수행
    }

    // 네트워크 에러가 발생하거나 방을 나가지 못했을 때
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError($"Disconnected from server: {cause}");
        // 필요한 경우 재접속 로직 추가
    }
}
