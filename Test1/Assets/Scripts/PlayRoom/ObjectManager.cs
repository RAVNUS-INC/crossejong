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
    public List<GameObject> createdWordList;
    public string inputWords;
    public int gridCount = 9;
    public GameObject[,] grid;
    public int dropCount = 0;
    public string Displayname; //�ڽ��� �г���
    public const string DISPLAYNAME_KEY = "DisplayName"; // ������ DisplayName
    public TMP_Text StatusMsg; //ī�带 ���� ���� ���� ǥ�� �ؽ�Ʈ
    public List<string> cardFrontRed = new List<string> { "��", "��", "��" };
    public List<string> cardFrontBlack; //���̵��� ���� ������ �޶���
    public List<string> cardFrontSpecial = new List<string> { "C", "B" };
    public List<string> usedIndices = new List<string>();
    public bool IsFirstTurn = true; // ù ������ �˸��� bool ����
    public bool IsCardDrop = false; // ī�带 �巡���ؼ� ������� �� true -> rpc�Լ� ȣ���� ������ ��
    public bool IsMyTurn = false; // �� ������ �ƴ����� ���� �巡�� �� ��ư Ȱ��ȭ
    public int MyCompleteWordCount = 0; // ���� �ܾ� �ϼ� Ƚ�� ����


    private void Start()
    {
        // ���� ���̵� �ҷ�����
        if (PhotonNetwork.InRoom)
        {
            Room room = PhotonNetwork.CurrentRoom;

            // 'DifficultyContents' Ű�� ����� ���� �����ͼ� �ٷ� List<string>���� ��ȯ
            if (room.CustomProperties.ContainsKey("DifficultyContents"))
            {
                string[] difficultyContentsArray = (string[])room.CustomProperties["DifficultyContents"];

                // string[]�� List<string>���� ��ȯ
                List<string> difficultyContentsList = new List<string>(difficultyContentsArray);

                // ���̵� ������ ������ ����
                cardFrontBlack = difficultyContentsList;

                // ���� List<string>���� ��� ����
                Debug.Log(string.Join(", ", cardFrontBlack));
            }
        }
        //���¸޽��� ����
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
        // �巡�� ��(ī�� ������ ��)�� �˸��� �޽����� ��ο��� ǥ��
        photonView.RPC("ShowDragStatus", RpcTarget.All, isDragging, UserInfoManager.instance.MyName);
    }
   
    [PunRPC]
    private void ShowDragStatus(bool isDragging, string name)
    {
        if (isDragging)
        {
            ObjectManager.instance.StatusMsg.text = $"{name}���� ī�带 ������ ��..";
        }
        else
        {
            ObjectManager.instance.StatusMsg.text = $"{name}���� �ܾ �Է� ��..";
        }  
    }


}
