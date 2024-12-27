using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainSettingsPopup : MonoBehaviour
{
    public GameObject settingsPopupPanel; // ���� �˾� �г�
    public Button openSettingsPopupButton; // ���� �˾� ���� ��ư
    public Button closeSettingsPopupButton; // ���� �˾� �ݱ� ��ư

    void Start()
    {

    }

    public void OpenSettingsPopup() // ���� �˾� ���� �޼���
    {
        settingsPopupPanel.SetActive(true); // �˾� Ȱ��ȭ
        closeSettingsPopupButton.gameObject.SetActive(true); // ���� �˾� �ݱ� ��ư Ȱ��ȭ
    }

    public void CloseSettingsPopup() // ���� �˾� �ݱ� �޼���
    {
        settingsPopupPanel.SetActive(false); // �˾� ��Ȱ��ȭ
        closeSettingsPopupButton.gameObject.SetActive(false); // ���� �˾� �ݱ� ��ư ��Ȱ��ȭ
    }
}
