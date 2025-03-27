using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UserProfileLoad;

public class GameResult : MonoBehaviourPunCallbacks
{
    public UserProfileLoad userProfileLoad; // playersŬ���� ����� ����
    public TurnManager turnManager; // �ܾ�ϼ�Ƚ�� ���� �� ����� ����

    public GameObject ResultPanel, GameResultPopup; // ���̰�� �ǳ�, �˾�â
    public TMP_Text EndMsg, TimeMsg; // ���� ���� �� ����� ��� �޽���, �˾� ���� ���ư� �޽���
    public GameObject[] ResultUserList; // ���̰�� �������� ����Ʈ
    public Image[] ResultUserImg; // ���̰�� �������� �����ʻ���
    public TMP_Text[] ResultUserName, ResultWordCount; // ���̰�� �������� �г���, ī�� ���� �ð�
    Coroutine BacktoMainRoutine;
    Coroutine EndGameDelayRoutine;

    private void Awake()
    {
        SetActive(); // ����Ʈ �⺻ ��Ȱ��ȭ �ʱ�ȭ
    }

    // ------VVV
    // ī�带 ���� ���� ������ ������ �ܾ�ϼ�Ƚ�� ������������ ������ ����Ʈ�κ��� ��� ǥ��
    // sortedplayers����Ʈ�� �ִ� �������� �������� ���â ���� ǥ��(������ ������ �÷����� ������)
    // ������ ī�带 �����ϸ� �ٸ� ��ο��� rpc�Լ��� ���� �˸� ��û
    // 2���� ��� -> �Ѹ��� ī�� �� ���� -> ��� �ٷ� ǥ��
    // ��� Ȯ�� ��ư ������ �������� ���ư�����
    // Ȯ�� ��ư�� ������ �ʾƵ� 15�ʵڿ� �������� �ڵ����� �̵��ϵ��� �˸��޽��� �����ϱ�

