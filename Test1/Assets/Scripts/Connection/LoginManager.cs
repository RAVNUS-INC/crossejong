using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
//using UnityEngine.UIElements;

// �α��� ȭ�� ��ü�� �����ϴ� �ڵ�
// �Ϲ� �̸��� �α��� ����� ��쿡�� �ڷ� ���ư� �� �ֵ��� �ϴ� �ڵ�
// ������ �α��� ����� ���, ��ư Ŭ�� �� �ٷ� �α��� �õ�
public class LoginManager : MonoBehaviour
{

    //playfabmanager��ũ��Ʈ ����
    public PlayFabManager playFabManager;

    //�г� ���� ����
    public GameObject emailPanel, registerPanel, playersetPanel, AlarmPanel; //�̸���, ȸ������, �����ʱ⼼��, �˶� �г� 4����

    //��ǲ ���� ����
    public InputField EmailInput, PasswordInput, UseridInput;  // �̸���, ��й�ȣ, ���̵� ��ǲ�ʵ�

    // ���� �޽��� �ؽ�Ʈ(��й�ȣ, �̸���, ID)
    public Text PasswordErrorText, EmailErrorText, IDErrorText;   

    //��� ��ư(��й�ȣ)
    public Toggle PasswordToggle;  // ��й�ȣ ���� ��ư

    //�̸��� ��Ģ ����
    private string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"; // �̸��� ���� Ȯ���� ���� ���Խ�


    public void Start()
    {
        // �ʱ� ���� ����
        ResetPasswordToggle(); //��� ��Ȱ��ȭ
        ResetWarningTexts(); //���޽��� ��Ȱ��ȭ

        //������ ����Ǹ� ��Ģ �޽��� ����
        EmailInput.onValueChanged.AddListener((text) => ValidateEmail(EmailInput.text));
        PasswordInput.onValueChanged.AddListener((text) => ValidatePassword(PasswordInput.text));
        UseridInput.onValueChanged.AddListener((text) => ValidateUserID(UseridInput.text));

        // ��й�ȣ ��� üũ�ϸ� �Է��� ���� ����(*�� ���ĺ� ���·�)
        PasswordToggle.onValueChanged.AddListener(TogglePasswordVisibility);
    }

    //playfabmanager��ũ��Ʈ ���� ��� ����
    public GameObject GetEmailLoginPanel() => emailPanel;
    public GameObject GetPlayerSetPanel() => playersetPanel;
    public GameObject GetAlarmPanel() => AlarmPanel;
    public GameObject GetRegisterPanel() => registerPanel;
    public InputField GetEmailInput() => EmailInput;
    public InputField GetPWInput() => PasswordInput;
    public InputField GetIDInput() => UseridInput;


    public void BackBtn1() //�ڷΰ��� ��ư�� ������ �� ->�ʱ� Ȩ ȭ������ �̵�
    {
        if (EmailInput != null) EmailInput.text = ""; //�� �ʱ�ȭ
        if (PasswordInput != null) PasswordInput.text = ""; //�� �ʱ�ȭ

        // PasswordToggle�� �ʱ� ���·� ����
        ResetPasswordToggle();

        // ��� �ؽ�Ʈ �����
        ResetWarningTexts();

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

    }


    // ��й�ȣ���� ����� �ʱ�ȭ(��Ȱ��ȭ ����)
    public void ResetPasswordToggle()
    {
        if (PasswordToggle != null)
        {
            PasswordToggle.isOn = false; // �⺻ ���·� �ǵ�����
        }
    }

    // �̸���, ��й�ȣ ��� �޽��� �ʱ�ȭ
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

    //���� ID �Է� ���� �˻翡 ���� ���޽��� ǥ��
    public void ValidateUserID(string input)
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
}
