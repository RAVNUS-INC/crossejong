using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] public GameObject playersetPanel; // ���� ���� �г�
    [SerializeField] public GameObject AlarmPanel; // �˶� �г�

    public PlayFabManager playFabManager; // PlayFabManager���� �Ҵ�
    private InputField emailInput;
    private InputField passwordInput;
    private Toggle passwordToggle;
    private Text emailWarningText;
    private Text passwordWarningText;

    public void Start()
    {
        // PlayFabManager�� EmailInput�� PasswordInput�� ����
        emailInput = playFabManager.EmailInput;
        passwordInput = playFabManager.PasswordInput;
        passwordToggle = playFabManager.PasswordToggle;
        emailWarningText = playFabManager.EmailErrorText;
        passwordWarningText = playFabManager.PasswordErrorText;

        // �ʱ� ���� ����
        ResetPasswordToggle();
        ResetWarningTexts();
    }

    public GameObject GetEmailLoginPanel() => emailLoginPanel;
    public GameObject GetPlayerSetPanel() => playersetPanel;
    public GameObject GetAlarmPanel() => AlarmPanel;

    public void ShowLoginPanel(string method)
    {
        // ó�� �α��� ��� �����ִ� ȭ���� Ȱ��ȭ
        loginMethodsPanel.SetActive(true); 

        // ��� �α��� �� ��������, �˶� �г� ��Ȱ��ȭ
        googleLoginPanel.SetActive(false);
        naverLoginPanel.SetActive(false);
        appleLoginPanel.SetActive(false);
        emailLoginPanel.SetActive(false);
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

    public void EmailBtn() //�̸��� �α��� ��ư�� ������ ��
    {
        emailLoginPanel.SetActive(true);
        loginMethodsPanel.SetActive(false);
    }

    public void BackBtn() //�ڷΰ��� ��ư�� ������ ��
    {
        emailLoginPanel.SetActive(false);
        loginMethodsPanel.SetActive(true);

        if (emailInput != null) emailInput.text = "";
        if (passwordInput != null) passwordInput.text = "";

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
