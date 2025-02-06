using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스
using System.Collections;
using UnityEngine.UI;
using Photon.Pun;

public class Countdown : //MonoBehaviour

    //-----------(서버 연결 시 주석해제)------------
    MonoBehaviourPun

{
    public TMP_Text countDownText; // TextMeshPro 사용
    public UserCard userCard;  // UserCard 참조
    public float startDelay = 1f; // 시작 딜레이
    public Button startGameButton; //게임 시작버튼
    public FieldCard fieldCard;

    private void Start()
    {
        //startGameButton.onClick.AddListener(StartCountDown);

        //서버 연결 시 주석 해제------------------------------------
        // 방장만 시작버튼 활성화, 실행 가능
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.onClick.AddListener(() =>
            photonView.RPC("StartCountDown", RpcTarget.All));
        }
    }

    //서버 연결 시 주석 해제------------------------------------
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

    //서버 연결 시 주석 해제------------------------------------
    private void StartGame()
    {
        //서버 연결 시 주석 해제------------------------------------
        //방장만 먼저 첫번째 카드를 고른다
        if (PhotonNetwork.IsMasterClient)
        {
            userCard.FirstUserCardArea();
            
        }
        // userCard.FirstUserCardArea();
        fieldCard.CreateDropAreas();

        if (PhotonNetwork.IsMasterClient)
        {
            fieldCard.FirstFieldCard();
        }
        // fieldCard.FirstFieldCard();
        userCard.SelectedUserCard();

    }
}
