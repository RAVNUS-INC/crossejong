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

// CardDrop 클래스는 드래그 가능한 카드가 드롭될 수 있는 슬롯을 관리하는 스크립트
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


    //    // 새로운 배열에 기존 그리드를 중앙으로 옮김
    //    for (int x = 0; x < ObjectManager.instance.gridCount - 2; x++)
    //    {
    //        for (int y = 0; y < ObjectManager.instance.gridCount - 2; y++)
    //        {
    //            newGrid[x + 1, y + 1] = ObjectManager.instance.grid[x, y];
    //        }
    //    }
    //    ObjectManager.instance.grid = newGrid;

    //    // 새로운 가장자리 셀 생성
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

    //    // 좌표 관련 인덱스 옮기기
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

            // 터치나 마우스 이벤트에서 드래그된 카드를 처리
            Vector3 newPosition = (Input.touchCount > 0) ? Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position) : Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

            card.transform.position = this.transform.position;
            card.transform.SetParent(transform);
            card.transform.parent.name = card.transform.name;

            if (fieldCard.fieldDisplayedCards.Contains(card))
            {
                Debug.Log("필드카드에 해당 카드가 이미 존재합니다");
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
                Debug.Log("보유카드에 해당 카드가 이미 존재하지 않습니다");
            }

            if (userCardFullPopup.fullDisplayedCards.Contains(card))
            {
                userCardFullPopup.fullDisplayedCards.Remove(card);
            }
            else
            {
                Debug.Log("전체보유카드에 해당 카드가 이미 존재하지 않습니다");
            }


            // RectTransform 설정
            RectTransform rect = card.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);  // 중앙 정렬 (anchorMin)
            rect.anchorMax = new Vector2(0.5f, 0.5f);  // 중앙 정렬 (anchorMax)
            rect.pivot = new Vector2(0.5f, 0.5f);      // 중앙 정렬 (pivot)
            rect.anchoredPosition = Vector2.zero;      // 부모의 중심에 위치

            ObjectManager.instance.moveCardObejct = null;
            ObjectManager.instance.SortAfterMove();
            CardDrag CD = card.GetComponent<CardDrag>();
            if (CD != null) Destroy(card.GetComponent<CardDrag>());

            ObjectManager.instance.isDragged = true;

            ObjectManager.instance.createdWord = card.name;

            ObjectManager.instance.rollBackList.Add(card.name); // 롤백 할지도 모르니 놓은 카드 리스트들을 저장

            ObjectManager.instance.createdWordList.Add(card);

            // 카드 놓인 그리드 위치 파악
            for (int x = 0; x < ObjectManager.instance.gridCount; x++)
            {
                for (int y = 0; y < ObjectManager.instance.gridCount; y++)
                {
                    if (ObjectManager.instance.createdWord == ObjectManager.instance.grid[x, y].transform.name)
                    {
                        ObjectManager.instance.cardIndexX = x;
                        ObjectManager.instance.FinIndexX.Add(x); // 전달할 리스트 배열에 저장

                        ObjectManager.instance.cardIndexY = y;
                        ObjectManager.instance.FinIndexY.Add(y); // 전달할 리스트 배열에 저장

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

            fieldCard.RollBackColorAreas(); // 여기에서 다른 모두가 놓은 카드를 그리드에 실시간으로 업데이트

            ObjectManager.instance.createdWord = ""; // 드롭한 카드 목록 비우기

        }
    }
}