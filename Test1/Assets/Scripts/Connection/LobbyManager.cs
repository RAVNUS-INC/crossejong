using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.UIElements;
//using static UnityEditor.Progress;
using static UnityEngine.EventSystems.PointerEventData;
using Button = UnityEngine.UI.Button;
using Debug = UnityEngine.Debug;
using Image = UnityEngine.UI.Image;

//방 생성 및 방 참여에 관한 코드
public class LobbyManager : MonoBehaviourPunCallbacks
{

    // 방 생성 관련 UI
    [SerializeField] TMP_InputField input_RoomName; //방 이름
    [SerializeField] Button[] btn_MaxPlayers, btn_Difficulty, btn_TimeLimit; // 최대인원, 난이도, 제한시간 버튼
    [SerializeField] Image[] PlayerSel, DiffSel, TimeSel; //선택된 상태를 나타내는 이미지 배열

    // 방 생성 시 이름 규칙 경고메시지
    [SerializeField] TMP_Text warningText;

    // 방 생성 버튼과 방 참여 버튼, 방 목록을 표시할 스크롤뷰
    [SerializeField] Button btn_CreateRoom, btn_JoinRoom; // 방 만들기, 참여 버튼
    [SerializeField] GameObject roomListItem; // 방 목록 프리팹

    // 방 생성 시 옵션들(선택하지 않아도 기본으로 설정)
    private int selectedMaxPlayers = 2; // 최대인원(2, 3, 4) +5명까지도 추가해야함!!
    private string selectedDifficulty = "초급"; // 난이도(초급, 중급, 고급)
    private int selectedTimeLimit = 15; // 카드 놓기까지 제한시간(15초, 30초, 45초)

    // 방 생성 옵션-인원을 제외한 항목의 인덱스
    private int selectedDifficultyIndex = 0; // 난이도 선택 인덱스
    private int selectedTimeLimitIndex = 0; // 제한시간 인덱스

    //생성된 방목록을 보여줄 스크롤뷰 콘텐츠 영역
    public Transform rtContent;
    // 방이름 최대 입력 길이
    private const int MaxLength = 12; 
    // 방 목록을 가지고 있는 Dictionary 변수
    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();
    // 생성된 방 이름을 저장하는 변수
    private string selectedRoomName = null;
    // 난이도 변경 객체 참조
    public ChangeLevel Changelevel; 

    private void Awake() 
    {
        ResetRoomSetPanel(); // 첫 메인 접속 시 최초 실행
    }

    // 방 만들 때 선택 옵션 버튼과 방이름 규칙에 관한 초기화(방 속성 x버튼 누를때도 실행-초기화)
    public void ResetRoomSetPanel()
    {
        // 기본 버튼 설정값 (0,0,0) 노란색으로 표시
        SetDefaultSelection(0);

        // 옵션 버튼 선택하면 노란색으로 바뀌도록 하는 코드
        MaxPlayerSet(btn_MaxPlayers);
        DifficultySet(btn_Difficulty);
        TimeLimitSet(btn_TimeLimit);

        // 방 이름 입력 필드 초기화
        input_RoomName.text = ""; //방 이름 기본 공백 상태
        warningText.text = ""; // 초기 경고 메시지 비우기
        selectedRoomName = null; //생성된 방 저장 변수 비우기
        btn_CreateRoom.interactable = false; // 처음에는 방 생성 버튼 비활성화
        btn_JoinRoom.interactable = false; // 처음에는 방 참여 버튼 비활성화
        input_RoomName.onValueChanged.AddListener(ValidateRoomName); //방 이름 작성할 시, 방 이름 규칙 검사

        // 기본 설정인 초급 난이도로 초기화
        Changelevel.ChangeLevelLow();

        UnityEngine.Debug.Log("메인화면 초기화 완료");
    }

