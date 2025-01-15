using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;


public class CapableAreaPopup : MonoBehaviour
{
    public UserCard userCard;
    public FieldCard fieldCard;
    public CardPool cardPool;
    public Transform capableContainer;
    public Transform blockPool;
    public GameObject capableAreaPopupPanel;
    public List<GameObject> capableDisplayedCards;
    public List<GameObject> blocks = new List<GameObject>();
    public List<GameObject> DisplayedBlocks = new List<GameObject>();
    public string blockList = "";
    public Vector2[] positions = new Vector2[4];

    // Start is called before the first frame update
    void Start()
    {
        CreateCapableArea();
    }

    public void MoveCardsToCapableArea()
    {
        cardPool.MoveCardsToTarGetArea(fieldCard.fieldDisplayedCards, capableContainer, capableDisplayedCards);
    }

    public void MoveCardsToFieldArea()
    {
        cardPool.MoveCardsToTarGetArea(capableDisplayedCards, fieldCard.fieldContainer, fieldCard.fieldDisplayedCards);
    }

    public void CreateCapableArea()
    {
        for (int i = 0; i < 4; i++)
        {
            // 카드 GameObject 생성
            GameObject block = new GameObject(blockList);
            block.transform.SetParent(blockPool, false);

            // RectTransform 설정
            RectTransform rectTransform = block.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200, 200); // 카드 크기 설정

            // Button 컴포넌트 추가
            Button blockButton = block.AddComponent<Button>();
            cardPool.ChangeCardColor(blockButton);

            // 카드 배경 이미지 추가
            Image cardFrontFeature = block.AddComponent<Image>();
            cardFrontFeature.sprite = cardPool.cardFrontImage; // 카드 앞면 이미지 설정
            cardFrontFeature.type = Image.Type.Sliced;

            block.SetActive(false);
            blocks.Add(block);
        }
    }

    public void MoveBlocksToCapableArea()
    {
        BlockPosition();

        cardPool.MoveCardsToTarGetArea(blocks, capableContainer, DisplayedBlocks);

        for (int i = 0; i < blocks.Count; i++)
        {
            RectTransform rectTransform = blocks[i].GetComponent<RectTransform>();
            Debug.Log("Index" + i);
            rectTransform.anchoredPosition = positions[i]; // UI 좌표 설정
        }
    }

    public void BlockPosition()
    {
        // 카드 배치 좌표 리스트 (4방향으로 배치)
        positions[0] = new Vector2(210, 0);      // 오른쪽
        positions[1] = new Vector2(0, 210);      // 위쪽
        positions[2] = new Vector2(-210, 0);     // 왼쪽
        positions[3] = new Vector2(0, -210);     // 아래쪽
    }

    public void MoveBlocksToBlockPool()
    {
        cardPool.MoveCardsToTarGetArea(DisplayedBlocks, blockPool, blocks);
    }

    public void CapableAreaPopupf(Button cardButton)
    {
        capableAreaPopupPanel.SetActive(true);
    }

}
