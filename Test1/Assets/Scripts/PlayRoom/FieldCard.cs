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

public class FieldCard : //MonoBehaviour
//서버 연결 시 주석 해제------------------------------------
MonoBehaviourPun
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

    //public void FirstFieldCard()
    //{
    //    List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1개의 랜덤 카드 얻기
    //    cardPool.GetCardsToTarGetArea(randomCards, fieldContainer, fieldDisplayedCards);

    //    GameObject middleObejcts = ObjectManager.instance.grid[3, 3];
    //    GameObject firstCards = randomCards[0];
    //    ObjectManager.instance.grid[3, 3].SetActive(true);
    //    firstCards.transform.SetParent(middleObejcts.transform, false);
    //    ObjectManager.instance.grid[3, 3] = firstCards;

    //    OnOffDropAreas();
    //}



    //서버 연결 시 주석 해제------------------------------------
    public void FirstFieldCard()
    {
        // 방장만 처음 1장의 카드 이름을 뽑음
        string[] randomCardNames = cardPool.GetRandomCardsName(1); // 이름을 받는 함수로 변경

        // 모든 플레이어들에게 인덱스리스트를 넘겨 첫 카드 오브젝트를 생성하도록 요청(배열->문자열)
        photonView.RPC("FirstFieldCardRequestAll", RpcTarget.All, string.Join(",", randomCardNames));
    }

    //방장 포함 모두가 첫 카드 추가를 수행하는 함수
    [PunRPC]
    public void FirstFieldCardRequestAll(string names)
    {
        string[] usedNames = names.Split(','); // 다시 배열로 변환
        foreach (string i in usedNames)
        {
            Debug.Log($"첫 카드 '{i}' 받음");
        }
        List<GameObject> randomCards = cardPool.GetRandomCardsObject(usedNames);

        cardPool.GetCardsToTarGetArea(randomCards, fieldContainer, fieldDisplayedCards);
        GameObject middleObejcts = ObjectManager.instance.grid[3, 3];
        GameObject firstCards = randomCards[0];
        ObjectManager.instance.grid[3, 3].SetActive(true);
        firstCards.transform.SetParent(middleObejcts.transform, false);
        ObjectManager.instance.grid[3, 3] = firstCards;

        OnOffDropAreas();
    }
    //서버 연결 시 주석 해제------------------------------------
}
