using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UserCardFullPopup : MonoBehaviour
{
    public GameObject userCardFullPanel; // ����ī�� ��ü���� �˾� �г�
    public Button openUserCardFullPopupButton; // ����ī�� ��ü���� �˾� ���� ��ư
    public Button closeUserCardFullPopupButton; // ����ī�� ��ü���� �˾� �ݱ� ��ư
    public GameObject userCardArea;  // ����ī�� ��ũ�Ѻ�

    public Transform fullPopupAreaContent; // FullPopupArea�� Content (��ũ�� ���� ���빰)
    public List<GameObject> displayedFullCards; // ����ڿ��� ǥ�õ� ��ü ī�� ����Ʈ

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
        userCardArea.SetActive(false); // ����ī�� ��ũ�Ѻ� ��Ȱ��ȭ
    }

    void CloseUserCardFullPopup() // ����ī�� ��ü���� �˾� �ݱ� �޼���
    {
        userCardFullPanel.SetActive(false); // ����ī�� ��ü���� �˾� ��Ȱ��ȭ
        closeUserCardFullPopupButton.gameObject.SetActive(false); // ����ī�� ��ü���� �˾� �ݱ� ��ư ��Ȱ��ȭ
        userCardArea.SetActive(true); // ����ī�� ��ũ�Ѻ� Ȱ��ȭ
    }


    void CreateRow(List<GameObject> rowCards, int rowIndex) // Row�� ���� FullPopupArea�� �߰�
    {
        GameObject row = new GameObject("Row " + rowIndex); // Row �̸��� "Row 1", "Row 2"ó�� ����
        row.transform.SetParent(fullPopupAreaContent, false); // FullPopupArea�� Content�� Row �߰�

        // HorizontalLayoutGroup ������Ʈ �߰�
        HorizontalLayoutGroup layoutGroup = row.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.padding = new RectOffset(15, 0, 0, 0); // ���� �е� 15 ����
        layoutGroup.spacing = 20; // ī�� ���� 20 ����
        layoutGroup.childAlignment = TextAnchor.MiddleLeft; // �ڽ� ����: Middle Left

        // Row�� ī����� ��ġ
        foreach (var card in rowCards)
        {
            card.transform.SetParent(row.transform); // Row�� ī�� ��ġ
        }
    }


    public void AddCardsToFullPopupArea()
    {
        // ī����� 5���� Row�� ������ ��ġ
        List<GameObject> cardsInRow = new List<GameObject>();

        int i = 0;
        int rowIndex = 1; // Row ��ȣ ����
        foreach (var card in displayedFullCards) // displayedFullCards�� ����
        {
            cardsInRow.Add(card);
            i++;

            // 5���� �� Row�� ������
            if (i % 5 == 0 || i == displayedFullCards.Count) // displayedFullCards�� ����
            {
                CreateRow(cardsInRow, rowIndex); // Row ��ȣ�� �Բ� ����
                rowIndex++; // ���� Row�� ��ȣ ����
                cardsInRow.Clear(); // Row�� ī�尡 �߰��Ǿ����� ���ο� Row�� �ʱ�ȭ
            }
        }
    }
}
