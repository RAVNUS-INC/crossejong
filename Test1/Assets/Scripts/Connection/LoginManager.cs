using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


// �α��� ȭ�� ��ü�� �����ϴ� �ڵ�
// ������ �α��� ����� ���, ��ư Ŭ�� �� �ٷ� �α��� �õ�
public class LoginManager : MonoBehaviour
{
    public UserSetManager  UserSetManager;

    public CanvasGroup AnimElement; // ��Ÿ���� ������� �� ������Ʈ

    //�г� ���� ����
    public GameObject emailPanel, registerPanel, playersetPanel, AlarmPanel, TouchPanel, Contone; //�̸���, ȸ������, �����ʱ⼼��, �˶� �г�, �ʱ���ġ �г�

    //��ǲ ���� ����
    public TMP_InputField EmailInput, PasswordInput, UseridInput;  // �̸���, ��й�ȣ, ���̵� ��ǲ�ʵ�

    // ���� �޽��� �ؽ�Ʈ(��й�ȣ, �̸���, ID)
    public TMP_Text PasswordErrorText, EmailErrorText, IDErrorText;

    // �˸� â �� ��ư��
    public Button NextBtn, OkBtn, ReBtn;

    // �α��� ���� ��ư��
    public Button LoginBtn, RegisterBtn, IsNewUserBtn;

    //��� ��ư(��й�ȣ)
    public Toggle PasswordToggle;  // ��й�ȣ ���� ��ư

    //�̸��� ��Ģ ����
    private string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"; // �̸��� ���� Ȯ���� ���� ���Խ�

    // �α��� �� �˾�â�� ǥ���� �˸� ����
    public TMP_Text popupText;

    //�˸�â �׽�Ʈ �޽���, ó�� ���� �׽�Ʈ �޽��� ��ġ
    public Text TestText, InitialTestText;

    public bool isLoginMode = true;  // �α��� ��� (true: �α���, false: ȸ������)
    public bool isTouched = false; // ��ġ ���� üũ

    // IPointerDownHandler �������̽��� ���� Ŭ�� �Ǵ� ��ġ �̺�Ʈ ����
    public void ShowLoginPanel()
    {
        isTouched = true;
        TouchPanel.SetActive(false);
        emailPanel.SetActive(true);
    }

    void Awake()
    {
        TouchPanel.SetActive(false);

        // ���� �� ���� �� playerprefs���� �ʱ�ȭ ����!

        //PlayerPrefs.DeleteAll();
        //PlayFabClientAPI.ForgetAllCredentials(); // �ڵ� �α��� ���� (PlayFab ���� ���� �ʱ�ȭ)

        // �ڵ� �α��� ����
        AutoLoginWithDeviceID();

        //Debug.Log("PlayerPrefs ���� �� ����̽� ���� ����");
    }

    void Start() 
    {
        // �α��� ���� Ȯ�� �� ù �α��� üũ
        //AutoLoginWithDeviceID();

        ResetPasswordToggle(); //��� ��Ȱ��ȭ
        ResetWarningTexts(); //���޽��� ��Ȱ��ȭ

        //������ ����Ǹ� ��Ģ �޽��� ����
        EmailInput.onValueChanged.AddListener((text) => ValidateEmail(EmailInput.text));
        PasswordInput.onValueChanged.AddListener((text) => ValidatePassword(PasswordInput.text));
        UseridInput.onValueChanged.AddListener((text) => ValidateUserID(UseridInput.text));

        // ��й�ȣ ��� üũ�ϸ� �Է��� ���� ����(*�� ���ĺ� ���·�)
        PasswordToggle.isOn = true; // ó���� üũ�Ǿ� ���̵���
        PasswordToggle.onValueChanged.AddListener(TogglePasswordVisibility);

        // �α���, ȸ������ ��ư�� �� ä������ Ȱ��ȭ
        LoginBtn.interactable = false;
        RegisterBtn.interactable = false;

        // �� ��ǲ�ʵ��� ���� �̺�Ʈ ���
        EmailInput.onValueChanged.AddListener(delegate { CheckInputFields(); });
        PasswordInput.onValueChanged.AddListener(delegate { CheckInputFields(); });
        UseridInput.onValueChanged.AddListener(delegate { CheckInputFields(); });

        Contone.SetActive(true); //�÷��̾� ��������1�� Ȱ��ȭ

    }

    void StartTwinkle()
    {
        // �ʱ� ���İ� ���� (����)
        AnimElement.alpha = 0f;

        // 0.8�� �������� 0 �� 1 �� 0 �ݺ�
        AnimElement.DOFade(1f, 1f)
            .SetLoops(-1, LoopType.Yoyo) // ���� �ݺ�, Yoyo ��� (�Դ� ����)
            .SetEase(Ease.InOutSine);    // �ε巴�� ���̵� ��/�ƿ�
    }

