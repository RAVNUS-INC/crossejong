using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor.ShaderKeywordFilter;

public class SavedCreateWords : MonoBehaviour
{
    public string _playerCreateWord;
    public int _playerCreateWordNum;
    public Dictionary<string, int> playerCreateWords = new Dictionary<string, int>();  // 플레이어가 만든 단어와 횟수
    public string csvFileName = "우왕좌왕세종대왕";  // 읽어 올 파일 이름
    public string filePath;
    public bool isFinish = false;  // 마지막 줄을 판별하기 위한 bool 타입 변수

    private void Start()
    {
        ReadCSV();
    }
    private void ReadCSV()
    {
        filePath = Path.Combine(Application.persistentDataPath, "PlayerWords.csv");
        LoadCSVData(); // 실행 시 CSV 데이터 불러오기
    }

    private void LoadCSVData()
    {
        if (!File.Exists(filePath))
        {
            Debug.Log("CSV 파일이 존재하지 않아 새로 생성합니다.");
            return;
        }

        string[] lines = File.ReadAllLines(filePath); // 모든 줄 읽기
        for (int i = 1; i < lines.Length; i++) // 첫 번째 줄(타이틀) 제외
        {
            string[] rowData = lines[i].Split(','); // 쉼표로 구분
            if (rowData.Length < 2) continue;

            string word = rowData[0].Trim(); // 단어
            int count = int.Parse(rowData[1].Trim()); // 횟수

            playerCreateWords[word] = count; // 딕셔너리에 저장
        }
    }

    public void AddWordToCSV(string newWord)
    {
        if (playerCreateWords.ContainsKey(newWord))
        {
            playerCreateWords[newWord] += 1; // 기존 단어면 +1
        }
        else
        {
            playerCreateWords[newWord] = 1; // 새로운 단어면 1로 설정
        }

        SaveCSVData(); // 변경된 데이터 저장
    }

    private void SaveCSVData()
    {
        List<string> lines = new List<string>
    {
        "_playerCreateWord,_playerCreateWordNum" // CSV 제목
    };

        foreach (var entry in playerCreateWords)
        {
            lines.Add($"{entry.Key},{entry.Value}"); // 단어, 횟수 추가
        }

        File.WriteAllLines(filePath, lines); // 파일로 저장
        Debug.Log("CSV 데이터 저장 완료!");
    }

    public void OnUserCreatesWord(string newWord)
    {
        AddWordToCSV(newWord);
    }


}
