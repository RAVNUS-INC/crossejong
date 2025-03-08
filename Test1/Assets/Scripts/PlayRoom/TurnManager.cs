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
    public static TurnManager instance; // �̱��� �ν��Ͻ�

    public UserProfileLoad userProfileLoad; // ���������� �̹��� ������ ���� ���
    public GetCard getCard; // ī�� �� �� ���� �� ���� ���
    public GameResult gameResult; // �ǳ� ��� �� ���
    public TurnChange turnChange; // ī�� ���� ���� ���
    public UserCard userCard; // ���� �ƴ� �� ī�� ��ü ���� ������ ���� ���
    public UserCardFullPopup userCardFullPopup; // ���� �ƴ� �� ī�� ��ü ���� ������ ���� ���

    public GameObject[] InTurnUserList; // �Ͽ� �ִ� ������ ���� �̹��� �迭
    public Image[] InTurnUserImg; // �Ͽ� �ִ� �������� �����ʻ���
    public TMP_Text[] InTurnUserName, timerText; // �Ͽ� �ִ� �������� �г���, ���� �ð��� �����ִ� �ؽ�Ʈ
    public Color overlayColor = new Color(0, 0, 0, 0.3f); // ������ �׸���
    public int NextPlayerNum, MyIndexNum; // ���� �÷��̾��� ���ͳѹ�, �� UI �ε��� ��ȣ
    public TMP_Text[] CardCount, InTurnCardCount; // �Ͽ� ���� ���� ���� ���� ī�� ���� ǥ�� �ؽ�Ʈ �迭

    public UnityEngine.UI.Button endFullPopupButton; //UserCardFullPopup �ݱ� ��ư

    private float remainingTime = 0f;
    Coroutine TurnRoutine;

    private void Awake()
    {
        instance = this;
    }

    // ī��Ʈ�ٿ� 3 2 1 �� ����
    public void AfterCountdown()
    {
        // ���� �� ��
        ObjectManager.instance.IsMyTurn = true;

        // ī�� �巡�� Ȱ��ȭ ���¸� �� ���ο� ���� ������
        SetActiveCards();

        // ���� ��ư��� Ȱ��ȭ �� ���� ���θ� �Ͽ� ���� ������
        SetActiveBtns();

        photonView.RPC("CurrentTurnUI", RpcTarget.All, UserInfoManager.instance.MyActNum);

        // ù ��° �÷��̾�� ���ƿ� ��, �ڷ�ƾ�� �ٽ� ����
        if (TurnRoutine == null)
        {
            TurnRoutine = StartCoroutine(StartTimer()); // �� �ڷ�ƾ ����
        }
    }

    [PunRPC]
    public void CurrentTurnUI(int nextnum)  // ���� �� UI ��ο��� ���� ������� ǥ��
    {

        for (int i = 0; i < userProfileLoad.sortedPlayers.Length; i++)
        {
            if (nextnum == userProfileLoad.sortedPlayers[i])
            {
                // ���� ���� �÷��̾� UI ����
                userProfileLoad.InRoomUserList[i].gameObject.SetActive(false);
                InTurnUserList[i].gameObject.SetActive(true);

                // �̹��� �� �̸� ���� ������Ʈ
                int index = userProfileLoad.userImageList[i];
                InTurnUserImg[i].sprite = userProfileLoad.profileImages[index];

                string name = userProfileLoad.userNameList[i];
                InTurnUserName[i].text = name;

                // ������ �̹��� ���� ���� �׸��� �߰�
                InTurnUserImg[i].color = overlayColor;
            }
            else
            {
                // ���� �ƴ� �÷��̾�� �⺻ UI ����
                userProfileLoad.InRoomUserList[i].gameObject.SetActive(true);
                InTurnUserList[i].gameObject.SetActive(false);
                timerText[i].text = "";
            }
        }

    }

    [PunRPC]
    void UpdateTimerRPC(int nextnum, float time) // �����ִ� �ð� UI ������Ʈ
    {
        for (int i = 0; i < userProfileLoad.sortedPlayers.Length; i++)
        {
            if (nextnum == userProfileLoad.sortedPlayers[i])
            {
                timerText[i].text = Mathf.CeilToInt(time).ToString(); // ���� �ð��� ������ ǥ��
            }
        }
    }

    IEnumerator StartTimer()
    {
        // ���� Ŀ���� �Ӽ����� "timeLimit" ���� ��������
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("timeLimit"))
        {
            // "timeLimit" ���� �����ͼ� ����� �� �ֽ��ϴ�.
            int timeLimit = (int)PhotonNetwork.CurrentRoom.CustomProperties["timeLimit"];
            remainingTime = Convert.ToSingle(timeLimit);
        }

        while (remainingTime > 0)
        {
            // ��� Ŭ���̾�Ʈ���� �ð� ������Ʈ ����
            photonView.RPC("UpdateTimerRPC", RpcTarget.All, UserInfoManager.instance.MyActNum, remainingTime);

            remainingTime -= 1f;

            yield return new WaitForSeconds(1f); // 1�� ���
        }
        OnTimerEnd();
    }

    // �ð��� ������ ��
    void OnTimerEnd()
    {
        if (TurnRoutine != null)
        {
            StopCoroutine(TurnRoutine); // �ڷ�ƾ ����
        }
        TurnRoutine = null;

        Debug.Log("�ð� �ʰ�! ���� �ѱ�ϴ�.");

        endFullPopupButton.onClick.Invoke(); // UserCardFullPopup â �ݱ�

        // ī�� �� �� �԰� ui ������Ʈ, �ѹ鵵 ����
        getCard.GetCardToUserCard();

        // ���� ���� �÷��̾� ã�� 
        FindNextPlayer();
    }

    public void FindNextPlayer() // ���� �÷��̾��� �ѹ� ã��(������ �÷��̾��� ��� 0�� �ε����� ��ȯ)
    {

        ObjectManager.instance.IsMyTurn = false; // �� ���� �ƴ�

        // ī�� �巡�� Ȱ��ȭ ���¸� �� ���ο� ���� ������
        SetActiveCards();

        // ���� ��ư��� Ȱ��ȭ �� ���� ���θ� �Ͽ� ���� ������
        SetActiveBtns();

        // �÷��̾� ��Ͽ��� ���� �÷��̾��� �ε����� ã��
        MyIndexNum = Array.IndexOf(userProfileLoad.sortedPlayers, UserInfoManager.instance.MyActNum);

        // ���� �÷��̾��� �ε����� ��� (������ �÷��̾��� ��� ��ȯ)
        int nextIndex = (MyIndexNum + 1) % userProfileLoad.sortedPlayers.Length;

        // ���� �÷��̾��� ���� �ѹ�
        int nextActorNumber = userProfileLoad.sortedPlayers[nextIndex];
        NextPlayerNum = nextActorNumber;
        Debug.Log($"[�� ����] ���� �÷��̾� ActorNumber: {NextPlayerNum}");

        // Ư�� ������ ���� �Լ��� �����ϵ��� ��û�ϱ�
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

    // �� ī�尡 ����, ī�带 �� �� �Ա�� ���� ���� ��(ī�� �߰� ��ư�� ������ ����)
    public void GoToNextTurnAndAddCard()
    {
        if (ObjectManager.instance.IsMyTurn) //���� �� ���� ��
        {
            if (TurnRoutine != null)
            {
                StopCoroutine(TurnRoutine); // ���� �ڷ�ƾ ����
            }
            TurnRoutine = null;

            Debug.Log("ī�带 �߰��ϰ� ���� �ѱ�ϴ�.");

            // ī�� �� �� �԰� ui ������Ʈ, �ѹ� ����
            getCard.GetCardToUserCard();
        }
    }

    // API �˻� ����� �������� �� - ������ ���Ƿ� ī�峻��Ϸ� ��ư Ŭ�� �� �ٷ� ����
    // ���� ī�� ������ 1�� �̸����� ��� �˻� - ������ ��ο��� �ǳ� ���� ��û
    public void TossNextTurn()
    {
        if (ObjectManager.instance.IsMyTurn) //���� �� ���� ��
        {
            if (TurnRoutine != null)
            {
                StopCoroutine(TurnRoutine); // ���� �ڷ�ƾ ����
            }
            TurnRoutine = null;

            Debug.Log("�ܾ� �ϼ� ����! ���� �ѱ�ϴ�.");

            // �ܾ�ϼ�Ƚ�� +1 ������Ű��
            ObjectManager.instance.MyCompleteWordCount++;

            // ���� ī�� ���� ui������Ʈ ��û, �� �ѱ�� ����
            turnChange.TurnEnd();
        }
    }

    public void LeaveRoom() // ���� ������ - exit ��ư�� ����
    {
        if (PhotonNetwork.InRoom)
        {
            // ������ ���� ���� ���̶��?
            // ���� �۵��ϴ� �ڷ�ƾ�� ���߰� ����������� �� �ѱ��
            if (ObjectManager.instance.IsMyTurn) //���� �� ���� ��
            {
                if (TurnRoutine != null)
                {
                    StopCoroutine(TurnRoutine); // ���� �ڷ�ƾ ����
                }
                TurnRoutine = null;

                FindNextPlayer(); // ���� ���� Ž��

                Debug.Log("���� ��: O. ������ �����մϴ�.");
            }
            else
            {
                // ������ ���� ���� �ƴ϶��?
                Debug.Log("���� ��: X. ������ �����մϴ�.");   
            }
            // ������ �� ���� �������� ��ΰ� ��Ȱ��ȭ �ϵ��� ��û
            photonView.RPC("LeftUserActive", RpcTarget.All, UserInfoManager.instance.MyActNum);

            // ���ͳѹ� ��ȣ ���� ��û�ϱ�, ������ ui ��ȭ ����
            userProfileLoad.photonView.RPC("RequestRemoveUserInfo", RpcTarget.MasterClient, UserInfoManager.instance.MyActNum);

            //������
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom() // ���� ���������� ������ �� ȣ��Ǵ� �ݹ�
    {
        Debug.Log("���̸� ���������� �����߽��ϴ�.");

        //�ε��� ui �ִϸ��̼� �����ֱ�
        LoadingSceneController.Instance.LoadScene("Main");
    }

    // ��ΰ� �����ϴ� �۾� ui����
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) // �÷��̾ ���� ���� ���� ������ ��
    {
        // ���� ������ ���ͳѹ� ã��
        int leftNum = otherPlayer.ActorNumber;
        Debug.Log($"���� ������ ���ͳѹ�: {leftNum}");

        // ���� ���� �濡 �ִ� �÷��̾ 2�� �̸��̶�� - ����� ������
        // ��Ƽ �׽�Ʈ �� �ּ� ����

        //if (userProfileLoad.sortedPlayers.Length < 2)
        //{
        //    if (ObjectManager.instance.IsMyTurn) //���� �� ���� ��
        //    {
        //        if (TurnRoutine != null)
        //        {
        //            StopCoroutine(TurnRoutine); // ���� �ڷ�ƾ ����
        //        }
        //        TurnRoutine = null;
        //    }
        //    Debug.Log($"���� �÷��̾ 2�� �̸����� ������ ����˴ϴ�.");

        //    // ���̰� ����Ǿ����� �˸��� �޽��� 1�� ���� ǥ�� �� ��� â ����
        //    gameResult.EndGameDelay();
        //}
    }

    [PunRPC]
    public void LeftUserActive(int leftNum)
    {
        // �÷��̾� ��Ͽ��� ���� �÷��̾��� �ε����� ã��
        int currentIndex = Array.IndexOf(userProfileLoad.sortedPlayers, leftNum);

        if (currentIndex >= 0)
        {
            userProfileLoad.InRoomUserList[currentIndex].gameObject.SetActive(false);
            InTurnUserList[currentIndex].gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("���� �ε����� ã�� �� �����ϴ�.");
        }
    }

    [PunRPC]
    private void SyncAllCardCount(int myCount, int index) // ī�� ���� UI�� ������Ʈ �ϴ� �Լ�
    {
        // �ε����� ���� �� ��ü�� �ε��� ��°�� ������ ���� �ؽ�Ʈ ������Ʈ ��ο��� ��û
        CardCount[index].text = myCount.ToString();
        InTurnCardCount[index].text = myCount.ToString();
    }

    public void FindMyIndex() // �� ���ͳѹ��� �������� ���� ���� UI �ε��� ��ġ ã��
    {
        // �÷��̾� ��Ͽ��� ���� �÷��̾��� �ε����� ã��
        MyIndexNum = Array.IndexOf(userProfileLoad.sortedPlayers, UserInfoManager.instance.MyActNum);
    }

    [PunRPC]
    public void ShowResultPopup() 
    {
        gameResult.EndMsg.gameObject.SetActive(false); // ���� ���� �޽��� ��Ȱ��ȭ
        gameResult.GameResultPopup.gameObject.SetActive(true); // ���� ���� �˾� Ȱ��ȭ
    }

    [PunRPC]
    public void ShowEndGameMsg() // ��ο��� ���� ���� �˸� �޽����� ��쵵�� �ϰ�, �ڽ��� �ڷ�ƾ�� �������̶�� ����
    {
        gameResult.ResultPanel.gameObject.SetActive(true); // ���� ��� �ǳ� Ȱ��ȭ(���)
        gameResult.EndMsg.gameObject.SetActive(true); // ���� ���� �޽��� Ȱ��ȭ
        gameResult.EndMsg.text = "���� ����!";

        gameResult.photonView.RPC("UpdateResultData", RpcTarget.All, UserInfoManager.instance.MyActNum, ObjectManager.instance.MyCompleteWordCount); // ��ο��� �ڽ��� ����� ������ - �ϼ�Ƚ���� ���ͳѹ�
        Debug.Log("���� �ϼ�Ƚ���� ��ο��� �����߽��ϴ�");
    }

    [PunRPC]
    public void StopTurnCoroutine() // ��ο��� ������, ���� ���� ����̶�� �ڷ�ƾ �����ϱ�
    {
        if (ObjectManager.instance.IsMyTurn) //���� ���� ���� ��
        {
            if (TurnRoutine != null)
            {
                StopCoroutine(TurnRoutine); // ���� ���� ���� �ڷ�ƾ ����
            }
            TurnRoutine = null;

            ObjectManager.instance.IsMyTurn = false; // �� ���� ��Ȱ��ȭ
        }
    }

    
    public void SetActiveBtns() // ���� ���� ���, ���� ���� ������� ��ư�� Ȱ��ȭ ���θ� ������
    {
        bool IsMyTurn = ObjectManager.instance.IsMyTurn;

        // ī�� �߰� ��ư 
        getCard.getCardButton.interactable = IsMyTurn;

        //ī�� ���� �Ϸ� ��ư
        turnChange.CardDropBtn.interactable = IsMyTurn;

        // ī�� ���� �Ϸ� ��ư�� ���� ���� ���̰�
        turnChange.CardDropBtn.gameObject.SetActive(true);

        // ��ǲ�ʵ�� �⺻������ �Ⱥ��̰�
        turnChange.cardInputField.gameObject.SetActive(false);
    }

    public void SetActiveCards()
    {
        // �� ī����� ������ Ȱ��ȭ
        userCard.DeActivateCard(userCard.displayedCards);

        // �� ī����� ������ Ȱ��ȭ - �˾�
        userCard.DeActivateCard(userCardFullPopup.fullDisplayedCards);
    }
}
