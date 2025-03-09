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
    public UserCardFullPopup userCardFullPopup; // 턴이 아닐 때 카드 객체 선택 방지를 위해 사용
    public TurnManager turnManager; // 자신의 인덱스 번호 알기 위해 사용
    public GameResult gameResult; // 결과 판넬 활성화를 위해 사용

    public int userCardCount; // 본인의 카드 개수
    public TMP_InputField cardInputField; // 카드 내고 입력하는 필드
    public Button CardDropBtn; // 카드 내고나서 누르는 버튼 - 턴에 따라 비활성화 활성화
    public string wordInput;
    public bool isContinue;
    public WordLists wordLists;
    public DictionaryAPI dictionaryAPI;

    public List<char> charList = new List<char>
     {'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'};



    private void Start()
    {
        // 카드를 내고 인풋필드에 입력할 때 한글만 입력 가능하도록 함
        cardInputField.onValueChanged.AddListener(OnlyKoreanOK);

        //카드 내기 완료 버튼을 처음엔 비활성화
        CardDropBtn.interactable = false;

        CardDropBtn.onClick.AddListener(() => {
            CardDropBtn.gameObject.SetActive(false); // CardDropBtn 비활성화
            cardInputField.gameObject.SetActive(true); // cardInputField 활성화
            cardInputField.text = ""; // 인풋필드 입력란을 비워놓음
        });
    }


    public void IsCreateWord()
    {
        Debug.Log(ObjectManager.instance.dropCount);
        wordInput = cardInputField.text;
        isContinue = true;

        if (wordInput.Length > ObjectManager.instance.dropCount)
        {
            if (ObjectManager.instance.createdWords.Contains(wordInput))  // 글자로 이루어진 단어일 경우
            {
                Debug.Log("글자로만 이루어진 단어를 사전 API 검사를 시작합니다");
                // wordInput  (사전 API 검사 돌리기)
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
                        if (ObjectManager.instance.createdWords[i] == charList[j])  // 자음카드가 포함된 경우
                        {
                            List<char> words = wordLists.choDictionary[charList[j]];
                            for (int k = 0; k < 588; k++)
                            {
                                if (wordInput[i] == words[k])
                                {
                                    Debug.Log("자음 카드로 이루어진 단어를 사전 API 검사를 시작합니다");
                                    // wordInput (사전 API 검사 돌리기)
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
                if (ObjectManager.instance.createdWords[i] == 'C' || ObjectManager.instance.createdWords[i] == 'B')  // 특수카드가 포함된 경우
                {
                    if (44032 <= wordInput[i] && wordInput[i] <= 54616)
                    {
                        Debug.Log("특수 카드로 이루어진 단어를 사전 API 검사를 시작합니다");
                        // wordInput (사전 API 검사 돌리기)
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
            Debug.Log("오류입니다");
            RollBackAreas();
        }

        
    }

    public void OnlyKoreanOK(string text) // 단어 입력필드에 한글만 작성할 수 있도록 함
    {
        // 한글을 제외한 모든 문자 제외
        // 한글만 허용하는 정규식 (띄어쓰기 포함 X)
        string koreanPattern = "^[가-힣]*$";

        if (!Regex.IsMatch(text, koreanPattern))
        {
            cardInputField.text = Regex.Replace(text, "[^가-힣]", ""); // 한글 이외의 문자 제거
        }
    }

    public void TurnEnd()
    {
        ObjectManager.instance.dropCount = 0;

        // 자신의 UI 인덱스 확인 및 업데이트
        turnManager.FindMyIndex();

        CountUserCard(userCard.displayedCards.Count);

        ObjectManager.instance.createdWordList.Clear();
    }

    public void CountUserCard(int count)  //자신의 카드 개수 업데이트
    {
        userCardCount = count; // 변수에 개수 저장

        // 모두에게 자신의 카드 개수 전달 요청하기 - 자신의 카드개수, 자신의 인덱스 번호
        turnManager.photonView.RPC("SyncAllCardCount", RpcTarget.All, userCardCount, turnManager.MyIndexNum);

        if (userCardCount == 0) // 카드를 다 소진했을 때 - 카드 개수가 현재 0개이면
        {
            // 놀이가 종료되었음을 알리는 메시지 1초 정도 표시 후 결과 창 띄우기
            gameResult.EndGameDelay();
        }
        else if ((userCardCount > 0) && (ObjectManager.instance.IsFirstTurn)) // 카드는 남아있고 지금이 첫 턴에서의 함수 호출이라면
        {
            // 턴 넘기기 방지를 위한 변수를 이제는 false로 변경
            ObjectManager.instance.IsFirstTurn = false;

            return; // 턴을 넘기지 않음
        }
        else // 첫 턴이 아닌 재호출이라면 턴을 넘김
        {
            turnManager.FindNextPlayer();
        }

    }

    public void RollBackAreas()
    {
        cardPool.GetCardsToTarGetArea(ObjectManager.instance.createdWordList, userCard.userCardContainer, userCard.displayedCards);
        fieldCard.RollBackColorAreas();
        userCard.SelectedUserCard();
    }

}