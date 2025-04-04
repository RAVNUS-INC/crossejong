using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using static UnityEngine.EventSystems.EventTrigger;

public class CardManager : MonoBehaviourPunCallbacks
{
    public static CardManager instance = null;
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

    public List<GameObject> CopyCards = new List<GameObject>(); //카드복제본(원본)

    // 룸 프로퍼티 키
    const string DECK_KEY = "Deck"; // 전체 카드 덱 (초기 54장)
    const string USED_CARDS_KEY = "UsedCards"; // 사용된 카드 리스트      
    const string FIELD_CARD_KEY = "FirstCards"; // 보드판 첫 카드 한 장

    // 롤백 프로퍼티 키
    const string DROPPED_CARDS_KEY = "DroppedCards";

    // 각 유저별 키 (예: Cards_1, Cards_2 ...)
    public string PLAYER_CARDS_KEY(int actorNumber) => $"Cards_{actorNumber}";

    public bool IsStart = true; //카드 배분 첫 시작을 나타냄


    //게임 시작 시 초기화 및 카드 배분--방장 호출에 수행(딱 한번만)
    public void InitializeAndDistributeCards()
    {
        // 카드 풀에서 카드를 생성한 뒤에 사용해야함
        List<GameObject> cards = CardPool.instance.cards;
        
        // 카드 섞기
        Shuffle(cards);

        Hashtable roomProps = new Hashtable();

        roomProps[DECK_KEY] = string.Join(",", cards); //원본 카드 문자열 리스트 저장
        roomProps[USED_CARDS_KEY] = ""; // 처음에 사용한 카드 리스트 초기화

        var players = PhotonNetwork.CurrentRoom.Players.Values.ToList();
        int numPlayers = players.Count;
        int cardsPerPlayer = GetCardsPerPlayer(numPlayers); // 2명=11장, 3명=10장 ...

        int currentIndex = 0;

        foreach (var player in players)
        {
            // 현재 인덱스에서 cardsPerPlayer만큼 카드 이름 가져오기(섞은 후 앞에서부터 ??장씩 연속으로 가져옴-인덱스 증가 후 반복)
            List<string> playerCardNames = cards
                .Skip(currentIndex)
                .Take(cardsPerPlayer)
                .Select(card => card.name)
                .ToList();

            currentIndex += cardsPerPlayer;

            // 해당 플레이어의 카드 목록을 저장 
            roomProps[PLAYER_CARDS_KEY(player.ActorNumber)] = string.Join(",", playerCardNames);

            Debug.Log($"현재 넘버: {player.ActorNumber}");
            Debug.Log($"카드리스트: {roomProps[PLAYER_CARDS_KEY(player.ActorNumber)]}");
        }

        // 사용된 카드 목록도 카드 이름으로 저장
        List<string> usedCardNames = cards
            .Take(currentIndex)
            .Select(card => card.name)
            .ToList();

        roomProps[USED_CARDS_KEY] = string.Join(",", usedCardNames);
        //Debug.Log("사용된 카드 전체: " + roomProps[USED_CARDS_KEY]);

        // 필드카드 한 장 뽑기
        if (currentIndex < cards.Count) // 남아있는 카드가 있는지 확인
        {
            string extraCard = cards[currentIndex].name;
            roomProps[FIELD_CARD_KEY] = extraCard;
            //Debug.Log($"첫 카드: {extraCard}");

            currentIndex++; // 추가 카드를 사용했으므로 인덱스 증가

            usedCardNames.Add(extraCard);
            roomProps[USED_CARDS_KEY] = string.Join(",", usedCardNames);
        }
        Debug.Log("최종 사용된 카드 전체: " + roomProps[USED_CARDS_KEY]);

        // 방 커스텀 프로퍼티에 반영
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
    }

    public void DropCardsUpdate(string cardName, int x, int y) //카드 드롭 시 보드판에 프로퍼티 업데이트
    {
        Hashtable DropProps = new Hashtable();

        string newEntry = $"{cardName}:{x}:{y}";
        string updatedValue = newEntry;

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(DROPPED_CARDS_KEY))
        {
            string existing = (string)PhotonNetwork.CurrentRoom.CustomProperties[DROPPED_CARDS_KEY];
            updatedValue = existing + "," + newEntry;
        }

        DropProps[DROPPED_CARDS_KEY] = updatedValue;
        PhotonNetwork.CurrentRoom.SetCustomProperties(DropProps); //프로퍼티 누적 업데이트

        //UI관련 업데이트
        TurnChange.instance.CardDropBtn.interactable = true; //카드 내기 버튼 활성화
        ObjectManager.instance.RollBackBtn.gameObject.SetActive(true); // 롤백버튼 활성화
        ObjectManager.instance.dropCount += 1; //드롭 횟수 증가

