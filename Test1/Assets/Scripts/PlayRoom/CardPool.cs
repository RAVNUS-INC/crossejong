using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using PlayFab.DataModels;
using DG.Tweening;

public class CardPool : MonoBehaviour
{
    public static CardPool instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        rectTransform = GetComponent<RectTransform>();  // RectTransform �ʱ�ȭ
    }

    public Transform cardContainer; // ī�� �θ� ������Ʈ
    public Sprite cardFrontImage; // ī�� �ո� �̹���
    public Sprite cardBackImage; // ī�� �޸� �̹���
    public Sprite specialCardFrontColorImage; // ī�� �ո� �̹���
    public Sprite specialCardFrontBlackImage; // ī�� �ո� �̹���
    public CardLists cardLists; // NewCard ����

    public TextMeshProUGUI tmpText; // TextMeshProUGUI ������Ʈ ����
    public TMP_FontAsset newFont; // ������ ���ο� Font Asset

    public List<GameObject> cards = new List<GameObject>(); // ������ ī�� ���

    public bool isFlipped = false; // ī�尡 ���������� ����
    public RectTransform rectTransform; // ī���� RectTransform


    void Start()
    {
        // ī�� ����
        CreateCard();
    }

    public void CreateCards(List<string> cardList, Color color)
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            // ī�� GameObject ����
            GameObject card = new GameObject(cardList[i]);
            card.transform.SetParent(cardContainer, false);

            // RectTransform ����
            RectTransform rectTransform = card.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200, 200); // ī�� ũ�� ����

            // Button ������Ʈ �߰�
            //Button button = card.AddComponent<Button>();

            if (cardList != cardLists.cardFrontSpecial)
            {
                // ī�� ��� �̹��� �߰�
                Image cardFrontFeature = card.AddComponent<Image>();
                cardFrontFeature.sprite = cardFrontImage; // ī�� �ո� �̹��� ����
                cardFrontFeature.type = Image.Type.Sliced;

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
                tmpText.text = cardList[i]; // �ؽ�Ʈ ����
                tmpText.fontSize = 120;
                tmpText.alignment = TextAlignmentOptions.Center;
                tmpText.color = color;
                tmpText.raycastTarget = false; // �ؽ�Ʈ ����ĳ��Ʈ ����
            }
            else
            {
                // ī�� ��� �̹��� �߰�
                Image cardFrontFeature = card.AddComponent<Image>();
                cardFrontFeature.sprite = specialCardFrontColorImage; // ī�� �ո� �̹��� ����
                cardFrontFeature.type = Image.Type.Sliced;
            }

            // ī�� ����Ʈ�� �߰�
            cards.Add(card);
        }
    }

    //public void ChangeCardColor(Button button)
    //{
    //    for (int i = 0; i < cards.Count; i++)
    //    {
    //        Button cardButton = cards[i].GetComponent<Button>(); // Button ������Ʈ ��������

    //        if (cardButton != null)
    //        {
    //            ColorBlock colorBlock = cardButton.colors;

    //            // �⺻ ���� ����
    //            colorBlock.normalColor = Color.white;
    //            // Ŭ������ �� ���� ����
    //            colorBlock.selectedColor = Color.grey;
    //            // ��Ȱ��ȭ�� ���� ���� ����
    //            colorBlock.disabledColor = Color.red;

    //            // ����� ���� ����� ��ư�� ����
    //            cardButton.colors = colorBlock;
    //        }
    //    }
    //}

    //public void FlipCard(GameObject card)
    //{
    //    Image cardFrontFeature = card.gameObject.GetComponent<Image>();
    //    Sprite savedCardFrontImage = card.gameObject.GetComponent<Sprite>();
    //    RectTransform cardRectTransform = card.GetComponent<RectTransform>();  // card�� RectTransform ��������

    //    if (cardFrontFeature.sprite != cardBackImage)
    //    {
    //        // ī�尡 �̹� ���������� �ո����� ���ư�
    //        cardRectTransform.DORotate(new Vector3(0, 180, 0), 0.1f, RotateMode.LocalAxisAdd) // 180�� ȸ��
    //            .OnComplete(() =>
    //            {
    //                cardFrontFeature.sprite = cardBackImage;
    //                // CardText��� �ڽ� ������Ʈ�� ã�Ƽ� ��Ȱ��ȭ
    //                Transform cardTextTransform = card.transform.Find("CardText");
    //                cardTextTransform.gameObject.SetActive(false);
    //            });
    //    }
    //    else
    //    {
    //        // ī�尡 �ո��� ���
    //        cardRectTransform.DORotate(new Vector3(0, 180, 0), 0.1f, RotateMode.LocalAxisAdd) // 180�� ȸ��
    //            .OnComplete(() =>
    //            {
    //                cardFrontFeature.sprite = savedCardFrontImage;
    //                Transform cardTextTransform = card.transform.Find("CardText");
    //                cardTextTransform.gameObject.SetActive(true);
    //            });
    //    }

    //    // �ø� ���¸� ������Ŵ
    //    isFlipped = !isFlipped;
    //}


    private void CreateCard()
    {
        CreateCards(cardLists.cardFrontRed, Color.red);
        CreateCards(cardLists.cardFrontBlack, Color.black);
        CreateCards(cardLists.cardFrontSpecial, Color.white);
        //ButtonColor(cards);
    }

    //public void ButtonColor(List<GameObject> targetList)
    //{
    //    // ��� ī�忡 ���� ChangeCardColor ����
    //    foreach (var card in targetList)
    //    {
    //        Button cardButton = card.GetComponent<Button>();
    //        if (cardButton != null)
    //        {
    //            // ��ư Ŭ�� �� ChangeCardColor �Լ� ����
    //            cardButton.onClick.AddListener(() => ChangeCardColor(cardButton));
    //        }
    //    }
    //}

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

    public void MoveCardsToTarGetArea(List<GameObject> startList, Transform targetArea, List<GameObject> targetList)
    {
        targetList.Clear();

        foreach (var card in startList)
        {
            MoveCardToParent(card, targetArea); // �� ī�带 TargetArea�� �̵�
            card.SetActive(true); // ī�尡 ���̵��� Ȱ��ȭ
            targetList.Add(card); // �������� ����Ʈ�� �߰�
        }

        SortCardIndex(targetList);
    }
    public void GetCardsToTarGetArea(List<GameObject> startList, Transform targetArea, List<GameObject> targetList)
    {
        foreach (var card in startList)
        {
            MoveCardToParent(card, targetArea); // �� ī�带 TargetArea�� �̵�
            card.SetActive(true); // ī�尡 ���̵��� Ȱ��ȭ
            targetList.Add(card); // �������� ����Ʈ�� �߰�
        }

        SortCardIndex(targetList);
    }

    public void SortCardIndex(List<GameObject> cardList)
    {
        if (cardList.Count != 0)
        {
            for (int i = 0; i < cardList.Count; i++)
            {
                CardDrag CD = cardList[i].GetComponent<CardDrag>();
                if (CD != null)
                {
                    CD.cardIndex = i;
                }
            }
        }
    }
}
