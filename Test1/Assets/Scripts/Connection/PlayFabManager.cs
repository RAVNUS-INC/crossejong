using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine;
using System.Text.RegularExpressions;

// �̸��Ϸ� �α���/ȸ�������� ������ ��

public class PlayFabManager : MonoBehaviour
{
    public LoginManager LoginManager; // �α��� �Ŵ��� ��ũ��Ʈ���� �Ϻ� ���� ����� ���� ����(����)

    public InputField EmailInput, PasswordInput; // �̸���, ��й�ȣ �Է� �ʵ�
    public Text popupText; // �˾�â�� ǥ���� �˸� ����

    // LoginManager���� ������ ������
    private GameObject EmailPanel; // �̸��� �α��� ȭ�� �г�
    private GameObject AlarmPanel; // �˶� �г�
    private GameObject PlayerSetPanel; // ���� ������ ���� �г�
    public Button nextBtn; // ���� Ȯ�� ��ư
    public Button retryBtn; // ��õ� Ȯ�� ��ư

    //��й�ȣ ���� ����
    public Text PasswordErrorText;   // ��й�ȣ ���� �޽��� �ؽ�Ʈ
    public Toggle PasswordToggle;  // ��й�ȣ ���� ��ư


    //�̸��� ���� ����
    private string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"; // �̸��� ���� Ȯ���� ���� ���Խ�
    public Text EmailErrorText;      // �̸��� ���� �޽��� �ؽ�Ʈ


    private void Start()
    {
        // LoginManager�������� �гε��� ������
        EmailPanel = LoginManager.GetEmailLoginPanel();
        AlarmPanel = LoginManager.GetAlarmPanel();
        PlayerSetPanel = LoginManager.GetPlayerSetPanel();

        // AlarmPanel �ȿ� �ִ� ��ư�� ã�Ƽ� ���
        retryBtn = AlarmPanel.transform.Find("Retrybtn").GetComponent<Button>();
        nextBtn = AlarmPanel.transform.Find("Nextbtn").GetComponent<Button>();

        // �̸��� �Է� �� ���� Ȯ��
        EmailInput.onValueChanged.AddListener(ValidateEmail);
        // ��й�ȣ �Է� �� ��Ģ Ȯ��
        PasswordInput.onValueChanged.AddListener(ValidatePassword);
        // Toggle�� onValueChanged �̺�Ʈ�� �Լ� ���
        PasswordToggle.onValueChanged.AddListener(TogglePasswordVisibility);

    }


    // �̸��� ���� Ȯ�� �Լ�
    private void ValidateEmail(string email)
    {
        if (Regex.IsMatch(email, emailPattern))
        {
            EmailErrorText.gameObject.SetActive(false); // �̸��� ������ �ùٸ��� ���� �޽��� ����
        }
        else
        {
            EmailErrorText.gameObject.SetActive(true);  // �̸��� ������ Ʋ���� ���� �޽��� ǥ��
            EmailErrorText.text = "�̸��� ������ �ùٸ��� �ʽ��ϴ�. (@�� .com�� ���Ե� �����̾�� �մϴ�.)";
        }
    }

    // ��й�ȣ ��Ģ Ȯ�� �Լ�
    private void ValidatePassword(string password)
    {
        if (password.Length <= 20 && Regex.IsMatch(password, @"^[a-zA-Z]+$"))
        {
            PasswordErrorText.gameObject.SetActive(false); // ��Ģ�� ������ ���� �޽��� ����
        }
        else
        {
            PasswordErrorText.gameObject.SetActive(true); // ��Ģ�� Ʋ���� ���� �޽��� ǥ��
            if (password.Length > 20)
            {
                PasswordErrorText.text = "��й�ȣ�� �ִ� 20�ڱ��� �Է��� �� �ֽ��ϴ�.";
            }
            else
            {
                PasswordErrorText.text = "��й�ȣ�� ���ĺ��� �����ؾ� �մϴ�.";
            }
        }
    }


    // ��й�ȣ ����/����� ��ȯ
    private void TogglePasswordVisibility(bool isOn)
    {
        if (isOn) // Toggle�� Ȱ��ȭ�� ��� (��й�ȣ ����)
        {
            PasswordInput.contentType = InputField.ContentType.Standard; // �Ϲ� �ؽ�Ʈ�� ǥ��
        }
        else // Toggle�� ��Ȱ��ȭ�� ��� (��й�ȣ �����)
        {
            PasswordInput.contentType = InputField.ContentType.Password; // *�� ǥ��
        }

        // ContentType ���� �� InputField�� ������Ʈ
        PasswordInput.ForceLabelUpdate();
    }

    // �α��� ��ư Ŭ�� ��
    public void LoginBtn()
    {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    // ȸ������ ��ư Ŭ�� ��
    public void RegisterBtn()
    {
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    // �α��� ���� ��
    public void OnLoginFailure(PlayFabError error)
    {
        print("�α��� ����");
        UpdateText("�α��ο� �����Ͽ����ϴ�.");
        AlarmPanel.SetActive(true);
        nextBtn.gameObject.SetActive(false);
        retryBtn.gameObject.SetActive(true);
        EmailPanel.SetActive(false);
    }

    // �α��� ���� ��
    public void OnLoginSuccess(LoginResult result)
    {
        print("�α��� ����");
        UpdateText("�α��ο� �����Ͽ����ϴ�.");
        AlarmPanel.SetActive(true);
        retryBtn.gameObject.SetActive(false);
        nextBtn.gameObject.SetActive(true);
        EmailPanel.SetActive(false);
    }

    // ȸ������ ���� ��
    public void OnRegisterFailure(PlayFabError error)
    {
        print("ȸ������ ����");
        UpdateText("ȸ�����Կ� �����Ͽ����ϴ�.");
        AlarmPanel.SetActive(true);
        nextBtn.gameObject.SetActive(false);
        retryBtn.gameObject.SetActive(true);
        EmailPanel.SetActive(false);
    }

    // ȸ������ ���� ��
    public void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        print("ȸ������ ����");
        UpdateText("ȸ�����Կ� �����Ͽ����ϴ�.");
        AlarmPanel.SetActive(true);
        retryBtn.gameObject.SetActive(false);
        nextBtn.gameObject.SetActive(true);
        EmailPanel.SetActive(false);
    }

    // �˾� �ؽ�Ʈ ���� ������Ʈ
    public void UpdateText(string message)
    {
        popupText.text = message;
    }

    public void RetryBtn() //����� ���߰� Ȯ�� ��ư
    {
        AlarmPanel.SetActive(false);
        EmailPanel.SetActive(true); //�̸��� �α��� �ٽ� �õ��ϱ�
    }

    public void NextBtn() //����� �߰� Ȯ�� ��ư
    {
        PlayerSetPanel.SetActive(true); //���� �������� �Ѿ��
        AlarmPanel.SetActive(false);
    }


}

