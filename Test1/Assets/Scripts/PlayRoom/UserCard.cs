using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using TMPro;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using static UserProfileLoad;
using System.Linq;
using Photon.Realtime;

public class UserCard : MonoBehaviourPun
{
    public static UserCard instance = null;

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
        
    }

    public CardPool cardPool; // CardPool 참조 
    public FieldCard fieldCard;
    public CardDrag cardDrag;
    public Transform userCardContainer; // UserCardArea의 Contents
    public List<GameObject> displayedCards; // UserCardArea에서 보여지는 카드 리스트

    //서버 연결 시 주석 해제------------------------------------
    private UserProfileLoad userProfileLoad; // UserProfileLoad 참조
    private List<UserProfileLoad.Player> players; // 플레이어 리스트
    private int[] sortedPlayers; // 정렬된 플레이어 리스트
                                 //서버 연결 시 주석 해제------------------------------------


    //UserCardArea로 11개의 랜덤 카드 이동
    public void FirstUserCardArea()
    {
        UserProfileLoad userProfileLoad = FindObjectOfType<UserProfileLoad>(); //  찾기
        players = userProfileLoad.GetPlayers(); // players 리스트에 접근
        Debug.Log($"players 리스트 길이: {players.Count}");

        // players 리스트를 myActNum 기준으로 오름차순 정렬
        sortedPlayers = players.OrderBy(player => player.myActNum)
                                   .Select(player => player.myActNum)
                                   .ToArray();
        Debug.Log($"오름차순 정렬 완료: {string.Join(", ", sortedPlayers)}");

        // 정렬된 플레이어 리스트와 함께 모든 유저에게 전달
        photonView.RPC("SyncSortedPlayers", RpcTarget.All, sortedPlayers);

        for (int i = 0; i < sortedPlayers.Length; i++) //players수만큼 반복
        {
            // 방장만 랜덤으로 11장의 카드 인덱스를 뽑음
            // 직렬화(list->int[])수행(rpc함수는 list를 인자로 받지 못함)
            string[] randomnames = cardPool.GetRandomCardsName(11);

            // 방장이 자신을 포함한 모든 유저에게 11장의 카드를 추가, 배치하도록 요청
            photonView.RPC("AddCardObjectToAll", RpcTarget.All, randomnames, i);
        }
    }

    // 플레이어 리스트를 순환하며 자신의 카드 추가하기
    [PunRPC]
    void AddCardObjectToAll(string[] RandomNames, int count)
    {
        Debug.Log("카드 추가를 수행하는 중");

        // 정렬된 리스트를 반복문으로 순차적으로 처리
        if (sortedPlayers[count] != PhotonNetwork.LocalPlayer.ActorNumber) return; //해당 인덱스 플레이어의 actnum이 나와 같다면 다음 수행
        Debug.Log($"현재 {count}번째 유저: 액터넘버 {sortedPlayers[count]}");
        foreach (string name in RandomNames) //뽑은 리스트들 기존 변수에 저장
        {
            Debug.Log($"{name}");
        }
        List<GameObject> randomCards = cardPool.GetRandomCardsObject(RandomNames); //랜덤인덱스에 해당하는 오브젝트 추가
        cardPool.MoveCardsToTarGetArea(randomCards, userCardContainer, displayedCards);
    }

    // 정렬된 플레이어 리스트 동기화
    [PunRPC]
    void SyncSortedPlayers(int[] sortedActNums)
    {
        sortedPlayers = sortedActNums;
        Debug.Log("정렬된 플레이어 리스트 동기화 완료");
    }


    public void SelectedUserCard()
    {
        for (int i = 0; i < displayedCards.Count; i++) { 
            GameObject card = displayedCards[i];

            Button cardButton = card.AddComponent<Button>();

            cardDrag = card.GetComponent<CardDrag>();
            if (cardDrag == null)
            {
                cardDrag = card.AddComponent<CardDrag>(); // CardDrag 컴포넌트 추가
                cardDrag.onDragParent = ObjectManager.instance.moveItemPanel;
            }

            // 카드가 드래그 가능한 상태로 설정 (필요한 초기화나 설정을 추가할 수 있음)
            cardButton.onClick.AddListener(() =>
            {
                // 카드가 클릭되었을 때 (원하는 동작을 추가할 수 있음)
                // 예: 카드 색상 변경, 드래그 시작 처리 등
                cardDrag.OnBeginDrag(null);  // 예시로 OnBeginDrag 호출 (이 부분은 실제로는 PointerEventData가 필요하므로 적절히 수정)
            });

            cardDrag.cardIndex = i;
        }
    }
}
