//using UnityEngine;
//using UnityEngine.EventSystems;
//using DG.Tweening;

//// CardDrag Ŭ������ Unity���� �巡�� ������ ī�带 �����ϴ� ��ũ��Ʈ
//public class CardDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
//{
//    public UserCard userCard; // UserCard ����
//    public GameObject beingDraggedCard; // �巡�� ���� ī��
//    private Vector3 startPosition; // ī�� ���� ��ġ
//    [SerializeField] private Transform onDragParent; // �巡�� �� ī�尡 ��ġ�� �θ�
//    [HideInInspector] public Transform startParent; // ���� �θ�

//public void OnBeginDrag(PointerEventData eventData)
//    {
//        // �巡�� ���� �ÿ� displayedCards���� ī�带 ã�� ����
//        foreach (var card in userCard.displayedCards)
//        {
//            if (card.GetComponent<Collider2D>().OverlapPoint(eventData.position)) // �巡�� ���� ��ġ�� �ִ� ī�带 ã��
//            {
//                beingDraggedCard = card; // �巡���� ī�� �Ҵ�
//                break; // ã�� ī��� �巡�� ����
//            }
//        }

//        // ī���� ���� ��ġ�� �θ� ����
//        startPosition = transform.position;
//        startParent = transform.parent;

//        // ī�尡 UI ����� ��� Raycast ������ ����
//        GetComponent<CanvasGroup>().blocksRaycasts = false;

//        // �巡���� ī���� �θ� onDragParent�� ����
//        transform.SetParent(onDragParent);
//    }

//    public void OnDrag(PointerEventData eventData)
//    {
//        // �巡�� �߿��� ī�尡 ���콺 ��ġ�� �������� ����
//        transform.position = Input.mousePosition;
//    }

//    public void OnEndDrag(PointerEventData eventData)
//    {
//        // �巡�� ����
//        beingDraggedCard = null; // �巡�� ���� ī�尡 ������ ����
//        GetComponent<CanvasGroup>().blocksRaycasts = true; // Raycast�� �ٽ� Ȱ��ȭ

//        // �巡�װ� ���� ��, ī�尡 ���� ��ġ�� ���ƿ����� ó��
//        if (transform.parent == onDragParent)
//        {
//            transform.position = startPosition;
//            transform.SetParent(startParent);
//        }
//    }


//}