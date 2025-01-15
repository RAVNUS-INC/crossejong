using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // DoTween ���ӽ����̽� �߰�

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
        rectTransform.DOScale(1.1f, 0.2f); // �巡�� ���� �� ũ�� Ȯ��
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
        rectTransform.DOScale(1f, 0.2f); // �巡�� ���� �� ���� ũ��� ����
        rectTransform.DOAnchorPos(initialPosition, 0.3f).SetEase(Ease.OutBack); // ���� ��ġ�� �ε巴�� ����
    }
}
