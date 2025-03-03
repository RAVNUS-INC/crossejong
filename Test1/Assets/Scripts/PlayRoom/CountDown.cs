using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스
using System.Collections;
using UnityEngine.UI;
using Photon.Pun;

public class Countdown : MonoBehaviourPun

{
    public TMP_Text countDownText; // TextMeshPro 사용
    public UserCard userCard;  // UserCard 참조
    public float startDelay = 1f; // 시작 딜레이
    public Button startGameButton; //게임 시작버튼
    public FieldCard fieldCard;
    public TurnManager turnMananger;
    public TurnChange turnChange;

    private void Start()
    {
        // 방장만 시작버튼 활성화, 실행 가능
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.onClick.AddListener(() =>
            photonView.RPC("StartCountDown", RpcTarget.All));
        }
    }

    [PunRPC]
    private void StartCountDown()
    {
        StartCoroutine(CountDownRoutine(1));
    }

    private IEnumerator CountDownRoutine(int count)
    {
        // 카운트다운 표시
        while (count > 0)
        {
            countDownText.text = count.ToString(); // TMP_Text로 설정
            yield return new WaitForSeconds(1f);
            count--;
        }

        // "시작!" 표시
        countDownText.text = "Start!"; // TMP_Text로 설정
        yield return new WaitForSeconds(startDelay);

        countDownText.gameObject.SetActive(false); // 카운트다운 텍스트 숨김
        StartGame();

    }

    private void StartGame()
    {
        //방장만 먼저 첫번째 카드를 고른다
        if (PhotonNetwork.IsMasterClient)
        {
            userCard.FirstUserCardArea();
            // 방장이 완료되었음을 알림 (RPC 호출)
            photonView.RPC("FirstUserCompleted", RpcTarget.All);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            fieldCard.FirstFieldCard();
            // 방장이 완료되었음을 알림 (RPC 호출)
            photonView.RPC("FirstFieldCompleted", RpcTarget.All);
        }

        // 첫사람(방장)부터 타이머 시작
        if (PhotonNetwork.IsMasterClient)
        {
            // 각자 모두 현재 카드 개수 세기 요청
            photonView.RPC("LetsCardCount", RpcTarget.All);

            // 방장부터 첫 카운트 다운 시작
            turnMananger.AfterCountdown();
        }

    }

    [PunRPC]
    private void FirstUserCompleted()
    {
        //Debug.Log("FirstUserCompleted 완료, 다음 진행");
        fieldCard.CreateDropAreas();
    }

    [PunRPC]
    private void FirstFieldCompleted()
    {
        //Debug.Log("FirstFieldCompleted 완료, 다음 진행");
        userCard.SelectedUserCard();
    }

    [PunRPC]
    private void LetsCardCount() // 자신의 카드 개수 업데이트
    {
        turnChange.TurnEnd(); 
    }
}
