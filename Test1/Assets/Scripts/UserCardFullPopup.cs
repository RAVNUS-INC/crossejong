using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UserCardFullPopup : MonoBehaviour
{
    public GameObject userCardFullPanel; // 보유카드 전체보기 팝업 패널
    public Button openUserCardFullPopupButton; // 보유카드 전체보기 팝업 열기 버튼
    public Button closeUserCardFullPopupButton; // 보유카드 전체보기 팝업 닫기 버튼
    public GameObject userCardArea;  // 보유카드 스크롤뷰

    public Transform fullPopupAreaContent; // FullPopupArea의 Content (스크롤 뷰의 내용물)
    public List<GameObject> displayedFullCards; // 사용자에게 표시된 전체 카드 리스트

    void Start()
    {

    }

    public void UserCardFullPopupf()
    {
        openUserCardFullPopupButton.onClick.AddListener(OpenUserCardFullPopup);  // 보유카드 전체보기 팝업 열기 버튼에 이벤트 추가
        closeUserCardFullPopupButton.onClick.AddListener(CloseUserCardFullPopup);  // 보유카드 전체보기 팝업 닫기 버튼에 이벤트 추가

        userCardFullPanel.SetActive(false); // 보유카드 전체보기 팝업 비활성화
        openUserCardFullPopupButton.gameObject.SetActive(true); // 보유카드 전체보기 팝업 열기 버튼을 활성화
        closeUserCardFullPopupButton.gameObject.SetActive(false); // 보유카드 전체보기 팝업 닫기 버튼을 비활성화
    }

    void OpenUserCardFullPopup() // 보유카드 전체보기 팝업 열기 메서드
    {
        userCardFullPanel.SetActive(true); // 보유카드 전체보기 팝업 활성화
        closeUserCardFullPopupButton.gameObject.SetActive(true); // 보유카드 전체보기 팝업 닫기 버튼 활성화
        userCardArea.SetActive(false); // 보유카드 스크롤뷰 비활성화
    }

    void CloseUserCardFullPopup() // 보유카드 전체보기 팝업 닫기 메서드
    {
        userCardFullPanel.SetActive(false); // 보유카드 전체보기 팝업 비활성화
        closeUserCardFullPopupButton.gameObject.SetActive(false); // 보유카드 전체보기 팝업 닫기 버튼 비활성화
        userCardArea.SetActive(true); // 보유카드 스크롤뷰 활성화
    }


    void CreateRow(List<GameObject> rowCards, int rowIndex) // Row를 만들어서 FullPopupArea에 추가
    {
        GameObject row = new GameObject("Row " + rowIndex); // Row 이름을 "Row 1", "Row 2"처럼 설정
        row.transform.SetParent(fullPopupAreaContent, false); // FullPopupArea의 Content에 Row 추가

        // HorizontalLayoutGroup 컴포넌트 추가
        HorizontalLayoutGroup layoutGroup = row.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.padding = new RectOffset(15, 0, 0, 0); // 좌측 패딩 15 설정
        layoutGroup.spacing = 20; // 카드 간격 20 설정
        layoutGroup.childAlignment = TextAnchor.MiddleLeft; // 자식 정렬: Middle Left

        // Row에 카드들을 배치
        foreach (var card in rowCards)
        {
            card.transform.SetParent(row.transform); // Row에 카드 배치
        }
    }


    public void AddCardsToFullPopupArea()
    {
        // 카드들을 5개씩 Row로 나누어 배치
        List<GameObject> cardsInRow = new List<GameObject>();

        int i = 0;
        int rowIndex = 1; // Row 번호 시작
        foreach (var card in displayedFullCards) // displayedFullCards로 수정
        {
            cardsInRow.Add(card);
            i++;

            // 5개씩 한 Row로 나누기
            if (i % 5 == 0 || i == displayedFullCards.Count) // displayedFullCards로 수정
            {
                CreateRow(cardsInRow, rowIndex); // Row 번호와 함께 전달
                rowIndex++; // 다음 Row로 번호 증가
                cardsInRow.Clear(); // Row에 카드가 추가되었으면 새로운 Row로 초기화
            }
        }
    }
}
