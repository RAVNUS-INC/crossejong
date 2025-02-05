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

public class FieldCard : MonoBehaviour
//-----------(서버 연결 시 주석해제)------------
    // MonoBehaviourPun
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

    // -----------(서버 연결 시 주석해제)------------
    // [PunRPC]
    public void FirstFieldCard()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1개의 랜덤 카드 얻기
        cardPool.GetCardsToTarGetArea(randomCards, fieldContainer, fieldDisplayedCards);

        GameObject middleObejcts = ObjectManager.instance.grid[3, 3];
        GameObject firstCards = randomCards[0];
        ObjectManager.instance.grid[3, 3].SetActive(true);
        firstCards.transform.SetParent(middleObejcts.transform, false);
        ObjectManager.instance.grid[3, 3] = firstCards;

        OnOffDropAreas();


        ////-----------(서버 연결 시 주석해제)------------
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    // 서버(방장)에서 랜덤 카드 생성
        //    // GameObject는 직접적으로 RPC로 전달할 수 없음. string으로 대체
        //    List<GameObject> randomCards = cardPool.GetRandomCards(1);

        //    // 다른 유저들에게 동기화를 해야하는데 gameobject객체를 넘겨줄 수 없음..
        //    cardPool.GetCardsToTarGetArea(randomCards, fieldContainer, fieldDisplayedCards);
        //    GameObject middleObejcts = ObjectManager.instance.grid[3, 3];
        //    GameObject firstCards = randomCards[0];
        //    ObjectManager.instance.grid[3, 3].SetActive(true);
        //    firstCards.transform.SetParent(middleObejcts.transform, false);
        //    ObjectManager.instance.grid[3, 3] = firstCards;

        //    photonView.RPC("SyncFirstFieldCard", RpcTarget.All, selectedCard.name);  // 카드 이름을 전송
        //}
    }

}
