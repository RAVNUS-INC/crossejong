using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
//using UnityEngine.UIElements;

// 로그인 화면 전체를 구성하는 코드
// 일반 이메일 로그인 방법일 경우에만 뒤로 돌아갈 수 있도록 하는 코드
// 나머지 로그인 방법의 경우, 버튼 클릭 시 바로 로그인 시도
public class LoginManager : MonoBehaviour
{
    //[SerializeField] private GameObject loginMethodsPanel; //로그인 방법들을 보여주는 첫 화면

    //[SerializeField] private GameObject googleLoginPanel; //구글 로그인 선택 시 화면
    //[SerializeField] private GameObject naverLoginPanel; //네이버 로그인 선택 시 화면
    //[SerializeField] private GameObject appleLoginPanel; //애플 로그인 선택 시 화면
    [SerializeField] public GameObject emailLoginPanel; //일반 이메일 로그인 선택 시 화면
    [SerializeField] public GameObject registerPanel; //회원가입 선택 시 화면
    [SerializeField] public GameObject playersetPanel; // 유저 세팅 패널
    [SerializeField] public GameObject AlarmPanel; // 알람 패널

    public PlayFabManager playFabManager; // PlayFabManager에서 할당
    private InputField emailInput;
    private InputField passwordInput;
    private InputField idInput;

    private Toggle passwordToggle;

    private Text emailWarningText;
    private Text passwordWarningText;

    public void Start()
    {
        // PlayFabManager의 EmailInput과 PasswordInput에 접근
        emailInput = playFabManager.EmailInput;
        passwordInput = playFabManager.PasswordInput;
        idInput = playFabManager.playerIDInputField;
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
    public GameObject GetRegisterPanel() => registerPanel;

    public void BackBtn1() //뒤로가기 버튼을 눌렀을 때 ->로그인 홈화면으로 이동
    {
        if (emailInput != null) emailInput.text = ""; //값 초기화
        if (passwordInput != null) passwordInput.text = ""; //값 초기화

        // PasswordToggle을 초기 상태로 설정
        ResetPasswordToggle();

        // 경고 텍스트 숨기기
        ResetWarningTexts();

    }

    public void BackBtn2() //뒤로가기 버튼을 눌렀을 때 ->로그인 홈화면으로 이동
    {

        if (emailInput != null) emailInput.text = ""; //값 초기화
        if (passwordInput != null) passwordInput.text = ""; //값 초기화
        if (idInput != null) idInput.text = ""; //값 초기화

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
