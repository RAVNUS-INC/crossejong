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
        Debug.Log("나는 방장인지 검사 중");
        // 정렬된 리스트를 반복문으로 순차적으로 처리
        if (userProfileLoad.sortedPlayers[count] == UserInfoManager.instance.MyActNum)
        {
            Debug.Log("내가 카드를 배분받을 차례");
            Debug.Log($"나는 현재 {count}번째 유저: Num {userProfileLoad.sortedPlayers[count]}");

            List<GameObject> randomCards = cardPool.GetRandomCardsObject(RandomNames); //랜덤인덱스에 해당하는 오브젝트 추가
            cardPool.MoveCardsToTarGetArea(randomCards, userCardContainer, displayedCards);

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
            //Button cardButton = card.AddComponent<Button>();

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


}
