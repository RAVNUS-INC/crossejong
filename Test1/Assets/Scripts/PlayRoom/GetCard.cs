using System.Collections.Generic;
using UnityEngine;

public class GetCard : MonoBehaviour
{
    public CardPool cardPool;
    public UserCard userCard;
    public void GetCardToUserCard()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1���� ���� ī�� ���
        cardPool.GetCardsToTarGetArea(randomCards, userCard.userCardContainer, userCard.displayedCards);
    }

}
