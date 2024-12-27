using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public MainSettingsPopup mainSettingsPopup; //MainSettingsPopup ��ũ��Ʈ ����
    public MakeRoomPopup makeRoomPopup; //MakeRoomPopup ��ũ��Ʈ ����

    void SetSettingsPopup()
    {
        mainSettingsPopup.openSettingsPopupButton.onClick.AddListener(mainSettingsPopup.OpenSettingsPopup);  // ���� �˾� ���� ��ư�� �̺�Ʈ �߰�
        mainSettingsPopup.closeSettingsPopupButton.onClick.AddListener(mainSettingsPopup.CloseSettingsPopup);  // ���� �˾� �ݱ� ��ư�� �̺�Ʈ �߰�

        mainSettingsPopup.settingsPopupPanel.SetActive(false); // ���� �˾� ��Ȱ��ȭ
        mainSettingsPopup.openSettingsPopupButton.gameObject.SetActive(true); // ���� �˾� ���� ��ư�� Ȱ��ȭ
        mainSettingsPopup.closeSettingsPopupButton.gameObject.SetActive(false); // ���� �˾� �ݱ� ��ư�� ��Ȱ��ȭ
    }

    void MakeRoomPopup()
    {
        makeRoomPopup.openMakeRoomPopupButton.onClick.AddListener(makeRoomPopup.OpenMakeRoomPopup);  // �游��� �˾� ���� ��ư�� �̺�Ʈ �߰�
        makeRoomPopup.closeMakeRoomPopupButton.onClick.AddListener(makeRoomPopup.CloseMakeRoomPopup);  // �游��� �˾� �ݱ� ��ư�� �̺�Ʈ �߰�

        makeRoomPopup.makeRoomPopupPanel.SetActive(false); // �游��� �˾� ��Ȱ��ȭ
        makeRoomPopup.openMakeRoomPopupButton.gameObject.SetActive(true); // �游��� �˾� ���� ��ư�� Ȱ��ȭ
        makeRoomPopup.closeMakeRoomPopupButton.gameObject.SetActive(false); // �游��� �˾� �ݱ� ��ư�� ��Ȱ��ȭ

        makeRoomPopup.makeRoomButton.onClick.AddListener(MoveToPlayRoom); // �� ����� ��ư Ŭ�� �̺�Ʈ�� �÷��̹� �̵� �޼��� �߰�
        makeRoomPopup.joinRoomButton.onClick.AddListener(MoveToPlayRoom); // �� �����ϱ� ��ư Ŭ�� �̺�Ʈ�� �÷��̹� �̵� �޼��� �߰�
    }


    void MoveToPlayRoom()
    {
        SceneManager.LoadScene("PlayRoom");
    }


    void Start()
    {
        SetSettingsPopup();
        MakeRoomPopup();
    }


    void Update()
    {
        
    }
}
