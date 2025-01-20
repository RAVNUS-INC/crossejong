using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

// CardDrag Ŭ������ Unity���� �巡�� ������ ī�带 �����ϴ� ��ũ��Ʈ
public class CardDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 initialPosition; // ī���� �ʱ� ��ġ�� ����
    private Transform originalParent; // ī���� ���� �θ� Transform�� ����
    private Canvas canvas; // ī�带 �巡���ϱ� ���� ������ Canvas
    private RectTransform rectTransform; // ī���� RectTransform ����
    private CardDrop cardDrop; // DropSlot�� �����ϴ� CardDrop ��ũ��Ʈ ����

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>(); // ī���� RectTransform ������Ʈ�� ������
        canvas = GetComponentInParent<Canvas>(); // �θ� ������Ʈ �� Canvas�� ������
        cardDrop = FindObjectOfType<CardDrop>(); // CardDrop ��ũ��Ʈ�� ���� ������Ʈ�� ã�� ����
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        initialPosition = rectTransform.anchoredPosition; // �巡�� ���� �� ī���� �ʱ� ��ġ�� ����
        originalParent = transform.parent; // �巡�� ���� �� ī���� ���� �θ� Transform�� ����
        rectTransform.DOScale(1.1f, 0.2f); // �巡�� ���� �� ī�� ũ�⸦ 1.1��� Ȯ�� (�ִϸ��̼� 0.2��)
        transform.SetParent(canvas.transform); // ī���� �θ� Canvas�� ���� (�ֻ��� ���̾�� �̵�)
        cardDrop.ShowDropSlots(); // ��� ������ ������ ȭ�鿡 ǥ��
    }

    public void OnDrag(PointerEventData eventData)
    {
        // �巡�� �� ���콺�� ȭ�� ��ǥ�� Canvas�� ���� ��ǥ�� ��ȯ
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, // ��ȯ ������ �� RectTransform
                eventData.position, // ���콺�� ȭ�� ��ǥ
                eventData.pressEventCamera, // ���콺�� Ŭ���� ī�޶�
                out Vector2 localPoint)) // ��ȯ�� ���� ��ǥ�� ������ ����
        {
            rectTransform.anchoredPosition = localPoint; // ī���� ��ġ�� ���콺 ��ġ�� ���� ������Ʈ
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.DOScale(1f, 0.2f); // �巡�� ���� �� ī�� ũ�⸦ ���� ũ��(1)�� ���� (�ִϸ��̼� 0.2��)
        cardDrop.HideDropSlots(); // �巡�� ���� �� ��� ������ ������ ȭ�鿡�� ����

        // �巡�װ� ����� ������ ���콺�� � DropSlot ���� �ִ��� Ȯ��
        GameObject hoveredSlot = cardDrop.GetHoveredDropSlot(eventData.position);

        if (hoveredSlot != null) // ���콺�� ��� ������ ���� ���� �ִ� ���
        {
            transform.SetParent(hoveredSlot.transform); // ī���� �θ� �ش� �������� ����
            rectTransform.anchoredPosition = Vector2.zero; // ������ �߽ɿ� ī�� ��ġ ����
        }
        else // ��� ������ ���� ���� ���� ���� ���
        {
            rectTransform.DOAnchorPos(initialPosition, 0.3f).SetEase(Ease.OutBack);
            // ���� ��ġ�� �ε巴�� ���� (�ִϸ��̼� 0.3��, Ease.OutBack ȿ�� ���)
            transform.SetParent(originalParent); // ī���� �θ� ���� �θ�� ����
        }
    }
}
