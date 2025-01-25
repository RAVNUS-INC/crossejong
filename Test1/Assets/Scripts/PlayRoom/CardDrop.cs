using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// CardDrop 클래스는 드래그 가능한 카드가 드롭될 수 있는 슬롯을 관리하는 스크립트
public class CardDrop : MonoBehaviour, IDropHandler
{
    public CardDrag cardDrag;

    private bool HasCard()
    {
        if (transform.childCount > 0)
            return true;
        else
            return false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject card = ObjectManager.instance.moveCardObejct;

        if (card != null && !HasCard())
        {
            card.GetComponent<Image>().raycastTarget = true;
            card.transform.position = this.transform.position;
            card.transform.SetParent(transform);
            ObjectManager.instance.moveCardObejct = null;
            ObjectManager.instance.SortAfterMove();
            CardDrag CD = card.GetComponent<CardDrag>();
            if (CD != null) Destroy(card.GetComponent<CardDrag>());
        }
    }
}