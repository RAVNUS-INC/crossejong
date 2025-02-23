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
    public string Displayname; //�ڽ��� �г���
    public const string DISPLAYNAME_KEY = "DisplayName"; // ������ DisplayName
    public TMP_Text StatusMsg; //ī�带 ���� ���� ���� ǥ�� �ؽ�Ʈ
    public List<string> cardFrontRed;
    public List<string> cardFrontBlack;
    public List<string> cardFrontSpecial;

    //���� ���� �� �ּ� ����------------------------------------
    public List<string> usedIndices = new List<string>();
    //���� ���� �� �ּ� ����------------------------------------

    private void Start()
    {
        Displayname = PlayerPrefs.GetString(DISPLAYNAME_KEY);
        StatusMsg.text = ""; //���¸޽��� ����
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
        // �巡�� ��(ī�� ���� ��)�� �˸��� �޽����� ��ο��� ǥ��
        photonView.RPC("ShowDragStatus", RpcTarget.All, isDragging, ObjectManager.instance.Displayname);
    }
   
    [PunRPC]
    private void ShowDragStatus(bool isDragging, string name)
    {
        if (isDragging)
        {
            ObjectManager.instance.StatusMsg.text = $"{name}���� ī�带 ���� ��..";
        }
        else
        {
            ObjectManager.instance.StatusMsg.text = $"{name}���� �ܾ �Է� ��..";
        }
        
    }

}