    public void SetStat() //���� �ʱ� ���� Ȯ�ι�ư(usersetmanager)�� ����
    {
        // 1. ���� ��� �� ��������
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
            result =>
            {
                // ���� ���� �����ͼ�
                var currentCompletionCount = result.Statistics.FirstOrDefault(stat => stat.StatisticName == "WordCompletionCount")?.Value ?? 0;

                // ���� ���� ���ϱ�
                var newCompletionCount = currentCompletionCount + ObjectManager.instance.MyCompleteWordCount;

                // 2. ���ο� ������ ��� ������Ʈ
                var request = new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>
                    {
                new StatisticUpdate { StatisticName = "WordCompletionCount", Value = newCompletionCount }
                    }
                };

                PlayFabClientAPI.UpdatePlayerStatistics(request,
                    (updateResult) => print("�ܾ�ϼ�Ƚ�� ���� �Ϸ�"),
                    (error) => print("���� ���� ����"));
            },
            error => print("���� ��� �� �������� ����"));
    }

    public void MainCheckTime()
    {
        SetStat(); // �ܾ�ϼ�Ƚ���� playfab�� ��� ������Ʈ

        //// ���â�� ������� 15�� Ÿ�̸Ӹ� ���� - ���� ���ޱ��� ���� �ð�
        //if (BacktoMainRoutine == null)
        //{
        //    BacktoMainRoutine = StartCoroutine(StartTimer()); // �� �ڷ�ƾ ����
        //}
    }
    public void EndGameDelay()
    {

        // ���� ���Ḧ �˸��� �޽��� �� 1�ʰ� ǥ�� Ÿ�̸� ����
        if (EndGameDelayRoutine == null)
        {
            EndGameDelayRoutine = StartCoroutine(EndGameTimer()); // �� �ڷ�ƾ ����
        }
    }

    IEnumerator StartTimer()
    {
        float remainingTime = 15f;

        while (remainingTime > 0)
        {
            TimeMsg.text = $"{remainingTime.ToString()}�� �ڿ� �������� ���ư��ϴ�.";

            remainingTime -= 1f;

            yield return new WaitForSeconds(1f); // 1�� ���
        }

        OnTimerEnd();
    }

    IEnumerator EndGameTimer()
    {
        float remainingTime = 1f;

        while (remainingTime > 0)
        {
            turnManager.photonView.RPC("ShowEndGameMsg", RpcTarget.All); // ����� ȭ�鿡 ���� �޽����� ��쵵�� ��

            remainingTime -= 1f;

            yield return new WaitForSeconds(1.5f); // 1.2�� ���
        }

        GameTimerEnd();
    }

    // ���â �����ֱ� �ð��� ������ ��
    void OnTimerEnd()
    {
        if (BacktoMainRoutine != null)
        {
            StopCoroutine(BacktoMainRoutine); // �ڷ�ƾ ����
        }
        BacktoMainRoutine = null;

        Debug.Log("15�ʰ� ���� �������� ���ư��ϴ�.");

        TurnManager.instance.LeaveRoom(); // ���� ���� ���� ���� ���� ���� ����
    }

    // ���� ���� ��� �޽����� ������ �� - ���â �����ֱ�
    void GameTimerEnd()
    {
        if (EndGameDelayRoutine != null)
        {
            StopCoroutine(EndGameDelayRoutine); // �ڷ�ƾ ����
        }
        EndGameDelayRoutine = null;

        // ����� ȭ�鿡 ��� â�� ��쵵�� ��û��
        turnManager.photonView.RPC("ShowResultPopup", RpcTarget.All); 

        // ��ο��� �ڷ�ƾ�� ���� ���� ��û��
        turnManager.photonView.RPC("StopTurnCoroutine", RpcTarget.All);

        //MainCheckTime(); // ���� �ǵ��ư��� Ÿ�̸� ����
    }

    public void OnConfirmButton() // ���� ��� Ȯ�� ��ư�� ������ �� -> ���� �̵�
    {
        TurnManager.instance.LeaveRoom(); // ���� ���� ���� ���� ���� ���� ����
    }

    public void SetActive()
    {
        // ��� ��������Ʈ�� �⺻������ ��Ȱ��ȭ(�ʿ��� �� Ȱ��ȭ�� ����)
        for (int i = 0; i < ResultUserList.Length; i++)
        {
            ResultUserList[i].SetActive(false);
        }
    }


    // ������ �ǳ��� �������� �� ��� �����͸� �̸� ������Ʈ - ������ ������ �� 1�ʰ� �����̰� �־�� �� ��
    [PunRPC]
    public void UpdateResultData(int actorNum, int completeCount) 
    {
        // �ش� ���ͳѹ��� �ش��ϴ� Player ã��
        Player targetPlayer = userProfileLoad.players.FirstOrDefault(p => p.myActNum == actorNum);

        if (targetPlayer != null)
        {
            targetPlayer.completeCount = completeCount; // �ܾ� �ϼ� Ƚ�� ����
            Debug.Log($"{targetPlayer.displayName}���� �ܾ� �ϼ� Ƚ��: {completeCount}");
        }

        // ��� ������ ������ ���ŵǾ����� Ȯ��
        if (userProfileLoad.players.All(p => p.completeCount >= 0))
        {
            Debug.Log("��� ������ �ܾ� �ϼ� Ƚ���� �޾ҽ��ϴ�!");

            // �÷��̾�� ������ ������������ ����
            userProfileLoad.players.Sort((x, y) => y.completeCount.CompareTo(x.completeCount));

            // ������ ������Ʈ
            UpdatePlayerInfo();
        }
    }

    public void UpdatePlayerInfo()
    {
        List<Player> players = userProfileLoad.players;

        // �� �÷��̾��� ������ UI�� ������Ʈ
        for (int i = 0; i < players.Count; i++)
        {
            // �÷��̾��� ������ ����
            Player currentPlayer = players[i];

            // �÷��̾��� �̹��� �ε����� �̸��� ��������
            int currentPlayerImgIndex = currentPlayer.imgIndex;
            string currentPlayerName = currentPlayer.displayName;
            int currentPlayerWordCount = currentPlayer.completeCount;

            ResultUserList[i].SetActive(true);
            ResultUserImg[i].sprite = userProfileLoad.profileImages[currentPlayerImgIndex];
            ResultUserName[i].text = currentPlayerName;
            ResultWordCount[i].text = $"{currentPlayerWordCount}ȸ";
        }
    }
}
