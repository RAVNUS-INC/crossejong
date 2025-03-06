using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// CardDrop 클래스는 드래그 가능한 카드가 드롭될 수 있는 슬롯을 관리하는 스크립트
public class CardDrop : MonoBehaviourPun, IDropHandler
{
    public CardDrag cardDrag;
    public UserCard userCard;
    public FieldCard fieldCard;
    public CardPool cardPool;


    void Awake()
    {
        fieldCard = FindObjectOfType<FieldCard>();
        userCard = FindObjectOfType<UserCard>();
    }


    private bool HasCard()
    {
        if (transform.childCount > 0)
            return true;
        else
            return false;
    }

    private bool IsColorWhite()
    {
        Image image = transform.GetComponent<Image>();
        if(image.color == Color.white)
            return true;
        else
            return false;
    }

    public void OnDrop(PointerEventData eventData)
    {

        GameObject card = ObjectManager.instance.moveCardObejct;

        if (card != null && !HasCard() && IsColorWhite())
        {
            card.GetComponent<Image>().raycastTarget = true;
            card.transform.position = this.transform.position;
            card.transform.SetParent(transform);
            card.transform.parent.name = card.transform.name;

            fieldCard.fieldDisplayedCards.Add(card);
            userCard.displayedCards.Remove(card);


            // RectTransform 설정
            RectTransform rect = card.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);  // 중앙 정렬 (anchorMin)
            rect.anchorMax = new Vector2(0.5f, 0.5f);  // 중앙 정렬 (anchorMax)
            rect.pivot = new Vector2(0.5f, 0.5f);      // 중앙 정렬 (pivot)
            rect.anchoredPosition = Vector2.zero;      // 부모의 중심에 위치

            ObjectManager.instance.moveCardObejct = null;
            ObjectManager.instance.SortAfterMove();
            CardDrag CD = card.GetComponent<CardDrag>();
            if (CD != null) Destroy(card.GetComponent<CardDrag>());

            ObjectManager.instance.isDragged = true;

            ObjectManager.instance.createdWord = card.name;

            // 카드 놓인 그리드 위치 파악
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

            ObjectManager.instance.IsCardDrop = true;

            fieldCard.OnOffDropAreas(); // 여기에서 다른 모두가 놓은 카드를 그리드에 실시간으로 업데이트

            ObjectManager.instance.dropCount += 1;

        }
    }
}