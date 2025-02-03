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
using System.Xml.Linq;

public class FieldCard : MonoBehaviour
{
    public Transform fieldContainer; // FieldArea의 Contents
    public CardPool cardPool; // CardPool 참조
    public List<GameObject> fieldDisplayedCards;
    public List<GameObject> fieldList;
    public List<GameObject> emptyList;
    public Transform fieldArea;
    public Transform emptyArea;

    public void CreateDropArea()
    {
        for (int i = 0; i < 11*11; i++)
        {
            GameObject empty = new GameObject("");
            empty.transform.SetParent(fieldArea, false);
            RectTransform rect = empty.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 200);
            Image img = empty.AddComponent<Image>();
            img.color = Color.white;
            empty.AddComponent<CardDrop>();
            fieldList.Add(empty);
        }
    }

    public void FirstFieldCard()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1개의 랜덤 카드 얻기
        cardPool.GetCardsToTarGetArea(randomCards, fieldContainer, fieldDisplayedCards);

        int middleIndex = fieldList.Count / 2;

        GameObject middleObject = fieldList[middleIndex];
        GameObject firstCard = randomCards[0];

        firstCard.transform.SetParent(middleObject.transform, false);
        fieldList[middleIndex] = firstCard;
    }

}
