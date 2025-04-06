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
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Image = UnityEngine.UI.Image;
using DG.Tweening;
using Button = UnityEngine.UI.Button;
using System.Linq; // 꼭 필요함!


public class TurnManager : MonoBehaviourPunCallbacks
{
    public static TurnManager instance; // 싱글톤 인스턴스

    public UserProfileLoad userProfileLoad; // 유저프로필 이미지 참조를 위해 사용
    public GetCard getCard; // 카드 한 장 먹을 때 참조 사용
    public GameResult gameResult; // 판넬 띄울 때 사용
    public TurnChange turnChange; // 카드 개수 위해 사용
    public UserCard userCard; // 턴이 아닐 때 카드 객체 선택 방지를 위해 사용
    public UserCardFullPopup userCardFullPopup; // 턴이 아닐 때 카드 객체 선택 방지를 위해 사용
    public FieldCard fieldCard; //단어완성 성공 시 다른 유저들 보드판에 실제 업데이트

    public GameObject[] InTurnUserList; // 턴에 있는 상태의 유저 이미지 배열
    public Image[] InTurnUserImg, timerImages; // 턴에 있는 유저들의 프로필사진, 남은 타이머 UI 이미지
    public TMP_Text[] InTurnUserName, timerText; // 턴에 있는 유저들의 닉네임, 남은 시간을 보여주는 텍스트
    public Color overlayColor = new Color(0, 0, 0, 0.3f); // 검정색 그림자
    public int NextPlayerNum; // 다음 플레이어의 액터넘버, 내 UI 인덱스 번호
    public TMP_Text[] CardCount, InTurnCardCount; // 턴에 없을 때와 있을 때의 카드 개수 표시 텍스트 배열

    public UnityEngine.UI.Button endFullPopupButton; //UserCardFullPopup 닫기 버튼

