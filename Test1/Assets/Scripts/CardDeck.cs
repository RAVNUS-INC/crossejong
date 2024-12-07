using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardDeck : MonoBehaviour
{
    public List<GameObject> cardPrefabs; // 카드 프리팹 리스트
    public Transform cardContainer; // 카드를 배치할 컨테이너
    public Button createCardButton; // 카드 생성 버튼
    public Button appealButton; // 이의신청 버튼
    public Button changeCardButton; // 보유카드 바꾸기 버튼
    public OptionPopup optionPopup; //OptionPopup 스크립트 연결

    public float cardSpacing = 115f; // 카드 간격
    public Vector2 startPosition = new Vector2(385, 142); // 카드 시작 위치

    private List<GameObject> displayedCards = new List<GameObject>(); // 화면에 표시된 카드 리스트
    private HashSet<int> selectedCardIndices = new HashSet<int>(); // 선택된 카드 인덱스 집합

    void Start()
    {
        createCardButton.onClick.AddListener(OnCreateCard); // 버튼 클릭 이벤트에 카드 생성 메서드 추가
        optionPopup.openOptionPopupButton.onClick.AddListener(optionPopup.OpenPopup);  // 옵션 팝업 열기 버튼에 이벤트 추가
        optionPopup.closeOptionPopupButton.onClick.AddListener(optionPopup.ClosePopup);  // 옵션 팝업 닫기 버튼에 이벤트 추가

        appealButton.gameObject.SetActive(false); // 이의신청 버튼 비활성화
        changeCardButton.gameObject.SetActive(false); // 보유카드 바꾸기 버튼 비활성화
        optionPopup.optionPopupPanel.SetActive(false); // 옵션 팝업 비활성화
        optionPopup.openOptionPopupButton.gameObject.SetActive(false); // 옵션 팝업 열기 버튼을 비활성화
        optionPopup.closeOptionPopupButton.gameObject.SetActive(false); // 옵션 팝업 닫기 버튼을 비활성화
    }

    private void OnCreateCard()
    {
        createCardButton.gameObject.SetActive(false); // 게임 시작 버튼 누르면 사라지게 하기

        CreateCards(); // 카드 생성 로직

        appealButton.gameObject.SetActive(true); // 이의신청 버튼 활성화
        changeCardButton.gameObject.SetActive(true); // 보유카드 바꾸기 버튼 활성화
        optionPopup.openOptionPopupButton.gameObject.SetActive(true); // 옵션 팝업 열기 버튼을 활성화
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

            // 카드 위치 설정
            rectTransform.anchoredPosition = new Vector2(startPosition.x + (i * cardSpacing), startPosition.y);

            // 카드의 스프라이트 설정은 이미 프리팹에 설정되어 있으므로 여기서는 필요 없음
            // 카드 프리팹에서 앞면과 뒷면 스프라이트가 이미 설정되어 있어야 합니다.

            displayedCards.Add(cardInstance); // 생성된 카드를 리스트에 추가
            i++;
        }
    }
}
