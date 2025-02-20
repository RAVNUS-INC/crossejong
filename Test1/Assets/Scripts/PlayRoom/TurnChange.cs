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
    public bool isCharWord;
    public bool isWord;
    public bool isSpecialWord;
    public WordLists wordLists;

    public List<char> charList = new List<char>
    {'��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��'};





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
                            Debug.Log("�ʼ��� ��ġ�մϴ�");
                            isCharWord = true;
                        }
                    }
                }
                else
                {
                    Debug.Log("����ī�尡 ��ġ���� �ʽ��ϴ�");
                    isCharWord = false;
                }

                if (ObjectManager.instance.createdWords[i] == '*')
                {
                    if (44032 <= wordInput[i] && wordInput[i] <= 54616)
                    {
                        Debug.Log("Ư��ī�尡 ��ġ�մϴ�");
                        isSpecialWord = true; 
                    }
                }
                else
                {
                    Debug.Log("Ư��ī�尡 ��ġ���� �ʽ��ϴ�");
                    isSpecialWord = false;
                }
            }
        }


        if (wordInput.Length > ObjectManager.instance.dropCount)
        {
            if (wordInput == ObjectManager.instance.createdWords)
            {
                Debug.Log("��ġ�մϴ�");
                isWord = true;
            }
            else
            {
                if (ObjectManager.instance.createdWords.Contains(wordInput))
                {
                    Debug.Log("�ֽ��ϴ�");
                    isWord = true;
                }
                else
                {
                    Debug.Log("�����ʽ��ϴ�");
                    isWord = false;
                }
            }
        }
        else
        {
            Debug.Log("�����Դϴ�");
            isWord = false;
        }

        if (isCharWord && isWord && isSpecialWord) 
        {
            Debug.Log("���� API �˻縦 �����մϴ�");
        }
        
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