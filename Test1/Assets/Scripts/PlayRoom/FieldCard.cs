using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Xml;
using PlayFab.DataModels;
using JetBrains.Annotations;
using System;
using Unity.Collections.LowLevel.Unsafe;

public class FieldCard : MonoBehaviour
{
    public Transform fieldContainer; // FieldArea�� Contents
    public CardPool cardPool; // CardPool ����
    public CapableAreaPopup capableAreaPopup;
    public List<string> shownCardData; // �ʵ忡 ���� ī�� ������ ����Ʈ
    public List<GameObject> fieldDisplayedCards;
    public GameObject capableAreaPopupPanel;
    public List<string> blockList = new List<string>{"", "", "", ""};
    public int row;
    public int col;
    public string[,] savedCardData;
    public int cardDataRowIndex;
    public int cardDataColIndex;

    void Start()
    {

    }

    public void StartCardShown()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1���� ���� ī�� ���
        cardPool.MoveCardsToTarGetArea(randomCards, fieldContainer, fieldDisplayedCards);

        RectTransform rectTransform = randomCards[0].GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
        TextMeshProUGUI textComponent = randomCards[0].GetComponentInChildren<TextMeshProUGUI>();
        string currentText = textComponent.text;
        SaveCardData(currentText);
        OnClickFieldCard();
        
    }

    public void OnClickFieldCard()
    {
        // ��� ī�忡 ���� ChangeCardColor ����
        foreach (var card in fieldDisplayedCards)
        {
            Button cardButton = card.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.onClick.AddListener(() => ShowCapableAreaPopup(cardButton));
                cardButton.onClick.AddListener(() => capableAreaPopup.MoveCardsToCapableArea());
                cardButton.onClick.AddListener(() => CreateCapableArea(cardButton));
            }
        }
    }

    public void CreateCapableArea(Button cardButton)
    {
        // ī�� ��ġ ��ǥ ����Ʈ
        Vector2[] positions = new Vector2[]
        {
            new Vector2(210, 0),
            new Vector2(0, 210),
            new Vector2(-210, 0),
            new Vector2(0, -210)
        };

        for (int i = 0; i < blockList.Count; i++)
        {
            // ī�� GameObject ����
            GameObject block = new GameObject(blockList[i]);
            block.transform.SetParent(capableAreaPopup.capableContainer, false);

            // RectTransform ����
            RectTransform rectTransform = block.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200, 200); // ī�� ũ�� ����
            rectTransform.anchoredPosition = positions[i]; // UI ��ǥ ����

            // Button ������Ʈ �߰�
            Button blockButton = block.AddComponent<Button>();
            cardPool.ChangeCardColor(blockButton);

            // ī�� ��� �̹��� �߰�
            Image cardFrontFeature = block.AddComponent<Image>();
            cardFrontFeature.sprite = cardPool.cardFrontImage; // ī�� �ո� �̹��� ����
            cardFrontFeature.type = Image.Type.Sliced;


            block.SetActive(true);
        }
    }

    public void ShowCapableAreaPopup(Button cardButton)
    {
        capableAreaPopupPanel.SetActive(true);
    }

    public void InitializeSavedCardData(int rows, int columns)
    {
        savedCardData = new string[rows, columns]; // �������� �迭 ũ�� ����
    }

    public void SaveCardData(string CardText)
    {
        if (savedCardData == null)
        {
            InitializeSavedCardData(3, 3);
            savedCardData[1, 1] = CardText;
            cardDataRowIndex = 1;
            cardDataColIndex = 1;
        }
        else
        {

        }
    }

    void Update()
    {
        
    }
}
