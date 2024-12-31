using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MakeRoomPopup : MonoBehaviour
{
    public GameObject makeRoomPopupPanel; // �游��� �˾� �г�
    public Button openMakeRoomPopupButton; // �游��� �˾� ���� ��ư
    public Button closeMakeRoomPopupButton; // �游��� �˾� �ݱ� ��ư

    public Button makeRoomButton; // 1��-�� ����� ��ư
    public Button joinRoomButton; // 1��-�� �����ϱ� ��ư

    public Button roomSetButton; // 2��-�� ���� ��ư
    public Button goToRoomButton; // ���� �Ϸ�� �� ����� ��ư
    public GameObject joinRoomPanel; // 2��-�濡 ���� �г�
    public Button joinButton; // ������ �濡 ���� ��ư

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

        makeRoomButton.onClick.AddListener(MakeRoomSetting); // �� ����� ��ư Ŭ�� �̺�Ʈ�� �� ���� ��ư �˾� �߰�
        joinRoomButton.onClick.AddListener(JoinRoomList); // �� �����ϱ� ��ư Ŭ�� �̺�Ʈ�� ���� �ǳ� �̵� �޼��� �߰�
    }

    void OpenMakeRoomPopup() // �� �����(�ѱ۳��� �����ϱ�) �˾� ���� �޼���
    {
        makeRoomPopupPanel.SetActive(true); // �˾� Ȱ��ȭ
        closeMakeRoomPopupButton.gameObject.SetActive(true); // �游��� �˾� �ݱ� ��ư Ȱ��ȭ
        makeRoomButton.gameObject.SetActive(true); // �� ����� ��ư Ȱ��ȭ
        joinRoomButton.gameObject.SetActive(true); // �� �����ϱ� ��ư Ȱ��ȭ

        roomSetButton.gameObject.SetActive(false); // �� ���� �˾� ��ư�� ��Ȱ��ȭ
        joinRoomPanel.gameObject.SetActive(false); // �� ���� �г��� ��Ȱ��ȭ
    }

    void CloseMakeRoomPopup() // �游��� �˾� �ݱ� �޼���
    {
        makeRoomPopupPanel.SetActive(false); // �˾� ��Ȱ��ȭ
        closeMakeRoomPopupButton.gameObject.SetActive(false); // �游��� �˾� �ݱ� ��ư ��Ȱ��ȭ
        makeRoomButton.gameObject.SetActive(false); // �� ����� ��ư ��Ȱ��ȭ
        joinRoomButton.gameObject.SetActive(false); // �� �����ϱ� ��ư ��Ȱ��ȭ
        roomSetButton.gameObject.SetActive(false); // �� ���� �˾� ��ư�� ��Ȱ��ȭ
    }

    void MakeRoomSetting() // �� ���� �˾� ���� �޼���
    {
        closeMakeRoomPopupButton.gameObject.SetActive(true); // �游��� �˾� �ݱ� ��ư Ȱ��ȭ
        roomSetButton.gameObject.SetActive(true); // �� ���� ��ư Ȱ��ȭ

        makeRoomButton.gameObject.SetActive(false); // �� ����� ��ư ��Ȱ��ȭ
        joinRoomButton.gameObject.SetActive(false); // �� �����ϱ� ��ư ��Ȱ��ȭ
    }

    void JoinRoomList() //�� ��� ���� �޼���
    {
        closeMakeRoomPopupButton.gameObject.SetActive(true); // �游��� �˾� �ݱ� ��ư Ȱ��ȭ
        joinRoomPanel.gameObject.SetActive(true); // �� ���� �г��� Ȱ��ȭ

        makeRoomButton.gameObject.SetActive(false); // �� ����� ��ư ��Ȱ��ȭ
        joinRoomButton.gameObject.SetActive(false); // �� �����ϱ� ��ư ��Ȱ��ȭ
        
    }

     void MoveToPlayRoom()
    {
        SceneManager.LoadScene("PlayRoom");
    }

   

}
