using System.Collections.Generic;
using UnityEngine;

public class GetCard : MonoBehaviour
{
    public CardPool cardPool;
    public UserCard userCard;
    public void GetCardToUserCard()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1개의 랜덤 카드 얻기
        cardPool.GetCardsToTarGetArea(randomCards, userCard.userCardContainer, userCard.displayedCards);
    }

}
