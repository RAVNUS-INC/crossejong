using ExitGames.Client.Photon.StructWrapping;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// CardDrop 클래스는 드래그 가능한 카드가 드롭될 수 있는 슬롯을 관리하는 스크립트
public class CardDrop : MonoBehaviour, IDropHandler
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

            ObjectManager.instance.movedCardPosition = card.transform.position;
            ObjectManager.instance.isDragged = true;
            Debug.Log(ObjectManager.instance.movedCardPosition);

            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    if (card == ObjectManager.instance.grid[x, y])
                    {
                        ObjectManager.instance.cardIndexX = x;
                        ObjectManager.instance.cardIndexY = y;
                        Debug.Log(ObjectManager.instance.cardIndexX);
                        Debug.Log(ObjectManager.instance.cardIndexY);
                    }
                    else
                        Debug.Log("일치하지 않습니다");
                }
            }
            fieldCard.OnOffDropAreas();
        }
        else
            Debug.Log("하얀색이 아닙니다");
    }
}