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
    public void FirstUserCardArea()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(11); // 11개의 랜덤 카드 얻기
        cardPool.MoveCardsToTarGetArea(randomCards, userCardContainer, displayedCards);
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
                cardButton.onClick.AddListener(() => capableAreaPopup.MoveCardsToCapableArea());
            }
        }
    }
}
