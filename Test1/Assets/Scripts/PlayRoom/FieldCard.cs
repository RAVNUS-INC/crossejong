using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Xml;
using PlayFab.DataModels;
using JetBrains.Annotations;
using System;

public class FieldCard : MonoBehaviour
{
    public Transform FieldContainer; // FieldArea�� Contents
    public CardPool cardPool; // CardPool ����
    public List<string> shownCardData; // �ʵ忡 ���� ī�� ������ ����Ʈ
    public List<GameObject> displayedFieldCard;
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
            cardPool.MoveCardToParent(card, FieldContainer); // �� ī�带 UserCardArea�� �̵�
            card.SetActive(true); // ī�尡 ���̵��� Ȱ��ȭ
            displayedFieldCard.Add(card); // �̵��� ī�带 ����Ʈ�� �߰�
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            // ī���� TextMeshProUGUI ������Ʈ ��������
            TextMeshProUGUI textComponent = card.GetComponentInChildren<TextMeshProUGUI>();
            // �ؽ�Ʈ �б�
            string currentText = textComponent.text;
            SaveCardData(currentText);
        }
    }


    public void ShowCapableCardArea()
    {

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
            ShowCapableCardArea();
        }
        else
        {

        }
    }

}
