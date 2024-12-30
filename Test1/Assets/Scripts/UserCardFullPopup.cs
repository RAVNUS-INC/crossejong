using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UserCardFullPopup : MonoBehaviour
{
    public GameObject userCardFullPanel; // ����ī�� ��ü���� �˾� �г�
    public Button openUserCardFullPopupButton; // ����ī�� ��ü���� �˾� ���� ��ư
    public Button closeUserCardFullPopupButton; // ����ī�� ��ü���� �˾� �ݱ� ��ư

    void Start()
    {

    }

    public void UserCardFullPopupf()
    {
        openUserCardFullPopupButton.onClick.AddListener(OpenUserCardFullPopup);  // ����ī�� ��ü���� �˾� ���� ��ư�� �̺�Ʈ �߰�
        closeUserCardFullPopupButton.onClick.AddListener(CloseUserCardFullPopup);  // ����ī�� ��ü���� �˾� �ݱ� ��ư�� �̺�Ʈ �߰�

        userCardFullPanel.SetActive(false); // ����ī�� ��ü���� �˾� ��Ȱ��ȭ
        openUserCardFullPopupButton.gameObject.SetActive(true); // ����ī�� ��ü���� �˾� ���� ��ư�� Ȱ��ȭ
        closeUserCardFullPopupButton.gameObject.SetActive(false); // ����ī�� ��ü���� �˾� �ݱ� ��ư�� ��Ȱ��ȭ
    }

    void OpenUserCardFullPopup() // ����ī�� ��ü���� �˾� ���� �޼���
    {
        userCardFullPanel.SetActive(true); // ����ī�� ��ü���� �˾� Ȱ��ȭ
        closeUserCardFullPopupButton.gameObject.SetActive(true); // ����ī�� ��ü���� �˾� �ݱ� ��ư Ȱ��ȭ
    }

    void CloseUserCardFullPopup() // ����ī�� ��ü���� �˾� �ݱ� �޼���
    {
        userCardFullPanel.SetActive(false); // ����ī�� ��ü���� �˾� ��Ȱ��ȭ
        closeUserCardFullPopupButton.gameObject.SetActive(false); // ����ī�� ��ü���� �˾� �ݱ� ��ư ��Ȱ��ȭ
    }

}
