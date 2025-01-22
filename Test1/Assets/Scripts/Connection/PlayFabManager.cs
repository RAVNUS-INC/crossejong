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

    public Text popupText; // �α��� �� �˾�â�� ǥ���� �˸� ����

    // LoginManager���� ������ ������
    private GameObject EmailPanel, RegisterPanel, AlarmPanel, PlayerSetPanel; //�̸���, ȸ������, �����ʱ⼼��, �˶� �г� 4����
    private InputField EmailInput, PasswordInput, UserIDInput; // �̸���, ��й�ȣ, ���̵� ��ǲ�ʵ�

    //�α��� �Է¿� ���� �˶� �г� ������ ��ư 3����(�̸� �����ϸ� �ȵ�)
    private Button nextBtn; // ���� Ȯ�� ��ư
    private Button retryBtn; // ��õ� Ȯ�� ��ư
    private Button okBtn; // �α��� ���� Ȯ�� ��ư



    private void Start()
    {
        // LoginManager�������� �гε��� ������
        EmailPanel = LoginManager.GetEmailLoginPanel();
        RegisterPanel = LoginManager.GetRegisterPanel();
        AlarmPanel = LoginManager.GetAlarmPanel();
        PlayerSetPanel = LoginManager.GetPlayerSetPanel();
        EmailInput = LoginManager.GetEmailInput();
        PasswordInput = LoginManager.GetPWInput();
        UserIDInput = LoginManager.GetIDInput();

        // AlarmPanel �ȿ� �ִ� ��ư�� ã�Ƽ� ���
        retryBtn = AlarmPanel.transform.Find("Retrybtn").GetComponent<Button>();
        nextBtn = AlarmPanel.transform.Find("Nextbtn").GetComponent<Button>();
        okBtn = AlarmPanel.transform.Find("OKbtn").GetComponent<Button>();
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
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UserIDInput.text };
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
        SetUIState(true, false, false, true, false, false); // �α��� ���� �� UI ���� ����
    }

    // �α��� ���� ��->���� �ѱ�� �κ�� �Ѿ����
    public void OnLoginSuccess(LoginResult result)
    {
        print("�α��� ����");
        UpdateText("�α��ο� �����Ͽ����ϴ�.");
        SetUIState(true, false, true, false, false, false); // �α��� ���� �� UI ���� ����
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
        nextBtn.gameObject.SetActive(nextBtnActive);
        okBtn.gameObject.SetActive(okBtnActive);
        retryBtn.gameObject.SetActive(retryBtnActive);
        EmailPanel.gameObject.SetActive(emailPanelActive);
        RegisterPanel.gameObject.SetActive(registerPanelActive);
    }
}

