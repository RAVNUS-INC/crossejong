using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SavedCreateWords : MonoBehaviour
{
    public DictionaryAPI dictionaryAPI;
    public Dictionary<string, int> createdWordCounts = new Dictionary<string, int>();

    public void SaveCreatedWord(string word)  // �ܾ� ���� �� ī��Ʈ
    {
        if (createdWordCounts.ContainsKey(word))
        {
            createdWordCounts[word]++;
        }
        else
        {
            createdWordCounts[word] = 1;
        }
    }

    public List<KeyValuePair<string, int>> GetTop10Words()  // ���� 10�� �ܾ� ���
    {
        return createdWordCounts.OrderByDescending(x => x.Value).Take(10).ToList();
    }

    //IEnumerator ProcessDictionaryCheck(string word)
    //{
    //    yield return StartCoroutine(dictionaryAPI.CheckWordExists(word));

    //    if (dictionaryAPI.isWordValid) // ���� �˻� ���� ��
    //    {
    //        SaveCreatedWord(word);  // �ܾ� ����
    //        Debug.Log($"�ܾ� ���� �Ϸ�: {word}, ��� Ƚ��: {createdWordCounts[word]}");
    //    }
    //}


}
