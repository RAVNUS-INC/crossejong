using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using PlayFab.DataModels;
using DG.Tweening;

public class CardPool : MonoBehaviour
{
    public static CardPool instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        rectTransform = GetComponent<RectTransform>();  // RectTransform 초기화
    }

    public Transform cardContainer; // 카드 부모 오브젝트
    public Sprite cardFrontImage; // 카드 앞면 이미지
    public Sprite cardBackImage; // 카드 뒷면 이미지
    public Sprite specialCardFrontColorImage; // 카드 앞면 이미지
    public Sprite specialCardFrontBlackImage; // 카드 앞면 이미지
    public CardLists cardLists; // NewCard 참조

    public TextMeshProUGUI tmpText; // TextMeshProUGUI 컴포넌트 참조
    public TMP_FontAsset newFont; // 변경할 새로운 Font Asset

    public List<GameObject> cards = new List<GameObject>(); // 생성된 카드 목록

    public bool isFlipped = false; // 카드가 뒤집혔는지 여부
    public RectTransform rectTransform; // 카드의 RectTransform


    void Start()
    {
        // 카드 생성
        CreateCard();
    }

    public void CreateCards(List<string> cardList, Color color)
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            // 카드 GameObject 생성
            GameObject card = new GameObject(cardList[i]);
            card.transform.SetParent(cardContainer, false);

            // RectTransform 설정
            RectTransform rectTransform = card.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200, 200); // 카드 크기 설정

            // Button 컴포넌트 추가
            //Button button = card.AddComponent<Button>();

            if (cardList != cardLists.cardFrontSpecial)
            {
                // 카드 배경 이미지 추가
                Image cardFrontFeature = card.AddComponent<Image>();
                cardFrontFeature.sprite = cardFrontImage; // 카드 앞면 이미지 설정
                cardFrontFeature.type = Image.Type.Sliced;

                // 텍스트 오브젝트 생성 (앞면용)
                GameObject textObject = new GameObject("CardText");
                textObject.transform.SetParent(card.transform, false);
                RectTransform textRect = textObject.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;

                TextMeshProUGUI tmpText = textObject.AddComponent<TextMeshProUGUI>();
                tmpText.font = newFont;
                tmpText.text = cardList[i]; // 텍스트 설정
                tmpText.fontSize = 120;
                tmpText.alignment = TextAlignmentOptions.Center;
                tmpText.color = color;
                tmpText.raycastTarget = false; // 텍스트 레이캐스트 제거
            }
            else
            {
                // 카드 배경 이미지 추가
                Image cardFrontFeature = card.AddComponent<Image>();
                cardFrontFeature.sprite = specialCardFrontColorImage; // 카드 앞면 이미지 설정
                cardFrontFeature.type = Image.Type.Sliced;
            }

            // 카드 리스트에 추가
            cards.Add(card);
        }
    }

    //public void ChangeCardColor(Button button)
    //{
    //    for (int i = 0; i < cards.Count; i++)
    //    {
    //        Button cardButton = cards[i].GetComponent<Button>(); // Button 컴포넌트 가져오기

    //        if (cardButton != null)
    //        {
    //            ColorBlock colorBlock = cardButton.colors;

    //            // 기본 색상 변경
    //            colorBlock.normalColor = Color.white;
    //            // 클릭했을 때 색상 변경
    //            colorBlock.selectedColor = Color.grey;
    //            // 비활성화된 상태 색상 변경
    //            colorBlock.disabledColor = Color.red;

    //            // 변경된 색상 블록을 버튼에 적용
    //            cardButton.colors = colorBlock;
    //        }
    //    }
    //}

    //public void FlipCard(GameObject card)
    //{
    //    Image cardFrontFeature = card.gameObject.GetComponent<Image>();
    //    Sprite savedCardFrontImage = card.gameObject.GetComponent<Sprite>();
    //    RectTransform cardRectTransform = card.GetComponent<RectTransform>();  // card의 RectTransform 가져오기

    //    if (cardFrontFeature.sprite != cardBackImage)
    //    {
    //        // 카드가 이미 뒤집혔으면 앞면으로 돌아감
    //        cardRectTransform.DORotate(new Vector3(0, 180, 0), 0.1f, RotateMode.LocalAxisAdd) // 180도 회전
    //            .OnComplete(() =>
    //            {
    //                cardFrontFeature.sprite = cardBackImage;
    //                // CardText라는 자식 오브젝트를 찾아서 비활성화
    //                Transform cardTextTransform = card.transform.Find("CardText");
    //                cardTextTransform.gameObject.SetActive(false);
    //            });
    //    }
    //    else
    //    {
    //        // 카드가 앞면일 경우
    //        cardRectTransform.DORotate(new Vector3(0, 180, 0), 0.1f, RotateMode.LocalAxisAdd) // 180도 회전
    //            .OnComplete(() =>
    //            {
    //                cardFrontFeature.sprite = savedCardFrontImage;
    //                Transform cardTextTransform = card.transform.Find("CardText");
    //                cardTextTransform.gameObject.SetActive(true);
    //            });
    //    }

    //    // 플립 상태를 반전시킴
    //    isFlipped = !isFlipped;
    //}


    private void CreateCard()
    {
        CreateCards(cardLists.cardFrontRed, Color.red);
        CreateCards(cardLists.cardFrontBlack, Color.black);
        CreateCards(cardLists.cardFrontSpecial, Color.white);
        //ButtonColor(cards);
    }

    //public void ButtonColor(List<GameObject> targetList)
    //{
    //    // 모든 카드에 대해 ChangeCardColor 연결
    //    foreach (var card in targetList)
    //    {
    //        Button cardButton = card.GetComponent<Button>();
    //        if (cardButton != null)
    //        {
    //            // 버튼 클릭 시 ChangeCardColor 함수 실행
    //            cardButton.onClick.AddListener(() => ChangeCardColor(cardButton));
    //        }
    //    }
    //}

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

    public void MoveCardsToTarGetArea(List<GameObject> startList, Transform targetArea, List<GameObject> targetList)
    {
        targetList.Clear();

        foreach (var card in startList)
        {
            MoveCardToParent(card, targetArea); // 각 카드를 TargetArea로 이동
            card.SetActive(true); // 카드가 보이도록 활성화
            targetList.Add(card); // 보여지는 리스트에 추가
        }

        SortCardIndex(targetList);
    }
    public void GetCardsToTarGetArea(List<GameObject> startList, Transform targetArea, List<GameObject> targetList)
    {
        foreach (var card in startList)
        {
            MoveCardToParent(card, targetArea); // 각 카드를 TargetArea로 이동
            card.SetActive(true); // 카드가 보이도록 활성화
            targetList.Add(card); // 보여지는 리스트에 추가
        }

        SortCardIndex(targetList);
    }

    public void SortCardIndex(List<GameObject> cardList)
    {
        if (cardList.Count != 0)
        {
            for (int i = 0; i < cardList.Count; i++)
            {
                CardDrag CD = cardList[i].GetComponent<CardDrag>();
                if (CD != null)
                {
                    CD.cardIndex = i;
                }
            }
        }
    }
}
