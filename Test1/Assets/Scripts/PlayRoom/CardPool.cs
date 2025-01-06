using System.Collections.Generic;
using UnityEngine;

public class CardPool : MonoBehaviour
{
    public GameObject cardPrefab; // 카드 프리팹
    public Transform cardContainer; // 카드 부모 오브젝트
    public List<GameObject> cards = new List<GameObject>(); // 카드 목록

    public Sprite cardBackImage; // 카드 뒷면 이미지
    public List<Sprite> cardFrontSprites; // 카드 앞면 이미지 목록

    void Start()
    {
        // 카드 생성
        CreateCard();
    }


    private void CreateCard()
    {
        // 카드 1~54 생성
        for (int i = 0; i < 54; i++)
        {
            GameObject card = Instantiate(cardPrefab, cardContainer);
            Card cardComponent = card.GetComponent<Card>();

            // 앞면과 뒷면 이미지 설정
            cardComponent.InitializeCard(cardBackImage, cardFrontSprites[i]);

            // 카드 리스트에 추가
            cards.Add(card);
        }

    }

    // 랜덤으로 11개의 카드 선택
    public List<GameObject> GetRandomCards(int count)
    {
        List<GameObject> randomCards = new List<GameObject>();
        HashSet<int> usedIndices = new HashSet<int>();

        while (randomCards.Count < count)
        {
            int randomIndex = Random.Range(0, cards.Count);
            if (!usedIndices.Contains(randomIndex))
            {
                usedIndices.Add(randomIndex);
                randomCards.Add(cards[randomIndex]);
            }
        }

        return randomCards;
    }


    // 카드 이동
    public void MoveCardToParent(GameObject card, Transform parent)
    {
        card.transform.SetParent(parent, false);
    }
}
