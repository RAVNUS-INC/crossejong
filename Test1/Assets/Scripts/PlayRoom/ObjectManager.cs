using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectManager : MonoBehaviourPun
{
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
    public int gridCount = 9;
    public GameObject[,] grid;
    public int dropCount = 0;
    public string Displayname; //자신의 닉네임
    public const string DISPLAYNAME_KEY = "DisplayName"; // 유저의 DisplayName
    public TMP_Text StatusMsg; //카드를 놓는 중의 상태 표시 텍스트
    public List<string> cardFrontRed;
    public List<string> cardFrontBlack;
    public List<string> cardFrontSpecial;

    //서버 연결 시 주석 해제------------------------------------
    public List<string> usedIndices = new List<string>();
    //서버 연결 시 주석 해제------------------------------------

    private void Start()
    {
        Displayname = PlayerPrefs.GetString(DISPLAYNAME_KEY);
        StatusMsg.text = ""; //상태메시지 비우기
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
        photonView.RPC("ShowDragStatus", RpcTarget.All, isDragging, ObjectManager.instance.Displayname);
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
