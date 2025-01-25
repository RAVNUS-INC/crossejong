//using UnityEngine;
//using UnityEngine.EventSystems;
//using DG.Tweening;

//// CardDrag 클래스는 Unity에서 드래그 가능한 카드를 구현하는 스크립트
//public class CardDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
//{
//    public UserCard userCard; // UserCard 참조
//    public GameObject beingDraggedCard; // 드래그 중인 카드
//    private Vector3 startPosition; // 카드 시작 위치
//    [SerializeField] private Transform onDragParent; // 드래그 중 카드가 위치할 부모
//    [HideInInspector] public Transform startParent; // 원래 부모

//public void OnBeginDrag(PointerEventData eventData)
//    {
//        // 드래그 시작 시에 displayedCards에서 카드를 찾아 선택
//        foreach (var card in userCard.displayedCards)
//        {
//            if (card.GetComponent<Collider2D>().OverlapPoint(eventData.position)) // 드래그 시작 위치에 있는 카드를 찾기
//            {
//                beingDraggedCard = card; // 드래그할 카드 할당
//                break; // 찾은 카드로 드래그 시작
//            }
//        }

//        // 카드의 원래 위치와 부모를 저장
//        startPosition = transform.position;
//        startParent = transform.parent;

//        // 카드가 UI 요소일 경우 Raycast 차단을 해제
//        GetComponent<CanvasGroup>().blocksRaycasts = false;

//        // 드래그할 카드의 부모를 onDragParent로 변경
//        transform.SetParent(onDragParent);
//    }

//    public void OnDrag(PointerEventData eventData)
//    {
//        // 드래그 중에는 카드가 마우스 위치를 따르도록 설정
//        transform.position = Input.mousePosition;
//    }

//    public void OnEndDrag(PointerEventData eventData)
//    {
//        // 드래그 종료
//        beingDraggedCard = null; // 드래그 중인 카드가 없도록 설정
//        GetComponent<CanvasGroup>().blocksRaycasts = true; // Raycast를 다시 활성화

//        // 드래그가 끝난 후, 카드가 원래 위치로 돌아오도록 처리
//        if (transform.parent == onDragParent)
//        {
//            transform.position = startPosition;
//            transform.SetParent(startParent);
//        }
//    }


//}