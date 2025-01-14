using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

public class UserCardFullPopup : MonoBehaviour
{
    public CardPool cardPool; // CardPool ���� 
    public UserCard userCard; // UserCard ����
    public Transform fullCardContainer; // FullPopupArea�� Content
    public List<GameObject> fullDisplayedCards; // FullPopup���� �������� ī�� ����Ʈ
    public List<GameObject> rowDisplayedCards;
    public int fullPopupRow;

    void Start()
    {
        // Optional: �ʱ�ȭ �۾�
    }

    public void MoveCardsToFullPopupArea()
    {
        fullPopupRow = fullDisplayedCards.Count / 5 + 1;
        cardPool.MoveCardsToTarGetArea(userCard.displayedCards, fullCardContainer, fullDisplayedCards);
    }
    
    public void MoveCardsToUserCardArea()
    {
        cardPool.MoveCardsToTarGetArea(fullDisplayedCards, userCard.userCardContainer, userCard.displayedCards);
    }
}
