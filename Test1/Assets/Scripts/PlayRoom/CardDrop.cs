using UnityEngine;
using UnityEngine.EventSystems;

// CardDrop 클래스는 드래그 가능한 카드가 드롭될 수 있는 슬롯을 관리하는 스크립트
public class CardDrop : MonoBehaviour, IDropHandler
{
    public CardDrag cardDrag;

    GameObject Card()
    {
        if (transform.childCount > 0)
            return transform.GetChild(0).gameObject;
        else
            return null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (Card() == null)
        {
            cardDrag.beingDraggedCard.transform.SetParent(transform);
            cardDrag.beingDraggedCard.transform.position = transform.position;
        }
    }
}
