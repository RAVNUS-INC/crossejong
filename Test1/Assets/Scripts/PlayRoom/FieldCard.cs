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
    public int rows; // �� ũ��
    public int columns; // �� ũ��
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

    public void FieldCardf()
    {
        foreach (var card in cardPool.GetRandomCards(1)) // ���� ī�� 1�� ����
        {
            cardPool.MoveCardToParent(card, FieldContainer); // ī�带 FieldArea�� �̵�
            card.SetActive(true); // ī�尡 ���̵��� Ȱ��ȭ
            displayedField.Add(card); // �̵��� ī�带 ����Ʈ�� �߰�
            AddCardToField(1, 1, "dd"); // 3x3 ����� (1,1)�� ī�� �߰�
        }
    }

    // ī�� �߰� �Լ�
    private void AddCardToField(int row, int column, string card)
    {
        // ��ȿ�� �ε������� Ȯ��
        if (row >= 0 && row < rows && column >= 0 && column < columns)
        {
            if (cardField[row, column] == null) // �ش� ��ġ�� ��� �ִ� ��쿡�� �߰�
            {
                cardField[row, column] = card; // 3x3 ��Ŀ� ī�� �߰�
            }
        }
    }


}
