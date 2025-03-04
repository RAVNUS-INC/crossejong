using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UserProfileLoad;

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

    private void Awake()
    {
        SetActive(); // 리스트 기본 비활성화 초기화
    }

    // ------VVV
    // 카드를 가장 먼저 소진한 유저들 단어완성횟수 오름차순으로 정리한 리스트로부터 결과 표시
    // sortedplayers리스트에 있는 유저들을 바탕으로 결과창 유저 표시(끝까지 게임을 플레이한 유저들)
    // 본인이 카드를 소진하면 다른 모두에게 rpc함수를 통해 알림 요청
    // 2명일 경우 -> 한명이 카드 다 소진 -> 결과 바로 표시
    // 결과 확인 버튼 누르면 메인으로 돌아가도록
    // 확인 버튼을 누르지 않아도 15초뒤에 메인으로 자동으로 이동하도록 알림메시지 수행하기

    public void MainCheckTime()
    {
        // 결과창이 띄워지면 15초 타이머를 시작 - 메인 도달까지 남은 시간
        if (BacktoMainRoutine == null)
        {
            BacktoMainRoutine = StartCoroutine(StartTimer()); // 새 코루틴 시작
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

    IEnumerator StartTimer()
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

    // 결과창 보여주기 시간이 끝났을 때
    void OnTimerEnd()
    {
        if (BacktoMainRoutine != null)
        {
            StopCoroutine(BacktoMainRoutine); // 코루틴 중지
        }
        BacktoMainRoutine = null;

        Debug.Log("15초가 지나 메인으로 돌아갑니다.");

        // 메인으로 돌아가기
        LoadingSceneController.Instance.LoadScene("Main");

        // 방 나가기
        PhotonNetwork.LeaveRoom();
    }

    // 게임 종료 대기 메시지가 끝났을 때 - 결과창 보여주기
    void GameTimerEnd()
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
    }

    public void OnConfirmButton() // 게임 결과 확인 버튼을 눌렀을 때 -> 메인 이동
    {
        if (PhotonNetwork.InRoom)
        {

            Debug.Log($"확인 버튼 클릭. 메인으로 이동합니다.");

            //로딩바 ui 애니메이션 보여주기
            LoadingSceneController.Instance.LoadScene("Main");

            // 방 나가기
            PhotonNetwork.LeaveRoom();

            // 메인 이동 후 다시 방 생성 시도 -> makeroom 씬 전환 문제 발생
        }

        if (BacktoMainRoutine != null)
        {
            StopCoroutine(BacktoMainRoutine); // 코루틴 중지
        }
        BacktoMainRoutine = null;
    }

    public override void OnLeftRoom() // 방을 성공적으로 나왔을 때
    {
        // 마지막 남은 유저가 나갈때 출력될 메시지
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("빈 방 자동 삭제 확인");
        }
    }

    public void SetActive()
    {
        // 모든 유저리스트는 기본적으로 비활성화(필요할 때 활성화할 것임)
        for (int i = 0; i < ResultUserList.Length; i++)
        {
            ResultUserList[i].SetActive(false);
        }
    }


    // 방장이 판넬이 보여지기 전 결과 데이터를 미리 업데이트 - 게임이 끝나고 약 1초간 딜레이가 있어야 할 것
    [PunRPC]
    public void UpdateResultData(int actorNum, int completeCount) 
    {
        // 방장만 수행
        //if (PhotonNetwork.IsMasterClient)
        //{
            // 해당 액터넘버에 해당하는 Player 찾기
            Player targetPlayer = userProfileLoad.players.FirstOrDefault(p => p.myActNum == actorNum);

            if (targetPlayer != null)
            {
                targetPlayer.completeCount = completeCount; // 단어 완성 횟수 저장
                Debug.Log($"{targetPlayer.displayName}님의 단어 완성 횟수: {completeCount}");
            }

            // 모든 유저의 정보가 수신되었는지 확인
            if (userProfileLoad.players.All(p => p.completeCount >= 0))
            {
                Debug.Log("모든 유저의 단어 완성 횟수를 받았습니다!");

                // 플레이어들 정보를 내림차순으로 정렬
                userProfileLoad.players.Sort((x, y) => y.completeCount.CompareTo(x.completeCount));

                // 정보를 업데이트
                UpdatePlayerInfo();
            }
        //}
    }

    public void UpdatePlayerInfo()
    {
        List<Player> players = userProfileLoad.players;

        // 각 플레이어의 정보를 UI에 업데이트
        for (int i = 0; i < players.Count; i++)
        {
            // 플레이어의 정보에 접근
            Player currentPlayer = players[i];

            // 플레이어의 이미지 인덱스와 이름을 가져오기
            int currentPlayerImgIndex = currentPlayer.imgIndex;
            string currentPlayerName = currentPlayer.displayName;
            int currentPlayerWordCount = currentPlayer.completeCount;

            ResultUserList[i].SetActive(true);
            ResultUserImg[i].sprite = userProfileLoad.profileImages[currentPlayerImgIndex];
            ResultUserName[i].text = currentPlayerName;
            ResultWordCount[i].text = $"{currentPlayerWordCount}회";
        }
    }
}
