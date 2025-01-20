using UnityEngine;
using UnityEngine.EventSystems;

// CardDrop Ŭ������ �巡�� ������ ī�尡 ��ӵ� �� �ִ� ������ �����ϴ� ��ũ��Ʈ
public class CardDrop : MonoBehaviour
{
    public GameObject[] dropSlots; // ��� ������ ���Ե��� �����ϴ� �迭

    void Awake()
    {
        // ��� DropSlot�� �ʱ�ȭ �� ��Ȱ��ȭ ���·� ����
        foreach (var slot in dropSlots)
        {
            slot.SetActive(false); // DropSlot ������Ʈ�� ��Ȱ��ȭ
        }
    }

    public void ShowDropSlots()
    {
        // �巡�� ���� �� ��� DropSlot�� ȭ�鿡 ǥ��
        foreach (var slot in dropSlots)
        {
            slot.SetActive(true); // DropSlot ������Ʈ�� Ȱ��ȭ
        }
    }

    public void HideDropSlots()
    {
        // �巡�� ���� �� ��� DropSlot�� ȭ�鿡�� ����
        foreach (var slot in dropSlots)
        {
            slot.SetActive(false); // DropSlot ������Ʈ�� ��Ȱ��ȭ
        }
    }

    public GameObject GetHoveredDropSlot(Vector2 pointerPosition)
    {
        // �巡�� ���� �� ���콺 Ŀ���� ��ġ�� ������ Ȯ��
        foreach (var slot in dropSlots)
        {
            RectTransform slotRect = slot.GetComponent<RectTransform>(); // ������ RectTransform�� ������
            // ���콺 Ŀ���� ������ ���� �ȿ� �ִ��� Ȯ��
            if (RectTransformUtility.RectangleContainsScreenPoint(slotRect, pointerPosition))
            {
                return slot; // ���콺 Ŀ���� ���� ���� ������ �ش� ���� ��ȯ
            }
        }
        return null; // ���� ���� ���� ������ null ��ȯ
    }
}
