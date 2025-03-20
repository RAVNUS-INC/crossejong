using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Text;

public class SavedCreateWords : MonoBehaviour
{
    public Dictionary<string, int> playerCreateWords = new Dictionary<string, int>();  // �÷��̾ ���� �ܾ�� Ƚ��
    public string filePath;

    private void Start()
    {
        ReadCSV();
    }
    private void ReadCSV()
    {
        filePath = Path.Combine(Application.dataPath, "Scripts/CSV/����ڵ�����.csv");  // ����Ƽ �� scripts�� CSV ���Ͽ� ����
        // ����� CSV ������ ������ ���� �ܺη� ���δ� �Լ��� �ʿ��� �� ����
        Debug.Log(filePath);  // /����ڵ�����.csv�� ����� ������ �� ���� �˻��� �ٿ��ֱ�� �ش� ��ġ�� �� �� ����
        LoadCSVData(); // ���� �� CSV ������ �ҷ�����
    }

    private void LoadCSVData()
    {
        if (!File.Exists(filePath))
        {
            Debug.Log("CSV ������ �������� �ʾ� ���� �����մϴ�.");
            return;
        }
        else
        {
            Debug.Log("CSV ������ �����մϴ�.");
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
            Debug.Log("���� �ܾ� +1");
        }
        else
        {
            playerCreateWords[newWord] = 1; // ���ο� �ܾ�� 1�� ����
            Debug.Log("���ο� �ܾ� �߰� �� +1");
        }

        SaveCSVData(); // ����� ������ ����
    }

    private void SaveCSVData()
    {
        List<string> lines = new List<string> { "_playerCreateWord,_playerCreateWordNum"};  // CVS ����

        foreach (var entry in playerCreateWords)
        {
            lines.Add($"{entry.Key},{entry.Value}"); // �ܾ�, Ƚ�� �߰�
        }

        // UTF-8 BOM�� �����ؼ� ����
        using (StreamWriter writer = new StreamWriter(filePath, false, new System.Text.UTF8Encoding(true)))
        {
            foreach (string line in lines)
            {
                writer.WriteLine(line);
            }
        }

        Debug.Log("CSV ������ ���� �Ϸ� (UTF-8 ���ڵ� ����)!");

        //File.WriteAllLines(filePath, lines); // ���Ϸ� ����
        //Debug.Log("CSV ������ ���� �Ϸ�!");
    }

    public void OnUserCreatesWord(string newWord)
    {
        Debug.Log("�ܾ� ������ �����մϴ�");
        AddWordToCSV(newWord);
    }


    // CSV ������ ����� �Լ� (�ʿ�� ���)
    private void DeleteCSVFile()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("CSV ������ �����Ǿ����ϴ�.");
        }
        else
        {
            Debug.Log("������ CSV ������ �����ϴ�.");
        }
    }

    // CSV ���� ������ ����� �Լ� (�ʿ�� ���)
    private void ClearCSVContent()
    {
        if (File.Exists(filePath))
        {
            File.WriteAllText(filePath, "_playerCreateWord,_playerCreateWordNum\n"); // ����� ����� �ʱ�ȭ
            Debug.Log("CSV ���� ������ �ʱ�ȭ�Ǿ����ϴ�.");
        }
        else
        {
            Debug.Log("CSV ������ �������� �ʾ� �ʱ�ȭ�� �� �����ϴ�.");
        }
    }


}
