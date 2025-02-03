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
    public Transform emptyArea;
    public int gridCount = 7;

    public void CreateDropArea()
    {
        for (int i = 0; i < gridCount*gridCount; i++)
        {
            GameObject empty = new GameObject("");
            empty.transform.SetParent(fieldContainer, false);
            RectTransform rect = empty.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 200);
            Image img = empty.AddComponent<Image>();
            img.color = Color.white;
            empty.AddComponent<CardDrop>();
            ObjectManager.instance.emptyList.Add(empty);
            Image image = empty.GetComponent<Image>();
            image.color = Color.clear;
        }
    }

    private void ChangeColor(int i)
    {
        Image image = ObjectManager.instance.emptyList[i].GetComponent<Image>();
        image.color = Color.white;
    }

    public void OnOffDropArea()
    {
        for (int j = 0; j < ObjectManager.instance.emptyList.Count; j++)
        {
            if (ObjectManager.instance.emptyList[j].transform.childCount == 1)
            {
                ChangeColor(j - 1);
                ChangeColor(j + 1);
            }
        }
    }

    public void FirstFieldCard()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1개의 랜덤 카드 얻기
        cardPool.GetCardsToTarGetArea(randomCards, fieldContainer, fieldDisplayedCards);

        int middleIndex = ObjectManager.instance.emptyList.Count / 2;

        GameObject middleObject = ObjectManager.instance.emptyList[middleIndex];
        GameObject firstCard = randomCards[0];

        ObjectManager.instance.emptyList[middleIndex].SetActive(true);
        firstCard.transform.SetParent(middleObject.transform, false);
        ObjectManager.instance.emptyList[middleIndex] = firstCard;
    }

}
