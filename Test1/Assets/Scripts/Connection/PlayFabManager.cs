using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine.Windows;

// �̸��Ϸ� �α���/ȸ�������� ������ ��

public class PlayFabManager : MonoBehaviour
{
    public LoginManager LoginManager; // �α��� �Ŵ��� ��ũ��Ʈ���� �Ϻ� ���� ����� ���� ����(����)
    public UserSetManager UserSetManager;    //�������øŴ��� ��ũ��Ʈ���� �Ϻ� ���� ����� ���� ����(����)

    public InputField EmailInput, PasswordInput, UseridInput;  // �̸���, ��й�ȣ, ���� ���̵� (�α��� ��)

    public Text popupText; // �˾�â�� ǥ���� �˸� ����

    // LoginManager���� ������ ������
    private GameObject EmailPanel; // �̸��� �α��� ȭ�� �г�
    private GameObject RegisterPanel; //ȸ������ ���� �� ȭ��
    private GameObject AlarmPanel; // �˶� �г�
    private GameObject PlayerSetPanel; // ���� ������ ���� �г�

    //��ư��
    private Button nextBtn; // ���� Ȯ�� ��ư
    private Button retryBtn; // ��õ� Ȯ�� ��ư
    private Button okBtn; // �α��� ���� Ȯ�� ��ư

    //��й�ȣ ���� ����
    public Text PasswordErrorText;   // ��й�ȣ ���� �޽��� �ؽ�Ʈ
    public Toggle PasswordToggle;  // ��й�ȣ ���� ��ư

    //�̸��� ���� ����
    private string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"; // �̸��� ���� Ȯ���� ���� ���Խ�
    public Text EmailErrorText;      // �̸��� ���� �޽��� �ؽ�Ʈ

    //����ID ���� ����
    public InputField playerIDInputField;  // PlayerID �Է¿���
    public Text IDErrorText; //PlayerID ���� �޽��� �ؽ�Ʈ

    private void Start()
    {

        // LoginManager�������� �гε��� ������
        EmailPanel = LoginManager.GetEmailLoginPanel();
        RegisterPanel = LoginManager.GetRegisterPanel();
        AlarmPanel = LoginManager.GetAlarmPanel();
        PlayerSetPanel = LoginManager.GetPlayerSetPanel();

        // AlarmPanel �ȿ� �ִ� ��ư�� ã�Ƽ� ���
        retryBtn = AlarmPanel.transform.Find("Retrybtn").GetComponent<Button>();
        nextBtn = AlarmPanel.transform.Find("Nextbtn").GetComponent<Button>();
        okBtn = AlarmPanel.transform.Find("OKbtn").GetComponent<Button>();

        // ��й�ȣ ��� üũ�ϸ� ���� �� �� ����
        PasswordToggle.onValueChanged.AddListener(TogglePasswordVisibility);


    }





    //���� ID �Է� ���� �˻翡 ���� ���޽��� ǥ��
    public void ValidateUserID(string input)
    {
        IDErrorText.gameObject.SetActive(true); // ��Ģ�� Ʋ���� ���� �޽��� ǥ��
        // ���� ����
        string inputID = input.Replace(" ", ""); //������ ������� �ʴ´�

        // �Է� ���� ����ִ� ��� �⺻ ��� �޽���
        if (string.IsNullOrEmpty(inputID))
        {
            IDErrorText.text = "ID�� �Է����ּ���";
            return;
        }

        // ���ĺ����θ� �̷�������� Ȯ�� (3�ڸ� �̻�)
        if ((Regex.IsMatch(inputID, @"[\u3131-\uD79D]"))) //�ѱ��� �����ϰ� ������
        {
            IDErrorText.text = "ID�� ���ĺ��� �����ؾ� �մϴ�.";
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




    // �̸��� ���� Ȯ�� �Լ�
    public void ValidateEmail(string email)
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
            EmailErrorText.text = "�̸��� ������ �ùٸ��� �ʽ��ϴ�. (@�� .com�� ���Ե� �����̾�� �մϴ�.)";
        }
        else
        {
            EmailErrorText.gameObject.SetActive(false); // �̸��� ������ �ùٸ��� ���� �޽��� ����
        }
        // �Է¶��� ������ ������ �� �ݿ�
        EmailInput.text = inputEmail;
    }

