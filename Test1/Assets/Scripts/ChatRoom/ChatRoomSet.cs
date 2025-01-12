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

    // UI 텍스트 연결
    public Text txtRoomName; // 방 이름
    public Text txtPlayerCount; // 현재인원/최대인원
    public Text txtDandT; // 난이도와 제한시간

    public Button[] DifButton; // 난이도 버튼 배열
    public Button[] TimeButton; // 제한시간 버튼 배열

    GameObject LobbyManager;

    void Start()
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
            LobbyManager lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>(); //씬이 전환되어도 객체정보는 유지

            object ReDifficulty = room.CustomProperties["DifficultyIndex"];
            object ReselectedTimeLimit = room.CustomProperties["TimeLimitIndex"];

            // 변수명을 로비매니저 변수와 똑같게(함수 사용을 위해)
            selectedDifficulty = (int)ReDifficulty; //int형변환
            selectedTimeLimit = (int)ReselectedTimeLimit; //int형변환

            // 처음 선택했던 버튼들(난이도, 제한시간)은 색상 다르게(초기설정 1번만) 
            lobbyManager.SetDefaultSelection(DifButton, selectedDifficulty);
            lobbyManager.SetDefaultSelection(TimeButton, selectedTimeLimit); 

            // 각 버튼 배열에 리스너 추가(클릭시 색상 변경) -> OK
            lobbyManager.DifficultySet(DifButton);
            lobbyManager.TimeLimitSet(TimeButton);



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


    // 플레이어가 방에 들어왔을 때 호출
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateRoomInfo(); // 방 정보 업데이트
    }

    // 플레이어가 방을 나갔을 때 호출
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateRoomInfo(); // 방 정보 업데이트
    }


    // 방 나가기 버튼이 호출하는 메서드
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
