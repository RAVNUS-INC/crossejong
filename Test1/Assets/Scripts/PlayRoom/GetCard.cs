using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using TMPro;
using Photon.Pun.Demo.PunBasics;


public class GetCard : MonoBehaviour
{
    public CardPool cardPool;
    public UserCard userCard;
    public CardDrag cardDrag;
    public Button getCardButton;

    public void GetCardToUserCard()
    {
        if (cardPool.cards.Count > 0)
        {
            List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1개의 랜덤 카드 얻기
            cardPool.GetCardsToTarGetArea(randomCards, userCard.userCardContainer, userCard.displayedCards);

            cardDrag = randomCards[0].GetComponent<CardDrag>();
            if (cardDrag == null)
            {
                cardDrag = randomCards[0].AddComponent<CardDrag>(); // CardDrag 컴포넌트 추가
                cardDrag.onDragParent = ObjectManager.instance.moveItemPanel;
            }
        }
        else
        {
            getCardButton.interactable = false;
        }
    }

}
