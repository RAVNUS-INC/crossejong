using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResult : MonoBehaviourPunCallbacks
{
    public GameObject ResultPanel; // 놀이결과 판넬
    public GameObject[] ResultUserList; // 놀이결과 유저들의 리스트
    public Image[] ResultUserImg; // 놀이결과 유저들의 프로필사진
    public TMP_Text[] ResultUserName, ResultUserTime; // 놀이결과 유저들의 닉네임, 카드 소진 시간

    void Start()
    {
        
    }

    // sortedplayers리스트에 있는 유저들을 바탕으로 결과창 유저 표시(끝까지 게임을 플레이한 유저들)

    // 카드를 가장 먼저 소진한 유저들 시간을 오름차순으로 정리한 리스트로부터 결과 표시

    // 2명일 경우 -> 한명이 카드 다 소진 -> 결과 바로 표시
    // 3명 이상일 경우 -> 한명이 카드 다 소진 -> 나머지 유저들끼리 놀이 진행 -> 결과 표시
    // 본인이 카드를 소진하면 방장에게 알림을 주면 됨. 방장은 현재 존재하는 유저들 중 카드 소진된 유저들이 1명을 제외한 모두라면 게임 결과창을 띄우도록 모두에게 요청.


    // 개개인마다 카드 배분 시점부터 카드 소진(0장) 시점까지 코루틴 함수 시간 측정 실행

    // 결과 확인 버튼 누르면 메인으로 돌아가도록
    // 확인 버튼을 누르지 않아도 15초뒤에 메인으로 자동으로 이동하도록 알림메시지 수행하기

    public void OnConfirmButton() // 게임 결과 확인 버튼을 눌렀을 때 -> 메인 이동
    {
        if (PhotonNetwork.InRoom)
        {

            Debug.Log($"확인 버튼 클릭. 메인으로 이동합니다.");

            //로딩바 ui 애니메이션 보여주기
            LoadingSceneController.Instance.LoadScene("Main");

            // 방 나가기
            PhotonNetwork.LeaveRoom();

            // 메인 이동 후 다시 방생성 시도 -> makeroom 씬 전환 문제 발생
        }
    }

    public override void OnLeftRoom() // 방을 성공적으로 나왔을 때
    {
        // 마지막 남은 유저가 나갈때 출력될 메시지
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("빈 방 자동 삭제 확인");
        }
    }
}
