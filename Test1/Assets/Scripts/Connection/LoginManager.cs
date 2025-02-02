using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

// 로그인 화면 전체를 구성하는 코드
// 일반 이메일 로그인 방법일 경우에만 뒤로 돌아갈 수 있도록 하는 코드
// 나머지 로그인 방법의 경우, 버튼 클릭 시 바로 로그인 시도
public class LoginManager : MonoBehaviour
{
    //패널 관련 선언
    public GameObject emailPanel, registerPanel, playersetPanel, AlarmPanel; //이메일, 회원가입, 유저초기세팅, 알람 패널 4종류

    //인풋 관련 선언
    public InputField EmailInput, PasswordInput, UseridInput;  // 이메일, 비밀번호, 아이디 인풋필드

    // 오류 메시지 텍스트(비밀번호, 이메일, ID)
    public Text PasswordErrorText, EmailErrorText, IDErrorText;

    // 알림 창 내 버튼들
    public Button NextBtn, OkBtn, ReBtn;

    //토글 버튼(비밀번호)
    public Toggle PasswordToggle;  // 비밀번호 보기 버튼

    //이메일 규칙 선언
    private string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"; // 이메일 형식 확인을 위한 정규식

    // 로그인 시 팝업창에 표시할 알림 내용
    public Text popupText; 

    //playerprefs에 저장할 내용들
    private const string FIRST_LOGIN_KEY = "IsFirstLogin"; // 첫 로그인 여부
    private const string DISPLAYNAME_KEY = "DisplayName"; // 유저의 DisplayName
    private const string IMAGEINDEX_KEY = "ImageIndex"; // 유저의 이미지 인덱스
    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // 저장 키


    void Start() // 초기 상태 설정
    {
        // 로그인 상태 확인 및 첫 로그인 체크
        CheckLoginStatus();

        ResetPasswordToggle(); //토글 비활성화
        ResetWarningTexts(); //경고메시지 비활성화

        //내용이 변경되면 규칙 메시지 띄우기
        EmailInput.onValueChanged.AddListener((text) => ValidateEmail(EmailInput.text));
        PasswordInput.onValueChanged.AddListener((text) => ValidatePassword(PasswordInput.text));
        UseridInput.onValueChanged.AddListener((text) => ValidateUserID(UseridInput.text));

        // 비밀번호 토글 체크하면 입력한 문자 보기(*을 알파벳 형태로)
        PasswordToggle.onValueChanged.AddListener(TogglePasswordVisibility);
    }

    private void CheckLoginStatus() //로그인 상태에 따라 다른 씬으로 이동
    {
        // PlayerPrefs에서 첫 로그인 상태 체크(기본값 1)
        bool isFirstLogin = !PlayerPrefs.HasKey(FIRST_LOGIN_KEY) || PlayerPrefs.GetInt(FIRST_LOGIN_KEY) == 1;

        if (isFirstLogin)
        {
            // 첫 로그인인 경우(1), 로그인으로 이동
            //SceneManager.LoadScene("Login");
            Debug.Log("첫 로그인 감지 -> 로그인 화면으로 이동");
        }
        else
        {
            // 이미 로그인된 유저라면(0) PlayFab 세션 확인
            if (PlayFabClientAPI.IsClientLoggedIn())
            {
                // 세션이 유효하면 로컬에서 저장된 정보 불러오기
                LoadUserInfoFromPrefs();

                // Main 씬으로 이동
                SceneManager.LoadScene("Main");
            }
            else
            {
                // 로그인 세션이 만료된 경우 (비활성화된 상태)
                // 로그인 화면으로 이동
                //SceneManager.LoadScene("Login");
            }
        }
    }
    private void LoadUserInfoFromPrefs() // 플레이어 정보 로컬에 저장된 값 불러오기
    {   
        // GetString의 두번째 값은 기본값을 나타냄
        string displayName = PlayerPrefs.GetString(DISPLAYNAME_KEY, "Guest");
        int imageIndex = PlayerPrefs.GetInt(IMAGEINDEX_KEY, 0);

        // 필요한 곳에 정보를 설정하거나 UI에 반영
        Debug.Log($"로컬 - DisplayName: {displayName}, ImageIndex: {imageIndex}");
    }

