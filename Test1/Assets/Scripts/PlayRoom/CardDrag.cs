using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.Drawing;
using System;
using Photon.Pun;
using TMPro;

// CardDrag 클래스는 Unity에서 드래그 가능한 카드를 구현하는 스크립트
public class CardDrag : MonoBehaviourPun, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public UserCard userCard; // UserCard 참조
    public GameObject beingDraggedCard; // 드래그 중인 카드
    private Vector3 startPosition; // 카드 시작 위치
    [SerializeField] public Transform onDragParent; // 드래그 중 카드가 위치할 부모
    [HideInInspector] public Transform startParent; // 원래 부모
    public int cardIndex;


    public void OnBeginDrag(PointerEventData eventData)
    {
        ObjectManager.instance.AlaramMsg.gameObject.SetActive(false);

        if (eventData == null) return;
        //if (eventData.pointerId >= 0) return;

        print(gameObject.name);

        ObjectManager.instance.moveCardObejct = gameObject;

        // 카드의 원래 위치와 부모를 저장
        startPosition = transform.position;
        startParent = transform.parent;

        ObjectManager.instance.startDragPosition = startPosition;

        // 카드가 UI 요소일 경우 Raycast 차단을 해제
        GetComponent<Image>().raycastTarget = false;

        // 드래그할 카드의 부모를 onDragParent로 변경
        transform.SetParent(onDragParent);

    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPosition;

        // PC와 모바일 입력 처리
        if (Input.touchCount > 0)
        {
            // 모바일 터치 입력
            newPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, -Camera.main.transform.position.z));
        }
        else
        {
            // PC 마우스 입력
            newPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        }

        // 드래그 중에는 카드가 마우스 또는 터치 위치를 따르도록 설정
        transform.position = newPosition;

        // 상태 메시지 업데이트 함수 호출
        ObjectManager.instance.ShowCardSelectingMessage(true);

        //eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = true;// Raycast를 다시 활성화

        // 드래그가 끝난 후, 카드가 원래 위치로 돌아오도록 처리
        if (transform.parent == onDragParent)
        {
            transform.position = startPosition;
            transform.SetParent(startParent);
            transform.SetSiblingIndex(cardIndex);
            ObjectManager.instance.moveCardObejct = null;

            CardPool.instance.SortCardIndex(UserCard.instance.displayedCards);

            // 카드 필드로 옮기면 UserCard, UserCardPopup 에서 옮긴거 제거
            // 일단 카드 넣으면 그리드 개수 변경 알아서
        }
    }

    
}