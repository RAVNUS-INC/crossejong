using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;
using System.Reflection;
using Photon.Realtime;
using static UserProfileLoad;
using PlayFab.ClientModels;
using Unity.VisualScripting;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public UserProfileLoad userProfileLoad; // 유저프로필 이미지 참조를 위해 사용
    public GetCard getCard; // 카드 한 장 먹을 때 참조 사용
    public GameResult gameResult; // 판넬 띄울 때 사용

    public GameObject[] InTurnUserList; // 턴에 있는 상태의 유저 이미지 배열
    public Image[] InTurnUserImg; // 턴에 있는 유저들의 프로필사진
    public TMP_Text[] InTurnUserName, timerText; // 턴에 있는 유저들의 닉네임, 남은 시간을 보여주는 텍스트
    public Color overlayColor = new Color(0, 0, 0, 0.3f); // 검정색 그림자
    private int MyNum, NextPlayerNum; // 다음 플레이어의 액터넘버
    public bool IsMyTurn = false; // 현재 턴인지 아닌지 확인
    Coroutine TurnRoutine;

    private void Start()
    {
        // 현재 내 액터넘버 찾기
        MyNum = PhotonNetwork.LocalPlayer.ActorNumber;

        // 턴일 때의 모든 유저프로필 정보를 미리 반영해 표시하기
        for (int i = 0; i < userProfileLoad.sortedPlayers.Length; i++)
        {
            int index = userProfileLoad.userImageList[i];
            InTurnUserImg[i].sprite = userProfileLoad.profileImages[index];

            string name = userProfileLoad.userNameList[i];
            InTurnUserName[i].text = name;
        }
    }

    // 카운트다운 3 2 1 후 실행
    public void AfterCountdown()
    {
        IsMyTurn = true;

        photonView.RPC("CurrentTurnUI", RpcTarget.All, MyNum);

        // 첫 번째 플레이어로 돌아올 때, 코루틴을 다시 시작
        if (TurnRoutine == null)
        {
            TurnRoutine = StartCoroutine(StartTimer()); // 새 코루틴 시작
        }
    }

    [PunRPC]
    public void CurrentTurnUI(int nextnum)  // 현재 턴 UI 모두에게 같은 모습으로 표시
    {
        for (int i = 0; i < userProfileLoad.sortedPlayers.Length; i++)
        {
            if (nextnum == userProfileLoad.sortedPlayers[i])
            {
                // 현재 턴인 플레이어 UI 설정
                userProfileLoad.InRoomUserList[i].gameObject.SetActive(false);
                InTurnUserList[i].gameObject.SetActive(true);

                // 프로필 이미지 위에 검은 그림자 추가
                InTurnUserImg[i].color = overlayColor;
            }
            else
            {
                // 턴이 아닌 플레이어는 기본 UI 설정
                userProfileLoad.InRoomUserList[i].gameObject.SetActive(true);
                InTurnUserList[i].gameObject.SetActive(false);
                timerText[i].text = "";
            }
        }
    }

    [PunRPC]
    void UpdateTimerRPC(int nextnum, float time) // 남아있는 시간 UI 업데이트
    {
        for (int i = 0; i < userProfileLoad.sortedPlayers.Length; i++)
        {
            if (nextnum == userProfileLoad.sortedPlayers[i])
            {
                timerText[i].text = Mathf.CeilToInt(time).ToString(); // 남은 시간을 정수로 표시
            }
        }
    }

    IEnumerator StartTimer()
    {
        float remainingTime = 15f;

        while (remainingTime > 0)
        {
            // 모든 클라이언트에게 시간 업데이트 전송
            photonView.RPC("UpdateTimerRPC", RpcTarget.All, MyNum, remainingTime);

            remainingTime -= 1f;

            yield return new WaitForSeconds(1f); // 1초 대기
        }

        OnTimerEnd();
    }

    // 시간이 끝났을 때
    void OnTimerEnd()
    {
        if (TurnRoutine != null)
        {
            StopCoroutine(TurnRoutine); // 코루틴 중지
        }
        TurnRoutine = null;

        IsMyTurn = false;

        Debug.Log("시간 초과! 턴을 넘깁니다.");

        // 카드 한 장 먹기
        getCard.GetCardToUserCard();

        // 다음 턴의 플레이어 찾기 
        FindNextPlayer();
    }

    public void FindNextPlayer() // 다음 플레이어의 넘버 찾기(마지막 플레이어일 경우 0번 인덱스로 순환)
    {
        // 플레이어 목록에서 현재 플레이어의 인덱스를 찾음
        int currentIndex = Array.IndexOf(userProfileLoad.sortedPlayers, MyNum);

        // 다음 플레이어의 인덱스를 계산 (마지막 플레이어일 경우 순환)
        int nextIndex = (currentIndex + 1) % userProfileLoad.sortedPlayers.Length;

        // 다음 플레이어의 액터 넘버
        int nextActorNumber = userProfileLoad.sortedPlayers[nextIndex];
        NextPlayerNum = nextActorNumber;
        Debug.Log($"[턴 순서] 다음 플레이어 ActorNumber: {NextPlayerNum}");

        // 특정 유저가 다음 함수를 실행하도록 요청하기
        photonView.RPC("RequestNextPlayer", RpcTarget.All, NextPlayerNum);
    }

    [PunRPC]
    public void RequestNextPlayer(int targetActorNumber)
    {
        if (targetActorNumber == MyNum)
        {
            AfterCountdown();
        }
        else
        {
            return;
        }
    }

    // 낼 카드가 없어, 카드를 한 장 먹기로 결정 했을 때(카드 추가 버튼에 리스너 연결)
    public void GoToNextTurnAndAddCard()
    {
        if (IsMyTurn) //현재 내 턴일 때
        {
            if (TurnRoutine != null)
            {
                StopCoroutine(TurnRoutine); // 현재 코루틴 중지
            }
            TurnRoutine = null;

            IsMyTurn = false; // 턴 상태 비활성화

            // 카드 한 장 먹기
            getCard.GetCardToUserCard();

            Debug.Log("카드를 추가하고 턴을 넘깁니다.");

            FindNextPlayer(); // 다음 턴을 탐색
        }
    }

    // API 검사 통과에 성공했을 때
    public void TossNextTurn()
    {
        if (IsMyTurn) //현재 내 턴일 때
        {
            if (TurnRoutine != null)
            {
                StopCoroutine(TurnRoutine); // 현재 코루틴 중지
            }
            TurnRoutine = null;

            IsMyTurn = false; // 턴 상태 비활성화

            Debug.Log("단어 완성 성공! 턴을 넘깁니다.");

            // 단어완성횟수 +1 증가시키기

            FindNextPlayer(); // 다음 턴을 탐색
        }
    }

    public void LeaveRoom() // 방을 나갈때 - exit 버튼에 연결
    {
        if (PhotonNetwork.InRoom)
        {
            // 나갈때 내가 현재 턴이라면?
            // 1. 현재 작동하던 코루틴을 멈추고 다음사람에게 턴 넘기기
            if (IsMyTurn) //현재 내 턴일 때
            {
                if (TurnRoutine != null)
                {
                    StopCoroutine(TurnRoutine); // 현재 코루틴 중지
                }
                TurnRoutine = null;

                IsMyTurn = false; // 턴 상태 비활성화

                Debug.Log("현재 턴: O. 게임을 퇴장합니다.");

                // 3. 방장에게 액터넘버 리스트에서 자신의 번호 삭제 요청하기, ui변화는 없음
                userProfileLoad.photonView.RPC("RequestRemoveUserInfo", RpcTarget.MasterClient, MyNum);

                FindNextPlayer(); // 다음 턴을 탐색

            }
            else
            {
                // 나갈때 내가 턴이 아니라면?
                Debug.Log("현재 턴: X. 게임을 퇴장합니다.");

                // 1. 액터넘버 리스트 자신 번호 삭제 요청하기, ui변화는 없음
                userProfileLoad.photonView.RPC("RequestRemoveUserInfo", RpcTarget.MasterClient, MyNum);
            }

            //나가기
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom() // 방을 성공적으로 나갔을 때 호출되는 콜백
    {
        Debug.Log("놀이를 성공적으로 종료했습니다.");

        //로딩바 ui 애니메이션 보여주기
        LoadingSceneController.Instance.LoadScene("Main");
    }

    // 모두가 수행하는 작업 ui관련
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) // 플레이어가 게임 도중 방을 나갔을 때
    {
        Debug.Log($"나간 유저의 액터넘버: {otherPlayer.ActorNumber}");

        // 플레이어 목록에서 현재 플레이어의 인덱스를 찾음
        int currentIndex = Array.IndexOf(userProfileLoad.sortedPlayers, otherPlayer.ActorNumber);

        // 1. 나간 해당 유저의 프로필 두가지 버전 모두 비활성화
        userProfileLoad.InRoomUserList[currentIndex].gameObject.SetActive(false);
        InTurnUserList[currentIndex].gameObject.SetActive(false);

        // 만약 현재 방에 있는 플레이어가 2명 미만이라면 - 결과는 정해짐
        if (userProfileLoad.sortedPlayers.Length < 2)
        {
            if (IsMyTurn) //현재 내 턴일 때
            {
                if (TurnRoutine != null)
                {
                    StopCoroutine(TurnRoutine); // 현재 코루틴 중지
                }
                TurnRoutine = null;
            }
            Debug.Log($"현재 플레이어가 2명 미만으로 게임이 종료됩니다.");

            gameResult.ResultPanel.SetActive(true); // 게임 결과 판넬 띄우기
        }
    }


}