    private void MaxPlayerSet(Button[] buttons)
    {
        // MaxPlayers 버튼에 리스너 추가
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.RemoveAllListeners(); // 기존 이벤트 제거
            buttons[i].onClick.AddListener(() => OnMaxPlayersButtonClicked(index, buttons)); //선택한 버튼의 색상 변경
        }
    }

    public void DifficultySet(Button[] buttons)
    {
        // Difficulty 버튼에 리스너 추가
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.RemoveAllListeners(); // 기존 이벤트 제거
            buttons[i].onClick.AddListener(() => OnDifficultyButtonClicked(index, buttons));
        }
    }

    public void TimeLimitSet(Button[] buttons)
    {
        // TimeLimit 버튼에 리스너 추가
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.RemoveAllListeners(); // 기존 이벤트 제거
            buttons[i].onClick.AddListener(() => OnTimeLimitButtonClicked(index, buttons));
        }
    }

    private void ValidateRoomName(string input) //방 이름의 규칙에 관한 코드
    {

        // 한글(완성형/자음/모음)과 숫자만 허용하는 정규식
        string validPattern = @"^[가-힣ㄱ-ㅎㅏ-ㅣ0-9\s]*$";

        // 입력 값이 패턴에 맞지 않으면 수정
        if (!Regex.IsMatch(input, validPattern))
        {
            warningText.text = "한글, 숫자, 공백만 입력 가능합니다.";
            btn_CreateRoom.interactable = false; // 방 생성 버튼 비활성화
        }
        else if (input.Length > MaxLength) // 길이 제한 초과 검사
        {
            warningText.text = $"최대 {MaxLength}자까지만 입력 가능합니다.";
            btn_CreateRoom.interactable = false; // 방 생성 버튼 비활성화
        }
        else if (input.Length == 0) // 빈 문자열 검사
        {
            warningText.text = "방 이름을 입력해주세요.";
            btn_CreateRoom.interactable = false; // 방 생성 버튼 비활성화
        }
        else
        {
            warningText.text = ""; // 규칙에 맞으면 경고 메시지 제거
            btn_CreateRoom.interactable = true; // 방 생성 버튼 활성화
        }
    }

    //방 목록의 변화가 있을 때 호출되는 함수(포톤 기본 제공)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);  // 기본 메서드 호출

        //Content에 자식으로 붙어있는 Item을 다 삭제
        DeleteRoomListItem();

        //dicRoomInfo 변수를 roomList를 이용해서 갱신
        UpdateRoomListItem(roomList);

        //dicRoom을 기반으로 roomListItem을 만들자
        CreateRoomListItem();

    }

    // 방 목록을 선택했을 때
    void SelectRoomItem(string roomName, GameObject button)
    {
        // 이전에 선택된 버튼의 색상을 초기화
        if (roomListItem != null)
        {
            var prevImage = roomListItem.GetComponent<Image>();
            if (prevImage != null)
            {
                prevImage.color = Color.white; // 기본 색상으로 복원
            }
        }

        // 현재 선택된 버튼의 색상을 노란색으로 변경
        roomListItem = button;
        var currentImage = roomListItem.GetComponent<Image>();
        if (currentImage != null)
        {
            currentImage.color = Color.yellow; // 노란색으로 설정
        }

        // 선택한 방 이름을 전달
        selectedRoomName = roomName;
        // 참여 버튼 활성화
        btn_JoinRoom.interactable = true; 
    }


    // 방 옵션 선택 시 이뤄지는 ui와 index 업데이트에 관한 코드
    void OnMaxPlayersButtonClicked(int index, Button[] PlayBtn)
    {
        switch (index) // 2, 3, 4 플레이어 옵션
        {
            case 0:
                selectedMaxPlayers = 2;
                break;
            case 1:
                selectedMaxPlayers = 3;
                break;
            case 2:
                selectedMaxPlayers = 4;
                break; 
        }
        UpdateButtonColors(PlayBtn, index);
        UnityEngine.Debug.Log("Selected Max Players: " + selectedMaxPlayers); //클릭할 때마다 메시지 출력
    }

    public void OnDifficultyButtonClicked(int index, Button[] difBtn)
    {
        switch (index) // 0: 초급, 1: 중급, 2: 고급
        {
            case 0:
                selectedDifficulty = "초급";
                // 데이터를 초급 난이도로 업데이트
                Changelevel.ChangeLevelLow();
                break;
            case 1:
                selectedDifficulty = "중급";
                // 데이터를 중급 난이도로 업데이트
                Changelevel.ChangeLevelMiddle();
                break;
            case 2:
                selectedDifficulty = "고급";
                // 데이터를 고급 난이도로 업데이트
                Changelevel.ChangeLevelHigh();
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
        UpdateButtonColors(TimBtn, index);
        UnityEngine.Debug.Log("Selected Time Limit: " + selectedTimeLimit);
    }

    // 방 생성 후 유저가 옵션 변경 패널을 열었을 때, 현재 방 옵션 색상을 칠해서 보여준다
    private void SetDefaultSelection(int defaultIndex)
    {

        for (int i = 0; i < 3; i++) //세번 반복(각 버튼 배열 길이)
        {
            if (i == defaultIndex) //현재 선택한 인덱스(0)와 i값이 같을때
            {
                PlayerSel[i].gameObject.SetActive(true);
                TimeSel[i].gameObject.SetActive(true);
                DiffSel[i].gameObject.SetActive(true);
            }
            else
            {
                PlayerSel[i].gameObject.SetActive(false);
                TimeSel[i].gameObject.SetActive(false);
                DiffSel[i].gameObject.SetActive(false);
            }
        }

    }

    // 선택된 버튼을 실제로 색칠하는 함수
    private void UpdateButtonColors(Button[] buttons, int selectedIndex)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == selectedIndex) //현재 선택한 인덱스와 i값이 같을때
            {
                if (buttons == btn_MaxPlayers)
                {
                    PlayerSel[i].gameObject.SetActive(true);
                }
                else if (buttons == btn_TimeLimit)
                {
                    TimeSel[i].gameObject.SetActive(true);
                }
                else
                {
                    DiffSel[i].gameObject.SetActive(true);
                }
            }
            else //현재 선택한 인덱스와 i값이 다를때
            {
                if (buttons == btn_MaxPlayers)
                {
                    PlayerSel[i].gameObject.SetActive(false);
                }
                else if (buttons == btn_TimeLimit)
                {
                    TimeSel[i].gameObject.SetActive(false);
                }
                else
                {
                    DiffSel[i].gameObject.SetActive(false);
                }
            }
        }
    }

    void DeleteRoomListItem() // 기존 방 정보 삭제할 때
    {

        foreach (Transform tr in rtContent)
        {
            Destroy(tr.gameObject);
        }
    }

    // 스크롤 뷰에 보여지는 방 목록을 갱신 할 때
    void UpdateRoomListItem(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                dicRoomInfo.Remove(info.Name); // 방 정보 삭제
            }
            else
            {
                dicRoomInfo[info.Name] = info; // 방 정보 추가 또는 업데이트
            }
        }
        UnityEngine.Debug.Log($"[DEBUG] 현재 방 개수: {dicRoomInfo.Count}");
        
    }

    // 생성된 방 목록을 스크롤 뷰에 보여줄 때
    void CreateRoomListItem()
    {
        int count = 0;
        foreach (RoomInfo info in dicRoomInfo.Values)
        {
            //방 정보 생성과 동시에 ScrollView-> Content의 자식으로 하자
            GameObject go = Instantiate(roomListItem, rtContent); //인자: 프리팹, 콘텐츠 순

            //생성된 item에서 RoomListItem 컴포넌트를 가져온다.
            RoomListItem item = go.GetComponent<RoomListItem>();

            // 난이도와 제한 시간을 커스텀 프로퍼티에서 가져옴
            string difficulty = info.CustomProperties.ContainsKey("difficulty") ?
                                info.CustomProperties["difficulty"].ToString() :
                                "없음";
            int timeLimit = info.CustomProperties.ContainsKey("timeLimit") ?
                            Convert.ToInt32(info.CustomProperties["timeLimit"]) :
                            0;

            // 가져온 컴포넌트가 가지고 있는 SetInfo 함수 실행(출력 형태 설정)
            item.SetInfo(info.Name, info.PlayerCount, info.MaxPlayers, difficulty, timeLimit);
            //난이도와 제한시간만 custom properties에 선언, 나머지는 photon에서 기본제공

            // item 클릭되었을 때 호출되는 함수 등록
            item.onDelegate = (roomName) =>
            {
                // SelectRoomItem을 바로 호출
                SelectRoomItem(roomName, go); // roomName과 현재 버튼(GameObject)을 전달 -> 선택된 방목록의 색상이 변경되도록
            };
            count++;
        }
        //UnityEngine.Debug.Log($"생성된 방 UI 개수: {count}");
    }

    // 방 생성 할 때(변수고정불변)
    public void OnClickCreateRoom()
    {
        //방 옵션
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)selectedMaxPlayers;

        //커스텀 룸 프로퍼티 설정(중요)
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
           //난이도, 제한시간 각각에 대한 인덱스와 실제 텍스트로 반영될 값들
            {"DifficultyIndex", selectedDifficultyIndex},  // 난이도 index
            {"difficulty", selectedDifficulty} ,  // 난이도 str값(초급,중급,고급)
            {"DifficultyContents", Changelevel.cardFrontBlack.ToArray() }, // 난이도 카드 내용(가,갸,거..), 전달을 위해 배열로 형태변경
            {"TimeLimitIndex", selectedTimeLimitIndex},  // 제한시간 index
            {"timeLimit", selectedTimeLimit}  // 제한시간 int값(15,30,45)
        };

        //로비에도 보이게 할 것인가?(목록에)->건드리면 X
        options.CustomRoomPropertiesForLobby = new string[] { "difficulty", "timeLimit" };

        //방 목록에 보이게 할것인가?
        options.IsVisible = true;

        //방 생성
        PhotonNetwork.CreateRoom(input_RoomName.text, options);

        //로딩바 ui 애니메이션 보여주기
        LoadingSceneController.Instance.LoadScene("MakeRoom");
    }

    public override void OnCreatedRoom() // 방 생성에 성공했을 때
    {
        UnityEngine.Debug.Log("방 생성 성공");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) //방 생성에 실패했을 때
    {
        base.OnCreateRoomFailed(returnCode, message);
        UnityEngine.Debug.Log("방 생성 실패" + message);
    }

    public void OnClickJoinRoom() // 방 입장
    {
        if (!string.IsNullOrEmpty(selectedRoomName)) //방이름이 뭐라도 있으면
        {
            PhotonNetwork.JoinRoom(selectedRoomName);    
        }
        //로딩바 ui 애니메이션 보여주기
        LoadingSceneController.Instance.LoadScene("MakeRoom");

    }

    public override void OnJoinedRoom() // 방에 입장했을 때 자동 호출
    {
        //Debug.Log("방 입장 성공!");

        // 메시지 큐 정지
        PhotonNetwork.IsMessageQueueRunning = false;
    }

    public override void OnJoinRoomFailed(short returnCode, string message) // 방 입장에 실패했을 때
    {
        base.OnJoinRoomFailed(returnCode, message);
        UnityEngine.Debug.Log("방 입장 실패" + message);
    }
}
