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
    public bool isContinue;
    public WordLists wordLists;
    public DictionaryAPI dictionaryAPI;

    public List<char> charList = new List<char>
    {'��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��'};





    public void IsCreateWord()
    {
        Debug.Log(ObjectManager.instance.dropCount);
        wordInput = cardInputField.text;
        isContinue = true;

        if (wordInput.Length > ObjectManager.instance.dropCount)
        {
            if (ObjectManager.instance.createdWords.Contains(wordInput))  // ���ڷ� �̷���� �ܾ��� ���
            {
                Debug.Log("���ڷθ� �̷���� �ܾ ���� API �˻縦 �����մϴ�");
                // wordInput (���� API �˻� ������)
                ObjectManager.instance.dropCount = 0;
                ObjectManager.instance.inputWords = wordInput;
                StartCoroutine(dictionaryAPI.CheckWordExists(wordInput));

            }

            for (int i = 0; i < ObjectManager.instance.createdWords.Length; i++)
            {
                if (isContinue == false)
                {
                    break;
                }
                else
                {
                    for (int j = 0; j < 19; j++)
                    {
                        if (ObjectManager.instance.createdWords[i] == charList[j])  // ����ī�尡 ���Ե� ���
                        {
                            List<char> words = wordLists.choDictionary[charList[j]];
                            for (int k = 0; k < 588; k++)
                            {
                                if (wordInput[i] == words[k])
                                {
                                    Debug.Log("���� ī��� �̷���� �ܾ ���� API �˻縦 �����մϴ�");
                                    // wordInput (���� API �˻� ������)
                                    isContinue = false;
                                    ObjectManager.instance.dropCount = 0;
                                    ObjectManager.instance.inputWords = wordInput;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < ObjectManager.instance.createdWords.Length; i++)
            {
                if (ObjectManager.instance.createdWords[i] == 'C' || ObjectManager.instance.createdWords[i] == 'B')  // Ư��ī�尡 ���Ե� ���
                {
                    if (44032 <= wordInput[i] && wordInput[i] <= 54616)
                    {
                        Debug.Log("Ư�� ī��� �̷���� �ܾ ���� API �˻縦 �����մϴ�");
                        // wordInput (���� API �˻� ������)
                        ObjectManager.instance.dropCount = 0;
                        ObjectManager.instance.inputWords = wordInput;
                        break;
                    }
                }
                else
                    break;
            }
        }
        else
        {
            Debug.Log("�����Դϴ�");
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