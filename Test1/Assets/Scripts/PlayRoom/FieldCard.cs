using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Xml;
using PlayFab.DataModels;
using JetBrains.Annotations;
using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using Unity.VisualScripting;
using System.Xml.Linq;
using Photon.Pun;
using System.Reflection;
using System.Linq;

public class FieldCard : MonoBehaviourPun

{
    public TurnChange turnChange;
    public UserCardFullPopup fullPopup;

    public Transform fieldContainer; // FieldArea의 Contents
    public CardPool cardPool; // CardPool 참조
    public List<GameObject> fieldDisplayedCards;
    public Transform emptyArea;
    public bool isRight;
    public bool isLeft;
    public bool isTop;
    public bool isBottom;


    public void CreateDropAreas()
    {
        ObjectManager.instance.grid = new GameObject[ObjectManager.instance.gridCount, ObjectManager.instance.gridCount];
        for (int x = 0; x < ObjectManager.instance.gridCount; x++)
        {
            for (int y = 0; y < ObjectManager.instance.gridCount; y++)
            {
                GameObject empty = new GameObject("");
                empty.transform.SetParent(fieldContainer, false);
                RectTransform rect = empty.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(200, 200);
                Image img = empty.AddComponent<Image>();
                img.color = Color.white;
                empty.AddComponent<CardDrop>();
                ObjectManager.instance.emptyList.Add(empty);
                Image image = empty.GetComponent<Image>();
                image.color = Color.clear;
                ObjectManager.instance.grid[x, y] = empty;

            }
        }
    }

    private void ChangeColorAreas(int x, int y)
    {
        Image image = ObjectManager.instance.grid[x, y].GetComponent<Image>();
        if (image.color != Color.white)
        {
            image.color = Color.white;
        }
    }

    public void RollBackColorAreas()
    {
        if (ObjectManager.instance.IsCardDrop)
        {
            // 나를 제외한 모든 유저들이 드롭한 카드를 보드판에 보이도록 요청함
            photonView.RPC("SyncDropCard", RpcTarget.Others, ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY, ObjectManager.instance.createdWord);

            turnChange.CardDropBtn.interactable = true; //카드 내기 버튼 활성화
            ObjectManager.instance.RollBackBtn.gameObject.SetActive(true); // 드롭한게 있으면 롤백버튼 보여주기

            ObjectManager.instance.IsCardDrop = false;
        }

        for (int x = 0; x < ObjectManager.instance.gridCount; x++)
        {
            for (int y = 0; y < ObjectManager.instance.gridCount; y++)
            {
                Image image = ObjectManager.instance.grid[x, y].GetComponent<Image>();
                image.color = Color.clear;
                if (ObjectManager.instance.grid[x, y].transform.childCount == 1)
                {
                    image.color = Color.white;
                }
            }
        }
        OnOffDropAreas();
    }

    public void OnOffDropAreas()
    {
        for (int x = 0; x < ObjectManager.instance.gridCount; x++)
        {
            for (int y = 0; y < ObjectManager.instance.gridCount; y++)
            {
                if (ObjectManager.instance.grid[x, y].transform.childCount == 1)
                {
                    if (x != 0)
                    {
                        ChangeColorAreas(x - 1, y);
                    }
                    if (x != ObjectManager.instance.gridCount - 1)
                    {
                        ChangeColorAreas(x + 1, y);
                    }
                    if (y != 0)
                    {
                        ChangeColorAreas(x, y - 1);
                    }
                    if (y != ObjectManager.instance.gridCount - 1)
                    { 
                        ChangeColorAreas(x, y + 1);
                    }

                }
            }
        }
    }
        

    public void IsRight()
    {
        if (ObjectManager.instance.cardIndexX != ObjectManager.instance.gridCount - 1)
        {
            if (ObjectManager.instance.grid[ObjectManager.instance.cardIndexX + 1, ObjectManager.instance.cardIndexY].transform.childCount == 1)
                isRight = true;
            else
                isRight = false;
        }
        else
            isRight = true;
    }

