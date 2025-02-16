using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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

    //서버 연결 시 주석 해제------------------------------------
    public List<string> usedIndices = new List<string>();
    //서버 연결 시 주석 해제------------------------------------

    public void SortAfterMove() {
        if (isFullPopup)
        {
            CardPool.instance.SortCardIndex(UserCardFullPopup.instance.fullDisplayedCards);
        }
        else
        {
            CardPool.instance.SortCardIndex(UserCard.instance.displayedCards);
        }
    }

    // 모든 플레이어에게 단어 전송
    [PunRPC]
    public void ShowCreatedWords(string word)
    {
        createdWords = word;
        Debug.Log($"새 단어: {word}");

    }

}
