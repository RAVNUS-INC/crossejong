using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SavedCreateWords : MonoBehaviour
{
    public DictionaryAPI dictionaryAPI;
    public Dictionary<string, int> createdWordCounts = new Dictionary<string, int>();

    public void SaveCreatedWord(string word)  // 단어 저장 및 카운트
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

    public List<KeyValuePair<string, int>> GetTop10Words()  // 상위 10개 단어 출력
    {
        return createdWordCounts.OrderByDescending(x => x.Value).Take(10).ToList();
    }

    //IEnumerator ProcessDictionaryCheck(string word)
    //{
    //    yield return StartCoroutine(dictionaryAPI.CheckWordExists(word));

    //    if (dictionaryAPI.isWordValid) // 사전 검사 성공 시
    //    {
    //        SaveCreatedWord(word);  // 단어 저장
    //        Debug.Log($"단어 저장 완료: {word}, 사용 횟수: {createdWordCounts[word]}");
    //    }
    //}


}
