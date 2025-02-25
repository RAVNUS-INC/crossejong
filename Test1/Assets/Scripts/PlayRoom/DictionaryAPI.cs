using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;  // JSON �Ľ��� ���� �ʿ�

public class DictionaryAPI : MonoBehaviour
{
    // API�� �⺻ URL�� ���� (���� �Ķ���� 'type=json' ����)
    private string apiUrl = "https://krdict.korean.go.kr/api/search?type=json";  // API ��������Ʈ

    // ������� API Ű (������)
    private string apiKey = "BD6ACB6A46D2336CBFB3EF7283A0279C";  // �װ� ������ ����Ű

    // �ܾ� ���� ���θ� üũ�ϴ� �ڷ�ƾ �Լ�
    public IEnumerator CheckWordExists(string word)
    {
        // ��û URL�� ���� (�˻�� ���� �Ķ���ͷ� �߰�)
        string url = apiUrl + "&key=" + apiKey + "&q=" + word;

        // ���� ��û URL�� �α׷� ��� (������)
        Debug.Log("Request URL: " + url);

        // UnityWebRequest ��ü�� ����Ͽ� HTTP GET ��û�� ����
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // 'User-Agent' ����� �����Ͽ� ������ ��û�� �޾Ƶ��̵��� ����
            request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

            // ��û�� �񵿱������� ������, ������ ��ٸ�
            yield return request.SendWebRequest();

            // ��û�� �������� ���
            if (request.result == UnityWebRequest.Result.Success)
            {
                // ���� �����͸� JSON ���·� �Ľ��Ͽ� MeansResponse ��ü�� ��ȯ
                MeansResponse response = JsonConvert.DeserializeObject<MeansResponse>(request.downloadHandler.text);

                // 'total' ���� 0�̸� �ܾ �������� ����, 0���� ũ�� �ܾ ������
                if (response.total > 0)
                {
                    Debug.Log("�ܾ �����մϴ�.");
                }
                else
                {
                    Debug.Log("�ܾ �����ϴ�.");
                }
            }
            else
            {
                // ��û ���� �� ���� �α� ���
                Debug.LogError("API ��û ����: " + request.error + "\nResponse: " + request.downloadHandler.text);
            }
        }
    }
}

// API ���信�� �ʿ��� �����͸� ������ Ŭ����
[System.Serializable]
public class MeansResponse
{
    // �˻��� ��ü ���� ����
    public int total;  // �˻��� ��ü ���� ����
}
