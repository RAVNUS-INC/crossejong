using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using TMPro;
using JetBrains.Annotations;
using System;
using Photon.Pun;
using UnityEditor;
using Unity.VisualScripting;
using System.Text.RegularExpressions;

public class TurnChange : MonoBehaviour
{
    public UserCard userCard;
    public FieldCard fieldCard;
    public CardPool cardPool;
    public UserCardFullPopup userCardFullPopup; // ���� �ƴ� �� ī�� ��ü ���� ������ ���� ���
    public TurnManager turnManager; // �ڽ��� �ε��� ��ȣ �˱� ���� ���
    public GameResult gameResult; // ��� �ǳ� Ȱ��ȭ�� ���� ���

    public int userCardCount; // ������ ī�� ����
    public TMP_InputField cardInputField; // ī�� ���� �Է��ϴ� �ʵ�
    public Button CardDropBtn; // ī�� �������� ������ ��ư - �Ͽ� ���� ��Ȱ��ȭ Ȱ��ȭ
    public string wordInput;
    public bool isContinue;
    public WordLists wordLists;
    public DictionaryAPI dictionaryAPI;

    public List<char> charList = new List<char>
    {'��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��'};

    

    private void Start()
    {
        // ī�带 ���� ��ǲ�ʵ忡 �Է��� �� �ѱ۸� �Է� �����ϵ��� ��
        cardInputField.onValueChanged.AddListener(OnlyKoreanOK);

        //ī�� ���� �Ϸ� ��ư�� ó���� ��Ȱ��ȭ
        CardDropBtn.interactable = false;

        CardDropBtn.onClick.AddListener(() => {
            CardDropBtn.gameObject.SetActive(false); // CardDropBtn ��Ȱ��ȭ
            cardInputField.gameObject.SetActive(true); // cardInputField Ȱ��ȭ
            cardInputField.text = ""; // ��ǲ�ʵ� �Է¶��� �������
        });
    }


    public void IsCreateWord()
    {
        Debug.Log(ObjectManager.instance.dropCount);
        wordInput = cardInputField.text;
        isContinue = true;

        if (wordInput.Length > ObjectManager.instance.dropCount)
        {
            if (ObjectManager.instance.createdWords.Contains(wordInput))  // ���ڷ� �̷���� �ܾ��� ���
            {
                Debug.Log("���ڷθ� �̷���� �ܾ ���� API �˻縦 �����մϴ�");
                // wordInput (���� API �˻� ������)
                ObjectManager.instance.dropCount = 0;
                ObjectManager.instance.inputWords = wordInput;
                StartCoroutine(dictionaryAPI.CheckWordExists(wordInput));
            }

            for (int i = 0; i < ObjectManager.instance.createdWords.Length; i++)
            {
                if (isContinue == false)
                {
                    break;
                }
                else
                {
                    for (int j = 0; j < 19; j++)
                    {
                        if (ObjectManager.instance.createdWords[i] == charList[j])  // ����ī�尡 ���Ե� ���
                        {
                            List<char> words = wordLists.choDictionary[charList[j]];
                            for (int k = 0; k < 588; k++)
                            {
                                if (wordInput[i] == words[k])
                                {
                                    Debug.Log("���� ī��� �̷���� �ܾ ���� API �˻縦 �����մϴ�");
                                    // wordInput (���� API �˻� ������)
                                    isContinue = false;
                                    ObjectManager.instance.dropCount = 0;
                                    ObjectManager.instance.inputWords = wordInput;
                                    StartCoroutine(dictionaryAPI.CheckWordExists(wordInput));
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < ObjectManager.instance.createdWords.Length; i++)
            {
                if (ObjectManager.instance.createdWords[i] == 'C' || ObjectManager.instance.createdWords[i] == 'B')  // Ư��ī�尡 ���Ե� ���
                {
                    if (44032 <= wordInput[i] && wordInput[i] <= 54616)
                    {
                        Debug.Log("Ư�� ī��� �̷���� �ܾ ���� API �˻縦 �����մϴ�");
                        // wordInput (���� API �˻� ������)
                        ObjectManager.instance.dropCount = 0;
                        ObjectManager.instance.inputWords = wordInput;
                        StartCoroutine(dictionaryAPI.CheckWordExists(wordInput));
                        break;
                    }
                }
                else
                    break;
            }
        }
        else
        {
            Debug.Log("�����Դϴ�");
            RollBackAreas();
        }

        
    }

    public void OnlyKoreanOK(string text) // �ܾ� �Է��ʵ忡 �ѱ۸� �ۼ��� �� �ֵ��� ��
    {
        // �ѱ��� ������ ��� ���� ����
        // �ѱ۸� ����ϴ� ���Խ� (���� ���� X)
        string koreanPattern = "^[��-�R]*$";

        if (!Regex.IsMatch(text, koreanPattern))
        {
            cardInputField.text = Regex.Replace(text, "[^��-�R]", ""); // �ѱ� �̿��� ���� ����
        }
    }

    public void TurnEnd()
    {
        ObjectManager.instance.dropCount = 0;

        // �ڽ��� UI �ε��� Ȯ�� �� ������Ʈ
        turnManager.FindMyIndex();

        CountUserCard(userCard.displayedCards.Count);
    }

    public void CountUserCard(int count) //�ڽ��� ī�� ���� ������Ʈ
    {
        userCardCount = count; // ������ ���� ����

        // ��ο��� �ڽ��� ī�� ���� ���� ��û�ϱ� - �ڽ��� ī�尳��, �ڽ��� �ε��� ��ȣ
        turnManager.photonView.RPC("SyncAllCardCount", RpcTarget.All, userCardCount, turnManager.MyIndexNum);

        if (userCardCount == 0) // ī�带 �� �������� �� - ī�� ������ ���� 0���̸�
        {
            // ���̰� ����Ǿ����� �˸��� �޽��� 1�� ���� ǥ�� �� ��� â ����
            gameResult.EndGameDelay();
        }
        else if ((userCardCount > 0) && (ObjectManager.instance.IsFirstTurn)) // ī��� �����ְ� ������ ù �Ͽ����� �Լ� ȣ���̶��(ù ��Ȳ���� ī�� ���� ������Ʈ�� ����)
        {
            // �� �ѱ�� ������ ���� ������ ������ false�� ����
            ObjectManager.instance.IsFirstTurn = false;

            return; // ���� �ѱ��� ����
        }
        else // ù ���� �ƴ� ��ȣ���̶�� ���� �ѱ�
        {
            turnManager.FindNextPlayer();
        }

    }

    public void RollBackAreas()
    {
        cardPool.MoveCardsToTarGetArea(ObjectManager.instance.createdWordList, userCard.userCardContainer, userCard.displayedCards);
        ObjectManager.instance.createdWordList.Clear();
        fieldCard.RollBackColorAreas();
        userCard.SelectedUserCard();
    }

}