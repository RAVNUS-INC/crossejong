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
    public int Count = 0;
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

    public void SetCapableArea()
    {
        if (Count == 0)
        {
            i = 0;
            CreateCapableArea();
        }
        else
        {
            i = 1;
            CreateCapableArea();
        }
    }

    public void CreateCapableArea()
    {
        for (int j = 0; j < (n + i) * (n + i); j++)
        {
            GameObject emptyObject = new GameObject("");
            emptyList.Add(emptyObject);
            emptyObject.SetActive(false);
        }
    }

    public void FirstFieldCard()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1���� ���� ī�� ���
        int middleIndex = fieldList.Count / 2;
        MoveEmptyToFieldArea();
        fieldList[middleIndex] = randomCards[0];
    }

    private void Start()
    {
        SetCapableArea();
    }

    public void MoveEmptyToFieldArea()
    {
        cardPool.MoveCardsToTarGetArea(emptyList, fieldArea, fieldList);
    }
}
