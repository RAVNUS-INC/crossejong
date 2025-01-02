using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UserCardFullPopup : MonoBehaviour
{
    public GameObject userCardFullPanel; // ����ī�� ��ü���� �˾� �г�
    public Button openUserCardFullPopupButton; // ����ī�� ��ü���� �˾� ���� ��ư
    public Button closeUserCardFullPopupButton; // ����ī�� ��ü���� �˾� �ݱ� ��ư
    public Transform fullPopupAreaContent; // FullPopupArea�� Content (��ũ�� ���� ���빰)
    public GameObject userCardArea; // ����ī�� ����

    public UserCard userCard; // UserCard ��ũ��Ʈ ����
    public List<GameObject> displayedFullCards = new List<GameObject>(); // ����ڿ��� ǥ�õ� ��ü ī�� ����Ʈ

    void Start()
    {
        UserCardFullPopupf();
    }

    public void UserCardFullPopupf()
    {
        openUserCardFullPopupButton.onClick.AddListener(OpenUserCardFullPopup);
        closeUserCardFullPopupButton.onClick.AddListener(CloseUserCardFullPopup);

    }

    void OpenUserCardFullPopup()
    {
        SyncCardsFromUserCard(); // UserCard ������ ����ȭ
        AddCardsToFullPopupArea(); // FullPopup�� ī�� ��ġ
        userCardFullPanel.SetActive(true);
        closeUserCardFullPopupButton.gameObject.SetActive(true);
        userCardArea.SetActive(false);
    }

    void CloseUserCardFullPopup()
    {
        userCardFullPanel.SetActive(false);
        closeUserCardFullPopupButton.gameObject.SetActive(false);
        userCardArea.SetActive(true);
    }

    public void SyncCardsFromUserCard()
    {
        displayedFullCards.Clear();
        displayedFullCards.AddRange(userCard.displayedCards);
    }

    void CreateRow(List<GameObject> rowCards, int rowIndex)
    {
        GameObject row = new GameObject("Row " + rowIndex);
        row.transform.SetParent(fullPopupAreaContent, false);

        // RectTransform�� �����մϴ�.
        RectTransform rowTransform = row.GetComponent<RectTransform>();
        if (rowTransform == null)
        {
            rowTransform = row.AddComponent<RectTransform>();
        }
        rowTransform.sizeDelta = new Vector2(1080, 200); // �� ũ�⸦ �����մϴ�.

        // HorizontalLayoutGroup�� �߰��մϴ�.
        HorizontalLayoutGroup layoutGroup = row.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.padding = new RectOffset(15, 0, 0, 0); // ���� ���� ����
        layoutGroup.spacing = 15; // ī�� �� ���� ����
        layoutGroup.childAlignment = TextAnchor.MiddleLeft; // ���� ����
        layoutGroup.childForceExpandWidth = false; // �ڽ� ����� �ʺ� ���� Ȯ�� ��Ȱ��ȭ
        layoutGroup.childForceExpandHeight = false; // �ڽ� ����� ���� ���� Ȯ�� ��Ȱ��ȭ

        // �� ī�带 �࿡ �߰��մϴ�.
        foreach (var card in rowCards)
        {
            GameObject cardInstance = Instantiate(card, row.transform);
            RectTransform cardRect = cardInstance.GetComponent<RectTransform>();
            if (cardRect != null)
            {
                cardRect.sizeDelta = new Vector2(180, 180); // ī�� ũ�⸦ 180x180���� ����
            }
        }

        // ������ �࿡ ī�尡 5���� �ƴ� ���, �� ������ ä���� X ��ǥ ���߱�
        int remainingSpace = 5 - rowCards.Count;
        for (int i = 0; i < remainingSpace; i++)
        {
            GameObject spacer = new GameObject("Spacer " + i);
            RectTransform spacerRect = spacer.AddComponent<RectTransform>();
            spacer.transform.SetParent(row.transform, false);
            spacerRect.sizeDelta = new Vector2(180, 180); // Spacer�� ũ�⸦ ī�� ũ��� �����ϰ� ����
            LayoutElement layoutElement = spacer.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 0; // Spacer�� ���� ������ �������� �ʵ��� ����
            layoutElement.minWidth = 201; // Spacer�� �ּ� �ʺ� ī�� ũ��� �����ϰ� ����
            layoutElement.preferredWidth = 201; // Spacer�� ��ȣ �ʺ� ī�� ũ��� �����ϰ� ����
        }
    }

    public void AddCardsToFullPopupArea()
    {
        foreach (Transform child in fullPopupAreaContent)
        {
            Destroy(child.gameObject);
        }

        List<GameObject> cardsInRow = new List<GameObject>();
        int rowIndex = 1;

        for (int i = 0; i < displayedFullCards.Count; i++)
        {
            cardsInRow.Add(displayedFullCards[i]);

            if (cardsInRow.Count == 5 || i == displayedFullCards.Count - 1)
            {
                CreateRow(cardsInRow, rowIndex);
                rowIndex++;
                cardsInRow.Clear();
            }
        }

        RectTransform contentRect = fullPopupAreaContent.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, rowIndex * 200);
    }

}