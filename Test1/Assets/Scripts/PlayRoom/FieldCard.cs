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
    public Transform FieldContainer; // FieldArea의 Contents
    public CardPool cardPool; // CardPool 참조
    public List<string> shownCardData; // 필드에 놓인 카드 데이터 리스트
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
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1개의 랜덤 카드 얻기
        foreach (var card in randomCards)
        {
            cardPool.MoveCardToParent(card, FieldContainer); // 각 카드를 UserCardArea로 이동
            card.SetActive(true); // 카드가 보이도록 활성화
            displayedFieldCard.Add(card); // 이동된 카드를 리스트에 추가
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            // 카드의 TextMeshProUGUI 컴포넌트 가져오기
            TextMeshProUGUI textComponent = card.GetComponentInChildren<TextMeshProUGUI>();
            // 텍스트 읽기
            string currentText = textComponent.text;
            SaveCardData(currentText);
        }
    }


    public void ShowCapableCardArea()
    {

    }

    public void InitializeSavedCardData(int rows, int columns)
    {
        savedCardData = new string[rows, columns]; // 동적으로 배열 크기 설정
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