    public void IsLeft()
    {
        if (ObjectManager.instance.cardIndexX != 0)
        {
            if (ObjectManager.instance.grid[ObjectManager.instance.cardIndexX - 1, ObjectManager.instance.cardIndexY].transform.childCount == 1)
                isLeft = true;
            else
                isLeft = false;
        }
        else
            isLeft = true;
    }
    public void IsBottom()
    {
        if (ObjectManager.instance.cardIndexY != ObjectManager.instance.gridCount - 1)
        {
            if (ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY + 1].transform.childCount == 1)
                isBottom = true;
            else
                isBottom = false;
        }
        else
            isBottom = true;
    }
    public void IsTop()
    {
        if (ObjectManager.instance.cardIndexY != 0)
        {
            if (ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY - 1].transform.childCount == 1)
                isTop = true;
            else
                isTop = false;
        }
        else
            isTop = true;
    }
    public void IsPosition()
    {
        IsLeft();
        IsRight();
        IsBottom();
        IsTop();
    }


    // 유저가 카드 내기 완료 버튼을 눌렀을 때
    public void createdWordEnd()
    {
        IsPosition();

        ObjectManager.instance.createdWords = "";

        if (isLeft)  // 왼쪽에 글자가 있을 때
        {
            int x = 0;
            for (int i = 1; ObjectManager.instance.grid[ObjectManager.instance.cardIndexX - i, ObjectManager.instance.cardIndexY].transform.childCount == 1; i++)
            {
                x = i;
            }
            for (int i = 0; ObjectManager.instance.grid[ObjectManager.instance.cardIndexX - x + i, ObjectManager.instance.cardIndexY].transform.childCount == 1; i++)
            {
                ObjectManager.instance.createdWords += ObjectManager.instance.grid[ObjectManager.instance.cardIndexX - x + i, ObjectManager.instance.cardIndexY].transform.name;

                if (ObjectManager.instance.cardIndexX - x + i == ObjectManager.instance.gridCount - 1)
                {
                    break;
                }
            }
            isLeft = false;
            isRight = false;
        }

        if (isRight)  // 오른쪽에 글자가 있을 때
        {
            for (int i = 0; ObjectManager.instance.grid[ObjectManager.instance.cardIndexX + i, ObjectManager.instance.cardIndexY].transform.childCount == 1; i++)
            {
                ObjectManager.instance.createdWords += ObjectManager.instance.grid[ObjectManager.instance.cardIndexX + i, ObjectManager.instance.cardIndexY].transform.name;
                if (ObjectManager.instance.cardIndexX + i == ObjectManager.instance.gridCount - 1)
                {
                    break;
                }
            }
        }

        if (isTop)  // 위에 글자가 있을 때
        {
            int y = 0;
            for (int i = 1; ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY - i].transform.childCount == 1; i++)
            {
                y = i;
            }
            for (int i = 0; ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY - y + i].transform.childCount == 1; i++)
            {
                ObjectManager.instance.createdWords += ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY - y + i].transform.name;

                if (ObjectManager.instance.cardIndexY - y + i == ObjectManager.instance.gridCount - 1)
                {
                    break;
                }
            }
            isTop = false;
            isBottom = false;
        }

        if (isBottom)  // 아래에 글자가 있을 때
        {
            for (int i = 0; ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY + i].transform.childCount == 1; i++)
            {
                ObjectManager.instance.createdWords += ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY + i].transform.name;

                if (ObjectManager.instance.cardIndexY + i == ObjectManager.instance.gridCount - 1)
                {
                    break;
                }
            }
        }

        // 완성된 단어 출력(가로 or 세로)
        Debug.Log(ObjectManager.instance.createdWords);

        // 단어 입력 중 상태메시지 전달
        ObjectManager.instance.ShowCardSelectingMessage(false);
    }

    [PunRPC] //카드를 놓은 사람을 제외한 나머지는 모두 카드의 좌표, 이름을 전달받아 그리드에 추가 수행
    public void SyncDropCard(int cardIndexX, int cardIndexY, string cardName) 
    {
        // 그리드 내에서 x, y 좌표에 해당하는 오브젝트 찾기
        GameObject targetGridObject = ObjectManager.instance.grid[cardIndexX, cardIndexY];

        GameObject originalCard = cardPool.cards.FirstOrDefault(card => card.name == cardName);
        GameObject targetCard = Instantiate(originalCard); // 새로운 복제본 생성

        if (targetCard != null && targetGridObject != null)
        {
            // 해당 카드의 오브젝트를 targetGridObject 위치로 이동시키기
            targetCard.transform.SetParent(targetGridObject.transform, false);

            // 부모 오브젝트 객체 이름 변경
            targetGridObject.name = cardName;

            // 그리드에 카드를 배치한 후, 드롭 영역 업데이트
            RollBackColorAreas();
        }
        else
        {
            Debug.LogError("카드를 찾을 수 없거나, 잘못된 그리드 위치입니다.");
        }
    }

    [PunRPC] //롤백 요청 카드를 받아 보드판에서 보이지 않게 함
    public void SyncRollCard(int[] cardIndexX, int[] cardIndexY, string[] cardNames) 
    {
        for (int i = 0; i < cardNames.Length; i++)
        {
            string cardName = cardNames[i];
            int x = cardIndexX[i];
            int y = cardIndexY[i];

            // 그리드 내에서 x, y 좌표에 해당하는 오브젝트 찾기
            GameObject targetGridObject = ObjectManager.instance.grid[x, y];

            // targetGridObject가 자식 요소를 가지고 있다면, 자식들을 모두 삭제-----------
            Transform targetTransform = targetGridObject.transform;

            for (int j = targetTransform.childCount - 1; j >= 0; j--)
            {
                Transform child = targetTransform.GetChild(j);
                Destroy(child.gameObject); // 자식 객체 삭제
            }

            // 부모 오브젝트 객체 이름 변경
            targetGridObject.name = "";
        }
        // 그리드에 카드를 제거한 후, 드롭 영역 업데이트
        // 일정 시간 후에 RollBackColorAreas 함수 호출
        Invoke("RollBackColorAreas", 0.07f); 
    }

    public void FirstFieldCard()
    {
        // 방장만 처음 1장의 카드 이름을 뽑음
        string[] randomCardNames = cardPool.GetRandomCardsName(1); // 이름을 받는 함수로 변경

        // 모든 플레이어들에게 인덱스리스트를 넘겨 첫 카드 오브젝트를 생성하도록 요청(배열->문자열)
        photonView.RPC("FirstFieldCardRequestAll", RpcTarget.All, string.Join(",", randomCardNames));
    }

    //방장 포함 모두가 첫 카드 추가를 수행하는 함수
    [PunRPC]
    public void FirstFieldCardRequestAll(string names)
    {
        string[] usedNames = names.Split(','); // 다시 배열로 변환
        foreach (string i in usedNames)
        {
            Debug.Log($"첫 카드 '{i}' 받음");
        }
        List<GameObject> randomCards = cardPool.GetRandomCardsObject(usedNames);

        cardPool.GetCardsToTarGetArea(randomCards, fieldContainer, fieldDisplayedCards);
        GameObject middleObejcts = ObjectManager.instance.grid[ObjectManager.instance.gridCount/2, ObjectManager.instance.gridCount / 2];
        GameObject firstCards = randomCards[0];
        ObjectManager.instance.grid[ObjectManager.instance.gridCount / 2, ObjectManager.instance.gridCount / 2].SetActive(true);
        firstCards.transform.SetParent(middleObejcts.transform, false);
        ObjectManager.instance.grid[ObjectManager.instance.gridCount / 2, ObjectManager.instance.gridCount / 2] = firstCards;

        firstCards.transform.parent.name = firstCards.transform.name;

        OnOffDropAreas();
    }

}
