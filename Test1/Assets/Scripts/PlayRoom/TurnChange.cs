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
    public TMP_InputField cardInputField;
    public string wordInput;
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


    }

    public void IsCreateWord()
    {
        Debug.Log(ObjectManager.instance.dropCount);
        wordInput = cardInputField.text;
        if (wordInput.Length > ObjectManager.instance.dropCount)
        {
            if (wordInput == ObjectManager.instance.createdWords)
            {
                Debug.Log("일치합니다");
            }
            else
            {
                if (ObjectManager.instance.createdWords.Contains(wordInput))
                {
                    Debug.Log("있습니다");
                }
                else
                    Debug.Log("있지않습니다");
            }
        }
        else
            Debug.Log("오류입니다");
    }
}