    private float TimeLimit = 0f; //방에서 설정된 제한시간
    private float remainingTime = 0f; //타이머에서 남은 시간
    Coroutine TurnRoutine;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 방의 커스텀 속성에서 "timeLimit" 값을 가져오기
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("timeLimit"))
        {
            // "timeLimit" 값을 가져옴
            TimeLimit = (int)PhotonNetwork.CurrentRoom.CustomProperties["timeLimit"];
        }

        //리스트 복제해놓기
        gameResult.allActorNums = userProfileLoad.ActPlayerIntList.ToList();
    }

    // 카운트다운 3 2 1 후 실행
    public void AfterCountdown()
    {
        // 현재 내 턴
        ObjectManager.instance.IsMyTurn = true;

        // 턴이 바뀌었으므로 버튼들 공통 상태 반영
        SetActiveBtns();

        // 카드 드래그 가능하게
        userCard.DeActivateCard(userCard.displayedCards, true);
        userCard.DeActivateCard(userCardFullPopup.fullDisplayedCards, true);

        // 카드를 다 쓴 상황에서는 카드 추가 버튼 비활성화 유지
        if (ObjectManager.instance.AllUsedCard)
        {
            getCard.getCardButton.interactable = false;
        }
        else
        {
            getCard.getCardButton.interactable = true;
        }
        
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
        for (int i = 0; i < userProfileLoad.ActPlayerIntList.Count; i++)
        {
            if (nextnum == userProfileLoad.ActPlayerIntList[i])
            {
                // 현재 턴인 플레이어 UI 설정
                userProfileLoad.InRoomUserList[i].gameObject.SetActive(false);
                InTurnUserList[i].gameObject.SetActive(true);

                // 이미지 및 이름 정보 업데이트
                int index = (int)userProfileLoad.GetProfileIndexByActorNumber(nextnum);
                InTurnUserImg[i].sprite = userProfileLoad.profileImages[index];

                string name = userProfileLoad.GetUserNameByActorNumber(nextnum);
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
        for (int i = 0; i < userProfileLoad.ActPlayerIntList.Count; i++)
        {
            if (nextnum == userProfileLoad.ActPlayerIntList[i])
            {
                timerText[i].text = Mathf.CeilToInt(time).ToString(); // 남은 시간을 정수로 표시
                timerImages[i].fillAmount = time / TimeLimit; //남은 시간에 맞게 타이머이미지 업데이트
            }
        }
    }

    IEnumerator StartTimer()
    {
        remainingTime = Convert.ToSingle(TimeLimit);

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

        if (!ObjectManager.instance.AllUsedCard)// 카드를 다 쓴 상태가 아직 아니라면
        {
            // 카드 한 장 먹고 ui 업데이트, 롤백 수행, 턴 넘기기
            getCard.GetCardToUserCard();
        }
        else //카드를 다 쓴 상태라면
        {
            // 나는 게임 턴에서 이제 제외됨
            ObjectManager.instance.EndMyTurn = true;

            // 모두에게 턴 제외 리스트 추가 및 동기화 요청(액터 번호를 넘겨줌)
            photonView.RPC("UpdateExcludedList", RpcTarget.All, UserInfoManager.instance.MyActNum);
        }

    }

    public void FindNextPlayer() // 다음 플레이어의 넘버 찾기(마지막 플레이어일 경우 0번 인덱스로 순환)
    {

        ObjectManager.instance.IsMyTurn = false; // 내 턴이 아님

        // 내 카드들의 선택 상태 변경
        userCard.DeActivateCard(userCard.displayedCards, false);
        userCard.DeActivateCard(userCardFullPopup.fullDisplayedCards, false);

        // 각종 버튼들과 활성화 및 선택 여부를 턴에 따라 설정함
        SetActiveBtns();

        // 카드 추가 버튼 비활성화
        getCard.getCardButton.interactable = false;
        
        // 다음 유저 인덱스 찾고 요청
        FindNextPlayerIndex();

        // 카드 전부 원위치
        turnChange.RollBackAreas();

        
    }

    private void FindNextPlayerIndex()
    {
        // 다음 플레이어의 인덱스를 계산 (마지막 플레이어일 경우 순환)
        int CurrentIndex = ObjectManager.instance.MyIndexNum;

        int NextIndex = (CurrentIndex + 1) % userProfileLoad.ActPlayerIntList.Count; // 다음 인덱스 계산

        while (ObjectManager.instance.turnExcluded.Contains(NextIndex)) //포함하지 않을때까지 돌림
        {
            CurrentIndex = NextIndex;  // 현재 인덱스 갱신
            NextIndex = (CurrentIndex + 1) % userProfileLoad.ActPlayerIntList.Count;  // 다음 인덱스 계산
        }

        // 다음 플레이어의 액터 넘버
        int nextActorNumber = userProfileLoad.ActPlayerIntList[NextIndex];

        NextPlayerNum = nextActorNumber;

        // 특정 유저가 다음 함수를 실행하도록 요청하기
        photonView.RPC("RequestNextPlayer", RpcTarget.All, NextPlayerNum);
    }

    [PunRPC]
    public void RequestNextPlayer(int targetActorNumber)
    {
        if (targetActorNumber == UserInfoManager.instance.MyActNum)
        {
            // 상태메시지 업데이트 요청
            photonView.RPC("RequestTurnMsg", RpcTarget.All, UserInfoManager.instance.MyName);

            if (ObjectManager.instance.IsFirstTurn == true)
            {
                // 카드 드래그 가능하게
                userCard.SelectedUserCard(userCard.displayedCards);
                userCard.SelectedUserCard(userCardFullPopup.fullDisplayedCards);
                // 나의 첫 턴은 끝
                ObjectManager.instance.IsFirstTurn = false;
            }
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

            // 카드 한 장 먹고 ui 업데이트, 롤백 수행, 턴 넘기기
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

            // 만든 단어, 리스트 모두 비우기
            ObjectManager.instance.rollBackList.Clear();
            ObjectManager.instance.FinIndexX.Clear(); // x좌표 정보 삭제
            ObjectManager.instance.FinIndexY.Clear(); // y좌표 정보 삭제
            ObjectManager.instance.createdWordList.Clear(); //객체 삭제  
            ObjectManager.instance.dropCount = 0; //카운트 0  

            // 단어완성횟수 +1 증가시키기
            ObjectManager.instance.MyCompleteWordCount++;

            // 나의 카드 개수 ui업데이트 요청, 턴 넘기기 수행
            turnChange.TurnEnd();
        }
    }

    public void LeaveRoom() // 방을 나갈때 - exit 나가기 버튼에 연결
    {
        if (PhotonNetwork.InRoom)
        {
            if (ObjectManager.instance.IsMyTurn) //현재 내 턴일 때
            {
                StopAllCoroutines(); // 실행되고 있는 모든 코루틴 중단

                FindNextPlayer(); // 다음 턴을 탐색

                Debug.Log("현재 턴: O. 게임을 퇴장합니다.");
            }
            else
            {
                // 나갈때 내가 턴이 아니라면?
                Debug.Log("현재 턴: X. 게임을 퇴장합니다.");   
            }

            //방장에게 자신의 단어완성횟수 전달
            photonView.RPC("ReceiveUserData", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, ObjectManager.instance.MyCompleteWordCount);

            //나가기
            PhotonNetwork.LeaveRoom();
        }
        //로딩바 ui 애니메이션 보여주기
        LoadingSceneController.Instance.LoadScene("Main");
    }

    public void DestroyPlayRoomAndAllChildren()
    {
        // PlayRoom 객체 찾기
        GameObject playRoom = GameObject.Find("PlayRoom");

        if (playRoom != null)
        {
            // PlayRoom 객체 하위의 모든 자식 객체들을 순차적으로 삭제
            foreach (Transform child in playRoom.transform)
            {
                PhotonView photonView = child.GetComponent<PhotonView>();

                if (photonView != null && photonView.IsMine)
                {
                    // PhotonView가 있는 객체는 네트워크에서도 삭제
                    PhotonNetwork.Destroy(child.gameObject);  // 네트워크에서 객체 삭제
                }
                else
                {
                    // PhotonView가 없는 일반 객체는 로컬에서 삭제
                    Destroy(child.gameObject);  // 로컬 씬에서 객체 삭제
                }
                //Debug.Log("삭제된 객체: " + child.gameObject.name);
            }

            // 이제 PlayRoom 객체 자체도 삭제
            Destroy(playRoom);
            Debug.Log("PlayRoom 객체와 하위 객체들 삭제 완료!");
        }
        else
        {
            Debug.LogWarning("PlayRoom 객체를 찾을 수 없습니다.");
        }
    }

    public override void OnLeftRoom() // 방을 성공적으로 나갔을 때 호출되는 콜백
    {
        Debug.Log("놀이를 성공적으로 종료했습니다.");

    }

    // 모두가 수행하는 작업 ui관련
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) // 플레이어가 게임 도중 방을 나갔을 때
    {
        // 나간 유저의 액터넘버 찾기
        int leftNum = otherPlayer.ActorNumber;
        Debug.Log($"나간 유저의 액터넘버: {leftNum}");

        LeftUserActive(leftNum); //프로필 비활성화

        // 만약 현재 방에 있는 플레이어가 1명 뿐이라면
        // 또는 현재 턴에 있는 사람이 1명 뿐이라면
        // 멀티 테스트 시 주석 해제
        if (userProfileLoad.ActPlayerIntList.Count < 2 || (ObjectManager.instance.turnExcluded.Count == userProfileLoad.ActPlayerIntList.Count - 1))
        {
            if (ObjectManager.instance.IsMyTurn) //현재 내 턴일 때
            {
                if (TurnRoutine != null)
                {
                    StopCoroutine(TurnRoutine); // 현재 코루틴 중지
                }
                TurnRoutine = null;
            }
            Debug.Log($"현재 플레이어가 2명 미만으로 게임이 종료됩니다.");

            // 놀이가 종료되었음을 알리는 메시지 1초 정도 표시 후 결과 창 띄우기
            gameResult.EndGameDelay();
        }
    }

    public void LeftUserActive(int leftNum) //누군가 나갔을 때나 턴제외 상황 ui처리
    {
        // 플레이어 목록에서 현재 플레이어의 인덱스를 찾음
        int currentIndex = FindMyIndex(leftNum);

        if (currentIndex >= 0)
        {
            // 기본 프로필만 활성화
            userProfileLoad.InRoomUserList[currentIndex].gameObject.SetActive(true);
            InTurnUserList[currentIndex].gameObject.SetActive(false);

            // 기본적으로 프로필 이미지 위에 검은 그림자 추가
            userProfileLoad.InRoomUserImg[currentIndex].color = overlayColor;


            //현재 유저가 방에 있다면 ->관전 중, 그게 아니면 나간 상태
            if (PhotonNetwork.CurrentRoom.PlayerCount == userProfileLoad.ActPlayerIntList.Count)
            {
                CardCount[currentIndex].text = "관전";
            }
            else
            {
                CardCount[currentIndex].text = "나감";

                //if (PhotonNetwork.LocalPlayer.IsMasterClient) //방장이 나간 사람의 프로퍼티를 기록해줌
                //{
                //    Hashtable hash = new Hashtable();
                //    hash[$"Left_{leftNum}"] = true;
                //    PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

                //    Debug.Log("나간 유저의 값을 기록했습니다");
                //}

                //액터넘버 삭제하기
                userProfileLoad.ActPlayerIntList.Remove(leftNum);
            }
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

    public int FindMyIndex(int Actnum) // 내 액터넘버를 바탕으로 현재 나의 UI 인덱스 위치 찾기
    {
        int index = userProfileLoad.ActPlayerIntList.IndexOf(Actnum);
        return index;
    }

    [PunRPC]
    public void ShowEndGameMsg() // 모두에게 게임 종료 알림 메시지를 띄우도록 하고, 자신의 코루틴이 진행중이라면 종료
    {
        //카드 개수, 단어 완성횟수 전달하기
        int myactnum = UserInfoManager.instance.MyActNum;
        Hashtable hash = new Hashtable();

        hash[$"CompletedWords_{myactnum}"] = ObjectManager.instance.MyCompleteWordCount;
        hash[$"CardLeft_{myactnum}"] = UserCard.instance.displayedCards.Count;
        hash[$"Left_{myactnum}"] = false;

        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

        Debug.Log("나의 완성횟수를 모두에게 전달했습니다");

        gameResult.ResultPanel.gameObject.SetActive(true); // 게임 결과 판넬 활성화(배경)
        gameResult.EndMsg.gameObject.SetActive(true); // 게임 종료 메시지 활성화
        gameResult.EndMsg.text = "놀이 종료!";
    }

    [PunRPC]
    public void ShowResultPopup()
    {
        gameResult.EndMsg.gameObject.SetActive(false); // 게임 종료 메시지 비활성화
        gameResult.GameResultPopup.gameObject.SetActive(true); // 게임 종료 팝업 활성화
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // "Left_" 키가 하나라도 포함되어 있으면 체크
        foreach (var key in propertiesThatChanged.Keys)
        {
            if (key.ToString().StartsWith("Left_"))
            {
                gameResult.CheckIfAllPlayersSubmitted();
                break;
            }
        }
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
            gameResult.MainCheckTime();
        }
    }

    public void SetActiveBtns() // 턴이 바뀌면 모두에게 공통의 버튼 활성화 여부를 정해줌
    {
        //카드 내기 완료 버튼
        turnChange.CardDropBtn.interactable = false;

        // 롤백버튼은 항상 처음 비활성화
        ObjectManager.instance.RollBackBtn.gameObject.SetActive(false);

        // 알람메시지 없애기
        ObjectManager.instance.AlaramMsg.gameObject.SetActive(false);
    }

    [PunRPC]
    public void ReceiveUserData(int senderActorNum, int data) //방장이 나간 유저의 완성횟수를 전달받음
    {
        Debug.Log($"유저 {senderActorNum} 로부터 받은 데이터: {data}");

        if (PhotonNetwork.LocalPlayer.IsMasterClient) //방장이 나간 사람의 프로퍼티를 기록해줌
        {
            Hashtable hash = new Hashtable();
            hash[$"Left_{senderActorNum}"] = true;
            hash[$"CompletedWords_{senderActorNum}"] = data;
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

            Debug.Log("나간 유저의 값을 기록했습니다");
        }
    }



    [PunRPC]
    private void RequestTurnMsg(string turnUsername) // 현재 턴 메시지를 모두가 업데이트하는 함수
    {
        ObjectManager.instance.StatusMsg.text = $"{turnUsername}님의 차례";
    }

    [PunRPC]
    private void UpdateExcludedList(int UserNum)
    {
        int UserIndex = FindMyIndex(UserNum);

        // 턴 제외 리스트에 해당 유저를 추가
        ObjectManager.instance.turnExcluded.Add(UserIndex);

        LeftUserActive(UserNum); //프로필 비활성화

        // 턴에서 제외된 사람의 수가 현재 게임 내 플레이어 수 - 1의 값과 같다면(턴에 한 명만 남은 상태)
        if (ObjectManager.instance.turnExcluded.Count == userProfileLoad.ActPlayerIntList.Count - 1)
        {
            // 남은 한 명의 승리이므로 게임 결과창 표시 - 방장의 요청에 의해
            if (PhotonNetwork.IsMasterClient)
            {
                // 놀이가 종료되었음을 알리는 메시지 1초 정도 표시 후 결과 창 띄우기
                gameResult.EndGameDelay();
            }
            else { return; }
        }
        else
        {
            if (ObjectManager.instance.EndMyTurn) //턴에서 제외된 사람이 다음 플레이어 요청
            {
                FindNextPlayer();
            }
            else { return; }
        }
    }
}
