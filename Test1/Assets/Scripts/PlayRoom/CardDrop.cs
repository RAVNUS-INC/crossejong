using UnityEngine;
using UnityEngine.EventSystems;

// CardDrop Ŭ������ �巡�� ������ ī�尡 ��ӵ� �� �ִ� ������ �����ϴ� ��ũ��Ʈ
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
