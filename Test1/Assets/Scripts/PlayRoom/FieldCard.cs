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
    public Transform fieldContainer; // FieldArea�� Contents
    public CardPool cardPool; // CardPool ����
    public List<string> shownCardData; // �ʵ忡 ���� ī�� ������ ����Ʈ
    public List<GameObject> fieldDisplayedCards;
    public int i;
    public int n = 3;
    public List<GameObject> fieldList;
    public List<GameObject> emptyList;
    public Transform fieldArea;
    public Transform emptyArea;


    public void StartCardShown()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1���� ���� ī�� ���
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
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1���� ���� ī�� ���
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
