using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using TMPro;

public class TurnChange : MonoBehaviour
{
    public UserCard userCard;
    public TMP_Text userCardCount; // TextMeshPro 사용
    private bool isMyTurn = true;
    public InputField cardInputField;
    public Text resultText;

    // Start is called before the first frame update
    public void UserTurnChange()
    {
        if (isMyTurn)
        {
            CountUserCard(userCard.displayedCards.Count);
        }

    }

    public void CountUserCard(int count)
    {
        userCardCount.text = count.ToString(); // TMP_Text로 설정
    }

    public void TurnEnd()
    {
        string cardInput = cardInputField.text;

    }

    public void CreateWord()
    {

    }
}
