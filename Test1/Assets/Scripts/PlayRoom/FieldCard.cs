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
using Photon.Pun;
using System.Reflection;
using System.Linq;

public class FieldCard : MonoBehaviour
//서버 연결 시 주석 해제------------------------------------
//MonoBehaviourPun
{
    public Transform fieldContainer; // FieldArea의 Contents
    public CardPool cardPool; // CardPool 참조
    public List<GameObject> fieldDisplayedCards;
    public Transform emptyArea;
    private bool isRight;
    private bool isLeft;
    private bool isTop;
    private bool isBottom;


    public void CreateDropAreas()
    {
        ObjectManager.instance.grid = new GameObject[ObjectManager.instance.gridCount, ObjectManager.instance.gridCount];
        for (int x = 0; x < ObjectManager.instance.gridCount; x++)
        {
            for (int y = 0; y < ObjectManager.instance.gridCount; y++)
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
                ObjectManager.instance.grid[x, y] = empty;

            }
        }
    }

    private void ChangeColorAreas(int x, int y)
    {
        Image image = ObjectManager.instance.grid[x, y].GetComponent<Image>();
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
                if (ObjectManager.instance.grid[x, y].transform.childCount == 1)
                {
                    ChangeColorAreas(x - 1, y);
                    ChangeColorAreas(x + 1, y);
                    ChangeColorAreas(x, y - 1);
                    ChangeColorAreas(x, y + 1);
                }
            }
        }

    }

    private void IsRight()
    {
        if (ObjectManager.instance.grid[ObjectManager.instance.cardIndexX + 1, ObjectManager.instance.cardIndexY].transform.childCount == 1)
            isRight = true;
        else
            isRight = false;
    }

    private void IsLeft()
    {
        if (ObjectManager.instance.grid[ObjectManager.instance.cardIndexX - 1, ObjectManager.instance.cardIndexY].transform.childCount == 1)
            isLeft = true;
        else
            isLeft = false;
    }
    private void IsBottom()
    {
        if (ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY + 1].transform.childCount == 1)
            isBottom = true;
        else
            isBottom = false;
    }
    private void IsTop()
    {
        if (ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY - 1].transform.childCount == 1)
            isTop = true;
        else
            isTop = false;
    }

    private void IsPosition()
    {
        IsLeft();
        IsRight();
        IsBottom();
        IsTop();
    }


    public void createdWordEnd()
    {
        for (int x = 0; x < ObjectManager.instance.gridCount; x++)
        {
            for (int y = 0; y < ObjectManager.instance.gridCount; y++)
            {
                if (ObjectManager.instance.createdWord == ObjectManager.instance.grid[x, y].transform.name)
                {
                    ObjectManager.instance.cardIndexX = x;
                    ObjectManager.instance.cardIndexY = y;
                }
            }
        }

        IsPosition();

        ObjectManager.instance.createdWords = "";

        if (isLeft)  // 왼쪽에 글자가 있을 때
        {
            int x = 0;
            for (int i = 1; ObjectManager.instance.grid[ObjectManager.instance.cardIndexX - i, ObjectManager.instance.cardIndexY].transform.childCount == 1; i++)
            {
                x = i;
            }
            for (int i = 0; ObjectManager.instance.grid[ObjectManager.instance.cardIndexX - x + i, ObjectManager.instance.cardIndexY].transform.childCount == 1; i++)
            {
                ObjectManager.instance.createdWords += ObjectManager.instance.grid[ObjectManager.instance.cardIndexX - x + i, ObjectManager.instance.cardIndexY].transform.name;
            }
            isLeft = false;
            isRight = false;
        }

        if (isRight)  // 오른쪽에 글자가 있을 때
        {
            for (int i = 0; ObjectManager.instance.grid[ObjectManager.instance.cardIndexX + i, ObjectManager.instance.cardIndexY].transform.childCount == 1; i++)
            {
                ObjectManager.instance.createdWords += ObjectManager.instance.grid[ObjectManager.instance.cardIndexX + i, ObjectManager.instance.cardIndexY].transform.name;
            }
        }

        if (isTop)  // 위에 글자가 있을 때
        {
            int y = 0;
            for (int i = 1; ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY - i].transform.childCount == 1; i++)
            {
                y = i;
            }
            for (int i = 0; ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY - y + i].transform.childCount == 1; i++)
            {
                ObjectManager.instance.createdWords += ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY - y + i].transform.name;
            }
            isTop = false;
            isBottom = false;
        }

        if (isBottom)  // 아래에 글자가 있을 때
        {
            for (int i = 0; ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY + i].transform.childCount == 1; i++)
            {
                ObjectManager.instance.createdWords += ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY + i].transform.name;
            }
        }

        Debug.Log(ObjectManager.instance.createdWords);

    }

    public void FirstFieldCard()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1개의 랜덤 카드 얻기
        cardPool.GetCardsToTarGetArea(randomCards, fieldContainer, fieldDisplayedCards);

        GameObject firstCards = randomCards[0];

        firstCards.transform.SetParent(ObjectManager.instance.grid[ObjectManager.instance.gridCount / 2, ObjectManager.instance.gridCount / 2].transform, false);
        ObjectManager.instance.grid[ObjectManager.instance.gridCount / 2, ObjectManager.instance.gridCount / 2] = firstCards;
        firstCards.transform.parent.name = firstCards.transform.name;

        OnOffDropAreas();
    }



    //서버 연결 시 주석 해제------------------------------------
    //public void FirstFieldCard()
    //{
    //    // 방장만 처음 1장의 카드 이름을 뽑음
    //    string[] randomCardNames = cardPool.GetRandomCardsName(1); // 이름을 받는 함수로 변경

    //    // 모든 플레이어들에게 인덱스리스트를 넘겨 첫 카드 오브젝트를 생성하도록 요청(배열->문자열)
    //    photonView.RPC("FirstFieldCardRequestAll", RpcTarget.All, string.Join(",", randomCardNames));
    //}

    ////방장 포함 모두가 첫 카드 추가를 수행하는 함수
    //[PunRPC]
    //public void FirstFieldCardRequestAll(string names)
    //{
    //    string[] usedNames = names.Split(','); // 다시 배열로 변환
    //    foreach (string i in usedNames)
    //    {
    //        Debug.Log($"첫 카드 '{i}' 받음");
    //    }
    //    List<GameObject> randomCards = cardPool.GetRandomCardsObject(usedNames);

    //    cardPool.GetCardsToTarGetArea(randomCards, fieldContainer, fieldDisplayedCards);
    //    GameObject middleObejcts = ObjectManager.instance.grid[3, 3];
    //    GameObject firstCards = randomCards[0];
    //    ObjectManager.instance.grid[3, 3].SetActive(true);
    //    firstCards.transform.SetParent(middleObejcts.transform, false);
    //    ObjectManager.instance.grid[3, 3] = firstCards;

    //    OnOffDropAreas();
    //}
    //서버 연결 시 주석 해제------------------------------------
}
