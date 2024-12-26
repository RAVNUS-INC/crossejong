using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public MainSettingsPopup mainSettingsPopup; //MainSettingsPopup ��ũ��Ʈ ����
    public Button gameStartButton; // ���� ���� ��ư

    void SetSettingsPopup()
    {
        mainSettingsPopup.openSettingsPopupButton.onClick.AddListener(mainSettingsPopup.OpenSettingsPopup);  // ���� �˾� ���� ��ư�� �̺�Ʈ �߰�
        mainSettingsPopup.closeSettingsPopupButton.onClick.AddListener(mainSettingsPopup.CloseSettingsPopup);  // ���� �˾� �ݱ� ��ư�� �̺�Ʈ �߰�

        mainSettingsPopup.settingsPopupPanel.SetActive(false); // ���� �˾� ��Ȱ��ȭ
        mainSettingsPopup.openSettingsPopupButton.gameObject.SetActive(true); // ���� �˾� ���� ��ư�� Ȱ��ȭ
        mainSettingsPopup.closeSettingsPopupButton.gameObject.SetActive(false); // ���� �˾� �ݱ� ��ư�� ��Ȱ��ȭ
    }

    void Start()
    {
        SetSettingsPopup();
        gameStartButton.onClick.AddListener(MoveToPlayRoom); // ���� ���� ��ư Ŭ�� �̺�Ʈ�� �÷��̹� �̵� �޼��� �߰�
    }

    void MoveToPlayRoom()
    {
        SceneManager.LoadScene("PlayRoom");
    }

    void Update()
    {
        
    }
}
