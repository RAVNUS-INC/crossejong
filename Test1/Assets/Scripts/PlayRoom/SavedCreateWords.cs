using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor.ShaderKeywordFilter;

public class SavedCreateWords : MonoBehaviour
{
    //public string _playerCreateWord;
    //public int _playerCreateWordNum;
    public Dictionary<string, int> playerCreateWords = new Dictionary<string, int>();  // 플레이어가 만든 단어와 횟수
    public string filePath;

    private void Start()
    {
        ReadCSV();
    }
    private void ReadCSV()
    {
        filePath = Path.Combine(Application.persistentDataPath, "우왕좌왕세종대왕.csv");
        Debug.Log(filePath);
        LoadCSVData(); // 실행 시 CSV 데이터 불러오기
    }

    private void LoadCSVData()
    {
        if (!File.Exists(filePath))
        {
            Debug.Log("CSV 파일이 존재하지 않아 새로 생성합니다.");
            return;
        }
        else
        {
            Debug.Log("CSV 파일이 존재합니다.");
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
            Debug.Log("기존 단어 +1");
        }
        else
        {
            playerCreateWords[newWord] = 1; // 새로운 단어면 1로 설정
            Debug.Log("새로운 단어 추가 및 +1");
        }

        SaveCSVData(); // 변경된 데이터 저장
    }

    private void SaveCSVData()
    {
        List<string> lines = new List<string> { "_playerCreateWord,_playerCreateWordNum"};  // CVS 제목

        foreach (var entry in playerCreateWords)
        {
            lines.Add($"{entry.Key},{entry.Value}"); // 단어, 횟수 추가
        }

        // UTF-8 BOM을 포함해서 저장
        using (StreamWriter writer = new StreamWriter(filePath, false, new System.Text.UTF8Encoding(true)))
        {
            foreach (string line in lines)
            {
                writer.WriteLine(line);
            }
        }

        Debug.Log("CSV 데이터 저장 완료 (UTF-8 인코딩 적용)!");

        //File.WriteAllLines(filePath, lines); // 파일로 저장
        //Debug.Log("CSV 데이터 저장 완료!");
    }

    public void OnUserCreatesWord(string newWord)
    {
        Debug.Log("단어 저장을 시작합니다");
        AddWordToCSV(newWord);
    }


    // CSV 파일을 지우는 함수
    private void DeleteCSVFile()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("CSV 파일이 삭제되었습니다.");
        }
        else
        {
            Debug.Log("삭제할 CSV 파일이 없습니다.");
        }
    }

    // CSV 파일 내용을 지우는 함수
    private void ClearCSVContent()
    {
        if (File.Exists(filePath))
        {
            File.WriteAllText(filePath, "_playerCreateWord,_playerCreateWordNum\n"); // 헤더만 남기고 초기화
            Debug.Log("CSV 파일 내용이 초기화되었습니다.");
        }
        else
        {
            Debug.Log("CSV 파일이 존재하지 않아 초기화할 수 없습니다.");
        }
    }


}
