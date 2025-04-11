using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;


public class SaveCreatedWords : MonoBehaviour
{
    public static SaveCreatedWords instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

    }

    private List<string> userCreateWordData = new List<string>();  // 출생년도와 플레이어가 만든 단어와 횟수
    private string filePath;
    public string cardLevelInfo;


    public void ReadCSV(string userBirthYear)
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "Scripts/CSV");
        TurnChange.instance.APIStatusMsg.text = "33밑";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);  // 폴더가 없으면 생성
            TurnChange.instance.APIStatusMsg.text = "36밑";
            Debug.Log("CSV 폴더가 없어서 새로 생성했습니다.");
        }
        else
        {
            Debug.Log("디렉토리가 이미 존재합니다.");
        }

        Debug.Log("파일 경로를 불러옵니다");
        //filePath = Path.Combine(Application.dataPath, "Scripts/CSV/" + userBirthYear + "_Userdata.csv");
        filePath = Path.Combine(Application.persistentDataPath, userBirthYear + "_Userdata.csv");
        TurnChange.instance.APIStatusMsg.text = "45밑";

        Debug.Log(filePath);

        if (!File.Exists(filePath))
        {
            TurnChange.instance.APIStatusMsg.text = "50밑";
            Debug.Log(userBirthYear + "_Userdata.csv 파일이 존재하지 않아 새로 생성합니다.");

            return;
        }
        else
        {
            Debug.Log(userBirthYear + "_Userdata.csv 파일이 존재합니다.");
        }
        TurnChange.instance.APIStatusMsg.text = "59밑";
        LoadCSVData();
        TurnChange.instance.APIStatusMsg.text = "60밑";
    }

    public void LoadCSVData()
    {
        userCreateWordData.Clear();  // 기존 리스트 초기화

        string[] lines = File.ReadAllLines(filePath); // 모든 줄 읽기

        for (int i = 1; i < lines.Length; i++) // 첫 번째 줄(타이틀) 제외
        {
            string[] rowData = lines[i].Split(','); // 쉼표로 구분
            if (rowData.Length < 4) continue;

            string birthYear = rowData[0].Trim(); // 출생년도
            string cardLevel = rowData[1].Trim();
            string word = rowData[2].Trim(); // 단어
            string count = rowData[3].Trim(); // 횟수

            // 리스트에 추가 (출생년도, 단어, 횟수)
            userCreateWordData.Add($"{birthYear},{cardLevel},{word},{count}");  //string 타입 변수들로 저장
        }
    }

    string NormalizeString(string input)
    {
        return input.Trim().Replace(" ", "").Normalize(NormalizationForm.FormC);
    }


    public void AddWordToCSV(string userBirthYear,string newWord)
    {
        TurnChange.instance.APIStatusMsg.text = $"단어: {newWord}";
        bool wordExists = false;
        string normalizedNewWord = NormalizeString(newWord);

        for (int i = 0; i < userCreateWordData.Count; i++)
        {
            string[] rowData = userCreateWordData[i].Split(','); // 쉼표로 데이터 분할

            if (rowData.Length < 4) continue;

            string birthYear = rowData[0].Trim();
            string cardLevel = rowData[1].Trim();
            string word = rowData[2].Trim();
            int count = int.Parse(rowData[3].Trim());  //int로 변환

            string existingWord = NormalizeString(word);

            if (birthYear == userBirthYear && cardLevel == cardLevelInfo && existingWord == normalizedNewWord) // 출생년도와 난이도가 같은 기존 단어가 있다면
            {
                count += 1; // 횟수 증가
                userCreateWordData[i] = $"{birthYear},{cardLevel},{word},{count}"; // 업데이트
                Debug.Log("기존 단어 +1");
                wordExists = true;
                break;
            }

        }
        if (wordExists == false)
        {
            cardLevelInfo = LobbyManager.instance.selectedDifficulty;
            userCreateWordData.Add($"{userBirthYear},{cardLevelInfo},{newWord},1");
            Debug.Log("새로운 단어 추가 및 +1");
        }

        SaveCSVData(); // 변경된 데이터 저장
    }

    // CSV 데이터 저장 메서드 추가
    public void SaveCSVData()
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("_birthYear,_cardLevel,_userCreateWord,_userCreateWordNum"); // CSV 헤더

            // 중복 제거 및 정렬 (선택 사항)
            HashSet<string> uniqueWords = new HashSet<string>(userCreateWordData);
            foreach (string line in uniqueWords)
            {
                sb.AppendLine(line);
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            Debug.Log("CSV 파일 저장 완료: " + filePath);
            TurnChange.instance.APIStatusMsg.text = $"{filePath}";
        }
        catch (Exception e)
        {
            Debug.LogError($"CSV 저장 중 오류 발생: {e.Message}");
        }
    }


    public void OnUserCreatesWord(string userBirthYear, string newWord)
    {
        Debug.Log("단어 저장을 시작합니다");
        TurnChange.instance.APIStatusMsg.text = "156밑";
        ReadCSV(userBirthYear);
        TurnChange.instance.APIStatusMsg.text = "157밑";
        AddWordToCSV(userBirthYear, newWord);
        //TurnChange.instance.APIStatusMsg.text = "158밑";
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
            File.WriteAllText(filePath, "_birthYear,_cardLevel,_userCreateWord,_userCreateWordNum\n"); // 헤더만 남기고 초기화
            Debug.Log("CSV 파일 내용이 초기화되었습니다.");
        }
        else
        {
            Debug.Log("CSV 파일이 존재하지 않아 초기화할 수 없습니다.");
        }
    }
}
