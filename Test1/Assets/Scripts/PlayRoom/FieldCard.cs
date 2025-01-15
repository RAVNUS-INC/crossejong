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
        foreach (var card in randomCards)
        {
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            TextMeshProUGUI textComponent = card.GetComponentInChildren<TextMeshProUGUI>();
            string currentText = textComponent.text;
            SaveCardData(currentText);
            OnClickFieldCard();
        }
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
            }
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
