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
    public Dictionary<string, int> playerCreateWords = new Dictionary<string, int>();  // �÷��̾ ���� �ܾ�� Ƚ��
    public string csvFileName = "����¿ռ������";  // �о� �� ���� �̸�
    public string filePath;
    public bool isFinish = false;  // ������ ���� �Ǻ��ϱ� ���� bool Ÿ�� ����

    private void Start()
    {
        ReadCSV();
    }
    private void ReadCSV()
    {
        filePath = Path.Combine(Application.persistentDataPath, "PlayerWords.csv");
        LoadCSVData(); // ���� �� CSV ������ �ҷ�����
    }

    private void LoadCSVData()
    {
        if (!File.Exists(filePath))
        {
            Debug.Log("CSV ������ �������� �ʾ� ���� �����մϴ�.");
            return;
        }

        string[] lines = File.ReadAllLines(filePath); // ��� �� �б�
        for (int i = 1; i < lines.Length; i++) // ù ��° ��(Ÿ��Ʋ) ����
        {
            string[] rowData = lines[i].Split(','); // ��ǥ�� ����
            if (rowData.Length < 2) continue;

            string word = rowData[0].Trim(); // �ܾ�
            int count = int.Parse(rowData[1].Trim()); // Ƚ��

            playerCreateWords[word] = count; // ��ųʸ��� ����
        }
    }

    public void AddWordToCSV(string newWord)
    {
        if (playerCreateWords.ContainsKey(newWord))
        {
            playerCreateWords[newWord] += 1; // ���� �ܾ�� +1
        }
        else
        {
            playerCreateWords[newWord] = 1; // ���ο� �ܾ�� 1�� ����
        }

        SaveCSVData(); // ����� ������ ����
    }

    private void SaveCSVData()
    {
        List<string> lines = new List<string>
    {
        "_playerCreateWord,_playerCreateWordNum" // CSV ����
    };

        foreach (var entry in playerCreateWords)
        {
            lines.Add($"{entry.Key},{entry.Value}"); // �ܾ�, Ƚ�� �߰�
        }

        File.WriteAllLines(filePath, lines); // ���Ϸ� ����
        Debug.Log("CSV ������ ���� �Ϸ�!");
    }

    public void OnUserCreatesWord(string newWord)
    {
        AddWordToCSV(newWord);
    }


}
