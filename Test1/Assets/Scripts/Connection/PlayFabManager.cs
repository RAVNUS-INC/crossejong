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
    public UserSetManager UserSetManager;    //유저세팅매니저 스크립트에서 일부 변수 사용을 위해 선언(별도)

    public InputField EmailInput, PasswordInput, UseridInput;  // 이메일, 비밀번호, 유저 아이디 (로그인 시)

    public Text popupText; // 팝업창에 표시할 알림 내용

    // LoginManager에서 가져온 변수들
    private GameObject EmailPanel; // 이메일 로그인 화면 패널
    private GameObject RegisterPanel; //회원가입 선택 시 화면
    private GameObject AlarmPanel; // 알람 패널
    private GameObject PlayerSetPanel; // 유저 프로필 설정 패널

    //버튼들
    private Button nextBtn; // 다음 확인 버튼
    private Button retryBtn; // 재시도 확인 버튼
    private Button okBtn; // 로그인 성공 확인 버튼

    //비밀번호 관련 선언
    public Text PasswordErrorText;   // 비밀번호 오류 메시지 텍스트
    public Toggle PasswordToggle;  // 비밀번호 보기 버튼

    //이메일 관련 선언
    private string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"; // 이메일 형식 확인을 위한 정규식
    public Text EmailErrorText;      // 이메일 오류 메시지 텍스트

    //유저ID 관련 선언
    public InputField playerIDInputField;  // PlayerID 입력영역
    public Text IDErrorText; //PlayerID 오류 메시지 텍스트

    private void Start()
    {

        // LoginManager에서에서 패널들을 가져옴
        EmailPanel = LoginManager.GetEmailLoginPanel();
        RegisterPanel = LoginManager.GetRegisterPanel();
        AlarmPanel = LoginManager.GetAlarmPanel();
        PlayerSetPanel = LoginManager.GetPlayerSetPanel();

        // AlarmPanel 안에 있는 버튼을 찾아서 사용
        retryBtn = AlarmPanel.transform.Find("Retrybtn").GetComponent<Button>();
        nextBtn = AlarmPanel.transform.Find("Nextbtn").GetComponent<Button>();
        okBtn = AlarmPanel.transform.Find("OKbtn").GetComponent<Button>();

        // 비밀번호 토글 체크하면 문자 볼 수 있음
        PasswordToggle.onValueChanged.AddListener(TogglePasswordVisibility);


    }





    //유저 ID 입력 조건 검사에 따른 경고메시지 표시
    public void ValidateUserID(string input)
    {
        IDErrorText.gameObject.SetActive(true); // 규칙이 틀리면 오류 메시지 표시
        // 공백 제거
        string inputID = input.Replace(" ", ""); //공백을 허용하지 않는다

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
        else if (inputPW.Length < 6 ) // 6자리 미만이면
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

    // 로그인 버튼 클릭 시
    public void LoginBtn()
    {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    // 회원가입 버튼 클릭 시
    public void RegisterBtn()
    {
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UseridInput.text};
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
        AlarmPanel.SetActive(true);
        nextBtn.gameObject.SetActive(false);
        okBtn.gameObject.SetActive(false);
        retryBtn.gameObject.SetActive(true); //재시도 활성화
        EmailPanel.SetActive(false);
        RegisterPanel.SetActive(false);
    }

    // 로그인 성공 시->세팅 넘기고 로비로 넘어가야지
    public void OnLoginSuccess(LoginResult result)
    {
        print("로그인 성공");
        UpdateText("로그인에 성공하였습니다.");
        AlarmPanel.SetActive(true);
        retryBtn.gameObject.SetActive(false);
        okBtn.gameObject.SetActive(true); //성공->메인 활성화
        nextBtn.gameObject.SetActive(false);
        EmailPanel.SetActive(false);
        RegisterPanel.SetActive(false);
    }

    // 회원가입 실패 시
    public void OnRegisterFailure(PlayFabError error)
    {
        print("회원가입 실패");
        UpdateText("회원가입에 실패하였습니다.");
        AlarmPanel.SetActive(true);
        nextBtn.gameObject.SetActive(false);
        okBtn.gameObject.SetActive(false);
        retryBtn.gameObject.SetActive(true); //재시도 활성화
        EmailPanel.SetActive(false);
        RegisterPanel.SetActive(false);
    }

    // 회원가입 성공 시
    public void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        print("회원가입 성공");
        UpdateText("회원가입에 성공하였습니다.");
        AlarmPanel.SetActive(true);
        nextBtn.gameObject.SetActive(true); //다음 활성화
        retryBtn.gameObject.SetActive(false);
        okBtn.gameObject.SetActive(false);
        EmailPanel.SetActive(false);
        RegisterPanel.SetActive(false);
    }

    // 팝업 텍스트 상태 업데이트
    public void UpdateText(string message)
    {
        popupText.text = message;
    }

}

