using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;

public class UserCardFullPopup : MonoBehaviour
{
    public static UserCardFullPopup instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public CardPool cardPool; // CardPool ���� 
    public UserCard userCard; // UserCard ����
    public Transform fullCardContainer; // FullPopupArea�� Content
    public List<GameObject> fullDisplayedCards; // FullPopup���� �������� ī�� ����Ʈ



    public void MoveCardsToFullPopupArea() 
    {
        cardPool.MoveCardsToTarGetArea(userCard.displayedCards, fullCardContainer, fullDisplayedCards);

       // userCard.SelectedUserCard(fullDisplayedCards);
    }
    
    public void MoveCardsToUserCardArea()
    {
        cardPool.MoveCardsToTarGetArea(fullDisplayedCards, userCard.userCardContainer, userCard.displayedCards);

       // userCard.SelectedUserCard(userCard.displayedCards);
    }

}
