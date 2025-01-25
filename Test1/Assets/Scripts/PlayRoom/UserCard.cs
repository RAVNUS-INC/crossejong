using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using TMPro;
using Photon.Pun.Demo.PunBasics;

public class UserCard : MonoBehaviour
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


    // UserCardArea로 11개의 랜덤 카드 이동
    public void FirstUserCardArea()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(11); // 11개의 랜덤 카드 얻기
        cardPool.MoveCardsToTarGetArea(randomCards, userCardContainer, displayedCards);
    }


    public void SelectedUserCard()
    {
        for (int i = 0; i < displayedCards.Count; i++) { 
            GameObject card = displayedCards[i];

            Button cardButton = card.GetComponent<Button>();

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
