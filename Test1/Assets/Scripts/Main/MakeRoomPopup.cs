using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MakeRoomPopup : MonoBehaviour
{
    public Button makeRoomButton; // �� ����� 
    public Button joinRoomButton; // �� �����ϱ� 
    public GameObject makeRoomPanel; // �� ���� �˾� �ǳ�

    public GameObject roomSetPanel; // �� ���� ���� 
    public Button goToRoomButton; // ���� �Ϸ�� �� ����� 

    public GameObject joinRoomPanel; // �ٸ� �濡 ����
    public Button joinButton; // ������ �濡 ���� 

    public Button ExitButton; // �ݱ� ��ư

    public void MakeRoomPopupf()
    {
        makeRoomButton.onClick.AddListener(MakeRoomSetting); // �� ����� ��ư Ŭ�� -> �� ���� �˾�
        joinRoomButton.onClick.AddListener(JoinRoomList); // �� �����ϱ� ��ư Ŭ�� -> ���� �ǳ� �̵�
        ExitButton.onClick.AddListener(ClosePopup); // �ݱ� ��ư ������ �ݱ�
    }

    void MakeRoomSetting() // �� ���� �˾� ���� �޼���
    {
        makeRoomPanel.gameObject.SetActive(true); // �� ���� �˾� �ǳ� Ȱ��ȭ
        roomSetPanel.gameObject.SetActive(true); // �� ���� �г� Ȱ��ȭ
        joinRoomPanel.gameObject.SetActive(false); // �� ���� �г� ��Ȱ��ȭ
    }

    void JoinRoomList() //�� ��� ���� �޼���
    {
        makeRoomPanel.gameObject.SetActive(true); // �� ���� �˾� �ǳ� Ȱ��ȭ
        joinRoomPanel.gameObject.SetActive(true); // �� ���� �г� Ȱ��ȭ
        roomSetPanel.gameObject.SetActive(false); // �� ���� �г� Ȱ��ȭ
    }
    void ClosePopup()
    {
        makeRoomPanel.gameObject.SetActive(false);
    }


    void MoveToPlayRoom()
    {
        SceneManager.LoadScene("PlayRoom");
    }

   

}
