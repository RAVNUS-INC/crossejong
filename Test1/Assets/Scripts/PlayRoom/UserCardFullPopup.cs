using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UserCardFullPopup : MonoBehaviour
{
    public GameObject userCardFullPanel; // 보유카드 전체보기 팝업 패널
    public Button openUserCardFullPopupButton; // 보유카드 전체보기 팝업 열기 버튼
    public Button closeUserCardFullPopupButton; // 보유카드 전체보기 팝업 닫기 버튼
    public Transform fullPopupAreaContent; // FullPopupArea의 Content (스크롤 뷰의 내용물)
    public GameObject userCardArea; // 보유카드 영역

    public UserCard userCard; // UserCard 스크립트 참조
    public List<GameObject> displayedFullCards = new List<GameObject>(); // 사용자에게 표시된 전체 카드 리스트

    void Start()
    {
        UserCardFullPopupf();
    }

    public void UserCardFullPopupf()
    {
        openUserCardFullPopupButton.onClick.AddListener(OpenUserCardFullPopup);
        closeUserCardFullPopupButton.onClick.AddListener(CloseUserCardFullPopup);

    }

    void OpenUserCardFullPopup()
    {
        SyncCardsFromUserCard(); // UserCard 데이터 동기화
        AddCardsToFullPopupArea(); // FullPopup에 카드 배치
        userCardFullPanel.SetActive(true);
        closeUserCardFullPopupButton.gameObject.SetActive(true);
        userCardArea.SetActive(false);
    }

    void CloseUserCardFullPopup()
    {
        userCardFullPanel.SetActive(false);
        closeUserCardFullPopupButton.gameObject.SetActive(false);
        userCardArea.SetActive(true);
    }

    public void SyncCardsFromUserCard()
    {
        displayedFullCards.Clear();
        displayedFullCards.AddRange(userCard.displayedCards);
    }

    void CreateRow(List<GameObject> rowCards, int rowIndex)
    {
        GameObject row = new GameObject("Row " + rowIndex);
        row.transform.SetParent(fullPopupAreaContent, false);

        // RectTransform을 설정합니다.
        RectTransform rowTransform = row.GetComponent<RectTransform>();
        if (rowTransform == null)
        {
            rowTransform = row.AddComponent<RectTransform>();
        }
        rowTransform.sizeDelta = new Vector2(1080, 200); // 행 크기를 설정합니다.

        // HorizontalLayoutGroup을 추가합니다.
        HorizontalLayoutGroup layoutGroup = row.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.padding = new RectOffset(15, 0, 0, 0); // 왼쪽 여백 설정
        layoutGroup.spacing = 15; // 카드 간 간격 설정
        layoutGroup.childAlignment = TextAnchor.MiddleLeft; // 왼쪽 정렬
        layoutGroup.childForceExpandWidth = false; // 자식 요소의 너비 강제 확장 비활성화
        layoutGroup.childForceExpandHeight = false; // 자식 요소의 높이 강제 확장 비활성화

        // 각 카드를 행에 추가합니다.
        foreach (var card in rowCards)
        {
            GameObject cardInstance = Instantiate(card, row.transform);
            RectTransform cardRect = cardInstance.GetComponent<RectTransform>();
            if (cardRect != null)
            {
                cardRect.sizeDelta = new Vector2(180, 180); // 카드 크기를 180x180으로 설정
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
            layoutElement.minWidth = 201; // Spacer의 최소 너비를 카드 크기와 동일하게 설정
            layoutElement.preferredWidth = 201; // Spacer의 선호 너비를 카드 크기와 동일하게 설정
        }
    }

    public void AddCardsToFullPopupArea()
    {
        foreach (Transform child in fullPopupAreaContent)
        {
            Destroy(child.gameObject);
        }

        List<GameObject> cardsInRow = new List<GameObject>();
        int rowIndex = 1;

        for (int i = 0; i < displayedFullCards.Count; i++)
        {
            cardsInRow.Add(displayedFullCards[i]);

            if (cardsInRow.Count == 5 || i == displayedFullCards.Count - 1)
            {
                CreateRow(cardsInRow, rowIndex);
                rowIndex++;
                cardsInRow.Clear();
            }
        }

        RectTransform contentRect = fullPopupAreaContent.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, rowIndex * 200);
    }

}