    void CheckInputFields() // ��ǲ �ʵ尡 �ϳ��� ��������� �α���/ȸ������ ��ư�� ��Ȱ��ȭ ���� ����
    {
        // ��� ��ǲ�ʵ尡 ������� ������ Ȯ��
        bool allFilled = !string.IsNullOrWhiteSpace(EmailInput.text) &&
                         !string.IsNullOrWhiteSpace(PasswordInput.text) &&
                         (isLoginMode || !string.IsNullOrWhiteSpace(UseridInput.text)); // ȸ�������� ���� Userid �˻�

        // ��� ���� �޽����� ��Ȱ��ȭ �������� Ȯ��
        bool noErrors = !PasswordErrorText.gameObject.activeSelf &&
                        !EmailErrorText.gameObject.activeSelf &&
                        (isLoginMode || !IDErrorText.gameObject.activeSelf); // ȸ�������� ���� Userid ���� üũ

        // ��ư Ȱ��ȭ ���� ����
        LoginBtn.interactable = allFilled && noErrors; ;
        RegisterBtn.interactable = allFilled && noErrors; ;
    }

    //private void LoadUserInfoFromPrefs() // �÷��̾� ���� ���ÿ� ����� �� �ҷ�����
    //{
    //    // GetString�� �ι�° ���� �⺻���� ��Ÿ��
    //    UserInfoManager.instance.MyName = PlayerPrefs.GetString(UserInfoManager.DISPLAYNAME_KEY, "Guest");
    //    UserInfoManager.instance.MyImageIndex = PlayerPrefs.GetInt(UserInfoManager.IMAGEINDEX_KEY, 0);

    //    // �ʿ��� ���� ������ �����ϰų� UI�� �ݿ�
    //   Debug.Log($"���� - DisplayName: {UserInfoManager.instance.MyName}, ImageIndex: {UserInfoManager.instance.MyImageIndex}");
    //}

    // �α���/ȸ������ ��ȯ �Լ� - �ű�ȸ���̽Ű���? ��ư�� ���� ��
    public void ToggleLoginMode()
    {
        isLoginMode = false;
    }

