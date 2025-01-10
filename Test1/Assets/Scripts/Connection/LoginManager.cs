using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject loginMethodsPanel; //�α��� ������� �����ִ� ù ȭ��

    [SerializeField] private GameObject googleLoginPanel; //���� �α��� ���� �� ȭ��
    [SerializeField] private GameObject naverLoginPanel; //���̹� �α��� ���� �� ȭ��
    [SerializeField] private GameObject appleLoginPanel; //���� �α��� ���� �� ȭ��
    [SerializeField] public GameObject emailLoginPanel; //�Ϲ� �̸��� �α��� ���� �� ȭ��
    [SerializeField] public GameObject registerPanel; //ȸ������ ���� �� ȭ��
    [SerializeField] public GameObject playersetPanel; // ���� ���� �г�
    [SerializeField] public GameObject AlarmPanel; // �˶� �г�

    public PlayFabManager playFabManager; // PlayFabManager���� �Ҵ�
    private InputField emailInput;
    private InputField passwordInput;
    private InputField idInput;

    private Toggle passwordToggle;

    private Text emailWarningText;
    private Text passwordWarningText;

    private Button loginButton;
    private Button newuserButton;

    public void Start()
    {
        // PlayFabManager�� EmailInput�� PasswordInput�� ����
        emailInput = playFabManager.EmailInput;
        passwordInput = playFabManager.PasswordInput;
        idInput = playFabManager.playerIDInputField;
        passwordToggle = playFabManager.PasswordToggle;
        emailWarningText = playFabManager.EmailErrorText;
        passwordWarningText = playFabManager.PasswordErrorText;

        // �ʱ� ���� ����
        ResetPasswordToggle();
        ResetWarningTexts();

        // �̸��� �α��� �г��� ��ư���� ã�ƿ���(�̸��� ��Ȯ�ؾ� ��)
        loginButton = GameObject.Find("LoginBtn").GetComponent<Button>();
        newuserButton = GameObject.Find("NewUserBtn").GetComponent<Button>();
    }

    public GameObject GetEmailLoginPanel() => emailLoginPanel;
    public GameObject GetPlayerSetPanel() => playersetPanel;
    public GameObject GetAlarmPanel() => AlarmPanel;
    public GameObject GetRegisterPanel() => registerPanel;

    public void ShowLoginPanel(string method)
    {
        // ó�� �α��� ��� �����ִ� ȭ���� Ȱ��ȭ
        loginMethodsPanel.SetActive(true); 

        // ��� �α��� �� ��������, �˶� �г� ��Ȱ��ȭ
        googleLoginPanel.SetActive(false);
        naverLoginPanel.SetActive(false);
        appleLoginPanel.SetActive(false);
        emailLoginPanel.SetActive(false);
        registerPanel.SetActive(false);
        playersetPanel.SetActive(false);
        AlarmPanel.SetActive(false);

        // ���õ� �α��� �г� Ȱ��ȭ
        switch (method)
        {
            case "Google":
                googleLoginPanel.SetActive(true);
                break;
            case "Naver":
                naverLoginPanel.SetActive(true);
                break;
            case "Apple":
                appleLoginPanel.SetActive(true);
                break;
            case "Email":
                emailLoginPanel.SetActive(true);
                break;
        }

    }

    public void IfNewUserBtn() //�ű� ȸ���� ��� ��ư Ŭ���� �ϸ�
    {
        registerPanel.SetActive(true);
        emailLoginPanel.SetActive(true);
        loginButton.gameObject.SetActive(false);
        newuserButton.gameObject.SetActive(false);
        //�̸��� �г��� �α��� ��ư�� �ű�ȸ�� ��ư�� ��Ȱ��ȭ ��Ų��

        //emailInput.interactable = true; //��ǲ Ȱ��ȭ
        //passwordInput.interactable = true; //��ǲ Ȱ��ȭ

    }

    public void EmailBtn() //�̸��� �α��� ��ư�� ������ ��
    {
        emailLoginPanel.SetActive(true); //�̸��� �г� Ȱ��ȭ
        loginMethodsPanel.SetActive(false); //�α��� �г� ��Ȱ��ȭ
    }

    public void BackBtn1() //�ڷΰ��� ��ư�� ������ �� ->�α��� Ȩȭ������ �̵�
    {
        emailLoginPanel.SetActive(false); //�̸��� �г� ��Ȱ��ȭ
        loginMethodsPanel.SetActive(true); //�α��� �г� Ȱ��ȭ

        if (emailInput != null) emailInput.text = ""; //�� �ʱ�ȭ
        if (passwordInput != null) passwordInput.text = ""; //�� �ʱ�ȭ

        // PasswordToggle�� �ʱ� ���·� ����
        ResetPasswordToggle();

        // ��� �ؽ�Ʈ �����
        ResetWarningTexts();

    }

    public void BackBtn2() //�ڷΰ��� ��ư�� ������ �� ->�α��� Ȩȭ������ �̵�
    {
        registerPanel.SetActive(false); //ȸ������ ȭ�� ��Ȱ��ȭ

        loginButton.gameObject.SetActive(true); //ȸ������>�α��� ��ư Ȱ��ȭ
        newuserButton.gameObject.SetActive(true); //ȸ������>�űԹ�ư Ȱ��ȭ

        if (emailInput != null) emailInput.text = ""; //�� �ʱ�ȭ
        if (passwordInput != null) passwordInput.text = ""; //�� �ʱ�ȭ
        if (idInput != null) idInput.text = ""; //�� �ʱ�ȭ

        // PasswordToggle�� �ʱ� ���·� ����
        ResetPasswordToggle();

        // ��� �ؽ�Ʈ �����
        ResetWarningTexts();

    }

    // ��й�ȣ���� ����� �ʱ�ȭ
    private void ResetPasswordToggle()
    {
        if (passwordToggle != null)
        {
            passwordToggle.isOn = false; // �⺻ ���·� �ǵ�����
        }
    }

    // �̸���, ��й�ȣ�� �ʱ�ȭ
    private void ResetWarningTexts()
    {
        if (emailWarningText != null) emailWarningText.gameObject.SetActive(false);
        if (passwordWarningText != null) passwordWarningText.gameObject.SetActive(false);
    }


}
