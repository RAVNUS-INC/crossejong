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
    //[SerializeField] private GameObject loginMethodsPanel; //�α��� ������� �����ִ� ù ȭ��

    //[SerializeField] private GameObject googleLoginPanel; //���� �α��� ���� �� ȭ��
    //[SerializeField] private GameObject naverLoginPanel; //���̹� �α��� ���� �� ȭ��
    //[SerializeField] private GameObject appleLoginPanel; //���� �α��� ���� �� ȭ��
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

        
    }

    public GameObject GetEmailLoginPanel() => emailLoginPanel;
    public GameObject GetPlayerSetPanel() => playersetPanel;
    public GameObject GetAlarmPanel() => AlarmPanel;
    public GameObject GetRegisterPanel() => registerPanel;

    public void BackBtn1() //�ڷΰ��� ��ư�� ������ �� ->�α��� Ȩȭ������ �̵�
    {
        if (emailInput != null) emailInput.text = ""; //�� �ʱ�ȭ
        if (passwordInput != null) passwordInput.text = ""; //�� �ʱ�ȭ

        // PasswordToggle�� �ʱ� ���·� ����
        ResetPasswordToggle();

        // ��� �ؽ�Ʈ �����
        ResetWarningTexts();

    }

    public void BackBtn2() //�ڷΰ��� ��ư�� ������ �� ->�α��� Ȩȭ������ �̵�
    {

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
