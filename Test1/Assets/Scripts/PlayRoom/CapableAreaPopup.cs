using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;


public class CapableAreaPopup : MonoBehaviour
{
    public UserCard userCard;
    public FieldCard fieldCard;
    public CardPool cardPool;
    public Transform capableContainer;
    public List<GameObject> capableDisplayedCards;


    // Start is called before the first frame update
    void Start()
    {

    }

    public void MoveCardsToCapableArea()
    {
        cardPool.MoveCardsToTarGetArea(fieldCard.fieldDisplayedCards, capableContainer, capableDisplayedCards);
    }

    public void MoveCardsToFieldArea()
    {
        cardPool.MoveCardsToTarGetArea(capableDisplayedCards, fieldCard.fieldContainer, fieldCard.fieldDisplayedCards);
    }


}
