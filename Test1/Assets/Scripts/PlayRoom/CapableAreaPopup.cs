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
            // ī�� GameObject ����
            GameObject block = new GameObject(blockList);
            block.transform.SetParent(blockPool, false);

            // RectTransform ����
            RectTransform rectTransform = block.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200, 200); // ī�� ũ�� ����

            // Button ������Ʈ �߰�
            Button blockButton = block.AddComponent<Button>();
            cardPool.ChangeCardColor(blockButton);

            // ī�� ��� �̹��� �߰�
            Image cardFrontFeature = block.AddComponent<Image>();
            cardFrontFeature.sprite = cardPool.cardFrontImage; // ī�� �ո� �̹��� ����
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
            rectTransform.anchoredPosition = positions[i]; // UI ��ǥ ����
        }
    }

    public void BlockPosition()
    {
        // ī�� ��ġ ��ǥ ����Ʈ (4�������� ��ġ)
        positions[0] = new Vector2(210, 0);      // ������
        positions[1] = new Vector2(0, 210);      // ����
        positions[2] = new Vector2(-210, 0);     // ����
        positions[3] = new Vector2(0, -210);     // �Ʒ���
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
