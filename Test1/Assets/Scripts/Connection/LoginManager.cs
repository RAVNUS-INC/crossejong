using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;


// 로그인 화면 전체를 구성하는 코드
// 나머지 로그인 방법의 경우, 버튼 클릭 시 바로 로그인 시도
public class LoginManager : MonoBehaviour
{
    public UserSetManager  UserSetManager;

    //패널 관련 선언
    public GameObject emailPanel, registerPanel, playersetPanel, AlarmPanel; //이메일, 회원가입, 유저초기세팅, 알람 패널 4종류

    //인풋 관련 선언
    public TMP_InputField EmailInput, PasswordInput, UseridInput;  // 이메일, 비밀번호, 아이디 인풋필드

    // 오류 메시지 텍스트(비밀번호, 이메일, ID)
    public TMP_Text PasswordErrorText, EmailErrorText, IDErrorText;

    // 알림 창 내 버튼들
    public Button NextBtn, OkBtn, ReBtn;

    //토글 버튼(비밀번호)
    public Toggle PasswordToggle;  // 비밀번호 보기 버튼

    //이메일 규칙 선언
    private string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"; // 이메일 형식 확인을 위한 정규식

    // 로그인 시 팝업창에 표시할 알림 내용
    public TMP_Text popupText;

    //알림창 테스트 메시지, 처음 접속 테스트 메시지 배치
    public Text TestText, InitialTestText; 

    void Awake()
    {
       // 실제 앱 빌드 시 playerprefs정보 초기화 수행!

       //PlayerPrefs.DeleteAll();
       //Debug.Log("PlayerPrefs 모두 삭제함");
    }

    void Start() 
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
        // 기기 ID로 자동 로그인
        AutoLoginWithDeviceID();

        // playerprefs에서 정보 불러오기(디버그를 위한)
        // LoadUserInfoFromPrefs();

        // 첫 로그인인 경우, 로그인으로 이동
        //SceneManager.LoadScene("Login");
        

    }
    private void LoadUserInfoFromPrefs() // 플레이어 정보 로컬에 저장된 값 불러오기
    {
        // GetString의 두번째 값은 기본값을 나타냄
        UserInfoManager.instance.MyName = PlayerPrefs.GetString(UserInfoManager.DISPLAYNAME_KEY, "Guest");
        UserInfoManager.instance.MyImageIndex = PlayerPrefs.GetInt(UserInfoManager.IMAGEINDEX_KEY, 0);

        // 필요한 곳에 정보를 설정하거나 UI에 반영
       Debug.Log($"로컬 - DisplayName: {UserInfoManager.instance.MyName}, ImageIndex: {UserInfoManager.instance.MyImageIndex}");
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
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UseridInput.text};
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

    // (다른 기기에서 이메일로)로그인 성공 시
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
            PlayerPrefs.SetString(UserInfoManager.DISPLAYNAME_KEY, displayName);
            PlayerPrefs.Save();
            UserInfoManager.instance.MyName = displayName;
            Debug.Log($"[playerprefs] DisplayName 저장 완료: {UserInfoManager.instance.MyName}");
            TestText.text = "DisplayName 저장";
        }, error =>
        {
            Debug.LogError("DisplayName 불러오기 실패: " + error.GenerateErrorReport());
        });

        // 2. ImageIndex 불러오기 (User Data 방식)
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), userDataResult =>
        {
            if (userDataResult.Data.ContainsKey(UserInfoManager.PROFILE_IMAGE_INDEX_KEY))
            {
                // 'ImageIndex' 키가 있으면 값을 파싱
                int imageIndex = int.Parse(userDataResult.Data[UserInfoManager.PROFILE_IMAGE_INDEX_KEY].Value);
                PlayerPrefs.SetInt(UserInfoManager.IMAGEINDEX_KEY, imageIndex);
                PlayerPrefs.Save();
                UserInfoManager.instance.MyImageIndex = imageIndex;
                Debug.Log($"[playerprefs] ImageIndex 저장 완료: {UserInfoManager.instance.MyImageIndex}");
                TestText.text = "ImageIndex 저장";
            }
            else
            {
                Debug.LogError("ImageIndex 값이 존재하지 않습니다");
            }
        }, error =>
        {
            Debug.LogError("ImageIndex 불러오기 실패: " + error.GenerateErrorReport());
        });
        // 3. 로그인 성공 후, 기기 ID와 연동 실행
        LinkDeviceID();
    }

    void LinkDeviceID() // 계정 로그인한 기기를 playfab과 직접 연동
    {
        var request = new LinkAndroidDeviceIDRequest
        {
            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier
        };

        PlayFabClientAPI.LinkAndroidDeviceID(request, result =>
        {
            Debug.Log("기기 ID와 PlayFab 계정 연동 완료");
            TestText.text = "기기 ID 연동 완료";
        },
        error =>
        {
            Debug.LogError("기기 ID 연동 실패: " + error.GenerateErrorReport());
            TestText.text = "기기 ID 연동 실패";
        });
    }

    public void AutoLoginWithDeviceID() // 연동된 기기를 통해 자동로그인 수행
    {
        var request = new LoginWithAndroidDeviceIDRequest
        {
            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = false // 이미 존재하는 계정만 로그인 (새 계정 생성 X)
        };

        PlayFabClientAPI.LoginWithAndroidDeviceID(request, result =>
        {
            Debug.Log("기기 ID로 자동 로그인 성공: " + result.PlayFabId);
            InitialTestText.text = "로그인 연결 상태";

            // 마스터 서버접속 요청 및 로비로 이동
            UserSetManager.OnClickConnect();
        },
        error =>
        {
            Debug.Log("첫 로그인 감지. 자동 로그인 실패: " + error.GenerateErrorReport());
            InitialTestText.text = "로그아웃 상태";
        });
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

        // 회원가입 성공 후, 기기 ID와 연동 실행
        LinkDeviceID();
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
            PasswordInput.contentType = TMP_InputField.ContentType.Standard; // 일반 텍스트로 표시
        }
        else // Toggle이 비활성화된 경우 (비밀번호 숨기기)
        {
            PasswordInput.contentType = TMP_InputField.ContentType.Password; // *로 표시
        }

        // ContentType 변경 후 InputField를 업데이트
        PasswordInput.ForceLabelUpdate();
    }
}
