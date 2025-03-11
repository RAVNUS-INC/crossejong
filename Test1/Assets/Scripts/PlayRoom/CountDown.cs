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
    public GameObject WaitingPanel; // 모두가 접속하기 전까지 보이는 패널(대기상태표시)
    public Image fieldArea; // 보드판 활성화를 위해
    private bool isCountingDown = false;

    public FieldCard fieldCard;
    public TurnManager turnMananger;
    public TurnChange turnChange;

    private void Start()
    {
        WaitingPanel.SetActive(true); // 맨 처음엔 패널 활성화

        // 방장만 시작버튼 활성화, 실행 가능
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    startGameButton.onClick.AddListener(() =>
        //    photonView.RPC("StartCountDown", RpcTarget.All));
        //}
    }

    [PunRPC]
    private void StartCountDown()
    {
        WaitingPanel.SetActive(false); // 게임 시작했으므로 패널 비활성화

        fieldArea.gameObject.SetActive(true); // 카운트다운 숫자가 보이게

        if (isCountingDown) return; // 이미 실행 중이면 중복 실행 방지
        isCountingDown = true;

        StartCoroutine(CountDownRoutine(1)); // 타이머 시작
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

        if (PhotonNetwork.IsMasterClient)
        {
            StartGame(); // 방장이라면 이 함수 수행
        }

        isCountingDown = false; // 타이머 종료 후, 다시 호출 가능하도록 설정
    }

    private void StartGame() // 방장만 수행
    {
        userCard.FirstUserCardArea(); // 방장이 카드를 몇장씩 뽑아 플레이어들에게 나눠줌

        fieldCard.FirstFieldCard(); // 방장이 첫 카드 뽑아 모두에게 수행 추가 요청

        // 각자 모두 현재 카드 개수 세기 요청
        photonView.RPC("LetsCardCount", RpcTarget.All);

        // 방장부터 첫 카운트 다운 시작
        turnMananger.AfterCountdown();
    }


    [PunRPC]
    private void LetsCardCount() // 자신의 카드 개수 업데이트
    {
        turnChange.TurnEnd(); 
    }
}
