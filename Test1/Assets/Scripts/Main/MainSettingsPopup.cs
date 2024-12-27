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

    public void MainSettingsPopupf()
    {
        openSettingsPopupButton.onClick.AddListener(OpenSettingsPopup);  // ���� �˾� ���� ��ư�� �̺�Ʈ �߰�
        closeSettingsPopupButton.onClick.AddListener(CloseSettingsPopup);  // ���� �˾� �ݱ� ��ư�� �̺�Ʈ �߰�

        settingsPopupPanel.SetActive(false); // ���� �˾� ��Ȱ��ȭ
        openSettingsPopupButton.gameObject.SetActive(true); // ���� �˾� ���� ��ư�� Ȱ��ȭ
        closeSettingsPopupButton.gameObject.SetActive(false); // ���� �˾� �ݱ� ��ư�� ��Ȱ��ȭ
    }

    void OpenSettingsPopup() // ���� �˾� ���� �޼���
    {
        settingsPopupPanel.SetActive(true); // �˾� Ȱ��ȭ
        closeSettingsPopupButton.gameObject.SetActive(true); // ���� �˾� �ݱ� ��ư Ȱ��ȭ
    }

    void CloseSettingsPopup() // ���� �˾� �ݱ� �޼���
    {
        settingsPopupPanel.SetActive(false); // �˾� ��Ȱ��ȭ
        closeSettingsPopupButton.gameObject.SetActive(false); // ���� �˾� �ݱ� ��ư ��Ȱ��ȭ
    }
}