        ObjectManager.instance.IsCardDrop = false;
    }

    public void RollBackCardsUpdate() //카드 롤백 시 보드판에 프로퍼티 업데이트(롤백 수행의 첫 시작)
    {
        //현재 DROPPED_CARDS_KEY 값이 드롭할 때만 갱신되기 때문에 새로 생성해 업데이트
        Hashtable props = new Hashtable
        {
            { "ROLLBACK_REQUEST", true }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }


    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) // 누군가에 의해 속성이 변경되면 자동 호출되는 함수
    {
        int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        string myCardsKey = PLAYER_CARDS_KEY(myActorNumber);

        if (propertiesThatChanged.ContainsKey(myCardsKey))
        {
            // 자신의 카드 배치, 필드카드 배치 수행(딱 한번만)
            string cardString = (string)propertiesThatChanged[myCardsKey];
            string[] myCardNames = cardString.Split(',');

            // 유저 카드 실제 객체 생성
            List<GameObject> MyCards = CardPool.instance.GetRandomCardsObject(myCardNames);
            CardPool.instance.GetCardsToTarGetArea(MyCards, UserCard.instance.userCardContainer, UserCard.instance.displayedCards); // 디스플레이 카드 상태 업데이트
            CardPool.instance.SortCardIndex(UserCard.instance.displayedCards);

            // 유저 카드를 배분받은 뒤, 드롭 영역 생성 수행
            FieldCard.instance.CreateDropAreas();

            // 첫 카드 필드에 배치 시작
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(FIELD_CARD_KEY))
            {
                string firstCardName = PhotonNetwork.CurrentRoom.CustomProperties[FIELD_CARD_KEY] as string;
                List<GameObject> FieldCardObject = CardPool.instance.GetRandomCardsObject(firstCardName);

                CardPool.instance.GetCardsToTarGetArea(FieldCardObject, FieldCard.instance.fieldContainer, FieldCard.instance.fieldDisplayedCards);
                GameObject middleObejcts = ObjectManager.instance.grid[ObjectManager.instance.gridCount / 2, ObjectManager.instance.gridCount / 2];
                GameObject firstCards = FieldCardObject[0];
                ObjectManager.instance.grid[ObjectManager.instance.gridCount / 2, ObjectManager.instance.gridCount / 2].SetActive(true);
                firstCards.transform.SetParent(middleObejcts.transform, false);
                ObjectManager.instance.grid[ObjectManager.instance.gridCount / 2, ObjectManager.instance.gridCount / 2] = firstCards;

                firstCards.transform.parent.name = firstCards.transform.name;
            }
            FieldCard.instance.OnOffDropAreas(); // 드롭 가능 영역 업데이트
            TurnChange.instance.TurnEnd(); //카드 개수 세기

            //첫 게임 준비과정을 수행했으니 다른 상황에 의해 속성이 바뀌어도 위 코드가 다시는 실행되지 않도록 함
            IsStart = false;

            if (PhotonNetwork.IsMasterClient)
            {
                TurnManager.instance.AfterCountdown(); //방장부터 턴 시작
            }
            else { return; }
        }

        //누군가 드롭한 결과를 바로 반영
        if (propertiesThatChanged.ContainsKey(DROPPED_CARDS_KEY))
        {

            string updatedValue = (string)propertiesThatChanged[DROPPED_CARDS_KEY];

            if (!string.IsNullOrEmpty(updatedValue))
            {
                // 값이 "CardA:1:2,CardB:2:3" 처럼 누적된 경우, 마지막 값만 분리
                string[] entries = updatedValue.Split(',');
                string lastEntry = entries[entries.Length - 1]; // 마지막 항목

                string[] parts = lastEntry.Split(':');
                if (parts.Length == 3)
                {
                    string cardName = parts[0];
                    int x = int.Parse(parts[1]);
                    int y = int.Parse(parts[2]);

                    // 원하는 함수 호출
                    FieldCard.instance.SyncDropCard(cardName, x, y);
                }
            }
        }

        //누군가 롤백한 결과를 바로 반영
        if (propertiesThatChanged.ContainsKey("ROLLBACK_REQUEST"))
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(DROPPED_CARDS_KEY))
            {
                string droppedData = (string)PhotonNetwork.CurrentRoom.CustomProperties[DROPPED_CARDS_KEY];
                FieldCard.instance.SyncRollCard(droppedData);

                Hashtable props = new Hashtable
                {
                    { DROPPED_CARDS_KEY, "" } // 빈 문자열로 덮어쓰기
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
        }

    }

    int GetCardsPerPlayer(int playerCount)
    {
        switch (playerCount)
        {
            case 2: return 11;
            case 3: return 10;
            case 4: return 9;
            case 5: return 7;
            default: return 11; // 기본값 (예외처리)
        }
    }

    private void Shuffle(List<GameObject> deck) // 카드 섞기
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (deck[i], deck[j]) = (deck[j], deck[i]);
        }
    }
}
