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

    public Transform fieldContainer; // FieldArea�� Contents
    public CardPool cardPool; // CardPool ����
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
            //�ٸ� ��� �����鿡�� ī�� ��� �̹��� ������Ʈ ��û
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


    // ������ ī�� ���� �Ϸ� ��ư�� ������ ��
    public void createdWordEnd()
    {
        IsPosition();

        ObjectManager.instance.createdWords = "";

        if (isLeft)  // ���ʿ� ���ڰ� ���� ��
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

        if (isRight)  // �����ʿ� ���ڰ� ���� ��
        {
            for (int i = 0; ObjectManager.instance.grid[ObjectManager.instance.cardIndexX + i, ObjectManager.instance.cardIndexY].transform.childCount == 1; i++)
            {
                ObjectManager.instance.createdWords += ObjectManager.instance.grid[ObjectManager.instance.cardIndexX + i, ObjectManager.instance.cardIndexY].transform.name;
            }
        }

        if (isTop)  // ���� ���ڰ� ���� ��
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

        if (isBottom)  // �Ʒ��� ���ڰ� ���� ��
        {
            for (int i = 0; ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY + i].transform.childCount == 1; i++)
            {
                ObjectManager.instance.createdWords += ObjectManager.instance.grid[ObjectManager.instance.cardIndexX, ObjectManager.instance.cardIndexY + i].transform.name;
            }
        }

        // �ϼ��� �ܾ� ���(���� or ����)
        Debug.Log(ObjectManager.instance.createdWords);

        // �ܾ� �Է� �� ���¸޽��� ����
        ObjectManager.instance.ShowCardSelectingMessage(false);
    }

    [PunRPC] //ī�带 ���� ����� ������ �������� ��� ī���� ��ǥ, �̸��� ���޹޾� �׸��忡 �߰� ����
    public void SyncDropCard(int cardIndexX, int cardIndexY, string cardName) //ī���� x,y��ǥ�� �̸��� ����
    {
        Debug.Log("���� ī�� ���޹���");

        // �׸��� ������ x, y ��ǥ�� �ش��ϴ� ������Ʈ ã��
        GameObject targetGridObject = ObjectManager.instance.grid[cardIndexX, cardIndexY];

        // �Է¹��� ī�� �̸��� �ش��ϴ� ������Ʈ ã��
        GameObject targetCard = null;

        foreach (GameObject card in cardPool.cards) // cardPool.cards�� ī����� ����� ����Ʈ�� ����
        {
            if (card.name == cardName) // ī�� �̸��� ��ġ�ϴ��� Ȯ��
            {
                targetCard = card;
                break; // ��ġ�ϴ� ī�� ã���� ����
            }
        }

        if (targetCard != null && targetGridObject != null)
        {
            // �ش� ī���� ������Ʈ�� targetGridObject ��ġ�� �̵���Ű��
            targetCard.SetActive(true);
            targetCard.transform.SetParent(targetGridObject.transform, false);
            ObjectManager.instance.grid[cardIndexX, cardIndexY] = targetCard; // �׸��忡 ī�� ���� ������Ʈ

            // �׸��忡 ī�带 ��ġ�� ��, ��� ���� ������Ʈ
            OnOffDropAreas();
        }
        else
        {
            Debug.LogError("ī�带 ã�� �� ���ų�, �߸��� �׸��� ��ġ�Դϴ�.");
        }
    }

    public void FirstFieldCard()
    {
        // ���常 ó�� 1���� ī�� �̸��� ����
        string[] randomCardNames = cardPool.GetRandomCardsName(1); // �̸��� �޴� �Լ��� ����

        // ��� �÷��̾�鿡�� �ε�������Ʈ�� �Ѱ� ù ī�� ������Ʈ�� �����ϵ��� ��û(�迭->���ڿ�)
        photonView.RPC("FirstFieldCardRequestAll", RpcTarget.All, string.Join(",", randomCardNames));
    }

    //���� ���� ��ΰ� ù ī�� �߰��� �����ϴ� �Լ�
    [PunRPC]
    public void FirstFieldCardRequestAll(string names)
    {
        string[] usedNames = names.Split(','); // �ٽ� �迭�� ��ȯ
        foreach (string i in usedNames)
        {
            Debug.Log($"ù ī�� '{i}' ����");
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
