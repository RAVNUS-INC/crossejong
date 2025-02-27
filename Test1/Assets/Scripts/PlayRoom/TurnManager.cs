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

public class TurnManager : MonoBehaviourPun
{
    public UserProfileLoad userProfileLoad; // 유저프로필 이미지 참조를 위해 사용

    public GameObject[] InTurnUserList; // 턴에 있는 상태의 유저 이미지 배열
    public Image[] InTurnUserImg; // 턴에 있는 유저들의 프로필사진
    public TMP_Text[] InTurnUserName; // 턴에 있는 유저들의 닉네임
    public TMP_Text[] timerText; // 남은 시간을 보여주는 텍스트
    public Color overlayColor = new Color(0, 0, 0, 0.7f); // 검정색 70%
    //private float remainingTime = 5f; // 30초 타이머
    private int MyNum, NextPlayerNum; // 다음 플레이어의 액터넘버
    public bool IsMyTurn = false; // 현재 턴인지 아닌지 확인
    Coroutine TurnRoutine;

    private void Start()
    {
        // 현재 내 액터넘버 찾기
        MyNum = PhotonNetwork.LocalPlayer.ActorNumber;
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
        float remainingTime = 5f;

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

        //photonView.RPC("UpdateTimerRPC", RpcTarget.All, MyNum, 0);

        Debug.Log("시간 초과! 턴을 넘깁니다.");

        // 다음 턴의 플레이어 찾기 
        FindNextPlayer();
    }

    public void FindNextPlayer()
    {
        // 다음 플레이어의 넘버 찾기(마지막 플레이어일 경우 0번 인덱스로 순환)
        // 플레이어 목록에서 현재 플레이어의 인덱스를 찾음
        Debug.Log(MyNum);
        int currentIndex = Array.IndexOf(userProfileLoad.sortedPlayers, MyNum);
        Debug.Log(currentIndex);
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
}
