using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;
using System.Reflection;
using Photon.Realtime;
using static UserProfileLoad;
using PlayFab.ClientModels;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public static TurnManager instance; // 싱글톤 인스턴스

    public UserProfileLoad userProfileLoad; // 유저프로필 이미지 참조를 위해 사용
    public GetCard getCard; // 카드 한 장 먹을 때 참조 사용
    public GameResult gameResult; // 판넬 띄울 때 사용
    public TurnChange turnChange; // 카드 개수 위해 사용
    public UserCard userCard; // 턴이 아닐 때 카드 객체 선택 방지를 위해 사용
    public UserCardFullPopup userCardFullPopup; // 턴이 아닐 때 카드 객체 선택 방지를 위해 사용

    public GameObject[] InTurnUserList; // 턴에 있는 상태의 유저 이미지 배열
    public Image[] InTurnUserImg; // 턴에 있는 유저들의 프로필사진
    public TMP_Text[] InTurnUserName, timerText; // 턴에 있는 유저들의 닉네임, 남은 시간을 보여주는 텍스트
    public Color overlayColor = new Color(0, 0, 0, 0.3f); // 검정색 그림자
    public int NextPlayerNum, MyIndexNum; // 다음 플레이어의 액터넘버, 내 UI 인덱스 번호
    public TMP_Text[] CardCount, InTurnCardCount; // 턴에 없을 때와 있을 때의 카드 개수 표시 텍스트 배열

    public UnityEngine.UI.Button endFullPopupButton; //UserCardFullPopup 닫기 버튼

    private float remainingTime = 0f;
    Coroutine TurnRoutine;

    private void Awake()
    {
        instance = this;
    }

    // 카운트다운 3 2 1 후 실행
    public void AfterCountdown()
    {
        // 현재 내 턴
        ObjectManager.instance.IsMyTurn = true;

        // 카드 드래그 활성화 상태를 턴 여부에 따라 설정함
        SetActiveCards();

        // 각종 버튼들과 활성화 및 선택 여부를 턴에 따라 설정함
        SetActiveBtns();

        photonView.RPC("CurrentTurnUI", RpcTarget.All, UserInfoManager.instance.MyActNum);

        // 첫 번째 플레이어로 돌아올 때, 코루틴을 다시 시작
        if (TurnRoutine == null)
        {
            TurnRoutine = StartCoroutine(StartTimer()); // 새 코루틴 시작
        }
    }

    [PunRPC]
    public void CurrentTurnUI(int nextnum)  // 현재 턴 UI 모두에게 같은 모습으로 표시
    {

        for (int i = 0; i < userProfileLoad.sortedPlayers.Length; i++)
        {
            if (nextnum == userProfileLoad.sortedPlayers[i])
            {
                // 현재 턴인 플레이어 UI 설정
                userProfileLoad.InRoomUserList[i].gameObject.SetActive(false);
                InTurnUserList[i].gameObject.SetActive(true);

                // 이미지 및 이름 정보 업데이트
                int index = userProfileLoad.userImageList[i];
                InTurnUserImg[i].sprite = userProfileLoad.profileImages[index];

                string name = userProfileLoad.userNameList[i];
                InTurnUserName[i].text = name;

                // 프로필 이미지 위에 검은 그림자 추가
                InTurnUserImg[i].color = overlayColor;
            }
            else
            {
                // 턴이 아닌 플레이어는 기본 UI 설정
                userProfileLoad.InRoomUserList[i].gameObject.SetActive(true);
                InTurnUserList[i].gameObject.SetActive(false);
                timerText[i].text = "";
            }
        }

    }

    [PunRPC]
    void UpdateTimerRPC(int nextnum, float time) // 남아있는 시간 UI 업데이트
    {
        for (int i = 0; i < userProfileLoad.sortedPlayers.Length; i++)
        {
            if (nextnum == userProfileLoad.sortedPlayers[i])
            {
                timerText[i].text = Mathf.CeilToInt(time).ToString(); // 남은 시간을 정수로 표시
            }
        }
    }

    IEnumerator StartTimer()
    {
        // 방의 커스텀 속성에서 "timeLimit" 값을 가져오기
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("timeLimit"))
        {
            // "timeLimit" 값을 가져와서 사용할 수 있습니다.
            int timeLimit = (int)PhotonNetwork.CurrentRoom.CustomProperties["timeLimit"];
            remainingTime = Convert.ToSingle(timeLimit);
        }

        while (remainingTime > 0)
        {
            // 모든 클라이언트에게 시간 업데이트 전송
            photonView.RPC("UpdateTimerRPC", RpcTarget.All, UserInfoManager.instance.MyActNum, remainingTime);

            remainingTime -= 1f;

            yield return new WaitForSeconds(1f); // 1초 대기
        }
        OnTimerEnd();
    }

    // 시간이 끝났을 때
    void OnTimerEnd()
    {
        if (TurnRoutine != null)
        {
            StopCoroutine(TurnRoutine); // 코루틴 중지
        }
        TurnRoutine = null;

        Debug.Log("시간 초과! 턴을 넘깁니다.");

        endFullPopupButton.onClick.Invoke(); // UserCardFullPopup 창 닫기

        // 카드 한 장 먹고 ui 업데이트, 롤백도 수행
        getCard.GetCardToUserCard();

        // 다음 턴의 플레이어 찾기 
        FindNextPlayer();
    }

    public void FindNextPlayer() // 다음 플레이어의 넘버 찾기(마지막 플레이어일 경우 0번 인덱스로 순환)
    {

        ObjectManager.instance.IsMyTurn = false; // 내 턴이 아님

        // 카드 드래그 활성화 상태를 턴 여부에 따라 설정함
        SetActiveCards();

        // 각종 버튼들과 활성화 및 선택 여부를 턴에 따라 설정함
        SetActiveBtns();

        // 플레이어 목록에서 현재 플레이어의 인덱스를 찾음
        MyIndexNum = Array.IndexOf(userProfileLoad.sortedPlayers, UserInfoManager.instance.MyActNum);

        // 다음 플레이어의 인덱스를 계산 (마지막 플레이어일 경우 순환)
        int nextIndex = (MyIndexNum + 1) % userProfileLoad.sortedPlayers.Length;

        // 다음 플레이어의 액터 넘버
        int nextActorNumber = userProfileLoad.sortedPlayers[nextIndex];
        NextPlayerNum = nextActorNumber;
        Debug.Log($"[턴 순서] 다음 플레이어 ActorNumber: {NextPlayerNum}");

        // 특정 유저가 다음 함수를 실행하도록 요청하기
        photonView.RPC("RequestNextPlayer", RpcTarget.All, NextPlayerNum);
    }

    [PunRPC]
    public void RequestNextPlayer(int targetActorNumber)
    {
        if (targetActorNumber == UserInfoManager.instance.MyActNum)
        {
            AfterCountdown();
        }
        else
        {
            return;
        }
    }

    // 낼 카드가 없어, 카드를 한 장 먹기로 결정 했을 때(카드 추가 버튼에 리스너 연결)
    public void GoToNextTurnAndAddCard()
    {
        if (ObjectManager.instance.IsMyTurn) //현재 내 턴일 때
        {
            if (TurnRoutine != null)
            {
                StopCoroutine(TurnRoutine); // 현재 코루틴 중지
            }
            TurnRoutine = null;

            Debug.Log("카드를 추가하고 턴을 넘깁니다.");

            // 카드 한 장 먹고 ui 업데이트, 롤백 수행
            getCard.GetCardToUserCard();
        }
    }

    // API 검사 통과에 성공했을 때 - 지금은 임의로 카드내기완료 버튼 클릭 시 바로 연결
    // 현재 카드 개수가 1장 미만인지 계속 검사 - 맞으면 모두에게 판넬 띄우기 요청
    public void TossNextTurn()
    {
        if (ObjectManager.instance.IsMyTurn) //현재 내 턴일 때
        {
            if (TurnRoutine != null)
            {
                StopCoroutine(TurnRoutine); // 현재 코루틴 중지
            }
            TurnRoutine = null;

            Debug.Log("단어 완성 성공! 턴을 넘깁니다.");

            // 단어완성횟수 +1 증가시키기
            ObjectManager.instance.MyCompleteWordCount++;

            // 나의 카드 개수 ui업데이트 요청, 턴 넘기기 수행
            turnChange.TurnEnd();
        }
    }

    public void LeaveRoom() // 방을 나갈때 - exit 버튼에 연결
    {
        if (PhotonNetwork.InRoom)
        {
            // 나갈때 내가 현재 턴이라면?
            // 현재 작동하던 코루틴을 멈추고 다음사람에게 턴 넘기기
            if (ObjectManager.instance.IsMyTurn) //현재 내 턴일 때
            {
                if (TurnRoutine != null)
                {
                    StopCoroutine(TurnRoutine); // 현재 코루틴 중지
                }
                TurnRoutine = null;

                FindNextPlayer(); // 다음 턴을 탐색

                Debug.Log("현재 턴: O. 게임을 퇴장합니다.");
            }
            else
            {
                // 나갈때 내가 턴이 아니라면?
                Debug.Log("현재 턴: X. 게임을 퇴장합니다.");   
            }
            // 나가기 전 나의 프로필을 모두가 비활성화 하도록 요청
            photonView.RPC("LeftUserActive", RpcTarget.All, UserInfoManager.instance.MyActNum);

            // 액터넘버 번호 삭제 요청하기, 기존의 ui 변화 주의
            userProfileLoad.photonView.RPC("RequestRemoveUserInfo", RpcTarget.MasterClient, UserInfoManager.instance.MyActNum);

            //나가기
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom() // 방을 성공적으로 나갔을 때 호출되는 콜백
    {
        Debug.Log("놀이를 성공적으로 종료했습니다.");

        //로딩바 ui 애니메이션 보여주기
        LoadingSceneController.Instance.LoadScene("Main");
    }

    // 모두가 수행하는 작업 ui관련
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) // 플레이어가 게임 도중 방을 나갔을 때
    {
        // 나간 유저의 액터넘버 찾기
        int leftNum = otherPlayer.ActorNumber;
        Debug.Log($"나간 유저의 액터넘버: {leftNum}");

        // 만약 현재 방에 있는 플레이어가 2명 미만이라면 - 결과는 정해짐
        // 멀티 테스트 시 주석 해제

        //if (userProfileLoad.sortedPlayers.Length < 2)
        //{
        //    if (ObjectManager.instance.IsMyTurn) //현재 내 턴일 때
        //    {
        //        if (TurnRoutine != null)
        //        {
        //            StopCoroutine(TurnRoutine); // 현재 코루틴 중지
        //        }
        //        TurnRoutine = null;
        //    }
        //    Debug.Log($"현재 플레이어가 2명 미만으로 게임이 종료됩니다.");

        //    // 놀이가 종료되었음을 알리는 메시지 1초 정도 표시 후 결과 창 띄우기
        //    gameResult.EndGameDelay();
        //}
    }

    [PunRPC]
    public void LeftUserActive(int leftNum)
    {
        // 플레이어 목록에서 현재 플레이어의 인덱스를 찾음
        int currentIndex = Array.IndexOf(userProfileLoad.sortedPlayers, leftNum);

        if (currentIndex >= 0)
        {
            userProfileLoad.InRoomUserList[currentIndex].gameObject.SetActive(false);
            InTurnUserList[currentIndex].gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("유저 인덱스를 찾을 수 없습니다.");
        }
    }

    [PunRPC]
    private void SyncAllCardCount(int myCount, int index) // 카드 개수 UI를 업데이트 하는 함수
    {
        // 인덱스에 따라 각 개체의 인덱스 번째에 각각의 개수 텍스트 업데이트 모두에게 요청
        CardCount[index].text = myCount.ToString();
        InTurnCardCount[index].text = myCount.ToString();
    }

    public void FindMyIndex() // 내 액터넘버를 바탕으로 현재 나의 UI 인덱스 위치 찾기
    {
        // 플레이어 목록에서 현재 플레이어의 인덱스를 찾음
        MyIndexNum = Array.IndexOf(userProfileLoad.sortedPlayers, UserInfoManager.instance.MyActNum);
    }

    [PunRPC]
    public void ShowResultPopup() 
    {
        gameResult.EndMsg.gameObject.SetActive(false); // 게임 종료 메시지 비활성화
        gameResult.GameResultPopup.gameObject.SetActive(true); // 게임 종료 팝업 활성화
    }

    [PunRPC]
    public void ShowEndGameMsg() // 모두에게 게임 종료 알림 메시지를 띄우도록 하고, 자신의 코루틴이 진행중이라면 종료
    {
        gameResult.ResultPanel.gameObject.SetActive(true); // 게임 결과 판넬 활성화(배경)
        gameResult.EndMsg.gameObject.SetActive(true); // 게임 종료 메시지 활성화
        gameResult.EndMsg.text = "놀이 종료!";

        gameResult.photonView.RPC("UpdateResultData", RpcTarget.All, UserInfoManager.instance.MyActNum, ObjectManager.instance.MyCompleteWordCount); // 모두에게 자신의 결과를 전달함 - 완성횟수와 액터넘버
        Debug.Log("나의 완성횟수를 모두에게 전달했습니다");
    }

    [PunRPC]
    public void StopTurnCoroutine() // 모두에게 보내되, 현재 턴인 사람이라면 코루틴 종료하기
    {
        if (ObjectManager.instance.IsMyTurn) //현재 본인 턴일 때
        {
            if (TurnRoutine != null)
            {
                StopCoroutine(TurnRoutine); // 현재 진행 중인 코루틴 중지
            }
            TurnRoutine = null;

            ObjectManager.instance.IsMyTurn = false; // 턴 상태 비활성화
        }
    }

    
    public void SetActiveBtns() // 턴이 끝난 사람, 이제 턴인 사람에게 버튼의 활성화 여부를 정해줌
    {
        bool IsMyTurn = ObjectManager.instance.IsMyTurn;

        // 카드 추가 버튼 
        getCard.getCardButton.interactable = IsMyTurn;

        //카드 내기 완료 버튼
        turnChange.CardDropBtn.interactable = IsMyTurn;

        // 카드 내기 완료 버튼이 가장 먼저 보이고
        turnChange.CardDropBtn.gameObject.SetActive(true);

        // 인풋필드는 기본적으로 안보이게
        turnChange.cardInputField.gameObject.SetActive(false);
    }

    public void SetActiveCards()
    {
        // 내 카드들의 선택을 활성화
        userCard.DeActivateCard(userCard.displayedCards);

        // 내 카드들의 선택을 활성화 - 팝업
        userCard.DeActivateCard(userCardFullPopup.fullDisplayedCards);
    }
}
