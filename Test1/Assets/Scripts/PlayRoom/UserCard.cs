using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

public class UserCard : MonoBehaviour
{
    public CardPool cardPool; // CardPool 참조 
    public Transform userCardContainer; // UserCardArea의 Contents
    public List<GameObject> displayedCards;

    void Start()
    {

    }

    public void MoveRandomCards()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(11); // 11개의 랜덤 카드 얻기
        foreach (var card in randomCards)
        {
            cardPool.MoveCardToParent(card, userCardContainer); // 각 카드를 UserCardArea로 이동
            card.SetActive(true); // 카드가 보이도록 활성화
            displayedCards.Add(card); // 이동된 카드를 리스트에 추가
        }
    }

}