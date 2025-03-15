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
                    ChangeColorAreas(x - 1, y);
                    ChangeColorAreas(x + 1, y);
                    ChangeColorAreas(x, y - 1);
                    ChangeColorAreas(x, y + 1);
                }
            }
        }

        if (ObjectManager.instance.IsCardDrop)
        {
            //다른 모든 유저들에게 카드 드롭 이미지 업데이트 요청
            photonView.RPC("SyncDropCard", RpcTarget.Others, ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY, ObjectManager.instance.createdWord);

            ObjectManager.instance.IsCardDrop = false;
        }
    }
        

    public void IsRight()
    {
        if (ObjectManager.instance.grid[ObjectManager.instance.cardIndexX + 1, ObjectManager.instance.cardIndexY].transform.childCount == 1)
            isRight = true;
        else
            isRight = false;
    }

    public void IsLeft()
    {
        if (ObjectManager.instance.grid[ObjectManager.instance.cardIndexX - 1, ObjectManager.instance.cardIndexY].transform.childCount == 1)
            isLeft = true;
        else
            isLeft = false;
    }
    public void IsBottom()
    {
        if (ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY + 1].transform.childCount == 1)
            isBottom = true;
        else
            isBottom = false;
    }
    public void IsTop()
    {
        if (ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY - 1].transform.childCount == 1)
            isTop = true;
        else
            isTop = false;
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
            }
            isLeft = false;
            isRight = false;
        }

        if (isRight)  // 오른쪽에 글자가 있을 때
        {
            for (int i = 0; ObjectManager.instance.grid[ObjectManager.instance.cardIndexX + i, ObjectManager.instance.cardIndexY].transform.childCount == 1; i++)
            {
                ObjectManager.instance.createdWords += ObjectManager.instance.grid[ObjectManager.instance.cardIndexX + i, ObjectManager.instance.cardIndexY].transform.name;
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
            }
            isTop = false;
            isBottom = false;
        }

        if (isBottom)  // 아래에 글자가 있을 때
        {
            for (int i = 0; ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY + i].transform.childCount == 1; i++)
            {
                ObjectManager.instance.createdWords += ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY + i].transform.name;
            }
        }

        // 완성된 단어 출력(가로 or 세로)
        Debug.Log(ObjectManager.instance.createdWords);

        // 단어 입력 중 상태메시지 전달
        ObjectManager.instance.ShowCardSelectingMessage(false);
    }

    [PunRPC] //카드를 놓은 사람을 제외한 나머지는 모두 카드의 좌표, 이름을 전달받아 그리드에 추가 수행
    public void SyncDropCard(int cardIndexX, int cardIndexY, string cardName) //카드의 x,y좌표와 이름을 전달
    {
        Debug.Log("상대방 카드 전달받음");

        // 그리드 내에서 x, y 좌표에 해당하는 오브젝트 찾기
        GameObject targetGridObject = ObjectManager.instance.grid[cardIndexX, cardIndexY];

        // 입력받은 카드 이름에 해당하는 오브젝트 찾기
        GameObject targetCard = null;

        foreach (GameObject card in cardPool.cards) // cardPool.cards는 카드들이 저장된 리스트로 가정
        {
            if (card.name == cardName) // 카드 이름이 일치하는지 확인
            {
                targetCard = card;
                break; // 일치하는 카드 찾으면 종료
            }
        }

        if (targetCard != null && targetGridObject != null)
        {
            // 해당 카드의 오브젝트를 targetGridObject 위치로 이동시키기
            targetCard.SetActive(true);
            targetCard.transform.SetParent(targetGridObject.transform, false);
            ObjectManager.instance.grid[cardIndexX, cardIndexY] = targetCard; // 그리드에 카드 정보 업데이트

            // 그리드에 카드를 배치한 후, 드롭 영역 업데이트
            OnOffDropAreas();
        }
        else
        {
            Debug.LogError("카드를 찾을 수 없거나, 잘못된 그리드 위치입니다.");
        }
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
