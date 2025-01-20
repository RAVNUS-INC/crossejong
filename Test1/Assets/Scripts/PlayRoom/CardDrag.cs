using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

// CardDrag 클래스는 Unity에서 드래그 가능한 카드를 구현하는 스크립트
public class CardDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 initialPosition; // 카드의 초기 위치를 저장
    private Transform originalParent; // 카드의 원래 부모 Transform을 저장
    private Canvas canvas; // 카드를 드래그하기 위해 참조할 Canvas
    private RectTransform rectTransform; // 카드의 RectTransform 참조
    private CardDrop cardDrop; // DropSlot을 관리하는 CardDrop 스크립트 참조

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>(); // 카드의 RectTransform 컴포넌트를 가져옴
        canvas = GetComponentInParent<Canvas>(); // 부모 오브젝트 중 Canvas를 가져옴
        cardDrop = FindObjectOfType<CardDrop>(); // CardDrop 스크립트를 가진 오브젝트를 찾아 참조
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        initialPosition = rectTransform.anchoredPosition; // 드래그 시작 시 카드의 초기 위치를 저장
        originalParent = transform.parent; // 드래그 시작 시 카드의 원래 부모 Transform을 저장
        rectTransform.DOScale(1.1f, 0.2f); // 드래그 시작 시 카드 크기를 1.1배로 확대 (애니메이션 0.2초)
        transform.SetParent(canvas.transform); // 카드의 부모를 Canvas로 설정 (최상위 레이어로 이동)
        cardDrop.ShowDropSlots(); // 드롭 가능한 슬롯을 화면에 표시
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 중 마우스의 화면 좌표를 Canvas의 로컬 좌표로 변환
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, // 변환 기준이 될 RectTransform
                eventData.position, // 마우스의 화면 좌표
                eventData.pressEventCamera, // 마우스를 클릭한 카메라
                out Vector2 localPoint)) // 변환된 로컬 좌표를 저장할 변수
        {
            rectTransform.anchoredPosition = localPoint; // 카드의 위치를 마우스 위치에 따라 업데이트
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.DOScale(1f, 0.2f); // 드래그 종료 시 카드 크기를 원래 크기(1)로 복원 (애니메이션 0.2초)
        cardDrop.HideDropSlots(); // 드래그 종료 시 드롭 가능한 슬롯을 화면에서 숨김

        // 드래그가 종료된 시점에 마우스가 어떤 DropSlot 위에 있는지 확인
        GameObject hoveredSlot = cardDrop.GetHoveredDropSlot(eventData.position);

        if (hoveredSlot != null) // 마우스가 드롭 가능한 슬롯 위에 있는 경우
        {
            transform.SetParent(hoveredSlot.transform); // 카드의 부모를 해당 슬롯으로 변경
            rectTransform.anchoredPosition = Vector2.zero; // 슬롯의 중심에 카드 위치 고정
        }
        else // 드롭 가능한 슬롯 위에 있지 않은 경우
        {
            rectTransform.DOAnchorPos(initialPosition, 0.3f).SetEase(Ease.OutBack);
            // 원래 위치로 부드럽게 복귀 (애니메이션 0.3초, Ease.OutBack 효과 사용)
            transform.SetParent(originalParent); // 카드의 부모를 원래 부모로 복원
        }
    }
}
