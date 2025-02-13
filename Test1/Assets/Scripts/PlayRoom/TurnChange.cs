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
    public TMP_Text userCardCount; // TextMeshPro ���
    public TMP_InputField cardInputField;
    public string wordInput;
    public WordLists wordLists;

    public void IsCreateWord()
    {
        wordInput = cardInputField.text;

        // ������ ���ԵǾ� �ִ��� Ȯ��
        if (wordInput.Contains("��") || wordInput.Contains("��") || wordInput.Contains("��"))
        {
            List<string> possibleWords = GeneratePossibleWords(wordInput);

            // ������ ������ �ܾ� �� ��ġ�ϴ� �� �ִ��� Ȯ��
            foreach (string word in possibleWords)
            {
                if (word == ObjectManager.instance.createdWords)
                {
                    Debug.Log("��ġ�մϴ�");
                    return;
                }
            }
            Debug.Log("��ġ�ϴ� �ܾ �����ϴ�.");
        }
        else
        {
            if (wordInput.Length > ObjectManager.instance.dropCount)
            {
                if (wordInput == ObjectManager.instance.createdWords)
                {
                    Debug.Log("��ġ�մϴ�");
                }
                else
                {
                    if (ObjectManager.instance.createdWords.Contains(wordInput))
                    {
                        Debug.Log("�ֽ��ϴ�");
                    }
                    else
                        Debug.Log("�����ʽ��ϴ�");
                }
            }
            else
            {
                Debug.Log("�����Դϴ�");
            }
        }
    }

    // ������ �ܾ� ����Ʈ ����
    public List<string> GeneratePossibleWords(string input)
    {
        List<string> possibleWords = new List<string>();

        // WordLists�� ��ųʸ� ��������
        Dictionary<char, List<char>> choMap = wordLists.choDictionary;

        // ���� ���� �ܾ ���� ������ ���� ����
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            // �ʼ����� Ȯ��
            if (choMap.ContainsKey(c))
            {
                // �ش� �ʼ��� ��ü�� �� �ִ� ��� ���ڸ� ������
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
        userCardCount.text = count.ToString(); // TMP_Text�� ����
    }

}
