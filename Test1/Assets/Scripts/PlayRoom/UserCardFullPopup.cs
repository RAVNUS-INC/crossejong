using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

public class UserCardFullPopup : MonoBehaviour
{
    public CardPool cardPool; // CardPool 참조 
    public UserCard userCard; // UserCard 참조
    public Transform fullCardContainer; // FullPopupArea의 Content
    public List<GameObject> fullDisplayedCards; // FullPopup에서 보여지는 카드 리스트


    public void MoveCardsToFullPopupArea()
    {
        cardPool.MoveCardsToTarGetArea(userCard.displayedCards, fullCardContainer, fullDisplayedCards);
    }
    
    public void MoveCardsToUserCardArea()
    {
        cardPool.MoveCardsToTarGetArea(fullDisplayedCards, userCard.userCardContainer, userCard.displayedCards);
    }
}
