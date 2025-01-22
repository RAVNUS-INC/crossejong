using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine.Windows;

// 이메일로 로그인/회원가입을 진행할 때

public class PlayFabManager : MonoBehaviour
{
    public LoginManager LoginManager; // 로그인 매니저 스크립트에서 일부 변수 사용을 위해 선언(별도)   

    public Text popupText; // 로그인 시 팝업창에 표시할 알림 내용

    // LoginManager에서 가져온 변수들
    private GameObject EmailPanel, RegisterPanel, AlarmPanel, PlayerSetPanel; //이메일, 회원가입, 유저초기세팅, 알람 패널 4종류
    private InputField EmailInput, PasswordInput, UserIDInput; // 이메일, 비밀번호, 아이디 인풋필드

    //로그인 입력에 대한 알람 패널 내부의 버튼 3종류(이름 변경하면 안됨)
    private Button nextBtn; // 다음 확인 버튼
    private Button retryBtn; // 재시도 확인 버튼
    private Button okBtn; // 로그인 성공 확인 버튼



    private void Start()
    {
        // LoginManager에서에서 패널들을 가져옴
        EmailPanel = LoginManager.GetEmailLoginPanel();
        RegisterPanel = LoginManager.GetRegisterPanel();
        AlarmPanel = LoginManager.GetAlarmPanel();
        PlayerSetPanel = LoginManager.GetPlayerSetPanel();
        EmailInput = LoginManager.GetEmailInput();
        PasswordInput = LoginManager.GetPWInput();
        UserIDInput = LoginManager.GetIDInput();

        // AlarmPanel 안에 있는 버튼은 찾아서 사용
        retryBtn = AlarmPanel.transform.Find("Retrybtn").GetComponent<Button>();
        nextBtn = AlarmPanel.transform.Find("Nextbtn").GetComponent<Button>();
        okBtn = AlarmPanel.transform.Find("OKbtn").GetComponent<Button>();
    }

    // 로그인 버튼 클릭 시
    public void LoginBtn()
    {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    // 회원가입 버튼 클릭 시
    public void RegisterBtn()
    {
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UserIDInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    // 단어완성횟수 변수 저장(초기값 0)
    public void SetStat()
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "WordCompletionCount", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => print("단어완성횟수 초기화 및 저장 완료"), (error) => print("변수 저장 실패"));
    }

    // 로그인 실패 시
    public void OnLoginFailure(PlayFabError error)
    {
        print("로그인 실패");
        UpdateText("로그인에 실패하였습니다.");
        SetUIState(true, false, false, true, false, false); // 로그인 실패 시 UI 상태 설정
    }

    // 로그인 성공 시->세팅 넘기고 로비로 넘어가야지
    public void OnLoginSuccess(LoginResult result)
    {
        print("로그인 성공");
        UpdateText("로그인에 성공하였습니다.");
        SetUIState(true, false, true, false, false, false); // 로그인 성공 시 UI 상태 설정
    }

    // 회원가입 실패 시
    public void OnRegisterFailure(PlayFabError error)
    {
        print("회원가입 실패");
        UpdateText("회원가입에 실패하였습니다.");
        SetUIState(true, false, false, true, false, false); // 회원가입 실패 시 UI 상태 설정
    }

    // 회원가입 성공 시
    public void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        print("회원가입 성공");
        UpdateText("회원가입에 성공하였습니다.");
        SetUIState(true, true, false, false, false, false); // 회원가입 성공 시 UI 상태 설정
    }

    // 팝업 텍스트 상태 업데이트
    public void UpdateText(string message)
    {
        popupText.text = message;
    }

    //여러 SetActive 호출을 한 번에 처리
    public void SetUIState(bool alarmPanelActive, bool nextBtnActive, bool okBtnActive, bool retryBtnActive, bool emailPanelActive, bool registerPanelActive)
    {
        AlarmPanel.gameObject.SetActive(alarmPanelActive);
        nextBtn.gameObject.SetActive(nextBtnActive);
        okBtn.gameObject.SetActive(okBtnActive);
        retryBtn.gameObject.SetActive(retryBtnActive);
        EmailPanel.gameObject.SetActive(emailPanelActive);
        RegisterPanel.gameObject.SetActive(registerPanelActive);
    }
}

