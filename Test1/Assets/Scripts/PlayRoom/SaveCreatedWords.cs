using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;


public class SaveCreatedWords : MonoBehaviour
{
    private List<string> userCreateWordData = new List<string>();  // 출생년도와 플레이어가 만든 단어와 횟수
    private string birthYear = "2016";
    private string dataPath = Application.dataPath;
    private string filePath;


    public void ReadCSV(string userBirthYear)
    {
        filePath = Path.Combine(dataPath, "Scripts/CSV/" + userBirthYear + "_UserData.csv");

        Debug.Log(filePath);

        if (!File.Exists(filePath))
        {
            Debug.Log(userBirthYear + "_UserData.csv 파일이 존재하지 않아 새로 생성합니다.");

            return;
        }
        else
        {
            Debug.Log(userBirthYear + "_UserData.csv 파일이 존재합니다.");
        }

        LoadCSVData();
    }

    public void LoadCSVData()
    {
        string[] lines = File.ReadAllLines(filePath); // 모든 줄 읽기

        for (int i = 1; i < lines.Length; i++) // 첫 번째 줄(타이틀) 제외
        {
            string[] rowData = lines[i].Split(','); // 쉼표로 구분
            if (rowData.Length < 3) continue;

            string birthYear = rowData[0].Trim(); // 출생년도
            string word = rowData[1].Trim(); // 단어
            string count = rowData[2].Trim(); // 횟수

            // 리스트에 추가 (출생년도, 단어, 횟수)
            userCreateWordData.Add($"{birthYear},{word},{count}");
        }
    }

    public void AddWordToCSV(string newWord)
    {
        TurnChange.instance.APIStatusMsg.text = $"단어: {newWord}";

        bool wordExists = false;

        for (int i = 0; i < userCreateWordData.Count; i++)
        {
            string[] rowData = userCreateWordData[i].Split(','); // 쉼표로 데이터 분할

            if (rowData.Length < 3) continue;

            string word = rowData[1].Trim();
            int count = int.Parse(rowData[2].Trim());

            if (word == newWord) // 기존 단어가 있다면
            {
                count += 1; // 횟수 증가
                userCreateWordData[i] = $"{birthYear},{word},{count}"; // 업데이트
                wordExists = true;
                Debug.Log("기존 단어 +1");
                break;
            }
        }

        if (!wordExists) // 새로운 단어 추가
        {
            userCreateWordData.Add($"{birthYear},{newWord},1");
            Debug.Log("새로운 단어 추가 및 +1");
        }

        SaveCSVData(); // 변경된 데이터 저장
    }

    // CSV 데이터 저장 메서드 추가
    public void SaveCSVData()
    {
        try
        {
            //TurnChange.instance.APIStatusMsg.text = "SaveCSVData 시작";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("_birthYear,_userCreateWord,_userCreateWordNum"); // CSV 헤더

            foreach (string line in userCreateWordData)
            {
                sb.AppendLine(line);
            }
            //TurnChange.instance.APIStatusMsg.text = $"마지막부분은 {lines[lines.Count - 1]}";

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            Debug.Log("CSV 파일 저장 완료: " + filePath);
            //TurnChange.instance.APIStatusMsg.text = "csv 최종 저장 완료";
        }
        catch (Exception e)
        {
            Debug.LogError($"CSV 저장 중 오류 발생: {e.Message}");
            //TurnChange.instance.APIStatusMsg.text = $"CSV 저장 오류: {e.Message}";
        }
    }
    public void OnUserCreatesWord(string newWord)
    {
        Debug.Log("단어 저장을 시작합니다");
        AddWordToCSV(newWord);
    }

    // CSV 파일을 지우는 함수 (필요시 사용)
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

    // CSV 파일 내용을 지우는 함수 (필요시 사용)
    private void ClearCSVContent()
    {
        if (File.Exists(filePath))
        {
            File.WriteAllText(filePath, "_birthYear,_userCreateWord,_userCreateWordNum\n"); // 헤더만 남기고 초기화
            Debug.Log("CSV 파일 내용이 초기화되었습니다.");
        }
        else
        {
            Debug.Log("CSV 파일이 존재하지 않아 초기화할 수 없습니다.");
        }
    }
}
