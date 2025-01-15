using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // DoTween 네임스페이스 추가

public class CardDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 initialPosition;
    private Canvas canvas;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        initialPosition = rectTransform.anchoredPosition;
        rectTransform.DOScale(1.1f, 0.2f); // 드래그 시작 시 크기 확대
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.DOScale(1f, 0.2f); // 드래그 종료 시 원래 크기로 복원
        rectTransform.DOAnchorPos(initialPosition, 0.3f).SetEase(Ease.OutBack); // 원래 위치로 부드럽게 복원
    }
}
