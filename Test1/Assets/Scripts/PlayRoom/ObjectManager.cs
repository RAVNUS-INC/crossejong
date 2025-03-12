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
    public List<string> rollBackList; // 롤백하면 삭제하기 위해 담아둔 문자열 리스트
    public List<GameObject> createdWordList;
    public string inputWords;
    public int gridCount = 9;
    public GameObject[,] grid;
    public int dropCount = 0;
    public TMP_Text StatusMsg; //카드를 놓는 중의 상태 표시 텍스트
    public List<string> cardFrontRed = new List<string> { "ㄱ", "ㅇ", "ㅎ" };
    public List<string> cardFrontBlack; //난이도에 따라 내용이 달라짐
    public List<string> cardFrontSpecial = new List<string> { "C", "B" };
    public List<string> usedIndices = new List<string>(); // 롤백 시에는 해당 문자를 리스트에서 제거해야함
    public bool IsFirstTurn = true; // 첫 시작을 알리는 bool 변수
    public bool IsCardDrop = false;  // 카드를 드래그해서 드롭했을 때 true -> rpc함수 호출의 조건이 됨
    public bool IsMyTurn = false;  // 내 턴인지 아닌지에 따라 드래그 및 버튼 활성화
    public int MyCompleteWordCount = 0; // 나의 단어 완성 횟수 변수


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


}
