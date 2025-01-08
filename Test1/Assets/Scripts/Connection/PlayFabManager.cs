using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

// 이메일로 로그인/회원가입을 진행할 때

public class PlayFabManager : MonoBehaviour
{
    public LoginManager LoginManager; // 로그인 매니저 스크립트에서 일부 변수 사용을 위해 선언(별도)

    public InputField EmailInput, PasswordInput, UseridInput; // 이메일, 비밀번호, 유저ID 입력 필드
    public Text popupText; // 팝업창에 표시할 알림 내용

    // LoginManager에서 가져온 변수들
    private GameObject EmailPanel; // 이메일 로그인 화면 패널
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
    public UserSetManager userSetManager;

    //신규회원 구분
    public Toggle isNewMemberToggle;  // 신규 회원 토글
    public GameObject playerIDInputField;  // PlayerID 입력영역

    private void Start()
    {
        // LoginManager에서에서 패널들을 가져옴
        EmailPanel = LoginManager.GetEmailLoginPanel();
        AlarmPanel = LoginManager.GetAlarmPanel();
        PlayerSetPanel = LoginManager.GetPlayerSetPanel();

        // AlarmPanel 안에 있는 버튼을 찾아서 사용
        retryBtn = AlarmPanel.transform.Find("Retrybtn").GetComponent<Button>();
        nextBtn = AlarmPanel.transform.Find("Nextbtn").GetComponent<Button>();
        okBtn = AlarmPanel.transform.Find("OKbtn").GetComponent<Button>();

        // 이메일 입력 시 형식 확인
        EmailInput.onValueChanged.AddListener(ValidateEmail);
        // 비밀번호 입력 시 규칙 확인
        PasswordInput.onValueChanged.AddListener(ValidatePassword);
        // Toggle의 onValueChanged 이벤트에 함수 등록
        PasswordToggle.onValueChanged.AddListener(TogglePasswordVisibility);

        // 토글 상태가 변경될 때마다 호출될 리스너 추가
        isNewMemberToggle.onValueChanged.AddListener(OnToggleValueChanged);

        // 초기 상태에 따라 PlayerID 입력란을 설정
        OnToggleValueChanged(isNewMemberToggle.isOn);

    }

    // 토글 값이 변경되면 호출되는 함수
    void OnToggleValueChanged(bool isNewMember)
    {
        if (isNewMember)
        {
            // 신규 회원일 때 PlayerID 입력란 표시
            playerIDInputField.SetActive(true);
        }
        else
        {
            // 기존 회원일 때 PlayerID 입력란 숨기기
            playerIDInputField.SetActive(false);
        }
    }




    // 이메일 형식 확인 함수
    private void ValidateEmail(string email)
    {
        if (Regex.IsMatch(email, emailPattern))
        {
            EmailErrorText.gameObject.SetActive(false); // 이메일 형식이 올바르면 오류 메시지 숨김
        }
        else
        {
            EmailErrorText.gameObject.SetActive(true);  // 이메일 형식이 틀리면 오류 메시지 표시
            EmailErrorText.text = "이메일 형식이 올바르지 않습니다. (@와 .com이 포함된 형식이어야 합니다.)";
        }
    }

    // 비밀번호 규칙 확인 함수
    private void ValidatePassword(string password)
    {
        if (password.Length <= 20 && Regex.IsMatch(password, @"^[a-zA-Z]+$"))
        {
            PasswordErrorText.gameObject.SetActive(false); // 규칙이 맞으면 오류 메시지 숨김
        }
        else
        {
            PasswordErrorText.gameObject.SetActive(true); // 규칙이 틀리면 오류 메시지 표시
            if (password.Length > 20)
            {
                PasswordErrorText.text = "비밀번호는 최대 20자까지 입력할 수 있습니다.";
            }
            else
            {
                PasswordErrorText.text = "비밀번호는 알파벳만 포함해야 합니다.";
            }
        }
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
    }

    // 팝업 텍스트 상태 업데이트
    public void UpdateText(string message)
    {
        popupText.text = message;
    }

    public void RetryBtn() //통과를 못했고 확인 버튼
    {
        AlarmPanel.SetActive(false);
        EmailPanel.SetActive(true); //이메일 로그인 다시 시도하기
    }

    public void NextBtn() //회원가입 통과를 했고 확인 버튼
    {
        PlayerSetPanel.SetActive(true); //유저 세팅으로 넘어가기
        AlarmPanel.SetActive(false);
    }

    public void OkBtn() //로그인 성공을 했고 메인으로 넘어가며 서버요청(닉네임 정보 받아오기)
    {
        userSetManager.OnClickConnect();
    }


}

