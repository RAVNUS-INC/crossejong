using UnityEngine;
using UnityEngine.EventSystems;

// CardDrop 클래스는 드래그 가능한 카드가 드롭될 수 있는 슬롯을 관리하는 스크립트
public class CardDrop : MonoBehaviour
{
    public GameObject[] dropSlots; // 드롭 가능한 슬롯들을 저장하는 배열

    void Awake()
    {
        // 모든 DropSlot을 초기화 시 비활성화 상태로 설정
        foreach (var slot in dropSlots)
        {
            slot.SetActive(false); // DropSlot 오브젝트를 비활성화
        }
    }

    public void ShowDropSlots()
    {
        // 드래그 시작 시 모든 DropSlot을 화면에 표시
        foreach (var slot in dropSlots)
        {
            slot.SetActive(true); // DropSlot 오브젝트를 활성화
        }
    }

    public void HideDropSlots()
    {
        // 드래그 종료 시 모든 DropSlot을 화면에서 숨김
        foreach (var slot in dropSlots)
        {
            slot.SetActive(false); // DropSlot 오브젝트를 비활성화
        }
    }

    public GameObject GetHoveredDropSlot(Vector2 pointerPosition)
    {
        // 드래그 종료 시 마우스 커서가 위치한 슬롯을 확인
        foreach (var slot in dropSlots)
        {
            RectTransform slotRect = slot.GetComponent<RectTransform>(); // 슬롯의 RectTransform을 가져옴
            // 마우스 커서가 슬롯의 영역 안에 있는지 확인
            if (RectTransformUtility.RectangleContainsScreenPoint(slotRect, pointerPosition))
            {
                return slot; // 마우스 커서가 슬롯 위에 있으면 해당 슬롯 반환
            }
        }
        return null; // 슬롯 위에 있지 않으면 null 반환
    }
}
