using System.Collections.Generic;
using UnityEngine;

public class GetCard : MonoBehaviour
{
    public CardPool cardPool;
    public UserCard userCard;
    public void GetCardToUserCard()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 11���� ���� ī�� ���
        cardPool.MoveCardsToTarGetArea(randomCards, userCard.userCardContainer, userCard.displayedCards);
    }

}
