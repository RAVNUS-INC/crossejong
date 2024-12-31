using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class UserCard : MonoBehaviour
{
    public List<GameObject> cardPrefabs; // 카드 프리팹 리스트
    public Transform cardContainer; // Scroll View의 Content
    public Button createCardButton; // 카드 생성 버튼

    public List<GameObject> displayedCards = new List<GameObject>(); // 화면에 표시된 카드 리스트
    public HashSet<int> selectedCardIndices = new HashSet<int>(); // 선택된 카드 인덱스 집합

    void Start()
    {
        createCardButton.onClick.AddListener(OnCreateCard); // 버튼 클릭 이벤트에 카드 생성 메서드 추가
    }

    void OnCreateCard()
    {
        createCardButton.gameObject.SetActive(false); // 게임 시작 버튼 누르면 사라지게 하기

        CreateCards(); // 카드 생성 로직
    }

    public void CreateCards()
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

    /*
    // 선택된 카드 인덱스를 반환하는 메서드 추가
    public HashSet<int> GetUsedCardIndices()
    {
        return selectedCardIndices;
    }
    */
}
