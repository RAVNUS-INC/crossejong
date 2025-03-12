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
using DG.Tweening.Plugins;

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
    public UserProfileLoad userProfileLoad; // UserProfileLoad 참조
    public UserCardFullPopup userCardFullPopup; // 턴이 아닐 때 카드 객체 선택 방지를 위해 사용
    public Countdown countDown; // 11장 배분 뒤 모두에게 요청 위해


    //UserCardArea로 11개의 랜덤 카드 이동
    public void FirstUserCardArea()
    {
        for (int i = 0; i < userProfileLoad.sortedPlayers.Length; i++) //players수만큼 반복
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
        // 정렬된 리스트를 반복문으로 순차적으로 처리
        if (userProfileLoad.sortedPlayers[count] == UserInfoManager.instance.MyActNum)
        {
            Debug.Log($"나는 현재 {count}번째 유저: Num {userProfileLoad.sortedPlayers[count]}");

            List<GameObject> randomCards = cardPool.GetRandomCardsObject(RandomNames); //랜덤인덱스에 해당하는 오브젝트 추가
            cardPool.GetCardsToTarGetArea(randomCards, userCardContainer, displayedCards); // 디스플레이 카드 상태 업데이트

            // 카드를 배분받은 뒤, 드롭 영역 생성 수행
            fieldCard.CreateDropAreas();
        }
        else // 자신의 차례가 아니면 끝내기
        {
            return;
        }
    }

    public void SelectedUserCard(List<GameObject> userLists)
    {
        for (int i = 0; i < userLists.Count; i++) { 
            GameObject card = userLists[i];

            Button cardButton = card.GetComponent<Button>();
            if (cardButton == null)
            {
                cardButton = card.AddComponent<Button>();
            }

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
        // 내 카드들의 선택을 비활성화
        DeActivateCard(displayedCards);

        // 내 카드들의 선택을 비활성화 - 팝업
        DeActivateCard(userCardFullPopup.fullDisplayedCards);
    }

    public void DeActivateCard(List<GameObject> fullDisplayedCards)
    {
        // 만약 내 턴이 아니라면
        if (!ObjectManager.instance.IsMyTurn)
        {
            // 현재 팝업에 있는 카드들의 선택을 비활성화하기
            foreach (GameObject obj in fullDisplayedCards)
            {
                Image image = obj.GetComponent<Image>();
                if (image != null)
                {
                    image.raycastTarget = false;  // 이미지 비활성화
                }
            }

        }
        else
        {
            // 현재 팝업에 있는 카드들의 선택을 활성화하기
            foreach (GameObject obj in fullDisplayedCards)
            {
                Image image = obj.GetComponent<Image>();
                if (image != null)
                {
                    image.raycastTarget = true;  // 이미지 활성화
                }
            }
        }
    }

    [PunRPC]
    void RemoveRollCard(string cardNames) // 롤백리스트를 받아 해당 위치 찾고 빈 객체로 만들기
    {
        // 받은 string을 다시 List<string>으로 변환
        List<string> rollbackList = new List<string>(cardNames.Split(','));

        foreach (string cardName in rollbackList) // 카드 이름 배열을 순회
        {
            for (int x = 0; x < 7; x++) // 보드의 X축 순회
            {
                for (int y = 0; y < 7; y++) // 보드의 Y축 순회
                {
                    if (ObjectManager.instance.grid[x, y].transform.childCount == 1)
                    {
                        GameObject card = ObjectManager.instance.grid[x, y].transform.gameObject; // 자식(카드) 가져오기

                        //Debug.Log($"카드 {card.name} 발견");

                        if (card.name == cardName) // 카드 이름이 일치하면
                        {
                            // 최상위 부모 객체에서 현재 자식들을 모두 분리
                            ObjectManager.instance.grid[x, y].transform.DetachChildren();

                            // 최상위 부모 객체에서 현재 자식들을 모두 분리
                            ObjectManager.instance.grid[x, y].transform.parent.DetachChildren();

                            Debug.Log($"보드에서 카드 {cardName} 제거 완료");
                            Debug.Log(ObjectManager.instance.grid[x, y].transform.childCount);
                        }  
                    }
                }
            }
        }
        // 드롭영역 업데이트
        fieldCard.RollBackColorAreas();
    }
}
