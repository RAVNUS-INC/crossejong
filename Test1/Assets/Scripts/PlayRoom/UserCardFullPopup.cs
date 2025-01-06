using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

public class UserCardFullPopup : MonoBehaviour
{
    public CardPool cardPool; // CardPool 참조 
    public UserCard userCard; // UserCard 참조
    public Transform fullCardContainer; // FullPopupArea의 Content
    public List<GameObject> fullDisplayedCards; // FullPopup에서 보여지는 카드 리스트

    void Start()
    {
        // Optional: 초기화 작업
    }

    // FullPopupArea로 11개의 랜덤 카드 이동
    public void MoveFullPopupArea()
    {
        List<GameObject> userCards = userCard.displayedCards; // UserCard에서 표시된 카드들 가져오기

        // 카드를 5개씩 묶어서 행(row)으로 생성
        List<GameObject> rowCards = new List<GameObject>();
        for (int i = 0; i < userCards.Count; i++)
        {
            rowCards.Add(userCards[i]);

            // 5개 카드마다 새로운 행을 생성
            if (rowCards.Count == 5 || i == userCards.Count - 1)
            {
                CreateRow(rowCards, fullDisplayedCards.Count); // 새로운 행 생성
                rowCards.Clear(); // 행이 끝났으므로 리스트 초기화
            }
        }

        //// 모든 카드들을 FullPopupArea로 이동
        //foreach (var card in userCards)
        //{
        //    cardPool.MoveCardToParent(card, fullCardContainer); // 각 카드를 FullPopupArea로 이동
        //    card.SetActive(true); // 카드가 보이도록 활성화
        //    fullDisplayedCards.Add(card); // 이동된 카드를 리스트에 추가
        //}
    }


    // UserFullPopupExit 버튼 클릭 시, FullPopup에서 UserCardArea로 카드를 복귀
    public void MoveCardsBackToUserCardArea()
    {
        foreach (var card in fullDisplayedCards)
        {
            cardPool.MoveCardToParent(card, userCard.userCardContainer); // 각 카드를 UserCardArea로 이동
            card.SetActive(true); // 카드가 보이도록 활성화
            userCard.displayedCards.Add(card); // UserCard의 리스트에 추가
        }

        // FullPopup에서 보여진 카드를 초기화
        fullDisplayedCards.Clear();
    }

    void CreateRow(List<GameObject> rowCards, int rowIndex)
    {
        GameObject row = new GameObject("Row " + rowIndex);
        row.transform.SetParent(fullCardContainer, false);

        // RectTransform을 설정합니다.
        RectTransform rowTransform = row.GetComponent<RectTransform>();
        if (rowTransform == null)
        {
            rowTransform = row.AddComponent<RectTransform>();
        }
        rowTransform.sizeDelta = new Vector2(1080, 200); // 행 크기 설정

        // HorizontalLayoutGroup을 추가합니다.
        HorizontalLayoutGroup layoutGroup = row.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.padding = new RectOffset(15, 0, 0, 0); // 왼쪽 여백 설정
        layoutGroup.spacing = 20; // 카드 간 간격 설정
        layoutGroup.childAlignment = TextAnchor.MiddleLeft; // 왼쪽 정렬
        layoutGroup.childForceExpandWidth = false; // 자식 너비 강제 확장 비활성화
        layoutGroup.childForceExpandHeight = false; // 자식 높이 강제 확장 비활성화

        // 각 카드를 행에 추가합니다.
        foreach (var card in rowCards)
        {
            GameObject cardInstance = Instantiate(card, row.transform);
            RectTransform cardRect = cardInstance.GetComponent<RectTransform>();
            if (cardRect != null)
            {
                cardRect.sizeDelta = new Vector2(180, 180); // 카드 크기 180x180으로 설정
            }
        }

        // 마지막 행에 카드가 5개가 아닌 경우, 빈 공간을 채워서 X 좌표 맞추기
        int remainingSpace = 5 - rowCards.Count;
        for (int i = 0; i < remainingSpace; i++)
        {
            GameObject spacer = new GameObject("Spacer " + i);
            RectTransform spacerRect = spacer.AddComponent<RectTransform>();
            spacer.transform.SetParent(row.transform, false);
            spacerRect.sizeDelta = new Vector2(180, 180); // Spacer의 크기를 카드 크기와 동일하게 설정
            LayoutElement layoutElement = spacer.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 0; // Spacer가 남는 공간을 차지하지 않도록 설정
            layoutElement.minWidth = 180; // Spacer의 최소 너비를 카드 크기와 동일하게 설정
            layoutElement.preferredWidth = 180; // Spacer의 선호 너비를 카드 크기와 동일하게 설정
        }
    }

}
