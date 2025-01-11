using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.PointerEventData;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

//방 생성 및 방 참여에 관한 코드
public class LobbyManager : MonoBehaviourPunCallbacks
{
    // 방 생성 관련 UI
    [SerializeField] InputField input_RoomName; //방 이름
    [SerializeField] Button[] btn_MaxPlayers; // 최대인원 버튼
    [SerializeField] Button[] btn_Difficulty; // 난이도 버튼
    [SerializeField] Button[] btn_TimeLimit; // 제한시간 버튼

    // 방 생성 시 이름 규칙 경고메시지
    [SerializeField] Text warningText;

    // 방 생성 버튼과 방 참여 버튼, 방 목록을 표시할 스크롤뷰
    [SerializeField] Button btn_CreateRoom; // 방 만들기 버튼
    [SerializeField] Button btn_JoinRoom; // 방 참여 버튼
    [SerializeField] GameObject roomListItem; // 방 목록 프리팹

    // 방 생성 시 필요한 변수 선언
    int selectedMaxPlayers = 0; // 최대인원(2, 3, 4명)
    int selectedDifficulty = 0; // 난이도(초급, 중급, 고급)
    int selectedTimeLimit = 0; // 카드 놓기까지 제한시간(15초, 30초, 45초)
    public Transform rtContent;
    private const int MaxLength = 12; // 방이름 최대 입력 길이

    // 방 목록을 가지고 있는 Dictionaly 변수
    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();

    void Start()
    {
        SetDefaultSelection(btn_MaxPlayers, 0, out selectedMaxPlayers);
        SetDefaultSelection(btn_Difficulty, 0, out selectedDifficulty);
        SetDefaultSelection(btn_TimeLimit, 0, out selectedTimeLimit);

        // 방 이름 입력 필드 초기화
        input_RoomName.text = ""; //방 이름 기본 공백 상태
        btn_CreateRoom.interactable = false; // 처음에는 방 생성 버튼 비활성화
        warningText.text = ""; // 초기 경고 메시지 비우기

        input_RoomName.onValueChanged.AddListener(ValidateRoomName); //방 이름 작성할 시, 방 이름 규칙 검사
        btn_CreateRoom.onClick.AddListener(OnClickCreateRoom); //방 생성 버튼 클릭 시, 방 생성 수행
        btn_JoinRoom.onClick.AddListener(OnClickJoinRoom); //방 참여 버튼 클릭 시, 방 참여 수행

        // MaxPlayers 버튼에 리스너 추가
        for (int i = 0; i < btn_MaxPlayers.Length; i++)
        {
            int index = i; // 클로저를 위해 로컬 변수 사용
            btn_MaxPlayers[i].onClick.AddListener(() => OnMaxPlayersButtonClicked(index));
        }

        // Difficulty 버튼에 리스너 추가
        for (int i = 0; i < btn_Difficulty.Length; i++)
        {
            int index = i;
            btn_Difficulty[i].onClick.AddListener(() => OnDifficultyButtonClicked(index));
        }

        // TimeLimit 버튼에 리스너 추가
        for (int i = 0; i < btn_TimeLimit.Length; i++)
        {
            int index = i;
            btn_TimeLimit[i].onClick.AddListener(() => OnTimeLimitButtonClicked(index));
        }

        UpdateButtonColors(btn_MaxPlayers, -1); // 초기화
        UpdateButtonColors(btn_Difficulty, -1); // 초기화
        UpdateButtonColors(btn_TimeLimit, -1); // 초기화
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
    private void OnDestroy()
    {
        // 이벤트 해제
        input_RoomName.onValueChanged.RemoveListener(ValidateRoomName);
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
        input_RoomName.text = roomName;
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

        // 방 이름을 입력 필드에 설정
        input_RoomName.text = roomName;
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
            // dicRoomInfo에 info 의 방이름으로 되어있는 key값이 존재하는가
            if (dicRoomInfo.ContainsKey(info.Name))
            {
                // 이미 삭제된 방이라면?
                if (info.RemovedFromList)
                {
                    dicRoomInfo.Remove(info.Name); // 방 정보를 삭제
                    continue;
                }
            }
            dicRoomInfo[info.Name] = info; // 방 정보를 추가, 업데이트
        }
    }


