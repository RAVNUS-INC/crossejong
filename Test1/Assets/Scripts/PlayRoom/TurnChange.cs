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
    public WordLists wordLists;

    public void IsCreateWord()
    {
        wordInput = cardInputField.text;

        // 자음이 포함되어 있는지 확인
        if (wordInput.Contains("ㄱ") || wordInput.Contains("ㅎ") || wordInput.Contains("ㅇ"))
        {
            List<string> possibleWords = GeneratePossibleWords(wordInput);

            // 생성된 가능한 단어 중 일치하는 게 있는지 확인
            foreach (string word in possibleWords)
            {
                if (word == ObjectManager.instance.createdWords)
                {
                    Debug.Log("일치합니다");
                    return;
                }
            }
            Debug.Log("일치하는 단어가 없습니다.");
        }
        else
        {
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
            {
                Debug.Log("오류입니다");
            }
        }
    }

    // 가능한 단어 리스트 생성
    public List<string> GeneratePossibleWords(string input)
    {
        List<string> possibleWords = new List<string>();

        // WordLists의 딕셔너리 가져오기
        Dictionary<char, List<char>> choMap = wordLists.choDictionary;

        // 자음 포함 단어에 대해 가능한 글자 생성
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            // 초성인지 확인
            if (choMap.ContainsKey(c))
            {
                // 해당 초성을 대체할 수 있는 모든 글자를 가져옴
                List<char> possibleChars = choMap[c];
                foreach (char pc in possibleChars)
                {
                    string newWord = input.Substring(0, i) + pc + input.Substring(i + 1);
                    possibleWords.Add(newWord);
                }
            }
        }
        return possibleWords;
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
