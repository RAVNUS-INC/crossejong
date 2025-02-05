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

    public void CreateDropAreas()
    {
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
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
                ObjectManager.instance.grid[x,y] = empty;

            }
        }
    }

    private void ChangeColorAreas(int x, int y)
    {
        Image image = ObjectManager.instance.grid[x,y].GetComponent<Image>();
        if (image.color != Color.white)
        {
            image.color = Color.white;
        }
    }

    public void OnOffDropAreas()
    {
        for (int x = 0; x < 7; x++) 
        {
            for (int y = 0; y < 7; y++) 
            { 
                if (ObjectManager.instance.grid[x,y].transform.childCount == 1)
                {
                    ChangeColorAreas(x - 1, y);
                    ChangeColorAreas(x + 1, y);
                    ChangeColorAreas(x, y - 1);
                    ChangeColorAreas(x, y + 1);
                }
            }
        }
        
    }

    public void createdWordEnd()
    {
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                if (ObjectManager.instance.createdWord == ObjectManager.instance.grid[x, y].transform.name)
                {
                    ObjectManager.instance.cardIndexX = x;
                    ObjectManager.instance.cardIndexY = y;
                    Debug.Log(ObjectManager.instance.cardIndexX);
                    Debug.Log(ObjectManager.instance.cardIndexY);
                }
            }
        }

        if (ObjectManager.instance.grid[ObjectManager.instance.cardIndexX + 1, ObjectManager.instance.cardIndexY].transform.childCount == 1)
        {
            ObjectManager.instance.createdWords = ObjectManager.instance.createdWord + ObjectManager.instance.grid[ObjectManager.instance.cardIndexX + 1, ObjectManager.instance.cardIndexY].transform.name;
            Debug.Log(ObjectManager.instance.createdWords);
        }
        else if(ObjectManager.instance.grid[ObjectManager.instance.cardIndexX - 1, ObjectManager.instance.cardIndexY].transform.childCount == 1)
        {
            ObjectManager.instance.createdWords = ObjectManager.instance.grid[ObjectManager.instance.cardIndexX - 1, ObjectManager.instance.cardIndexY].transform.name + ObjectManager.instance.createdWord;
            Debug.Log(ObjectManager.instance.createdWords);
        }
        else if (ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY + 1].transform.childCount == 1)
        {
            ObjectManager.instance.createdWords = ObjectManager.instance.createdWord + ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY + 1].transform.name;
            Debug.Log(ObjectManager.instance.createdWords);
        }
        else if (ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY - 1].transform.childCount == 1)
        {
            ObjectManager.instance.createdWords = ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY - 1].transform.name + ObjectManager.instance.createdWord;
            Debug.Log(ObjectManager.instance.createdWords);
        }
        else
            Debug.Log("일치하지 않습니다");
    }

    public void FirstFieldCard()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1개의 랜덤 카드 얻기
        cardPool.GetCardsToTarGetArea(randomCards, fieldContainer, fieldDisplayedCards);

        GameObject firstCards = randomCards[0];
        firstCards.transform.SetParent(ObjectManager.instance.grid[3, 3].transform, false);
        ObjectManager.instance.grid[3,3] = firstCards;
        firstCards.transform.parent.name = firstCards.transform.name;

        OnOffDropAreas();
    }

}
