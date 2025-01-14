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
    public Transform fieldContainer; // FieldArea의 Contents
    public CardPool cardPool; // CardPool 참조
    public List<string> shownCardData; // 필드에 놓인 카드 데이터 리스트
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
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1개의 랜덤 카드 얻기
        foreach (var card in randomCards)
        {
            cardPool.MoveCardToParent(card, fieldContainer); // 각 카드를 FieldArea로 이동
            card.SetActive(true); // 카드가 보이도록 활성화
            fieldDisplayedCards.Add(card); // 이동된 카드를 리스트에 추가
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            // 카드의 TextMeshProUGUI 컴포넌트 가져오기
            TextMeshProUGUI textComponent = card.GetComponentInChildren<TextMeshProUGUI>();
            // 텍스트 읽기
            string currentText = textComponent.text;
            SaveCardData(currentText);
        }
    }

    public void OnClickFieldCard()
    {
        // 모든 카드에 대해 ChangeCardColor 연결
        foreach (var card in fieldDisplayedCards)
        {
            Button cardButton = card.GetComponent<Button>();
            if (cardButton != null)
            {
                // 버튼 클릭 시 ChangeCardColor 함수 실행
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
