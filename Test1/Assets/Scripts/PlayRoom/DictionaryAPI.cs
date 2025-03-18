using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml;  // XML 파싱을 위한 네임스페이스

public class DictionaryAPI : MonoBehaviour
{
    public TurnChange turnChange;

    private string apiUrl = "https://krdict.korean.go.kr/api/search";  // API 엔드포인트
    private string apiKey = "BD6ACB6A46D2336CBFB3EF7283A0279C";  // 네가 설정한 인증키

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
                Debug.Log("Response: " + request.downloadHandler.text);  // 응답 내용 출력

                try
                {
                    // XML 파싱
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(request.downloadHandler.text);  // 응답 내용 XML로 로드

                    // XML에서 <total> 값 추출
                    XmlNode totalNode = xmlDoc.SelectSingleNode("//total");
                    int total = totalNode != null ? int.Parse(totalNode.InnerText) : 0;

                    if (total > 0)
                    {
                        // <word>와 <pos> 태그 가져와서 검색어와 비교
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

                                // 입력값과 동일한 단어이며 품사가 "명사"인지 확인
                                if (foundWord == word && pos == "명사")
                                {
                                    wordExists = true;
                                    break;
                                }
                            }
                        }

                        if (wordExists)
                        {
                            Debug.Log("단어 '" + word + "'가 존재하며, 명사입니다.");

                            // 단어가 확인되면 턴 넘기기
                            TurnManager.instance.TossNextTurn();
                        }
                        else
                        {
                            Debug.Log("단어 '" + word + "'가 존재하지 않거나, 명사가 아닙니다.");
                            turnChange.RollBackAreas(); // API검사에 통과하지 못했으므로 카드를 다시 돌려놓기
                            ObjectManager.instance.AlaramMsg.gameObject.SetActive(true);
                            ObjectManager.instance.AlaramMsg.text = "해당 단어는 존재하지 않거나, 명사가 아닙니다.";
                        }
                    }
                    else
                    {
                        Debug.Log("단어 '" + word + "'가 존재하지 않습니다.");
                        turnChange.RollBackAreas(); // API검사에 통과하지 못했으므로 카드를 다시 돌려놓기
                        ObjectManager.instance.AlaramMsg.gameObject.SetActive(true);
                        ObjectManager.instance.AlaramMsg.text = "존재하지 않는 단어입니다.";
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
                turnChange.RollBackAreas(); // API 요청 실패로 카드를 다시 돌려놓기
            }
        }
    }
}
