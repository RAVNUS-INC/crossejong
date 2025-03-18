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

public class TurnChange : MonoBehaviourPun
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
            else
            {
                RollBackAreas();
                ObjectManager.instance.AlaramMsg.gameObject.SetActive(true);
                ObjectManager.instance.AlaramMsg.text = "단어를 올바르게 입력해주세요.";
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
            ObjectManager.instance.AlaramMsg.gameObject.SetActive(true);
            ObjectManager.instance.AlaramMsg.text = "만든 단어와 입력한 단어가 일치하지 않습니다.";
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
        turnManager.photonView.RPC("SyncAllCardCount", RpcTarget.All, userCardCount, ObjectManager.instance.MyIndexNum);

        if (userCardCount == 0) // 카드를 다 소진했을 때 - 카드 개수가 현재 0개이면
        {
            // 놀이가 종료되었음을 알리는 메시지 1초 정도 표시 후 결과 창 띄우기
            gameResult.EndGameDelay();
        }
        else if (userCardCount > 0) // 처음 카드개수 셀 때만 이 함수를 거침. 카드는 남아있고 지금이 첫 턴에서의 함수 호출일 때 수행
        {
            if (ObjectManager.instance.IsFirstTurn == true)
            {
                // 턴 넘기기 방지를 위한 변수를 이제는 false로 변경
                ObjectManager.instance.IsFirstTurn = false;

                if (ObjectManager.instance.IsMyTurn == true)
                {
                    // 카드 드래그 가능하게
                    userCard.SelectedUserCard(userCard.displayedCards);
                    userCard.SelectedUserCard(userCardFullPopup.fullDisplayedCards);
                }
                else
                {
                    // 카드 드래그 불가능하게
                    userCard.DeActivateCard(userCard.displayedCards, false);
                    userCard.DeActivateCard(userCardFullPopup.fullDisplayedCards, false);
                }

                return; // 턴을 넘기지 않음
            }
            else // 첫 턴이 아닌 재호출이라면 턴을 넘김
            {
                turnManager.FindNextPlayer();
            }
        }
        else 
        {
            return;
        }

    }

    // 롤백 버튼을 누르면 수행되는 함수
    public void RollBackAreas()
    {
        for (int i = 0; i < ObjectManager.instance.createdWordList.Count; i++)
        {
            ObjectManager.instance.createdWordList[i].transform.parent.name = "";
        }
        
        if (ObjectManager.instance.createdWordList.Count > 0)
        {
            cardPool.GetCardsToTarGetArea(ObjectManager.instance.createdWordList, userCard.userCardContainer, userCard.displayedCards); //디스플레이에 카드 되돌리기
            fieldCard.RollBackColorAreas();
            userCard.SelectedUserCard(userCard.displayedCards);
        }
        
        // 다른 유저에게 내가 롤백했음을 알려 카드를 다시 되돌리도록 요청
        // ObjectManager.instance.rollBackList.ToArray() 리스트를 배열로 전환해 받아서 이것을 사용
        //  - 보드판에 해당 문자가 있는지 돌면서 검사 - 있으면 해당 위치 파악, 해당 위치 빈 객체로 초기화
        if (ObjectManager.instance.rollBackList.Count > 0) // 롤백할 무언가가 있으면
        {
            //다른 유저들에게 보드판의 카드 롤백을 요청함
            fieldCard.photonView.RPC(
                "SyncRollCard", RpcTarget.Others, 
                ObjectManager.instance.FinIndexX.ToArray(), 
                ObjectManager.instance.FinIndexY.ToArray(), 
                ObjectManager.instance.rollBackList.ToArray()
                );

            //다른 유저들에게 보드판의 카드 롤백 애니메이션을 요청함
            CardAnimation.instance.photonView.RPC(
                "RollBackCardAnimation", RpcTarget.All,
                ObjectManager.instance.MyIndexNum
                );

            // 롤백리스트, 드롭카운트 초기화
            ObjectManager.instance.rollBackList.Clear(); //문자열 정보 삭제
            ObjectManager.instance.FinIndexX.Clear(); // x좌표 정보 삭제
            ObjectManager.instance.FinIndexY.Clear(); // y좌표 정보 삭제
            ObjectManager.instance.createdWordList.Clear(); //객체 삭제
            ObjectManager.instance.dropCount = 0; // 드롭카운트 초기화

            // UI 상태 초기화
            ObjectManager.instance.RollBackBtn.gameObject.SetActive(false);
            CardDropBtn.gameObject.SetActive(true);
            CardDropBtn.interactable = false;
            cardInputField.gameObject.SetActive(false);
            ObjectManager.instance.inputWords = "";
        }
    }

}