using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObjectManager : MonoBehaviourPun
{
    public CardPool cardPool; //카드 생성을 위해 연결
    public static ObjectManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(transform.parent.gameObject);
    }

    public Transform moveItemPanel;
    public GameObject moveCardObejct;
    public bool isFullPopup;
    public int score;
    public int time;
    public bool isDragged = false;
    public List<GameObject> emptyList;
    public int cardIndexX;
    public int cardIndexY;
    public string createdWord;
    public string createdWords;
    public List<string> rollBackList; // 최종 전달할 문자열 리스트
    public List<int> FinIndexX; //최종 전달할 카드들의 x좌표 배열들
    public List<int> FinIndexY; //최종 전달할 카드들의 y좌표 배열들
    public List<GameObject> createdWordList;
    public string inputWords;
    public int gridCount;
    public GameObject[,] grid;
    public int dropCount = 0;
    public TMP_Text StatusMsg; //카드를 놓는 중의 상태 표시 텍스트
    public TMP_Text AlaramMsg; //잘못 놓았을 때 알람 텍스트
    public List<string> cardFrontRed = new List<string> { "ㄱ", "ㅇ", "ㅎ" };
    public List<string> cardFrontBlack; //난이도에 따라 내용이 달라짐
    public List<string> cardFrontSpecial = new List<string> { "C", "B" };
    public List<string> usedIndices = new List<string>(); // 롤백 시에는 해당 문자를 리스트에서 제거해야함
    public bool IsFirstTurn = true; // 첫 시작을 알리는 bool 변수
    public bool IsCardDrop = false;  // 카드를 드래그해서 드롭했을 때 true -> rpc함수 호출의 조건이 됨
    public bool IsMyTurn = false;  // 내 턴인지 아닌지에 따라 드래그 및 버튼 활성화
    public bool AllUsedCard = false; // 54장의 카드를 다 썼는지 아닌지에 따라 상태 변경\
    public bool EndMyTurn = false; // 나의 턴이 게임 내에서 종료되었음을 나타냄.
    public List<int> turnExcluded = new List<int>(); // 턴 제외 리스트 관리
    public int MyCompleteWordCount = 0; // 나의 단어 완성 횟수 변수
    public int MyIndexNum; //게임 내 나의 UI 인덱스번호
    public Button RollBackBtn; //롤백버튼은 놓은 게 있으면 활성화
    public Vector3 startDragPosition; // 드래그 시작 위치


    private void Start()
    {
        // 방의 난이도 불러오기
        if (PhotonNetwork.InRoom)
        {
            Room room = PhotonNetwork.CurrentRoom;

            // 'DifficultyContents' 키에 저장된 값을 가져와서 바로 List<string>으로 변환
            if (room.CustomProperties.ContainsKey("DifficultyContents"))
            {
                string[] difficultyContentsArray = (string[])room.CustomProperties["DifficultyContents"];

                /// string[]을 List<string>으로 변환
                List<string> difficultyContentsList = new List<string>(difficultyContentsArray);

                // 난이도 내용을 변수에 전달
                cardFrontBlack = difficultyContentsList;

                // 이제 List<string>으로 사용 가능
                Debug.Log(string.Join(", ", cardFrontBlack));

                // 난이도 반영된 카드로 생성
                cardPool.CreateCard();
            }
        }
        //상태메시지 비우기
        StatusMsg.text = "";

        AlaramMsg.gameObject.SetActive(false); // 알람메시지 처음엔 안보이게

        RollBackBtn.gameObject.SetActive(false); //롤백버튼 비활
    }

    public void SortAfterMove() 
    {
        if (isFullPopup)
        {
            CardPool.instance.SortCardIndex(UserCardFullPopup.instance.fullDisplayedCards);
        }
        else
        {
            CardPool.instance.SortCardIndex(UserCard.instance.displayedCards);
        }
    }
    public void ShowCardSelectingMessage(bool isDragging)
    {
        // 드래그 중(카드 고르는 중)을 알리는 메시지를 모두에게 표시
        photonView.RPC("ShowDragStatus", RpcTarget.All, isDragging, UserInfoManager.instance.MyName);
    }
   
    [PunRPC]
    private void ShowDragStatus(bool isDragging, string name)
    {
        if (isDragging)
        {
            ObjectManager.instance.StatusMsg.text = $"{name}님이 카드를 고르는 중..";
        }
        else
        {
            ObjectManager.instance.StatusMsg.text = $"{name}님이 단어를 입력 중..";
        }
    }

    // 메시지를 2초 동안 띄우는 메서드
    public void ShowMessageFor2Seconds(string message)
    {
        StartCoroutine(ShowMessageCoroutine(message));
    }

    private IEnumerator ShowMessageCoroutine(string message)
    {
        AlaramMsg.text = message;  // 메시지 설정
        yield return new WaitForSeconds(2.5f);  // 2초 기다리기
        AlaramMsg.text = "";  // 메시지 제거
    }
}
