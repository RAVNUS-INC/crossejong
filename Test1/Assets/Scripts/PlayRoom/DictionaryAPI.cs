using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml;  // XML 파싱을 위한 네임스페이스

public class DictionaryAPI : MonoBehaviour
{
    private string apiUrl = "https://krdict.korean.go.kr/api/search?type=json";  // API 엔드포인트
    private string apiKey = "BD6ACB6A46D2336CBFB3EF7283A0279C";  // 네가 설정한 인증키

    public IEnumerator CheckWordExists(string word)
    {
        string url = apiUrl + "&key=" + apiKey + "&q=" + word;
        Debug.Log("Request URL: " + url);
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);  // 응답 내용 출력

                try
                {
                    // XML 파싱
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(request.downloadHandler.text);  // 응답 내용 XML로 로드

                    // XML에서 total 값 추출
                    XmlNode totalNode = xmlDoc.SelectSingleNode("//total");
                    int total = totalNode != null ? int.Parse(totalNode.InnerText) : 0;

                    if (total > 0)
                    {
                        Debug.Log("단어가 존재합니다.");
                    }
                    else
                    {
                        Debug.Log("단어가 없습니다.");
                    }
                }
                catch (Exception e)
                {
                    // XML 파싱 오류 처리
                    Debug.LogError("XML 파싱 오류: " + e.Message);
                }
            }
            else
            {
                // 요청 실패 시 에러 로그 출력
                Debug.LogError("API 요청 실패: " + request.error + "\nResponse: " + request.downloadHandler.text);
            }
        }
    }
}
