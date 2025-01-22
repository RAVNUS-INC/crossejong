using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Xml;
using PlayFab.DataModels;
using JetBrains.Annotations;
using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using Unity.VisualScripting;

public class FieldCard : MonoBehaviour
{
    public Transform fieldContainer; // FieldArea의 Contents
    public CardPool cardPool; // CardPool 참조
    public List<string> shownCardData; // 필드에 놓인 카드 데이터 리스트
    public List<GameObject> fieldDisplayedCards;
    public int i;
    public int n = 3;
    public List<GameObject> fieldList;
    public List<GameObject> emptyList;
    public Transform fieldArea;
    public Transform emptyArea;


    public void StartCardShown()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1개의 랜덤 카드 얻기
        cardPool.MoveCardsToTarGetArea(randomCards, fieldContainer, fieldDisplayedCards);

        RectTransform rectTransform = randomCards[0].GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void CreateCapableArea()
    {
        for (int j = 0; j < (n + i) * (n + i); j++)
        {
            GameObject empty = new GameObject("");
            fieldList.Add(empty );
            empty.transform.SetParent(fieldArea, false);
        }
    }

    public void FirstFieldCard()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1개의 랜덤 카드 얻기
        int middleIndex = fieldList.Count / 2;

        GameObject middleObject = fieldList[middleIndex];
        GameObject firstCard = randomCards[0];

        firstCard.transform.SetParent(middleObject.transform, false);
        fieldList[middleIndex] = firstCard;
    }

    private void Start()
    {
        CreateCapableArea();
    }
}
