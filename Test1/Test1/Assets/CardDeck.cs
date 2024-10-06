using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardDeck : MonoBehaviour
{
    [System.Serializable]
    public class Card
    {
        public string cardName; // 카드 이름
        public Sprite cardImage; // 카드 이미지 (Sprite 타입)
    }

    public List<Card> cardList = new List<Card>(); // 카드 리스트 (인스펙터에서 설정 가능)
    public GameObject cardPrefab; // 카드 프리팹
    public Transform cardContainer; // 카드를 배치할 컨테이너
    public float cardSpacing = 115f; // 카드 간격
    public Button createCardButton; // 카드 생성 버튼
    private List<Card> selectedCards = new List<Card>(); // 이미 선택된 카드 리스트

    void Start()
    {
        createCardButton.onClick.AddListener(OnCreateCard); // 버튼 클릭 이벤트에 카드 생성 메서드 추가
    }

    private void OnCreateCard()
    {
        CreateCard(); // 카드 생성 로직
        createCardButton.interactable = false; // 버튼을 비활성화
    }

    public void CreateCard()
    {
        // 카드가 11개 이하일 때만 생성
        if (selectedCards.Count >= cardList.Count)
        {
            Debug.Log("모든 카드가 이미 선택되었습니다.");
            return;
        }

        for (int i = 0; i < 11; i++) // 11장의 카드를 생성
        {
            int randomIndex;
            Card selectedCard;

            do
            {
                randomIndex = Random.Range(0, cardList.Count);
                selectedCard = cardList[randomIndex];
            } while (selectedCards.Contains(selectedCard)); // 이미 선택된 카드인지 확인

            selectedCards.Add(selectedCard); // 선택된 카드 리스트에 추가

            GameObject cardInstance = Instantiate(cardPrefab, cardContainer);
            cardInstance.GetComponent<Image>().sprite = selectedCard.cardImage; // 카드 이미지 설정
            cardInstance.name = selectedCard.cardName; // 카드 이름 설정

            RectTransform rectTransform = cardInstance.GetComponent<RectTransform>();

            if (i == 0)
            {
                rectTransform.anchoredPosition = new Vector2(385, 142);
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(385 + (i * cardSpacing), 142);
            }

            Debug.Log($"Card Name: {selectedCard.cardName}, Card Image: {selectedCard.cardImage}");
        }
    }
}