    // 생성된 방 목록을 스크롤 뷰에 보여줄 때
    void CreateRoomListItem() 
    {
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

            /// 가져온 컴포넌트가 가지고 있는 SetInfo 함수 실행
            item.SetInfo(info.Name, info.PlayerCount, info.MaxPlayers, difficulty, timeLimit);

            // item 클릭되었을 때 호출되는 함수 등록
            item.onDelegate = (roomName) =>
            {
                // SelectRoomItem을 바로 호출
                SelectRoomItem(roomName, go); // roomName과 현재 버튼(GameObject)을 전달 -> 선택된 방목록의 색상이 변경되도록
            };
        }
    }


    // 방 생성 할 때
    public void OnClickCreateRoom()
    {
        //방 옵션
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)selectedMaxPlayers;
        string difficultyText = GetDifficultyText(selectedDifficulty);

        //커스텀 룸 프로퍼티 설정(중요)
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {"maxPlayers", selectedMaxPlayers},   // 최대 플레이어 수 설정
            {"difficulty", difficultyText},
            {"timeLimit", selectedTimeLimit}
        };

        //로비에서도 보여줄 프로퍼티 설정
        options.CustomRoomPropertiesForLobby = new string[] { "maxPlayers", "difficulty", "timeLimit" };

        //방 목록에 보이게 할것인가?
        options.IsVisible = true;

        //방 생성
        PhotonNetwork.CreateRoom(input_RoomName.text, options);
    }


    public override void OnCreatedRoom() // 방 생성에 성공했을 때
    {
        base.OnCreatedRoom();

        UnityEngine.Debug.Log("방 생성 성공");

        PhotonNetwork.LoadLevel("MakeRoom");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) //방 생성에 실패했을 때
    {
        base.OnCreateRoomFailed(returnCode, message);
        UnityEngine.Debug.Log("방 생성 실패" + message);
    }

    public void OnClickJoinRoom() // 방 입장
    {
        PhotonNetwork.JoinRoom(input_RoomName.text);

    }

    public override void OnJoinedRoom() // 방 입장에 성공했을 때
    {
        base.OnJoinedRoom();

        UnityEngine.Debug.Log("방 입장 성공");

        PhotonNetwork.LoadLevel("MakeRoom");
    }

    public override void OnJoinRoomFailed(short returnCode, string message) // 방 입장에 실패했을 때
    {
        base.OnJoinRoomFailed(returnCode, message);
        UnityEngine.Debug.Log("방 입장 실패" + message);
    }



    // 방 생성을 위한 옵션 선택 시 이뤄지는 ui의 변화와 index 업데이트에 관한 코드
    void OnMaxPlayersButtonClicked(int index)
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
        UpdateButtonColors(btn_MaxPlayers, index);
        UnityEngine.Debug.Log("Selected Max Players: " + selectedMaxPlayers);
    }

    void OnDifficultyButtonClicked(int index)
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
        // selectedDifficulty 값을 기반으로 실제 문자열로 반환
        string difficultyText = GetDifficultyText(selectedDifficulty);
        UpdateButtonColors(btn_Difficulty, index);
        UnityEngine.Debug.Log("Selected Difficulty: " + difficultyText);
    }

    // selectedDifficulty의 값이 2, 3, 4일 때 각각 "초급", "중급", "고급"이라는 문자열을 출력
    string GetDifficultyText(int difficulty) 
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

    void OnTimeLimitButtonClicked(int index)
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
        UpdateButtonColors(btn_TimeLimit, index);
        UnityEngine.Debug.Log("Selected Time Limit: " + selectedTimeLimit);
    }

    private void SetDefaultSelection(Button[] buttons, int defaultIndex, out int selectedValue)
    {
        selectedValue = defaultIndex;

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;

            ColorBlock colorBlock = buttons[i].colors;
            colorBlock.normalColor = Color.white; // 기본 색상 화이트
            colorBlock.selectedColor = Color.yellow; //선택된 색상 싸이언
            buttons[i].colors = colorBlock;

            buttons[i].onClick.AddListener(() =>
            {
                UpdateButtonColors(buttons, index); //버튼 색상 갱신
            });
        }

        UpdateButtonColors(buttons, defaultIndex);
    }

    // 버튼 배열의 색상 업데이트 함수
    void UpdateButtonColors(Button[] buttons, int selectedIndex)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            ColorBlock colorBlock = buttons[i].colors;
            if (i == selectedIndex)
            {
                colorBlock.normalColor = Color.yellow;
            }
            else
            {
                colorBlock.normalColor = Color.white;
            }
            buttons[i].colors = colorBlock;
        }
    }
}