    // �α��� ��ư Ŭ�� ��
    public void TryLogin() //�α��� ��ư�� ����
    {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    // ȸ������ ��ư Ŭ�� ��
    public void TryRegister() //ȸ������ ��ư�� ����
    {
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UseridInput.text};
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    // �ܾ�ϼ�Ƚ�� ���� ����(�ʱⰪ 0)
    public void SetStat() //���� �ʱ� ���� Ȯ�ι�ư(usersetmanager)�� ����
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "WordCompletionCount", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => print("�ܾ�ϼ�Ƚ�� �ʱ�ȭ �� ���� �Ϸ�"), (error) => print("���� ���� ����"));
    }

    // �α��� ���� ��
    public void OnLoginFailure(PlayFabError error)
    {
        print("�α��� ����");
        UpdateText("�α��ο� �����Ͽ����ϴ�.");
        SetUIState(true, false, false, true, false, false); // �α��� ���� �� UI ���� ����
    }

    // (�ٸ� ��⿡�� �̸��Ϸ�)�α��� ���� ��
    public void OnLoginSuccess(LoginResult result)
    {
        print("�α��� ����");
        UpdateText("�α��ο� �����Ͽ����ϴ�.");

        // �α��� ���� �� UI ���� ����
        SetUIState(true, false, true, false, false, false);

        // 1. DisplayName �ҷ����� (���� ���� ���)
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result =>
        {
            string displayName = result.AccountInfo.TitleInfo.DisplayName;
            PlayerPrefs.SetString(UserInfoManager.DISPLAYNAME_KEY, displayName);
            PlayerPrefs.Save();
            UserInfoManager.instance.MyName = displayName;
            Debug.Log($"[playerprefs] DisplayName ���� �Ϸ�: {UserInfoManager.instance.MyName}");
            TestText.text = "DisplayName ����";
        }, error =>
        {
            Debug.LogError("DisplayName �ҷ����� ����: " + error.GenerateErrorReport());
        });

        // 2. ImageIndex �ҷ����� (User Data ���)
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), userDataResult =>
        {
            if (userDataResult.Data.ContainsKey(UserInfoManager.PROFILE_IMAGE_INDEX_KEY))
            {
                // 'ImageIndex' Ű�� ������ ���� �Ľ�
                int imageIndex = int.Parse(userDataResult.Data[UserInfoManager.PROFILE_IMAGE_INDEX_KEY].Value);
                PlayerPrefs.SetInt(UserInfoManager.IMAGEINDEX_KEY, imageIndex);
                PlayerPrefs.Save();
                UserInfoManager.instance.MyImageIndex = imageIndex;
                Debug.Log($"[playerprefs] ImageIndex ���� �Ϸ�: {UserInfoManager.instance.MyImageIndex}");
                TestText.text = "ImageIndex ����";
            }
            else
            {
                Debug.LogError("ImageIndex ���� �������� �ʽ��ϴ�");
            }
        }, error =>
        {
            Debug.LogError("ImageIndex �ҷ����� ����: " + error.GenerateErrorReport());
        });
        // 3. �α��� ���� ��, ��� ID�� ���� ����
        LinkDeviceID();
    }

    void LinkDeviceID() // ���� �α����� ��⸦ playfab�� ���� ����
    {
        var request = new LinkAndroidDeviceIDRequest
        {
            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier
        };

        PlayFabClientAPI.LinkAndroidDeviceID(request, result =>
        {
            Debug.Log("��� ID�� PlayFab ���� ���� �Ϸ�");
            TestText.text = "��� ID ���� �Ϸ�";
        },
        error =>
        {
            Debug.LogError("��� ID ���� ����: " + error.GenerateErrorReport());
            TestText.text = "��� ID ���� ����";
        });
    }

    void UnlinkDeviceID() // ���� �α����� ��⸦ ����
    {
        var request = new UnlinkAndroidDeviceIDRequest
        {
            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier
        };

        PlayFabClientAPI.UnlinkAndroidDeviceID(request, result =>
        {
            // PlayFab ���� ���� �ʱ�ȭ
            Debug.Log("��� ID ���� ���� �Ϸ�");
            InitialTestText.text = "��� ���� ������. ù �α��� ������";

            // ��ġ�г� Ȱ��ȭ
            TouchPanel.SetActive(true);
            StartTwinkle();
        },
        error =>
        {
            Debug.LogError("��� ID ���� ���� ����: " + error.GenerateErrorReport());
            InitialTestText.text = "��� ���� ���� ����";
        });

    }

    public void AutoLoginWithDeviceID() // ������ ��⸦ ���� �ڵ��α��� ����
    {
        var request = new LoginWithAndroidDeviceIDRequest
        {
            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = false // �̹� �����ϴ� ������ �α��� (�� ���� ���� X)
        };

        PlayFabClientAPI.LoginWithAndroidDeviceID(request, result =>
        {
            Debug.Log("��� ID�� �ڵ� �α��� ����: " + result.PlayFabId);
            InitialTestText.text = "�α��� ���� ����";

            // ������ �������� ��û �� �κ�� �̵�
            UserSetManager.OnClickConnect();
        },
        error =>
        {
            Debug.Log("ù �α��� ����. �ڵ� �α��� ����: " + error.GenerateErrorReport());
            InitialTestText.text = "�α׾ƿ� ����";
            // ��ġ�г� Ȱ��ȭ
            TouchPanel.SetActive(true);
            StartTwinkle();
        });
    }

    // ȸ������ ���� ��
    public void OnRegisterFailure(PlayFabError error)
    {
        print("ȸ������ ����");
        UpdateText("ȸ�����Կ� �����Ͽ����ϴ�.");
        SetUIState(true, false, false, true, false, false); // ȸ������ ���� �� UI ���� ����
    }

    // ȸ������ ���� ��
    public void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        print("ȸ������ ����");
        UpdateText("ȸ�����Կ� �����Ͽ����ϴ�.");
        SetUIState(true, true, false, false, false, false); // ȸ������ ���� �� UI ���� ����

        // ȸ������ ���� ��, ��� ID�� ���� ����
        LinkDeviceID();
    }

    // �˾� �ؽ�Ʈ ���� ������Ʈ
    public void UpdateText(string message)
    {
        popupText.text = message;
    }

    //���� SetActive ȣ���� �� ���� ó��
    public void SetUIState(bool alarmPanelActive, bool nextBtnActive, bool okBtnActive, bool retryBtnActive, bool emailPanelActive, bool registerPanelActive)
    {
        AlarmPanel.gameObject.SetActive(alarmPanelActive);
        NextBtn.gameObject.SetActive(nextBtnActive);
        OkBtn.gameObject.SetActive(okBtnActive);
        ReBtn.gameObject.SetActive(retryBtnActive);
        emailPanel.gameObject.SetActive(emailPanelActive);
        registerPanel.gameObject.SetActive(registerPanelActive);
    }

    public void BackBtn2() //�ڷΰ��� ��ư�� ������ �� ->�α��� ȭ������ �̵�
    {

        var inputFields = new[] { EmailInput, PasswordInput, UseridInput };

        foreach (var inputField in inputFields)
        {
            if (inputField != null)
            {
                inputField.text = ""; // �� �ʱ�ȭ
            }
        }

        // PasswordToggle�� �ʱ� ���·� ����
        ResetPasswordToggle();

        // ��� �ؽ�Ʈ �����
        ResetWarningTexts();

        isLoginMode = true;

    }

    public void ResetPasswordToggle() // ��й�ȣ���� ����� �ʱ�ȭ(��Ȱ��ȭ ����)
    {
        if (PasswordToggle != null)
        {
            PasswordToggle.isOn = false; // �⺻ ���·� �ǵ�����
        }
    }
    
    public void ResetWarningTexts() // �̸���, ��й�ȣ ��� �޽��� �ʱ�ȭ
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
 
    public void ValidateUserID(string input) //���� ID �Է� ���� �˻翡 ���� ���޽��� ǥ��
    {
        // ��Ģ�� Ʋ���� ���� �޽��� ǥ��
        IDErrorText.gameObject.SetActive(true);
        // ���� ����
        string inputID = input.Replace(" ", "");

        // �Է� ���� ����ִ� ��� �⺻ ��� �޽���
        if (string.IsNullOrEmpty(inputID))
        {
            IDErrorText.text = "ID�� �Է����ּ���";
            return;
        }

        // ���ĺ����θ� �̷�������� Ȯ�� (3�ڸ� �̻�)
        if ((!Regex.IsMatch(inputID, @"^[a-zA-Z0-9]+$"))) //�ѱ��� �����ϰ� ������
        {
            IDErrorText.text = "���ĺ��� ���ڸ� �Է� �����մϴ�.";
        }
        else if (inputID.Length < 3)
        {
            IDErrorText.text = "�ּ� 3�ڸ� �̻��̾�� �մϴ�.";
        }
        else
        {
            IDErrorText.gameObject.SetActive(false);
        }
        // �Է¶��� ������ ������ �� �ݿ�
        UseridInput.text = inputID;
    }

    public void ValidateEmail(string email) // �̸��� ���� Ȯ�� �Լ�
    {
        EmailErrorText.gameObject.SetActive(true); // ��Ģ�� Ʋ���� ���� �޽��� ǥ��
        // ���� ����
        string inputEmail = email.Replace(" ", ""); //������ ������� �ʴ´�

        if (string.IsNullOrEmpty(inputEmail))
        {
            EmailErrorText.text = "�̸����� �Է����ּ���";
            return;
        }

        if (!Regex.IsMatch(inputEmail, emailPattern))
        {
            EmailErrorText.text = "�̸��� ������ �ùٸ��� �ʽ��ϴ�.";
        }
        else
        {
            EmailErrorText.gameObject.SetActive(false); // �̸��� ������ �ùٸ��� ���� �޽��� ����
        }
        // �Է¶��� ������ ������ �� �ݿ�
        EmailInput.text = inputEmail;
    }

    public void ValidatePassword(string password) // ��й�ȣ ��Ģ Ȯ�� �Լ�
    {
        PasswordErrorText.gameObject.SetActive(true); // ��Ģ�� Ʋ���� ���� �޽��� ǥ��
        string inputPW = password.Replace(" ", ""); //������ ������� �ʴ´�
        // �Է� ���� ����ִ� ��� �⺻ ��� �޽���
        if (string.IsNullOrEmpty(inputPW))
        {
            PasswordErrorText.text = "��й�ȣ�� �Է����ּ���";
            return;
        }
        if ((!Regex.IsMatch(inputPW, @"^[a-zA-Z0-9]+$")))
        {
            PasswordErrorText.text = "���ĺ��� ���ڸ� �Է��� �����մϴ�.";
        }
        else if (inputPW.Length < 6) // 6�ڸ� �̸��̸�
        {
            PasswordErrorText.text = "�ּ� 6�ڸ� �̻��̾�� �մϴ�.";
        }
        else if (inputPW.Length > 20) //���̰� 20�� �̻��̸�
        {
            PasswordErrorText.text = "�ִ� 20�ڱ��� �Է��� �� �ֽ��ϴ�.";
        }
        else
        {
            PasswordErrorText.gameObject.SetActive(false);
        }
        // �Է¶��� ������ ������ �� �ݿ�
        PasswordInput.text = inputPW;
    }

    private void TogglePasswordVisibility(bool isOn) // ��й�ȣ ����/����� ��ȯ
    {
        if (isOn) // Toggle�� Ȱ��ȭ�� ��� (��й�ȣ ����)
        {
            PasswordInput.contentType = TMP_InputField.ContentType.Standard; // �Ϲ� �ؽ�Ʈ�� ǥ��
        }
        else // Toggle�� ��Ȱ��ȭ�� ��� (��й�ȣ �����)
        {
            PasswordInput.contentType = TMP_InputField.ContentType.Password; // *�� ǥ��
        }

        // ContentType ���� �� InputField�� ������Ʈ
        PasswordInput.ForceLabelUpdate();
    }
}
