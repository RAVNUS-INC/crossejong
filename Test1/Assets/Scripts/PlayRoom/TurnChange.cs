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
    public bool isCharWord;
    public bool isWord;
    public bool isSpecialWord;
    public WordLists wordLists;

    public List<char> charList = new List<char>
    {'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'};





    public void IsCreateWord()
    {
        Debug.Log(ObjectManager.instance.dropCount);
        wordInput = cardInputField.text;
        isCharWord = false;
        isWord = false;
        isSpecialWord = false;

        for (int i = 0; i < ObjectManager.instance.createdWords.Length; i++) 
        {
            for (int j = 0; j < 19; j++)
            {
                if (ObjectManager.instance.createdWords[i] == charList[j])
                {
                    List<char> words = wordLists.choDictionary[charList[j]];
                    for (int k = 0; k < 587; k++)
                    {
                        if (wordInput[i] == words[k])
                        {
                            Debug.Log("초성이 일치합니다");
                            isCharWord = true;
                        }
                    }
                }
                else
                {
                    Debug.Log("자음카드가 일치하지 않습니다");
                    isCharWord = false;
                }

                if (ObjectManager.instance.createdWords[i] == '*')
                {
                    if (44032 <= wordInput[i] && wordInput[i] <= 54616)
                    {
                        Debug.Log("특수카드가 일치합니다");
                        isSpecialWord = true; 
                    }
                }
                else
                {
                    Debug.Log("특수카드가 일치하지 않습니다");
                    isSpecialWord = false;
                }
            }
        }


        if (wordInput.Length > ObjectManager.instance.dropCount)
        {
            if (wordInput == ObjectManager.instance.createdWords)
            {
                Debug.Log("일치합니다");
                isWord = true;
            }
            else
            {
                if (ObjectManager.instance.createdWords.Contains(wordInput))
                {
                    Debug.Log("있습니다");
                    isWord = true;
                }
                else
                {
                    Debug.Log("있지않습니다");
                    isWord = false;
                }
            }
        }
        else
        {
            Debug.Log("오류입니다");
            isWord = false;
        }

        if (isCharWord && isWord && isSpecialWord) 
        {
            Debug.Log("사전 API 검사를 시작합니다");
        }
        
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