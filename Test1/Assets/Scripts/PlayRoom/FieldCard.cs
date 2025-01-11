using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Xml;
using PlayFab.DataModels;

public class FieldCard : MonoBehaviour
{
    public Transform FieldContainer; // FieldArea의 Contents
    public List<GameObject> displayedField; // FieldArea에서 보여지는 카드 리스트
    public string[,] cardField; // 2차원 배열로 카드 필드 저장
    public int rows = 3; // 행 크기
    public int columns = 3; // 열 크기
    public CardPool cardPool; // NewCardPool 참조

    void Start()
    {
        cardField = new string[rows, columns];
        InitializeCardField();
    }

    // 필드 초기화 함수
    private void InitializeCardField()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                cardField[i, j] = null; // 초기값: null
            }
        }
    }

    public void CardField()
    {

    }

    public void StartFieldCard()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1개의 랜덤 카드 얻기
        foreach (var card in randomCards)
        {
            cardPool.MoveCardToParent(card, FieldContainer); // 각 카드를 UserCardArea로 이동
            card.SetActive(true); // 카드가 보이도록 활성화
            displayedField.Add(card); // 이동된 카드를 리스트에 추가
        }
    }

    // 카드 추가 함수
    private void AddCardToField(int row, int col, string cardData)
    {
        if (row >= 0 && row < rows && col >= 0 && col < columns)
        {
            cardField[row, col] = cardData;
        }
    }

}
