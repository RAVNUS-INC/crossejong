//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections.Generic;

//public class NewCardPool : MonoBehaviour
//{
//    public Transform cardContainer; // 카드 부모 오브젝트
//    public List<GameObject> cardList = new List<GameObject>(); // 카드 목록

//    void Start()
//    {
//        // 카드 생성
//        CreateCard();
//    }


//    private void CreateCard()
//    {
//        // 카드 1~54 생성
//        for (int i = 0; i < 54; i++)
//        {
//            Button card = Instantiate(cardPrefab, cardContainer);
//            BeforeCard cardComponent = card.GetComponent<BeforeCard>();

//            // 앞면과 뒷면 이미지 설정
//            cardComponent.InitializeCard(cardBackImage, cardFrontSprites[i]);

//            // 카드 리스트에 추가
//            cards.Add(card);
//        }

//    }

//    // 랜덤으로 11개의 카드 선택
//    public List<GameObject> GetRandomCards(int count)
//    {
//        List<GameObject> randomCards = new List<GameObject>();
//        HashSet<int> usedIndices = new HashSet<int>();

//        while (randomCards.Count < count)
//        {
//            int randomIndex = Random.Range(0, cards.Count);
//            if (!usedIndices.Contains(randomIndex))
//            {
//                usedIndices.Add(randomIndex);
//                randomCards.Add(cards[randomIndex]);
//            }
//        }

//        return randomCards;
//    }


//    // 카드 이동
//    public void MoveCardToParent(GameObject card, Transform parent)
//    {
//        card.transform.SetParent(parent, false);
//    }
//}
