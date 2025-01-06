using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 초기 유저 설정 화면에서 로비로 넘어갈때 작동하는 코드
public class ConnectionManager : MonoBehaviourPunCallbacks
{
    private ConnectionManager s_instance;
    public ConnectionManager Instance { get { return s_instance; } }

    [SerializeField]
    InputField inputText; //닉네임 입력
    [SerializeField]
    Button confirmButton; //제출 버튼
    [SerializeField]
    Text warningText; // 경고 메시지를 출력할 UI 텍스트

    private const int MaxLength = 5; // 최대 입력 길이

    void Start()
    {
        confirmButton.interactable = false; // 기본적으로 버튼 비활성화
        warningText.text = ""; // 초기 경고 메시지 비우기

        //내용이 변경되었을때
        inputText.onValueChanged.AddListener(ValidateNickname);

        //내용을 제출했을때
        inputText.onSubmit.AddListener(OnSubmit);

        ////커서가 다른곳을 누르면
        //inputText.onEndEdit.AddListener(
        //    (string s) =>
        //    {
        //        Debug.Log("OnEndmit" + s);
        //    }
        //);

        confirmButton.onClick.AddListener(OnClickConnect);
    }

    private void ValidateNickname(string input)
    {
        /// 한글(완성형/자음/모음)과 숫자만 허용하는 정규식
        string validPattern = @"^[가-힣ㄱ-ㅎㅏ-ㅣ0-9]*$";

        // 공백 제거
        input = input.Replace(" ", "");

        // 입력 값이 패턴에 맞지 않으면 수정
        if (!Regex.IsMatch(input, validPattern))
        {
            warningText.text = "한글과 숫자만 입력 가능합니다.";
            confirmButton.interactable = false; // 확인 버튼 비활성화
        }
        else if (input.Length > MaxLength) // 길이 제한 초과 검사
        {
            warningText.text = $"최대 {MaxLength}자까지만 입력 가능합니다.";
            confirmButton.interactable = false; // 확인 버튼 비활성화
        }
        else if (input.Length == 0) // 빈 문자열 검사
        {
            warningText.text = "닉네임을 입력해주세요.";
            confirmButton.interactable = false; // 확인 버튼 비활성화
        }
        else
        {
            warningText.text = ""; // 규칙에 맞으면 경고 메시지 제거
            confirmButton.interactable = true; // 확인 버튼 활성화
        }

    }

    private void OnDestroy()
    {
        // 이벤트 해제
        inputText.onValueChanged.RemoveListener(ValidateNickname);
    }

    void OnSubmit(string s) // s는 문자열
    {
        Debug.Log("OnSubmit " + s); // 닉네임을 입력하고 제출했음을 알림
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("마스터 서버 접속 성공");

        //나의 이름을 포톤에 설정
        PhotonNetwork.NickName = inputText.text;

        //로비진입
        PhotonNetwork.JoinLobby();
    }

    //Lobby 진입에 성공했으면 호출되는 함수
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        //메인 씬으로 이동
        PhotonNetwork.LoadLevel("Main");

        print("로비 진입 성공");

    }
    public void OnClickConnect()
    {
        // 마스터 서버 접속 요청
        PhotonNetwork.ConnectUsingSettings();
    }
}
