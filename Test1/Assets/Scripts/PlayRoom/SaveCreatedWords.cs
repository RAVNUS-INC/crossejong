using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;


public class SaveCreatedWords : MonoBehaviour
{
    private List<string> userCreateWordData = new List<string>();  // 플레이어가 만든 단어와 횟수
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
        
    }

}
