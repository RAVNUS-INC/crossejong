using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  // DoTween »ç¿ë

public class PlayRoomCardDeck : MonoBehaviour
{
    public Transform cardDeck;
    public Transform usercardAreaContent;
    public GameObject cardPrefab;
    public int numberOfCards = 11;
    public float cardSpacing = 100f;

    // Start is called before the first frame update
    void Start()
    {
        DealCards();
    }

    void DealCards()
    {
        for (int i = 0; i < numberOfCards; i++)
        {
            GameObject userCard = Instantiate(cardPrefab, cardDeck.position, Quaternion.identity, cardDeck);
            Vector3 targetPosition = GetCardTargetPosition(i);
            userCard.transform.SetParent(usercardAreaContent);
            userCard.transform.localScale = Vector3.one;
            userCard.transform.DOMove(targetPosition, 1f).SetEase(Ease.OutQuad).OnComplete()
        }
    }

    Vector3 GetCardTargetPosition(int cardIndex)
    {
        float xPosition = cardIndex * cardSpacing;
        return new Vector3(usercardAreaContent.position.x + xPosition, usercardAreaContent.position.y, usercardAreaContent.position.z);
    }
}
