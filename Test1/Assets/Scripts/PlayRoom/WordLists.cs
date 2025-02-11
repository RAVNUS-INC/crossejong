using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WordLists : MonoBehaviour
{
    public List<char> wordList1 = new List<char>();
    public List<char> wordList2 = new List<char>();
    public List<char> wordList3 = new List<char>();
    public List<char> wordList4 = new List<char>();
    public List<char> wordList5 = new List<char>();
    public List<char> wordList6 = new List<char>();
    public List<char> wordList7 = new List<char>();
    public List<char> wordList8 = new List<char>();
    public List<char> wordList9 = new List<char>();
    public List<char> wordList10 = new List<char>();
    public List<char> wordList11 = new List<char>();
    public List<char> wordList12 = new List<char>();
    public List<char> wordList13 = new List<char>();
    public List<char> wordList14 = new List<char>();
    public List<char> wordList15 = new List<char>();
    public List<char> wordList16 = new List<char>();
    public List<char> wordList17 = new List<char>();
    public List<char> wordList18 = new List<char>();
    public List<char> wordList19 = new List<char>();

    // "가"의 문자코드 44032에 587을 더하면 "깋"이 나옴
    // "아"의 문자코드 50500
    // "하"의 문자코드 54616

    public void SavedWordList()
    {
        WordList(44032 + (588 * 0), wordList1);      // "가"의 문자코드 ***
        WordList(44032 + (588 * 1), wordList2);      // "까"의 문자코드 
        WordList(44032 + (588 * 2), wordList3);      // "나"의 문자코드 
        WordList(44032 + (588 * 3), wordList4);      // "다"의 문자코드 
        WordList(44032 + (588 * 4), wordList5);      // "따"의 문자코드 
        WordList(44032 + (588 * 5), wordList6);      // "라"의 문자코드 
        WordList(44032 + (588 * 6), wordList7);      // "마"의 문자코드 
        WordList(44032 + (588 * 7), wordList8);      // "바"의 문자코드 
        WordList(44032 + (588 * 8), wordList9);      // "빠"의 문자코드 
        WordList(44032 + (588 * 9), wordList10);     // "사"의 문자코드 
        WordList(44032 + (588 * 10), wordList11);    // "싸"의 문자코드 
        WordList(44032 + (588 * 11), wordList12);    // "아"의 문자코드 ***
        WordList(44032 + (588 * 12), wordList13);    // "자"의 문자코드 
        WordList(44032 + (588 * 13), wordList14);    // "짜"의 문자코드 
        WordList(44032 + (588 * 14), wordList15);    // "차"의 문자코드 
        WordList(44032 + (588 * 15), wordList16);    // "카"의 문자코드 
        WordList(44032 + (588 * 16), wordList17);    // "타"의 문자코드 
        WordList(44032 + (588 * 17), wordList18);    // "파"의 문자코드 
        WordList(44032 + (588 * 18), wordList19);    // "하"의 문자코드 ***
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

        for (int i = 0; i < list.Count; i++)
        {
            wordList.Add((char)list[i]);
        }
    }
}
