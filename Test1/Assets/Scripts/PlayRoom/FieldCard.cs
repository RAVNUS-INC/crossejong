using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Xml;
using PlayFab.DataModels;

public class FieldCard : MonoBehaviour
{
    public Transform FieldContainer; // FieldArea의 Contents
    public CardPool cardPool; // CardPool 참조
    public List<string> shownCardData; // 필드에 놓인 카드 데이터 리스트
    public List<GameObject> displayedFieldCard; 
    void Start()
    {

    }

    public void StartCardShown()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1개의 랜덤 카드 얻기
        foreach (var card in randomCards)
        {
            cardPool.MoveCardToParent(card, FieldContainer); // 각 카드를 UserCardArea로 이동
            card.SetActive(true); // 카드가 보이도록 활성화
            displayedFieldCard.Add(card); // 이동된 카드를 리스트에 추가
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }


    void ShowCapableCardArea()
    {

    }

    void SaveCardData()
    {

    }

}
