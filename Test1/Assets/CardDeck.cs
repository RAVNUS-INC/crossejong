using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardDeck : MonoBehaviour
{
    [System.Serializable]
    public class Card
    {
        public string cardName; // ī�� �̸�
        public Sprite cardImage; // ī�� �̹��� (Sprite Ÿ��)
    }

    public List<Card> cardList = new List<Card>(); // ī�� ����Ʈ (�ν����Ϳ��� ���� ����)
    public GameObject cardPrefab; // ī�� ������
    public Transform cardContainer; // ī�带 ��ġ�� �����̳�
    public float cardSpacing = 115f; // ī�� ����
    public Button createCardButton; // ī�� ���� ��ư
    private List<Card> selectedCards = new List<Card>(); // �̹� ���õ� ī�� ����Ʈ

    void Start()
    {
        createCardButton.onClick.AddListener(OnCreateCard); // ��ư Ŭ�� �̺�Ʈ�� ī�� ���� �޼��� �߰�
    }

    private void OnCreateCard()
    {
        CreateCard(); // ī�� ���� ����
        createCardButton.interactable = false; // ��ư�� ��Ȱ��ȭ
    }

    public void CreateCard()
    {
        // ī�尡 11�� ������ ���� ����
        if (selectedCards.Count >= cardList.Count)
        {
            Debug.Log("��� ī�尡 �̹� ���õǾ����ϴ�.");
            return;
        }

        for (int i = 0; i < 11; i++) // 11���� ī�带 ����
        {
            int randomIndex;
            Card selectedCard;

            do
            {
                randomIndex = Random.Range(0, cardList.Count);
                selectedCard = cardList[randomIndex];
            } while (selectedCards.Contains(selectedCard)); // �̹� ���õ� ī������ Ȯ��

            selectedCards.Add(selectedCard); // ���õ� ī�� ����Ʈ�� �߰�

            GameObject cardInstance = Instantiate(cardPrefab, cardContainer);
            cardInstance.GetComponent<Image>().sprite = selectedCard.cardImage; // ī�� �̹��� ����
            cardInstance.name = selectedCard.cardName; // ī�� �̸� ����

            RectTransform rectTransform = cardInstance.GetComponent<RectTransform>();

            if (i == 0)
            {
                rectTransform.anchoredPosition = new Vector2(385, 142);
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(385 + (i * cardSpacing), 142);
            }

            Debug.Log($"Card Name: {selectedCard.cardName}, Card Image: {selectedCard.cardImage}");
        }
    }
}
