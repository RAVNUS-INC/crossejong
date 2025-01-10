using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

public class UserCard : MonoBehaviour
{
    public NewCardPool newCardPool; // CardPool 참조 
    public Transform userCardContainer; // UserCardArea의 Contents
    public List<GameObject> displayedCards; // UserCardArea에서 보여지는 카드 리스트

    void Start()
    {
        // Optional: 초기화 작업
    }

    // UserCardArea로 11개의 랜덤 카드 이동
    public void MoveUserCardArea()
    {
        List<GameObject> randomCards = newCardPool.GetRandomCards(11); // 11개의 랜덤 카드 얻기
        foreach (var card in randomCards)
        {
            newCardPool.MoveCardToParent(card, userCardContainer); // 각 카드를 UserCardArea로 이동
            card.SetActive(true); // 카드가 보이도록 활성화
            displayedCards.Add(card); // 이동된 카드를 리스트에 추가
        }
    }

    // FullPopup에서 카드를 복귀시키는 메서드
    public void MoveCardsBackToUserCardArea(List<GameObject> cardsToReturn)
    {
        foreach (var card in cardsToReturn)
        {
            newCardPool.MoveCardToParent(card, userCardContainer); // 각 카드를 UserCardArea로 이동
            card.SetActive(true); // 카드가 보이도록 활성화
            displayedCards.Add(card); // UserCard의 리스트에 추가
        }
    }
}