    // ��й�ȣ ��Ģ Ȯ�� �Լ�
    public void ValidatePassword(string password)
    {
        PasswordErrorText.gameObject.SetActive(true); // ��Ģ�� Ʋ���� ���� �޽��� ǥ��
        string inputPW = password.Replace(" ", ""); //������ ������� �ʴ´�
        // �Է� ���� ����ִ� ��� �⺻ ��� �޽���
        if (string.IsNullOrEmpty(inputPW))
        {
            PasswordErrorText.text = "��й�ȣ�� �Է����ּ���";
            return;
        }

        if ((Regex.IsMatch(inputPW, @"[\u3131-\uD79D]")))
        {
            PasswordErrorText.text = "��й�ȣ�� ���ĺ��� �����ؾ� �մϴ�.";
        }
        else if (inputPW.Length < 6 ) // 6�ڸ� �̸��̸�
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
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UseridInput.text};
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    // �ܾ�ϼ�Ƚ�� ���� ����(�ʱⰪ 0)
    public void SetStat()
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "WordCompletionCount", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => print("�ܾ�ϼ�Ƚ�� �ʱ�ȭ �� ���� �Ϸ�"), (error) => print("���� ���� ����"));
    }

    // �α��� ���� ��
    public void OnLoginFailure(PlayFabError error)
    {
        print("�α��� ����");
        UpdateText("�α��ο� �����Ͽ����ϴ�.");
        AlarmPanel.SetActive(true);
        nextBtn.gameObject.SetActive(false);
        okBtn.gameObject.SetActive(false);
        retryBtn.gameObject.SetActive(true); //��õ� Ȱ��ȭ
        EmailPanel.SetActive(false);
        RegisterPanel.SetActive(false);
    }

    // �α��� ���� ��->���� �ѱ�� �κ�� �Ѿ����
    public void OnLoginSuccess(LoginResult result)
    {
        print("�α��� ����");
        UpdateText("�α��ο� �����Ͽ����ϴ�.");
        AlarmPanel.SetActive(true);
        retryBtn.gameObject.SetActive(false);
        okBtn.gameObject.SetActive(true); //����->���� Ȱ��ȭ
        nextBtn.gameObject.SetActive(false);
        EmailPanel.SetActive(false);
        RegisterPanel.SetActive(false);
    }

    // ȸ������ ���� ��
    public void OnRegisterFailure(PlayFabError error)
    {
        print("ȸ������ ����");
        UpdateText("ȸ�����Կ� �����Ͽ����ϴ�.");
        AlarmPanel.SetActive(true);
        nextBtn.gameObject.SetActive(false);
        okBtn.gameObject.SetActive(false);
        retryBtn.gameObject.SetActive(true); //��õ� Ȱ��ȭ
        EmailPanel.SetActive(false);
        RegisterPanel.SetActive(false);
    }

    // ȸ������ ���� ��
    public void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        print("ȸ������ ����");
        UpdateText("ȸ�����Կ� �����Ͽ����ϴ�.");
        AlarmPanel.SetActive(true);
        nextBtn.gameObject.SetActive(true); //���� Ȱ��ȭ
        retryBtn.gameObject.SetActive(false);
        okBtn.gameObject.SetActive(false);
        EmailPanel.SetActive(false);
        RegisterPanel.SetActive(false);
    }

    // �˾� �ؽ�Ʈ ���� ������Ʈ
    public void UpdateText(string message)
    {
        popupText.text = message;
    }

}

