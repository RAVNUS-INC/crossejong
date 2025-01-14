using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

public class UserCard : MonoBehaviour
{
    public CardPool cardPool; // CardPool 참조 
    public Transform userCardContainer; // UserCardArea의 Contents
    public List<GameObject> displayedCards; // UserCardArea에서 보여지는 카드 리스트
    public CapableAreaPopup capableAreaPopup;

    void Start()
    {

    }

    // UserCardArea로 11개의 랜덤 카드 이동
    public void MoveUserCardArea()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(11); // 11개의 랜덤 카드 얻기
        foreach (var card in randomCards)
        {
            cardPool.MoveCardToParent(card, userCardContainer); // 각 카드를 UserCardArea로 이동
            card.SetActive(true); // 카드가 보이도록 활성화
            displayedCards.Add(card); // 이동된 카드를 리스트에 추가
        }
    }

    // FullPopup에서 카드를 복귀시키는 메서드
    public void MoveCardsBackToUserCardArea(List<GameObject> cardsToReturn)
    {
        foreach (var card in cardsToReturn)
        {
            cardPool.MoveCardToParent(card, userCardContainer); // 각 카드를 UserCardArea로 이동
            card.SetActive(true); // 카드가 보이도록 활성화
            displayedCards.Add(card); // UserCard의 리스트에 추가
        }
    }

    public void SelectedUserCard()
    {
        // 모든 카드에 대해 ChangeCardColor 연결
        foreach (var card in displayedCards)
        {
            Button cardButton = card.GetComponent<Button>();
            if (cardButton != null)
            {
                // 버튼 클릭 시 ChangeCardColor 함수 실행
                cardButton.onClick.AddListener(() => capableAreaPopup.CapableVisible(cardButton));
            }
        }
    }
}
