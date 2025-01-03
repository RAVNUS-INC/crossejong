using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StartCard : MonoBehaviour
{
    /*
    public UserCard userCard; // UserCard 참조

    void Start()
    {
        
    }

    public void CreateStartCard()
    {
        // 이미 표시된 카드가 있다면 제거
        foreach (var card in displayedCards)
        {
            Destroy(card);
        }
        displayedCards.Clear(); // 리스트 초기화
        selectedCardIndices.Clear(); // 선택된 카드 인덱스 초기화

        // 랜덤으로 11개의 카드 인덱스를 선택
        while (selectedCardIndices.Count < 11)
        {
            int randomIndex = Random.Range(0, cardPrefabs.Count);
            selectedCardIndices.Add(randomIndex);
        }

        // 선택된 카드 인덱스에 따라 카드 생성
        int i = 0;
        foreach (int index in selectedCardIndices)
        {
            GameObject cardInstance = Instantiate(cardPrefabs[index], cardContainer);
            RectTransform rectTransform = cardInstance.GetComponent<RectTransform>();

            // 카드 크기 설정 (200x200)
            rectTransform.sizeDelta = new Vector2(200, 200);

            displayedCards.Add(cardInstance); // 생성된 카드를 리스트에 추가
            i++;
        }
    }
    */
}