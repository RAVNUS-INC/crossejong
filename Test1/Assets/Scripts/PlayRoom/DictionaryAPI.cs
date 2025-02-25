using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;  // JSON 파싱을 위해 필요

public class DictionaryAPI : MonoBehaviour
{
    // API의 기본 URL을 설정 (쿼리 파라미터 'type=json' 포함)
    private string apiUrl = "https://krdict.korean.go.kr/api/search?type=json";  // API 엔드포인트

    // 사용자의 API 키 (인증용)
    private string apiKey = "BD6ACB6A46D2336CBFB3EF7283A0279C";  // 네가 설정한 인증키

    // 단어 존재 여부를 체크하는 코루틴 함수
    public IEnumerator CheckWordExists(string word)
    {
        // 요청 URL을 생성 (검색어를 쿼리 파라미터로 추가)
        string url = apiUrl + "&key=" + apiKey + "&q=" + word;

        // 최종 요청 URL을 로그로 출력 (디버깅용)
        Debug.Log("Request URL: " + url);

        // UnityWebRequest 객체를 사용하여 HTTP GET 요청을 보냄
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // 'User-Agent' 헤더를 설정하여 서버가 요청을 받아들이도록 도움
            request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

            // 요청을 비동기적으로 보내고, 응답을 기다림
            yield return request.SendWebRequest();

            // 요청이 성공했을 경우
            if (request.result == UnityWebRequest.Result.Success)
            {
                // 응답 데이터를 JSON 형태로 파싱하여 MeansResponse 객체로 변환
                MeansResponse response = JsonConvert.DeserializeObject<MeansResponse>(request.downloadHandler.text);

                // 'total' 값이 0이면 단어가 존재하지 않음, 0보다 크면 단어가 존재함
                if (response.total > 0)
                {
                    Debug.Log("단어가 존재합니다.");
                }
                else
                {
                    Debug.Log("단어가 없습니다.");
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

// API 응답에서 필요한 데이터를 저장할 클래스
[System.Serializable]
public class MeansResponse
{
    // 검색된 전체 어휘 개수
    public int total;  // 검색된 전체 어휘 개수
}
