using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Xml;
using PlayFab.DataModels;

public class FieldCard : MonoBehaviour
{
    public Transform FieldContainer; // FieldArea�� Contents
    public List<GameObject> displayedField; // FieldArea���� �������� ī�� ����Ʈ
    public string[,] cardField; // 2���� �迭�� ī�� �ʵ� ����
    public int rows = 3; // �� ũ��
    public int columns = 3; // �� ũ��
    public CardPool cardPool; // NewCardPool ����

    void Start()
    {
        cardField = new string[rows, columns];
        InitializeCardField();
    }

    // �ʵ� �ʱ�ȭ �Լ�
    private void InitializeCardField()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                cardField[i, j] = null; // �ʱⰪ: null
            }
        }
    }

    public void CardField()
    {

    }

    public void StartFieldCard()
    {
        List<GameObject> randomCards = cardPool.GetRandomCards(1); // 1���� ���� ī�� ���
        foreach (var card in randomCards)
        {
            cardPool.MoveCardToParent(card, FieldContainer); // �� ī�带 UserCardArea�� �̵�
            card.SetActive(true); // ī�尡 ���̵��� Ȱ��ȭ
            displayedField.Add(card); // �̵��� ī�带 ����Ʈ�� �߰�
        }
    }

    // ī�� �߰� �Լ�
    private void AddCardToField(int row, int col, string cardData)
    {
        if (row >= 0 && row < rows && col >= 0 && col < columns)
        {
            cardField[row, col] = cardData;
        }
    }

}
