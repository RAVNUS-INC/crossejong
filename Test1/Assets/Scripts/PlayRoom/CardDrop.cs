using ExitGames.Client.Photon.StructWrapping;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// CardDrop Ŭ������ �巡�� ������ ī�尡 ��ӵ� �� �ִ� ������ �����ϴ� ��ũ��Ʈ
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

    public void OnDrop(PointerEventData eventData)
    {
        GameObject card = ObjectManager.instance.moveCardObejct;

        if (card != null && !HasCard())
        {
            card.GetComponent<Image>().raycastTarget = true;
            card.transform.position = this.transform.position;
            card.transform.SetParent(transform);


            fieldCard.fieldDisplayedCards.Add(card);
            userCard.displayedCards.Remove(card);


            // RectTransform ����
            RectTransform rect = card.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);  // �߾� ���� (anchorMin)
            rect.anchorMax = new Vector2(0.5f, 0.5f);  // �߾� ���� (anchorMax)
            rect.pivot = new Vector2(0.5f, 0.5f);      // �߾� ���� (pivot)
            rect.anchoredPosition = Vector2.zero;      // �θ��� �߽ɿ� ��ġ

            ObjectManager.instance.moveCardObejct = null;
            ObjectManager.instance.SortAfterMove();
            CardDrag CD = card.GetComponent<CardDrag>();
            if (CD != null) Destroy(card.GetComponent<CardDrag>());

            ObjectManager.instance.movedCardPosition = card.transform.position;
            ObjectManager.instance.isDragged = true;
            Debug.Log(ObjectManager.instance.movedCardPosition);
        }
    }
}