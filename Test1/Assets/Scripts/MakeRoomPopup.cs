using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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

    public void OpenMakeRoomPopup() // �游��� �˾� ���� �޼���
    {
        makeRoomPopupPanel.SetActive(true); // �˾� Ȱ��ȭ
        closeMakeRoomPopupButton.gameObject.SetActive(true); // �游��� �˾� �ݱ� ��ư Ȱ��ȭ
        makeRoomButton.gameObject.SetActive(true); // �� ����� ��ư Ȱ��ȭ
        joinRoomButton.gameObject.SetActive(true); // �� �����ϱ� ��ư Ȱ��ȭ
    }

    public void CloseMakeRoomPopup() // �游��� �˾� �ݱ� �޼���
    {
        makeRoomPopupPanel.SetActive(false); // �˾� ��Ȱ��ȭ
        closeMakeRoomPopupButton.gameObject.SetActive(false); // �游��� �˾� �ݱ� ��ư ��Ȱ��ȭ
        makeRoomButton.gameObject.SetActive(false); // �� ����� ��ư ��Ȱ��ȭ
        joinRoomButton.gameObject.SetActive(false); // �� �����ϱ� ��ư ��Ȱ��ȭ
    }
}
