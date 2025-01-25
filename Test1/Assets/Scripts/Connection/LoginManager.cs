using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

    //playfabmanager스크립트 연결
    public PlayFabManager playFabManager;

    //패널 관련 선언
    public GameObject emailPanel, registerPanel, playersetPanel, AlarmPanel; //이메일, 회원가입, 유저초기세팅, 알람 패널 4종류

    //인풋 관련 선언
    public InputField EmailInput, PasswordInput, UseridInput;  // 이메일, 비밀번호, 아이디 인풋필드

    // 오류 메시지 텍스트(비밀번호, 이메일, ID)
    public Text PasswordErrorText, EmailErrorText, IDErrorText;   

    //토글 버튼(비밀번호)
    public Toggle PasswordToggle;  // 비밀번호 보기 버튼

    //이메일 규칙 선언
    private string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"; // 이메일 형식 확인을 위한 정규식


    public void Start()
    {
        // 초기 상태 설정
        ResetPasswordToggle(); //토글 비활성화
        ResetWarningTexts(); //경고메시지 비활성화

        //내용이 변경되면 규칙 메시지 띄우기
        EmailInput.onValueChanged.AddListener((text) => ValidateEmail(EmailInput.text));
        PasswordInput.onValueChanged.AddListener((text) => ValidatePassword(PasswordInput.text));
        UseridInput.onValueChanged.AddListener((text) => ValidateUserID(UseridInput.text));

        // 비밀번호 토글 체크하면 입력한 문자 보기(*을 알파벳 형태로)
        PasswordToggle.onValueChanged.AddListener(TogglePasswordVisibility);
    }

    //playfabmanager스크립트 공통 사용 변수
    public GameObject GetEmailLoginPanel() => emailPanel;
    public GameObject GetPlayerSetPanel() => playersetPanel;
    public GameObject GetAlarmPanel() => AlarmPanel;
    public GameObject GetRegisterPanel() => registerPanel;
    public InputField GetEmailInput() => EmailInput;
    public InputField GetPWInput() => PasswordInput;
    public InputField GetIDInput() => UseridInput;


    public void BackBtn1() //뒤로가기 버튼을 눌렀을 때 ->초기 홈 화면으로 이동
    {
        if (EmailInput != null) EmailInput.text = ""; //값 초기화
        if (PasswordInput != null) PasswordInput.text = ""; //값 초기화

        // PasswordToggle을 초기 상태로 설정
        ResetPasswordToggle();

        // 경고 텍스트 숨기기
        ResetWarningTexts();

    }

    public void BackBtn2() //뒤로가기 버튼을 눌렀을 때 ->로그인 화면으로 이동
    {

        var inputFields = new[] { EmailInput, PasswordInput, UseridInput };

        foreach (var inputField in inputFields)
        {
            if (inputField != null)
            {
                inputField.text = ""; // 값 초기화
            }
        }

        // PasswordToggle을 초기 상태로 설정
        ResetPasswordToggle();

        // 경고 텍스트 숨기기
        ResetWarningTexts();

    }


    // 비밀번호보기 토글의 초기화(비활성화 상태)
    public void ResetPasswordToggle()
    {
        if (PasswordToggle != null)
        {
            PasswordToggle.isOn = false; // 기본 상태로 되돌리기
        }
    }

    // 이메일, 비밀번호 경고 메시지 초기화
    public void ResetWarningTexts()
    {
        var errorTexts = new[] { EmailErrorText, PasswordErrorText, IDErrorText };

        foreach (var errorText in errorTexts)
        {
            if (errorText != null)
            {
                errorText.gameObject.SetActive(false);
            }
        }
    }

    //유저 ID 입력 조건 검사에 따른 경고메시지 표시
    public void ValidateUserID(string input)
    {
        // 규칙이 틀리면 오류 메시지 표시
        IDErrorText.gameObject.SetActive(true);
        // 공백 제거
        string inputID = input.Replace(" ", "");

        // 입력 값이 비어있는 경우 기본 경고 메시지
        if (string.IsNullOrEmpty(inputID))
        {
            IDErrorText.text = "ID를 입력해주세요";
            return;
        }

        // 알파벳으로만 이루어졌는지 확인 (3자리 이상)
        if ((Regex.IsMatch(inputID, @"[\u3131-\uD79D]"))) //한글을 포함하고 있으면
        {
            IDErrorText.text = "ID는 알파벳만 포함해야 합니다.";
        }
        else if (inputID.Length < 3)
        {
            IDErrorText.text = "최소 3자리 이상이어야 합니다.";
        }
        else
        {
            IDErrorText.gameObject.SetActive(false);
        }
        // 입력란에 공백을 제거한 값 반영
        UseridInput.text = inputID;
    }


    // 이메일 형식 확인 함수
    public void ValidateEmail(string email)
    {
        EmailErrorText.gameObject.SetActive(true); // 규칙이 틀리면 오류 메시지 표시
        // 공백 제거
        string inputEmail = email.Replace(" ", ""); //공백을 허용하지 않는다

        if (string.IsNullOrEmpty(inputEmail))
        {
            EmailErrorText.text = "이메일을 입력해주세요";
            return;
        }

        if (!Regex.IsMatch(inputEmail, emailPattern))
        {
            EmailErrorText.text = "이메일 형식이 올바르지 않습니다. (@와 .com이 포함된 형식이어야 합니다.)";
        }
        else
        {
            EmailErrorText.gameObject.SetActive(false); // 이메일 형식이 올바르면 오류 메시지 숨김
        }
        // 입력란에 공백을 제거한 값 반영
        EmailInput.text = inputEmail;
    }

    // 비밀번호 규칙 확인 함수
    public void ValidatePassword(string password)
    {
        PasswordErrorText.gameObject.SetActive(true); // 규칙이 틀리면 오류 메시지 표시
        string inputPW = password.Replace(" ", ""); //공백을 허용하지 않는다
        // 입력 값이 비어있는 경우 기본 경고 메시지
        if (string.IsNullOrEmpty(inputPW))
        {
            PasswordErrorText.text = "비밀번호를 입력해주세요";
            return;
        }

        if ((Regex.IsMatch(inputPW, @"[\u3131-\uD79D]")))
        {
            PasswordErrorText.text = "비밀번호는 알파벳만 포함해야 합니다.";
        }
        else if (inputPW.Length < 6) // 6자리 미만이면
        {
            PasswordErrorText.text = "최소 6자리 이상이어야 합니다.";
        }
        else if (inputPW.Length > 20) //길이가 20자 이상이면
        {
            PasswordErrorText.text = "최대 20자까지 입력할 수 있습니다.";
        }
        else
        {
            PasswordErrorText.gameObject.SetActive(false);
        }
        // 입력란에 공백을 제거한 값 반영
        PasswordInput.text = inputPW;
    }


    // 비밀번호 보기/숨기기 전환
    private void TogglePasswordVisibility(bool isOn)
    {
        if (isOn) // Toggle이 활성화된 경우 (비밀번호 보기)
        {
            PasswordInput.contentType = InputField.ContentType.Standard; // 일반 텍스트로 표시
        }
        else // Toggle이 비활성화된 경우 (비밀번호 숨기기)
        {
            PasswordInput.contentType = InputField.ContentType.Password; // *로 표시
        }

        // ContentType 변경 후 InputField를 업데이트
        PasswordInput.ForceLabelUpdate();
    }
}
