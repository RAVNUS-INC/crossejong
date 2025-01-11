using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class NewCardPool : MonoBehaviour
{
    public Transform cardContainer; // ī�� �θ� ������Ʈ
    public Sprite cardBackImage; // ī�� �޸� �̹���
    public Sprite cardFrontColorImage; // ī�� �ո� �̹���
    public Sprite cardFrontBlackImage; // ī�� �ո� �̹���
    public NewCard newCard; // NewCard ����

    public TextMeshProUGUI tmpText; // TextMeshProUGUI ������Ʈ ����
    public TMP_FontAsset newFont; // ������ ���ο� Font Asset

    public List<GameObject> cards = new List<GameObject>(); // ������ ī�� ���

    void Start()
    {
        // ī�� ����
        CreateCard();
    }

    public void CreateCardRed()
    {
        for (int i = 0; i < newCard.cardFrontRed.Count; i++)
        {
            // ī�� GameObject ����
            GameObject card = new GameObject(newCard.cardFrontRed[i]);
            card.transform.SetParent(cardContainer, false);

            // RectTransform ����
            RectTransform rectTransform = card.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200, 200); // ī�� ũ�� ����

            // Button ������Ʈ �߰�
            Button button = card.AddComponent<Button>();

            // ī�� ��� �̹��� �߰�
            Image cardBackground = card.AddComponent<Image>();
            cardBackground.sprite = cardBackImage; // ī�� �޸� �̹��� ����
            cardBackground.type = Image.Type.Sliced;

            // �ؽ�Ʈ ������Ʈ ���� (�ո��)
            GameObject textObject = new GameObject("CardText");
            textObject.transform.SetParent(card.transform, false);
            RectTransform textRect = textObject.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            TextMeshProUGUI tmpText = textObject.AddComponent<TextMeshProUGUI>();
            tmpText.font = newFont;
            tmpText.text = newCard.cardFrontRed[i]; // �ؽ�Ʈ ����
            tmpText.fontSize = 120;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.color = Color.red;

            // ī�� ����Ʈ�� �߰�
            cards.Add(card);
        }
    }

    public void CreateCardBlack()
    {
        for (int i = 0; i < newCard.cardFrontBlack.Count; i++)
        {
            // ī�� GameObject ����
            GameObject card = new GameObject(newCard.cardFrontBlack[i]);
            card.transform.SetParent(cardContainer, false);

            // RectTransform ����
            RectTransform rectTransform = card.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200, 200); // ī�� ũ�� ����

            // Button ������Ʈ �߰�
            Button button = card.AddComponent<Button>();

            // ī�� ��� �̹��� �߰�
            Image cardBackground = card.AddComponent<Image>();
            cardBackground.sprite = cardBackImage; // ī�� �޸� �̹��� ����
            cardBackground.type = Image.Type.Sliced;

            // �ؽ�Ʈ ������Ʈ ���� (�ո��)
            GameObject textObject = new GameObject("CardText");
            textObject.transform.SetParent(card.transform, false);
            RectTransform textRect = textObject.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            TextMeshProUGUI tmpText = textObject.AddComponent<TextMeshProUGUI>();
            tmpText.font = newFont;
            tmpText.text = newCard.cardFrontBlack[i]; // �ؽ�Ʈ ����
            tmpText.fontSize = 120;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.color = Color.black;

            // ī�� ����Ʈ�� �߰�
            cards.Add(card);
        }
    }

    public void CreateCardSpecialColor()
    {
        // ī�� GameObject ����
        GameObject card = new GameObject("�÷�");
        card.transform.SetParent(cardContainer, false);

        // RectTransform ����
        RectTransform rectTransform = card.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 200); // ī�� ũ�� ����

        // Button ������Ʈ �߰�
        Button button = card.AddComponent<Button>();

        // ī�� ��� �̹��� �߰� (parent object�� �߰�)
        Image cardBackground = card.AddComponent<Image>();
        cardBackground.sprite = cardBackImage; // ī�� �޸� �̹��� ����
        cardBackground.type = Image.Type.Sliced;

        // �ո� �̹����� �ڽ� GameObject�� �߰�
        GameObject frontImageObject = new GameObject("FrontImage");
        frontImageObject.transform.SetParent(card.transform, false);
        Image cardFrontground = frontImageObject.AddComponent<Image>();
        cardFrontground.sprite = cardFrontColorImage; // ī�� �ո� �̹��� ����
        cardFrontground.type = Image.Type.Sliced;

        // ī�� ����Ʈ�� �߰�
        cards.Add(card);
    }

    public void CreateCardSpecialBlack()
    {
        // ī�� GameObject ����
        GameObject card = new GameObject("���");
        card.transform.SetParent(cardContainer, false);

        // RectTransform ����
        RectTransform rectTransform = card.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 200); // ī�� ũ�� ����

        // Button ������Ʈ �߰�
        Button button = card.AddComponent<Button>();

        // ī�� ��� �̹��� �߰� (parent object�� �߰�)
        Image cardBackground = card.AddComponent<Image>();
        cardBackground.sprite = cardBackImage; // ī�� �޸� �̹��� ����
        cardBackground.type = Image.Type.Sliced;

        // �ո� �̹����� �ڽ� GameObject�� �߰�
        GameObject frontImageObject = new GameObject("FrontImage");
        frontImageObject.transform.SetParent(card.transform, false);
        Image cardFrontground = frontImageObject.AddComponent<Image>();
        cardFrontground.sprite = cardFrontBlackImage; // ī�� �ո� �̹��� ����
        cardFrontground.type = Image.Type.Sliced;

        // ī�� ����Ʈ�� �߰�
        cards.Add(card);
    }

    public void ChangeCardColor(Button button)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Button cardButton = cards[i].GetComponent<Button>(); // Button ������Ʈ ��������

            if (cardButton != null)
            {
                ColorBlock colorBlock = cardButton.colors;

                // �⺻ ���� ����
                colorBlock.normalColor = Color.white;
                // Ŭ������ �� ���� ����
                colorBlock.highlightedColor = Color.grey;
                // Ŭ������ �� ���� ����
                colorBlock.pressedColor = Color.grey;
                // ��Ȱ��ȭ�� ���� ���� ����
                colorBlock.disabledColor = Color.red;

                // ����� ���� ����� ��ư�� ����
                cardButton.colors = colorBlock;
            }
        }
    }


    private void CreateCard()
    {
        CreateCardRed();
        CreateCardBlack();
        CreateCardSpecialColor();
        CreateCardSpecialBlack();

        // ��� ī�忡 ���� ChangeCardColor ����
        foreach (var card in cards)
        {
            Button cardButton = card.GetComponent<Button>();
            if (cardButton != null)
            {
                // ��ư Ŭ�� �� ChangeCardColor �Լ� ����
                cardButton.onClick.AddListener(() => ChangeCardColor(cardButton));
            }
        }
    }

    // �������� 11���� ī�� ����
    public List<GameObject> GetRandomCards(int count)
    {
        List<GameObject> randomCards = new List<GameObject>();
        HashSet<int> usedIndices = new HashSet<int>();

        while (randomCards.Count < count)
        {
            int randomIndex = Random.Range(0, cards.Count);
            if (!usedIndices.Contains(randomIndex))
            {
                usedIndices.Add(randomIndex);
                randomCards.Add(cards[randomIndex]);
            }
        }

        return randomCards;
    }


    // ī�� �̵�
    public void MoveCardToParent(GameObject card, Transform parent)
    {
        card.transform.SetParent(parent, false);
    }
}
