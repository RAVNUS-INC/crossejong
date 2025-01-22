using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Xml;
using PlayFab.DataModels;
using JetBrains.Annotations;
using System;
using Unity.Collections.LowLevel.Unsafe;

public class CapableArea : MonoBehaviour
{
    public FieldCard fieldCard;

    public void ShowCapableArea()
    {
        if (fieldCard != null) 
        {
            for (int i = 0; i < fieldCard.fieldDisplayedCards.Count; i++)
            {
                RectTransform rectTransform = fieldCard.fieldDisplayedCards[i].GetComponent<RectTransform>();
                
            }
        }
    }
}