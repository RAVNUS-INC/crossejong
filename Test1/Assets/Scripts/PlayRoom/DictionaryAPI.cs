using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml;  // XML �Ľ��� ���� ���ӽ����̽�

public class DictionaryAPI : MonoBehaviour
{
    public TurnChange turnChange;

    private string apiUrl = "https://krdict.korean.go.kr/api/search";  // API ��������Ʈ
    private string apiKey = "BD6ACB6A46D2336CBFB3EF7283A0279C";  // �װ� ������ ����Ű

    public IEnumerator CheckWordExists(string word)
    {
        string url = apiUrl + "?key=" + apiKey + "&q=" + word;
        Debug.Log("Request URL: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);  // ���� ���� ���

                try
                {
                    // XML �Ľ�
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(request.downloadHandler.text);  // ���� ���� XML�� �ε�

                    // XML���� <total> �� ����
                    XmlNode totalNode = xmlDoc.SelectSingleNode("//total");
                    int total = totalNode != null ? int.Parse(totalNode.InnerText) : 0;

                    if (total > 0)
                    {
                        // <word>�� <pos> �±� �����ͼ� �˻���� ��
                        XmlNodeList itemNodes = xmlDoc.SelectNodes("//item");
                        bool wordExists = false;

                        foreach (XmlNode itemNode in itemNodes)
                        {
                            XmlNode wordNode = itemNode.SelectSingleNode("word");
                            XmlNode posNode = itemNode.SelectSingleNode("pos");

                            if (wordNode != null && posNode != null)
                            {
                                string foundWord = wordNode.InnerText.Trim();
                                string pos = posNode.InnerText.Trim();

                                // �Է°��� ������ �ܾ��̸� ǰ�簡 "���"���� Ȯ��
                                if (foundWord == word && pos == "���")
                                {
                                    wordExists = true;
                                    break;
                                }
                            }
                        }

                        if (wordExists)
                        {
                            Debug.Log("�ܾ� '" + word + "'�� �����ϸ�, ����Դϴ�.");

                            // �ܾ Ȯ�εǸ� �� �ѱ��
                            TurnManager.instance.TossNextTurn();
                        }
                        else
                        {
                            Debug.Log("�ܾ� '" + word + "'�� �������� �ʰų�, ��簡 �ƴմϴ�.");
                            turnChange.RollBackAreas(); // API�˻翡 ������� �������Ƿ� ī�带 �ٽ� ��������
                            ObjectManager.instance.AlaramMsg.gameObject.SetActive(true);
                            ObjectManager.instance.AlaramMsg.text = "�ش� �ܾ�� �������� �ʰų�, ��簡 �ƴմϴ�.";
                        }
                    }
                    else
                    {
                        Debug.Log("�ܾ� '" + word + "'�� �������� �ʽ��ϴ�.");
                        turnChange.RollBackAreas(); // API�˻翡 ������� �������Ƿ� ī�带 �ٽ� ��������
                        ObjectManager.instance.AlaramMsg.gameObject.SetActive(true);
                        ObjectManager.instance.AlaramMsg.text = "�������� �ʴ� �ܾ��Դϴ�.";
                    }
                }
                catch (Exception e)
                {
                    // XML �Ľ� ���� ó��
                    Debug.LogError("XML �Ľ� ����: " + e.Message);
                }
            }
            else
            {
                // ��û ���� �� ���� �α� ���
                Debug.LogError("API ��û ����: " + request.error + "\nResponse: " + request.downloadHandler.text);
                turnChange.RollBackAreas(); // API ��û ���з� ī�带 �ٽ� ��������
            }
        }
    }
}
