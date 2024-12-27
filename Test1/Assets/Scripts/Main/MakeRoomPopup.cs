using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MakeRoomPopup : MonoBehaviour
{
    public GameObject makeRoomPopupPanel; // �游��� �˾� �г�
    public Button openMakeRoomPopupButton; // �游��� �˾� ���� ��ư
    public Button closeMakeRoomPopupButton; // �游��� �˾� �ݱ� ��ư
    public Button makeRoomButton; // �� ����� ��ư
    public Button joinRoomButton; // �� �����ϱ� ��ư

    void Start()
    {

    }

    public void MakeRoomPopupf()
    {
        openMakeRoomPopupButton.onClick.AddListener(OpenMakeRoomPopup);  // �游��� �˾� ���� ��ư�� �̺�Ʈ �߰�
        closeMakeRoomPopupButton.onClick.AddListener(CloseMakeRoomPopup);  // �游��� �˾� �ݱ� ��ư�� �̺�Ʈ �߰�

        makeRoomPopupPanel.SetActive(false); // �游��� �˾� ��Ȱ��ȭ
        openMakeRoomPopupButton.gameObject.SetActive(true); // �游��� �˾� ���� ��ư�� Ȱ��ȭ
        closeMakeRoomPopupButton.gameObject.SetActive(false); // �游��� �˾� �ݱ� ��ư�� ��Ȱ��ȭ

        makeRoomButton.onClick.AddListener(MoveToPlayRoom); // �� ����� ��ư Ŭ�� �̺�Ʈ�� �÷��̹� �̵� �޼��� �߰�
        joinRoomButton.onClick.AddListener(MoveToPlayRoom); // �� �����ϱ� ��ư Ŭ�� �̺�Ʈ�� �÷��̹� �̵� �޼��� �߰�
    }

    void OpenMakeRoomPopup() // �游��� �˾� ���� �޼���
    {
        makeRoomPopupPanel.SetActive(true); // �˾� Ȱ��ȭ
        closeMakeRoomPopupButton.gameObject.SetActive(true); // �游��� �˾� �ݱ� ��ư Ȱ��ȭ
        makeRoomButton.gameObject.SetActive(true); // �� ����� ��ư Ȱ��ȭ
        joinRoomButton.gameObject.SetActive(true); // �� �����ϱ� ��ư Ȱ��ȭ
    }

    void CloseMakeRoomPopup() // �游��� �˾� �ݱ� �޼���
    {
        makeRoomPopupPanel.SetActive(false); // �˾� ��Ȱ��ȭ
        closeMakeRoomPopupButton.gameObject.SetActive(false); // �游��� �˾� �ݱ� ��ư ��Ȱ��ȭ
        makeRoomButton.gameObject.SetActive(false); // �� ����� ��ư ��Ȱ��ȭ
        joinRoomButton.gameObject.SetActive(false); // �� �����ϱ� ��ư ��Ȱ��ȭ
    }

     void MoveToPlayRoom()
    {
        SceneManager.LoadScene("PlayRoom");
    }

}
