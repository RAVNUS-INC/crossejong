using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 로그인 화면 전체를 구성하는 코드
// 일반 이메일 로그인 방법일 경우에만 뒤로 돌아갈 수 있도록 하는 코드
// 나머지 로그인 방법의 경우, 버튼 클릭 시 바로 로그인 시도
public class LoginManager : MonoBehaviour
{
    [SerializeField] private GameObject loginMethodsPanel; //로그인 방법들을 보여주는 첫 화면

    [SerializeField] private GameObject googleLoginPanel; //구글 로그인 선택 시 화면
    [SerializeField] private GameObject naverLoginPanel; //네이버 로그인 선택 시 화면
    [SerializeField] private GameObject appleLoginPanel; //애플 로그인 선택 시 화면
    [SerializeField] public GameObject emailLoginPanel; //일반 이메일 로그인 선택 시 화면
    [SerializeField] public GameObject playersetPanel; // 유저 세팅 패널
    [SerializeField] public GameObject AlarmPanel; // 알람 패널

    public PlayFabManager playFabManager; // PlayFabManager에서 할당
    private InputField emailInput;
    private InputField passwordInput;
    private Toggle passwordToggle;
    private Text emailWarningText;
    private Text passwordWarningText;

    public void Start()
    {
        // PlayFabManager의 EmailInput과 PasswordInput에 접근
        emailInput = playFabManager.EmailInput;
        passwordInput = playFabManager.PasswordInput;
        passwordToggle = playFabManager.PasswordToggle;
        emailWarningText = playFabManager.EmailErrorText;
        passwordWarningText = playFabManager.PasswordErrorText;

        // 초기 상태 설정
        ResetPasswordToggle();
        ResetWarningTexts();
    }

    public GameObject GetEmailLoginPanel() => emailLoginPanel;
    public GameObject GetPlayerSetPanel() => playersetPanel;
    public GameObject GetAlarmPanel() => AlarmPanel;

    public void ShowLoginPanel(string method)
    {
        // 처음 로그인 방법 보여주는 화면은 활성화
        loginMethodsPanel.SetActive(true); 

        // 모든 로그인 및 유저세팅, 알람 패널 비활성화
        googleLoginPanel.SetActive(false);
        naverLoginPanel.SetActive(false);
        appleLoginPanel.SetActive(false);
        emailLoginPanel.SetActive(false);
        playersetPanel.SetActive(false);
        AlarmPanel.SetActive(false);

        // 선택된 로그인 패널 활성화
        switch (method)
        {
            case "Google":
                googleLoginPanel.SetActive(true);
                break;
            case "Naver":
                naverLoginPanel.SetActive(true);
                break;
            case "Apple":
                appleLoginPanel.SetActive(true);
                break;
            case "Email":
                emailLoginPanel.SetActive(true);
                break;
        }

    }

    public void EmailBtn() //이메일 로그인 버튼을 눌렀을 때
    {
        emailLoginPanel.SetActive(true);
        loginMethodsPanel.SetActive(false);
    }

    public void BackBtn() //뒤로가기 버튼을 눌렀을 때
    {
        emailLoginPanel.SetActive(false);
        loginMethodsPanel.SetActive(true);

        if (emailInput != null) emailInput.text = "";
        if (passwordInput != null) passwordInput.text = "";

        // PasswordToggle을 초기 상태로 설정
        ResetPasswordToggle();

        // 경고 텍스트 숨기기
        ResetWarningTexts();

    }

    // 비밀번호보기 토글의 초기화
    private void ResetPasswordToggle()
    {
        if (passwordToggle != null)
        {
            passwordToggle.isOn = false; // 기본 상태로 되돌리기
        }
    }

    // 이메일, 비밀번호의 초기화
    private void ResetWarningTexts()
    {
        if (emailWarningText != null) emailWarningText.gameObject.SetActive(false);
        if (passwordWarningText != null) passwordWarningText.gameObject.SetActive(false);
    }


}
