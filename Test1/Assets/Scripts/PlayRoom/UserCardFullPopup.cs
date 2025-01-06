using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

public class UserCardFullPopup : MonoBehaviour
{
    public CardPool cardPool; // CardPool ���� 
    public UserCard userCard; // UserCard ����
    public Transform fullCardContainer; // FullPopupArea�� Content
    public List<GameObject> fullDisplayedCards; // FullPopup���� �������� ī�� ����Ʈ

    void Start()
    {
        // Optional: �ʱ�ȭ �۾�
    }

    // FullPopupArea�� 11���� ���� ī�� �̵�
    public void MoveFullPopupArea()
    {
        List<GameObject> userCards = userCard.displayedCards; // UserCard���� ǥ�õ� ī��� ��������

        // ī�带 5���� ��� ��(row)���� ����
        List<GameObject> rowCards = new List<GameObject>();
        for (int i = 0; i < userCards.Count; i++)
        {
            rowCards.Add(userCards[i]);

            // 5�� ī�帶�� ���ο� ���� ����
            if (rowCards.Count == 5 || i == userCards.Count - 1)
            {
                CreateRow(rowCards, fullDisplayedCards.Count); // ���ο� �� ����
                rowCards.Clear(); // ���� �������Ƿ� ����Ʈ �ʱ�ȭ
            }
        }

        //// ��� ī����� FullPopupArea�� �̵�
        //foreach (var card in userCards)
        //{
        //    cardPool.MoveCardToParent(card, fullCardContainer); // �� ī�带 FullPopupArea�� �̵�
        //    card.SetActive(true); // ī�尡 ���̵��� Ȱ��ȭ
        //    fullDisplayedCards.Add(card); // �̵��� ī�带 ����Ʈ�� �߰�
        //}
    }


    // UserFullPopupExit ��ư Ŭ�� ��, FullPopup���� UserCardArea�� ī�带 ����
    public void MoveCardsBackToUserCardArea()
    {
        foreach (var card in fullDisplayedCards)
        {
            cardPool.MoveCardToParent(card, userCard.userCardContainer); // �� ī�带 UserCardArea�� �̵�
            card.SetActive(true); // ī�尡 ���̵��� Ȱ��ȭ
            userCard.displayedCards.Add(card); // UserCard�� ����Ʈ�� �߰�
        }

        // FullPopup���� ������ ī�带 �ʱ�ȭ
        fullDisplayedCards.Clear();
    }

    void CreateRow(List<GameObject> rowCards, int rowIndex)
    {
        GameObject row = new GameObject("Row " + rowIndex);
        row.transform.SetParent(fullCardContainer, false);

        // RectTransform�� �����մϴ�.
        RectTransform rowTransform = row.GetComponent<RectTransform>();
        if (rowTransform == null)
        {
            rowTransform = row.AddComponent<RectTransform>();
        }
        rowTransform.sizeDelta = new Vector2(1080, 200); // �� ũ�� ����

        // HorizontalLayoutGroup�� �߰��մϴ�.
        HorizontalLayoutGroup layoutGroup = row.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.padding = new RectOffset(15, 0, 0, 0); // ���� ���� ����
        layoutGroup.spacing = 20; // ī�� �� ���� ����
        layoutGroup.childAlignment = TextAnchor.MiddleLeft; // ���� ����
        layoutGroup.childForceExpandWidth = false; // �ڽ� �ʺ� ���� Ȯ�� ��Ȱ��ȭ
        layoutGroup.childForceExpandHeight = false; // �ڽ� ���� ���� Ȯ�� ��Ȱ��ȭ

        // �� ī�带 �࿡ �߰��մϴ�.
        foreach (var card in rowCards)
        {
            GameObject cardInstance = Instantiate(card, row.transform);
            RectTransform cardRect = cardInstance.GetComponent<RectTransform>();
            if (cardRect != null)
            {
                cardRect.sizeDelta = new Vector2(180, 180); // ī�� ũ�� 180x180���� ����
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
            layoutElement.minWidth = 180; // Spacer�� �ּ� �ʺ� ī�� ũ��� �����ϰ� ����
            layoutElement.preferredWidth = 180; // Spacer�� ��ȣ �ʺ� ī�� ũ��� �����ϰ� ����
        }
    }

}
