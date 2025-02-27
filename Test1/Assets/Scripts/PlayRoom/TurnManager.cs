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

public class TurnManager : MonoBehaviourPun
{
    public UserProfileLoad userProfileLoad; // ���������� �̹��� ������ ���� ���

    public GameObject[] InTurnUserList; // �Ͽ� �ִ� ������ ���� �̹��� �迭
    public Image[] InTurnUserImg; // �Ͽ� �ִ� �������� �����ʻ���
    public TMP_Text[] InTurnUserName; // �Ͽ� �ִ� �������� �г���
    public TMP_Text[] timerText; // ���� �ð��� �����ִ� �ؽ�Ʈ
    public Color overlayColor = new Color(0, 0, 0, 0.7f); // ������ 70%
    //private float remainingTime = 5f; // 30�� Ÿ�̸�
    private int MyNum, NextPlayerNum; // ���� �÷��̾��� ���ͳѹ�
    public bool IsMyTurn = false; // ���� ������ �ƴ��� Ȯ��
    Coroutine TurnRoutine;

    private void Start()
    {
        // ���� �� ���ͳѹ� ã��
        MyNum = PhotonNetwork.LocalPlayer.ActorNumber;
    }

    // ī��Ʈ�ٿ� 3 2 1 �� ����
    public void AfterCountdown()
    {
        IsMyTurn = true;

        photonView.RPC("CurrentTurnUI", RpcTarget.All, MyNum);

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
        float remainingTime = 5f;

        while (remainingTime > 0)
        {
            // ��� Ŭ���̾�Ʈ���� �ð� ������Ʈ ����
            photonView.RPC("UpdateTimerRPC", RpcTarget.All, MyNum, remainingTime);

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

        IsMyTurn = false;

        //photonView.RPC("UpdateTimerRPC", RpcTarget.All, MyNum, 0);

        Debug.Log("�ð� �ʰ�! ���� �ѱ�ϴ�.");

        // ���� ���� �÷��̾� ã�� 
        FindNextPlayer();
    }

    public void FindNextPlayer()
    {
        // ���� �÷��̾��� �ѹ� ã��(������ �÷��̾��� ��� 0�� �ε����� ��ȯ)
        // �÷��̾� ��Ͽ��� ���� �÷��̾��� �ε����� ã��
        Debug.Log(MyNum);
        int currentIndex = Array.IndexOf(userProfileLoad.sortedPlayers, MyNum);
        Debug.Log(currentIndex);
        // ���� �÷��̾��� �ε����� ��� (������ �÷��̾��� ��� ��ȯ)
        int nextIndex = (currentIndex + 1) % userProfileLoad.sortedPlayers.Length;
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
        if (targetActorNumber == MyNum)
        {
            AfterCountdown();
        }
        else
        {
            return;
        }

    }
}
