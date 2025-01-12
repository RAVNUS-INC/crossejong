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
    public int rows; // 행 크기
    public int columns; // 열 크기
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

    public void FieldCardf()
    {
        foreach (var card in cardPool.GetRandomCards(1)) // 랜덤 카드 1장 선택
        {
            cardPool.MoveCardToParent(card, FieldContainer); // 카드를 FieldArea로 이동
            card.SetActive(true); // 카드가 보이도록 활성화
            displayedField.Add(card); // 이동된 카드를 리스트에 추가
            AddCardToField(1, 1, "dd"); // 3x3 행렬의 (1,1)에 카드 추가
        }
    }

    // 카드 추가 함수
    private void AddCardToField(int row, int column, string card)
    {
        // 유효한 인덱스인지 확인
        if (row >= 0 && row < rows && column >= 0 && column < columns)
        {
            if (cardField[row, column] == null) // 해당 위치가 비어 있는 경우에만 추가
            {
                cardField[row, column] = card; // 3x3 행렬에 카드 추가
            }
        }
    }


}
