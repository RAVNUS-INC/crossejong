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
    public List<GameObject> fieldDisplayedCards;
    //public int i = 0;
    //public int n = 3;
    public List<GameObject> fieldList;
    public List<GameObject> emptyList;
    public Transform fieldArea;
    public Transform emptyArea;

    //public void CreateCapableArea()
    //{
    //    for (int j = 0; j < (n + i) * (n + i); j++)
    //    {
    //        GameObject empty = new GameObject("");
    //        fieldList.Add(empty);
    //        empty.transform.SetParent(fieldArea, false);
    //        RectTransform RT = empty.AddComponent<RectTransform>();
    //        RT.sizeDelta = new Vector2(200, 200);
    //        Image img = empty.AddComponent<Image>();
    //        img.color = Color.clear;
    //        empty.AddComponent<CardDrop>();
    //    }
    //}

    public void CreateCapableArea()
    {
        for (int i = 0; i < fieldDisplayedCards.Count; i++)
        {

        }
    }

    public void EmptyPosition()
    {

    }

    public void CreateDropArea()
    {
        List<GameObject> emptyList = new List<GameObject>();
        int n = emptyList.Count;
        Vector2[] emptyPosition = new Vector2[]
            {
                new Vector2(0, 200),
                new Vector2(-200, 0),
                new Vector2(0, -200),
                new Vector2(200, 0)
            };

        for (int i = 0; i < 4; i++)
        {
            GameObject empty = new GameObject("");
            empty.transform.SetParent(fieldArea, false);
            RectTransform rect = empty.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 200);
            rect.position = emptyPosition[i];
            Image img = empty.AddComponent<Image>();
            img.color = Color.white;
            empty.AddComponent<CardDrop>();
        }
    }

    public void FirstFieldCard()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1���� ���� ī�� ���

        RectTransform rect = randomCards[0].GetComponent<RectTransform>();
        rect.position = new Vector2(0, 0);

        fieldDisplayedCards.Add(randomCards[0]);


        //int middleIndex = fieldList.Count / 2;

        //GameObject middleObject = fieldList[middleIndex];
        //GameObject firstCard = randomCards[0];

        //firstCard.transform.SetParent(middleObject.transform, false);
        //fieldList[middleIndex] = firstCard;
    }

}
