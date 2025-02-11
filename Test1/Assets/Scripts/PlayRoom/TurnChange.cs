using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using TMPro;
using JetBrains.Annotations;
using System;

public class TurnChange : MonoBehaviour
{
    public UserCard userCard;
    public TMP_Text userCardCount; // TextMeshPro 사용
    public TMP_InputField cardInputField;
    public string wordInput;
    public Text resultText;
    public List<char> wordLists1 = new List<char>();
    public List<char> wordLists2 = new List<char>();
    public List<char> wordLists3 = new List<char>();

    // "가"의 문자코드 44032에 587을 더하면 "깋"이 나옴
    // "아"의 문자코드 50500
    // "하"의 문자코드 54616

    private void Start()
    {
        WordList(44032, wordLists1);
        WordList(50500, wordLists2);
        WordList(54616, wordLists3);
    }

    public void WordList(int x, List<char> wordList)
    {
        List<int> list = new List<int>();

        for (int i = 1; i <= 21; i++)
        {
            for (int j = 1; j <= 28; j++)
            {
                list.Add(x);
                x++;
            }
        }

        for (int i = 0; i <list.Count; i++)
        {
            wordList.Add((char)list[i]);
        }
    }

    public void IsCreateWord()
    {
        Debug.Log(ObjectManager.instance.dropCount);
        wordInput = cardInputField.text;

        if (wordInput.Contains("ㄱ"))
        {

        }

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

    public void TurnEnd()
    {
        ObjectManager.instance.dropCount = 0;

        CountUserCard(userCard.displayedCards.Count);
    }

    public void CountUserCard(int count)
    {
        userCardCount.text = count.ToString(); // TMP_Text로 설정
    }

}
