using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using static UserProfileLoad;
using System.Globalization;


public class GameResult : MonoBehaviourPunCallbacks
{
    public UserProfileLoad userProfileLoad; // players클래스 사용을 위해
    public TurnManager turnManager; // 단어완성횟수 변수 값 사용을 위해

    public GameObject ResultPanel, GameResultPopup; // 놀이결과 판넬, 팝업창
    public TMP_Text EndMsg, TimeMsg; // 게임 종료 후 잠깐의 대기 메시지, 팝업 메인 돌아갈 메시지
    public GameObject[] ResultUserList; // 놀이결과 유저들의 리스트
    public Image[] ResultUserImg; // 놀이결과 유저들의 프로필사진
    public TMP_Text[] ResultUserName, ResultWordCount; // 놀이결과 유저들의 닉네임, 카드 소진 시간
    Coroutine BacktoMainRoutine;
    Coroutine EndGameDelayRoutine;

    public List<int> allActorNums = new List<int>(); //초기 리스트 복제

    private void Awake()
    {
        SetActive(); // 리스트 기본 비활성화 초기화
    }

    public void SetStat() //유저 초기 세팅 확인버튼(usersetmanager)에 연결
    {
        // 1. 현재 통계 값 가져오기
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
            result =>
            {
                // 기존 값을 가져와서
                var currentCompletionCount = result.Statistics.FirstOrDefault(stat => stat.StatisticName == "WordCompletionCount")?.Value ?? 0;

                // 기존 값에 더하기
                var newCompletionCount = currentCompletionCount + ObjectManager.instance.MyCompleteWordCount;

                // 2. 새로운 값으로 통계 업데이트
                var request = new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                    {
                new StatisticUpdate { StatisticName = "WordCompletionCount", Value = newCompletionCount }
                    }
                };

                PlayFabClientAPI.UpdatePlayerStatistics(request,
                    (updateResult) => print("단어완성횟수 저장 완료"),
                    (error) => print("변수 저장 실패"));
            },
            error => print("현재 통계 값 가져오기 실패"));
    }

    public void MainCheckTime()
    {
        SetStat(); // 단어완성횟수를 playfab에 즉시 업데이트
    }

   public  void CheckIfAllPlayersSubmitted() //모두가 해시 업데이트 했는지 여부 검사
   {
        Hashtable roomProps = PhotonNetwork.CurrentRoom.CustomProperties;

        string result = string.Join(", ", allActorNums);
        Debug.Log("[체크]복제리스트 " + result);

        foreach (int actNum in allActorNums)
        {
            string leftKey = $"Left_{actNum}";

            if (!roomProps.ContainsKey(leftKey))
            {
                Debug.Log($"아직 제출 안 한 유저: {actNum}");
                return;
            }

        }

        Debug.Log("모든 유저 제출 완료! 순위 계산 시작");
        CheckAndRankPlayersByCardCount(); // 여기서 순위 매기는 함수 호출
    }

    void CheckAndRankPlayersByCardCount() //순위계산시작
    {
        Hashtable roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
        Dictionary<int, (int cardCount, int wordCount)> actorToCardCount = new Dictionary<int, (int, int)>();
        List<int> leavers = new List<int>();

        string result = string.Join(", ", allActorNums);
        Debug.Log("[계산]복제리스트 " + result);

        foreach (int actorNum in allActorNums) // 전체 유저 기준
        {
            string leftKey = $"Left_{actorNum}";
            string cardKey = $"CardLeft_{actorNum}";
            string wordKey = $"CompletedWords_{actorNum}";

            bool isLeaver = roomProps.ContainsKey(leftKey) && (bool)roomProps[leftKey] == true;

            if (isLeaver)
            {
                leavers.Add(actorNum); // 나간 유저는 따로 저장
                //Debug.Log("나간 유저 추가 성공");
            }
            else if (roomProps.ContainsKey(cardKey) && roomProps.ContainsKey(wordKey))
            {
                int cardCount = (int)roomProps[cardKey];
                int wordCount = (int)roomProps[wordKey];
                actorToCardCount[actorNum] = (cardCount, wordCount);
                //Debug.Log("기존 유저 추가 성공");
            }
        }

        // 카드 개수 기준 오름차순 정렬
        var sortedPlayers = actorToCardCount.OrderBy(pair => pair.Value.cardCount)
                                     .Select(pair => pair.Key)
                                     .ToList();

        // 마지막에 나간 유저들 붙이기
        sortedPlayers.AddRange(leavers);

        Debug.Log("최종 액터넘버 순위 (ActorNumber): " + string.Join(", ", sortedPlayers));

        int index = 0;
        foreach (int actorNum in sortedPlayers)
        {
            // 유저 프로필 정보 가져오기
            int currentPlayerImgIndex = (int)userProfileLoad.GetProfileIndexByActorNumber(actorNum); // 프로필 이미지 인덱스
            string currentPlayerName = userProfileLoad.GetUserNameByActorNumber(actorNum); // 이름

            ResultUserList[index].SetActive(true);

            //공통ui업데이트
            ResultUserName[index].text = currentPlayerName;
            ResultUserImg[index].sprite = userProfileLoad.profileImages[currentPlayerImgIndex];

            // UI 채우기
            int wordCount = (int)roomProps[$"CompletedWords_{actorNum}"];
            ResultWordCount[index].text = $"{wordCount}회";

            index++;
        }
    }


    public void EndGameDelay()
    {

        // 게임 종료를 알리는 메시지 약 1초간 표시 타이머 시작
        if (EndGameDelayRoutine == null)
        {
            EndGameDelayRoutine = StartCoroutine(EndGameTimer()); // 새 코루틴 시작
        }
    }

    IEnumerator StartTimer() //지금은 안쓰는 함수
    {
        float remainingTime = 15f;

        while (remainingTime > 0)
        {
            TimeMsg.text = $"{remainingTime.ToString()}초 뒤에 메인으로 돌아갑니다.";

            remainingTime -= 1f;

            yield return new WaitForSeconds(1f); // 1초 대기
        }

        OnTimerEnd();
    }

    IEnumerator EndGameTimer()
    {
        float remainingTime = 1f;

        while (remainingTime > 0)
        {
            turnManager.photonView.RPC("ShowEndGameMsg", RpcTarget.All); // 모두의 화면에 종료 메시지를 띄우도록 함

            remainingTime -= 1f;

            yield return new WaitForSeconds(1.5f); // 1.2초 대기
        }

        GameTimerEnd();
    }

    void OnTimerEnd() //결과창 보여주기 시간이 끝났을 때 - 지금은 안쓰는 함수
    {
        if (BacktoMainRoutine != null)
        {
            StopCoroutine(BacktoMainRoutine); // 코루틴 중지
        }
        BacktoMainRoutine = null;

        Debug.Log("15초가 지나 메인으로 돌아갑니다.");

        TurnManager.instance.LeaveRoom(); // 게임 도중 방을 나갈 때와 같은 원리
    }

    void GameTimerEnd() // 게임 종료 대기 메시지가 끝났을 때 - 결과창 보여주기
    {
        if (EndGameDelayRoutine != null)
        {
            StopCoroutine(EndGameDelayRoutine); // 코루틴 중지
        }
        EndGameDelayRoutine = null;

        // 모두의 화면에 결과 창을 띄우도록 요청함
        turnManager.photonView.RPC("ShowResultPopup", RpcTarget.All); 

        // 모두에게 코루틴을 멈출 것을 요청함
        turnManager.photonView.RPC("StopTurnCoroutine", RpcTarget.All);

        //MainCheckTime(); // 메인 되돌아가는 타이머 시작
    }


    public void SetActive()
    {
        // 모든 유저리스트는 기본적으로 비활성화(필요할 때 활성화할 것임)
        for (int i = 0; i < ResultUserList.Length; i++)
        {
            ResultUserList[i].SetActive(false);
        }
    }

}
