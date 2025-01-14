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
    public List<string> shownCardData; // �ʵ忡 ���� ī�� ������ ����Ʈ
    public List<GameObject> fieldDisplayedCards;
    public GameObject capableAreaPopup;
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
        foreach (var card in randomCards)
        {
            cardPool.MoveCardToParent(card, fieldContainer); // �� ī�带 FieldArea�� �̵�
            card.SetActive(true); // ī�尡 ���̵��� Ȱ��ȭ
            fieldDisplayedCards.Add(card); // �̵��� ī�带 ����Ʈ�� �߰�
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            // ī���� TextMeshProUGUI ������Ʈ ��������
            TextMeshProUGUI textComponent = card.GetComponentInChildren<TextMeshProUGUI>();
            // �ؽ�Ʈ �б�
            string currentText = textComponent.text;
            SaveCardData(currentText);
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
                // ��ư Ŭ�� �� ChangeCardColor �Լ� ����
                cardButton.onClick.AddListener(() => ShowCapableAreaPopup(cardButton));
            }
        }
    }

    public void ShowCapableAreaPopup(Button cardButton)
    {
        capableAreaPopup.SetActive(true);
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
        OnClickFieldCard();
    }
}
