using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OptionPopup : MonoBehaviour
{
    public GameObject optionPopupPanel; // �ɼ� �˾� �г�
    public Button openOptionPopupButton; // �ɼ� �˾� ���� ��ư
    public Button closeOptionPopupButton; // �ɼ� �˾� �ݱ� ��ư
    public Button surrenderButton; // �׺� Ȯ�� ��ư

    void Start()
    {

    }

    public void OpenPopup() // �ɼ� �˾� ���� �޼���
    {
        optionPopupPanel.SetActive(true); // �˾� Ȱ��ȭ
        openOptionPopupButton.gameObject.SetActive(false); // �ɼ� �˾� ���� ��ư ��Ȱ��ȭ
        closeOptionPopupButton.gameObject.SetActive(true); // �ɼ� �˾� �ݱ� ��ư Ȱ��ȭ
        surrenderButton.gameObject.SetActive(true); // �׺� Ȯ�� ��ư Ȱ��ȭ
    }

    public void ClosePopup() // �ɼ� �˾� �ݱ� �޼���
    {
        optionPopupPanel.SetActive(false); // �˾� ��Ȱ��ȭ
        openOptionPopupButton.gameObject.SetActive(true); // �ɼ� �˾� ���� ��ư Ȱ��ȭ
        closeOptionPopupButton.gameObject.SetActive(false); // �ɼ� �˾� �ݱ� ��ư ��Ȱ��ȭ
        surrenderButton.gameObject.SetActive(false); // �׺� Ȯ�� ��ư ��Ȱ��ȭ
    }
}
