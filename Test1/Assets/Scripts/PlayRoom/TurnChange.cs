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
    public Text resultText;
    public List<char> wordList1 = new List<char>();
    public List<char> wordList2 = new List<char>();
    public List<char> wordList3 = new List<char>();

    // "��"�� �����ڵ� 44032�� 587�� ���ϸ� "��"�� ����
    // "��"�� �����ڵ� 50500
    // "��"�� �����ڵ� 55204

    private void Start()
    {
        WordList(44032, wordList1);
        WordList(50500, wordList2);
        WordList(55204, wordList3);
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

        Debug.Log(new string(wordList.ToArray()));
    }

    public void IsCreateWord()
    {
        Debug.Log(ObjectManager.instance.dropCount);
        wordInput = cardInputField.text;

        if (wordInput.Contains("��"))
        {

        }

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
            Debug.Log("�����Դϴ�");
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
