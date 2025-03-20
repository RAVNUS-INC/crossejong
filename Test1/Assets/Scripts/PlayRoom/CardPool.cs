using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using PlayFab.DataModels;
using DG.Tweening;
using System;
using System.Reflection;
using Random = UnityEngine.Random;
using Photon.Pun;

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

    public TextMeshProUGUI tmpText; // TextMeshProUGUI ������Ʈ ����
    public TMP_FontAsset newFont; // ������ ���ο� Font Asset

    public List<GameObject> cards = new List<GameObject>(); // ������ ī�� ���

    public RectTransform rectTransform; // ī���� RectTransform

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

            if (cardList != ObjectManager.instance.cardFrontSpecial)
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
                if (cardList[i] == "C")
                {
                    // ī�� ��� �̹��� �߰�
                    Image cardFrontFeature = card.AddComponent<Image>();
                    cardFrontFeature.sprite = specialCardFrontColorImage; // ī�� �ո� �̹��� ����
                    cardFrontFeature.type = Image.Type.Sliced;

                    GameObject textObject = new GameObject("C");
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
                    Image cardFrontFeature = card.AddComponent<Image>();
                    cardFrontFeature.sprite = specialCardFrontBlackImage; // ī�� �ո� �̹��� ����
                    cardFrontFeature.type = Image.Type.Sliced;

                    GameObject textObject = new GameObject("B");
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

            }

            // ī�� ����Ʈ�� �߰�
            cards.Add(card);
        }
    }

    public void CreateCard()
    {
        CreateCards(ObjectManager.instance.cardFrontRed, Color.red);
        CreateCards(ObjectManager.instance.cardFrontBlack, Color.black);
        CreateCards(ObjectManager.instance.cardFrontSpecial, Color.clear);
    }


    //�������� 1���� ī�� ����(change) ���ڿ� ����Ʈ ��ȯ(���常 ����)
    public string[] GetRandomCardsName(int count)
    {
        List<string> usedCardNames = new List<string>(); // ���� �� ���� ī�� �̸��� ����
        while (usedCardNames.Count < count)
        {
            int randomIndex = UnityEngine.Random.Range(0, cards.Count);
            string cardName = cards[randomIndex].name; // ī���� �̸��� ���

            // �ش� ī�尡 ���� ������ �ʾҴٸ�
            if (!ObjectManager.instance.usedIndices.Contains(cardName))
            {
                usedCardNames.Add(cardName); // ī�� �̸��� �߰�
                ObjectManager.instance.usedIndices.Add(cardName); // ���常 �� ����Ʈ�� ����
            }
        }

        return usedCardNames.ToArray(); // ī�� �̸� �迭�� ��ȯ
    }


    // ���� �ε��� ����Ʈ�� ���� ī�� gameobject ����(���� ���� �� �������� ����)
    public List<GameObject> GetRandomCardsObject(string[] usednames)
    {
        List<GameObject> randomCards = new List<GameObject>();

        foreach (string name in usednames)
        {
            GameObject foundCard = cards.Find(c => c.name == name); // �̸����� �˻�
            randomCards.Add(foundCard);
            Debug.Log($"����ī�� '{name}' ������");
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
        if (targetList == UserCardFullPopup.instance.fullDisplayedCards) { }
        else
        {
            targetList.Clear();
        }

        foreach (var card in startList)
        {
            MoveCardToParent(card, targetArea); // �� ī�带 TargetArea�� �̵�
            card.SetActive(true); // ī�尡 ���̵��� Ȱ��ȭ

            // ���� Ÿ�ٸ���Ʈ�� Ǯ�˾� ����Ʈ��� ī�� �߰��� �������� �ʴ´� -> �̹� ���÷��̸���Ʈ �߰��� �� ����ȭ ��
            if (targetList == UserCardFullPopup.instance.fullDisplayedCards) { }
            else
            {
                targetList.Add(card); // �������� ����Ʈ�� �߰�
            }

            for (int i = 0; i < cards.Count; i++) 
            {
                if (card == cards[i])
                {
                    cards.RemoveAt(i);
                }
            }
        }
        SortCardIndex(targetList);
    }

    public void GetCardsToTarGetArea(List<GameObject> startList, Transform targetArea, List<GameObject> targetList)
    {
        foreach (var card in startList)
        {
            MoveCardToParent(card, targetArea); // �� ī�带 TargetArea�� �̵�
            card.SetActive(true); // ī�尡 ���̵��� Ȱ��ȭ

            if (targetList.Contains(card))
            {
                Debug.Log("�ߺ� ī�带 �߰����� �ʽ��ϴ�.");
            }
            else
            {
                targetList.Add(card); // �������� ����Ʈ�� �߰�

                // Ÿ�ٸ���Ʈ�� ����ī�帮��Ʈ�� ���
                if (targetList == UserCard.instance.displayedCards)
                {
                    // �˾�ī�� ����Ʈ���� ����ī�� ����Ʈ�� �����ϰ� ī�� ���� �ٷ� ����ȭ
                    if (!UserCardFullPopup.instance.fullDisplayedCards.Contains(card))
                    {
                        UserCardFullPopup.instance.fullDisplayedCards.Add(card);
                    }
                }
                
            }


            for (int i = 0; i < cards.Count; i++)
            {
                if (card == cards[i])
                {
                    cards.RemoveAt(i);
                }

            }
        }

        SortCardIndex(targetList);
    }


    public void SortCardIndex(List<GameObject> cardList)
    {
        // �����ٶ� �� ����
        UserCard.instance.displayedCards.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.CurrentCulture));

        // ���ĵ� ������� UI�� ����
        for (int i = 0; i < UserCard.instance.displayedCards.Count; i++)
        {
            UserCard.instance.displayedCards[i].transform.SetSiblingIndex(i);
        }

        //if (cardList.Count != 0)
        //{
        //    for (int i = 0; i < cardList.Count; i++)
        //    {
        //        CardDrag CD = cardList[i].GetComponent<CardDrag>();
        //        if (CD != null)
        //        {
        //            CD.cardIndex = i;
        //        }
        //    }
        //}
    }

}