    // 로그인 버튼 클릭 시
    public void LoginBtn() //로그인 버튼에 연결
    {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    // 회원가입 버튼 클릭 시
    public void RegisterBtn() //회원가입 버튼에 연결
    {
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UseridInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    // 단어완성횟수 변수 저장(초기값 0)
    public void SetStat() //유저 초기 세팅 확인버튼(usersetmanager)에 연결
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

        // 로그인 성공 시 UI 상태 설정
        SetUIState(true, false, true, false, false, false);

        // 1. DisplayName 불러오기 (계정 정보 방식)
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result =>
        {
            string displayName = result.AccountInfo.TitleInfo.DisplayName;
            PlayerPrefs.SetString(DISPLAYNAME_KEY, displayName);
            PlayerPrefs.Save();
            Debug.Log($"[playerprefs] DisplayName 저장 완료: {displayName}");
        }, error =>
        {
            Debug.LogError("DisplayName 불러오기 실패: " + error.GenerateErrorReport());
        });

        // 2. ImageIndex 불러오기 (User Data 방식)
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), userDataResult =>
        {
            if (userDataResult.Data.ContainsKey(PROFILE_IMAGE_INDEX_KEY))
            {
                // 'ImageIndex' 키가 있으면 값을 파싱
                int imageIndex = int.Parse(userDataResult.Data[PROFILE_IMAGE_INDEX_KEY].Value);
                PlayerPrefs.SetInt(IMAGEINDEX_KEY, imageIndex);
                PlayerPrefs.Save();
                Debug.Log($"[playerprefs] ImageIndex 저장 완료: {imageIndex}");
            }
            else
            {
                Debug.LogError("ImageIndex 값이 존재하지 않습니다");
            }
        }, error =>
        {
            Debug.LogError("ImageIndex 불러오기 실패: " + error.GenerateErrorReport());
        });

        // 3. 로그인 상태를 false로 설정
        PlayerPrefs.SetInt(FIRST_LOGIN_KEY, 0);
        PlayerPrefs.Save();
        Debug.Log($"[playerprefs] 로그인 상태 저장 완료: {FIRST_LOGIN_KEY}");
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
        NextBtn.gameObject.SetActive(nextBtnActive);
        OkBtn.gameObject.SetActive(okBtnActive);
        ReBtn.gameObject.SetActive(retryBtnActive);
        emailPanel.gameObject.SetActive(emailPanelActive);
        registerPanel.gameObject.SetActive(registerPanelActive);
    }

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

    public void ResetPasswordToggle() // 비밀번호보기 토글의 초기화(비활성화 상태)
    {
        if (PasswordToggle != null)
        {
            PasswordToggle.isOn = false; // 기본 상태로 되돌리기
        }
    }
    
    public void ResetWarningTexts() // 이메일, 비밀번호 경고 메시지 초기화
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

    
    public void ValidateUserID(string input) //유저 ID 입력 조건 검사에 따른 경고메시지 표시
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
        if ((!Regex.IsMatch(inputID, @"^[a-zA-Z0-9]+$"))) //한글을 포함하고 있으면
        {
            IDErrorText.text = "알파벳과 숫자만 입력 가능합니다.";
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

    public void ValidateEmail(string email) // 이메일 형식 확인 함수
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

    public void ValidatePassword(string password) // 비밀번호 규칙 확인 함수
    {
        PasswordErrorText.gameObject.SetActive(true); // 규칙이 틀리면 오류 메시지 표시
        string inputPW = password.Replace(" ", ""); //공백을 허용하지 않는다
        // 입력 값이 비어있는 경우 기본 경고 메시지
        if (string.IsNullOrEmpty(inputPW))
        {
            PasswordErrorText.text = "비밀번호를 입력해주세요";
            return;
        }
        if ((!Regex.IsMatch(inputPW, @"^[a-zA-Z0-9]+$")))
        {
            PasswordErrorText.text = "알파벳과 숫자만 입력이 가능합니다.";
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

    private void TogglePasswordVisibility(bool isOn) // 비밀번호 보기/숨기기 전환
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
