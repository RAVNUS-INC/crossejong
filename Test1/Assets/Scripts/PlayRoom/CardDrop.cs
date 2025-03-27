using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// CardDrop Ŭ������ �巡�� ������ ī�尡 ��ӵ� �� �ִ� ������ �����ϴ� ��ũ��Ʈ
public class CardDrop : MonoBehaviourPun, IDropHandler
{
    public CardDrag cardDrag;
    public UserCard userCard;
    public FieldCard fieldCard;
    public CardPool cardPool;
    public UserCardFullPopup userCardFullPopup;
    


    void Awake()
    {
        fieldCard = FindObjectOfType<FieldCard>();
        userCard = FindObjectOfType<UserCard>();
        userCardFullPopup = FindObjectOfType<UserCardFullPopup>();
    }


    private bool HasCard()
    {
        if (transform.childCount > 0)
            return true;
        else
            return false;
    }

    private bool IsColorWhite()
    {
        Image image = transform.GetComponent<Image>();
        if(image.color == Color.white)
            return true;
        else
            return false;
    }

    //public void ExtendDropAreas()
    //{
    //    ObjectManager.instance.gridCount += 2;
    //    GameObject[,] newGrid = new GameObject[ObjectManager.instance.gridCount, ObjectManager.instance.gridCount];

    //    //    0  0  0  (0, 0) (0, 1)
    //    //    0  0  0
    //    //    0  0  0


    //    //    1  1  1  1  1
    //    //    1  0  0  0  1  (1, 1) (1, 2)
    //    //    1  0  0  0  1
    //    //    1  0  0  0  1
    //    //    1  1  1  1  1


    //    // ���ο� �迭�� ���� �׸��带 �߾����� �ű�
    //    for (int x = 0; x < ObjectManager.instance.gridCount - 2; x++)
    //    {
    //        for (int y = 0; y < ObjectManager.instance.gridCount - 2; y++)
    //        {
    //            newGrid[x + 1, y + 1] = ObjectManager.instance.grid[x, y];
    //        }
    //    }
    //    ObjectManager.instance.grid = newGrid;

    //    // ���ο� �����ڸ� �� ����
    //    for (int x = 0; x < ObjectManager.instance.gridCount; x++)
    //    {
    //        for (int y = 0; y < ObjectManager.instance.gridCount; y++)
    //        {
    //            if (ObjectManager.instance.grid[x, y] == null)
    //            {
    //                GameObject empty = new GameObject("");
    //                empty.transform.SetParent(fieldCard.fieldContainer, false);
    //                RectTransform rect = empty.AddComponent<RectTransform>();
    //                rect.sizeDelta = new Vector2(200, 200);
    //                Image img = empty.AddComponent<Image>();
    //                img.color = Color.clear;
    //                empty.AddComponent<CardDrop>();
    //                ObjectManager.instance.emptyList.Add(empty);
    //                ObjectManager.instance.grid[x, y] = empty;
    //            }
    //        }
    //    }

    //    // ��ǥ ���� �ε��� �ű��
    //    ObjectManager.instance.cardIndexX += 1;
    //    ObjectManager.instance.cardIndexY += 1;

    //    for (int i = 0; i < ObjectManager.instance.FinIndexX.Count; i++)
    //    {
    //        ObjectManager.instance.FinIndexX[i] += 1;
    //    }
    //    for (int i = 0; i < ObjectManager.instance.FinIndexY.Count; i++)
    //    {
    //        ObjectManager.instance.FinIndexY[i] += 1;
    //    }

    //    fieldCard.OnOffDropAreas();
    //}


    public void OnDrop(PointerEventData eventData)
    {
        GameObject card = ObjectManager.instance.moveCardObejct;

        if (card != null && !HasCard() && IsColorWhite())
        {
            card.GetComponent<Image>().raycastTarget = true;

            // ��ġ�� ���콺 �̺�Ʈ���� �巡�׵� ī�带 ó��
            Vector3 newPosition = (Input.touchCount > 0) ? Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position) : Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

            card.transform.position = this.transform.position;
            card.transform.SetParent(transform);
            card.transform.parent.name = card.transform.name;

            if (fieldCard.fieldDisplayedCards.Contains(card))
            {
                Debug.Log("�ʵ�ī�忡 �ش� ī�尡 �̹� �����մϴ�");
            }
            else
            {
                fieldCard.fieldDisplayedCards.Add(card);
            }

            if (userCard.displayedCards.Contains(card))
            {
                userCard.displayedCards.Remove(card);
            }
            else
            {
                Debug.Log("����ī�忡 �ش� ī�尡 �̹� �������� �ʽ��ϴ�");
            }

            if (userCardFullPopup.fullDisplayedCards.Contains(card))
            {
                userCardFullPopup.fullDisplayedCards.Remove(card);
            }
            else
            {
                Debug.Log("��ü����ī�忡 �ش� ī�尡 �̹� �������� �ʽ��ϴ�");
            }


            // RectTransform ����
            RectTransform rect = card.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);  // �߾� ���� (anchorMin)
            rect.anchorMax = new Vector2(0.5f, 0.5f);  // �߾� ���� (anchorMax)
            rect.pivot = new Vector2(0.5f, 0.5f);      // �߾� ���� (pivot)
            rect.anchoredPosition = Vector2.zero;      // �θ��� �߽ɿ� ��ġ

            ObjectManager.instance.moveCardObejct = null;
            ObjectManager.instance.SortAfterMove();
            CardDrag CD = card.GetComponent<CardDrag>();
            if (CD != null) Destroy(card.GetComponent<CardDrag>());

            ObjectManager.instance.isDragged = true;

            ObjectManager.instance.createdWord = card.name;

            ObjectManager.instance.rollBackList.Add(card.name); // �ѹ� ������ �𸣴� ���� ī�� ����Ʈ���� ����

            ObjectManager.instance.createdWordList.Add(card);

            // ī�� ���� �׸��� ��ġ �ľ�
            for (int x = 0; x < ObjectManager.instance.gridCount; x++)
            {
                for (int y = 0; y < ObjectManager.instance.gridCount; y++)
                {
                    if (ObjectManager.instance.createdWord == ObjectManager.instance.grid[x, y].transform.name)
                    {
                        ObjectManager.instance.cardIndexX = x;
                        ObjectManager.instance.FinIndexX.Add(x); // ������ ����Ʈ �迭�� ����

                        ObjectManager.instance.cardIndexY = y;
                        ObjectManager.instance.FinIndexY.Add(y); // ������ ����Ʈ �迭�� ����

                        //if (x == 0 || x == ObjectManager.instance.gridCount - 1)
                        //{
                        //    ExtendDropAreas();
                        //}
                        //if (y == 0 || y == ObjectManager.instance.gridCount - 1)
                        //{
                        //    ExtendDropAreas();
                        //}
                    }
                }
            }
            ObjectManager.instance.dropCount += 1;
            ObjectManager.instance.IsCardDrop = true;

            fieldCard.RollBackColorAreas(); // ���⿡�� �ٸ� ��ΰ� ���� ī�带 �׸��忡 �ǽð����� ������Ʈ

            ObjectManager.instance.createdWord = ""; // ����� ī�� ��� ����

        }
    }
}