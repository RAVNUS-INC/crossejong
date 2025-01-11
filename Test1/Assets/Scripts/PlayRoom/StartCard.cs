//using UnityEngine;
//using UnityEngine.UI;
//using DG.Tweening;
//using System.Collections.Generic;

//public class StartCard : MonoBehaviour
//{
//    public NewCardPool newCardPool;
//    public Transform userCardContainer; // UserCardArea의 Contents
//    public List<GameObject> displayedCard; // UserCardArea에서 보여지는 카드 리스트

//    public void MoveUserCardArea()
//    {
//        List<GameObject> randomCards = newCardPool.GetRandomCards(1); // 1개의 랜덤 카드 얻기
//        foreach (var card in randomCards)
//        {
//            newCardPool.MoveCardToParent(card, userCardContainer); // 각 카드를 UserCardArea로 이동
//            card.SetActive(true); // 카드가 보이도록 활성화
//            displayedCard.Add(card); // 이동된 카드를 리스트에 추가
//        }
//    }

//    void Start()
//    {

//    }
//}