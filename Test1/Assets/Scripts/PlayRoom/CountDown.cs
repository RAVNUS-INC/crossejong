using UnityEngine;
using TMPro; // TextMeshPro�� ����ϱ� ���� ���ӽ����̽�
using System.Collections;
using UnityEngine.UI;
using Photon.Pun;

public class Countdown : MonoBehaviourPun

{
    public TMP_Text countDownText; // TextMeshPro ���
    public UserCard userCard;  // UserCard ����
    public float startDelay = 1f; // ���� ������
    public Button startGameButton; //���� ���۹�ư
    public GameObject WaitingPanel; // ��ΰ� �����ϱ� ������ ���̴� �г�(������ǥ��)
    public Image fieldArea; // ������ Ȱ��ȭ�� ����

    public FieldCard fieldCard;
    public TurnManager turnMananger;
    public TurnChange turnChange;

    private void Start()
    {
        WaitingPanel.SetActive(true); // �� ó���� �г� Ȱ��ȭ

        // ���常 ���۹�ư Ȱ��ȭ, ���� ����
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    startGameButton.onClick.AddListener(() =>
        //    photonView.RPC("StartCountDown", RpcTarget.All));
        //}
    }

    [PunRPC]
    private void StartCountDown()
    {
        WaitingPanel.SetActive(false); // ���� ���������Ƿ� �г� ��Ȱ��ȭ

        fieldArea.gameObject.SetActive(true); // ī��Ʈ�ٿ� ���ڰ� ���̰�

        StartCoroutine(CountDownRoutine(1)); // Ÿ�̸� ����
    }

    private IEnumerator CountDownRoutine(int count)
    {
        // ī��Ʈ�ٿ� ǥ��
        while (count > 0)
        {
            countDownText.text = count.ToString(); // TMP_Text�� ����
            yield return new WaitForSeconds(1f);
            count--;
        }

        // "����!" ǥ��
        countDownText.text = "Start!"; // TMP_Text�� ����
        yield return new WaitForSeconds(startDelay);

        countDownText.gameObject.SetActive(false); // ī��Ʈ�ٿ� �ؽ�Ʈ ����
        StartGame();

    }

    private void StartGame()
    {
        //���常 ���� ù��° ī�带 ����
        if (PhotonNetwork.IsMasterClient)
        {
            userCard.FirstUserCardArea();
            // ������ �Ϸ�Ǿ����� �˸� (RPC ȣ��)
            photonView.RPC("FirstUserCompleted", RpcTarget.All);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            fieldCard.FirstFieldCard();
            // ������ �Ϸ�Ǿ����� �˸� (RPC ȣ��)
            photonView.RPC("FirstFieldCompleted", RpcTarget.All);
        }

        // ù���(����)���� Ÿ�̸� ����
        if (PhotonNetwork.IsMasterClient)
        {
            // ���� ��� ���� ī�� ���� ���� ��û
            photonView.RPC("LetsCardCount", RpcTarget.All);

            // ������� ù ī��Ʈ �ٿ� ����
            turnMananger.AfterCountdown();
        }

    }

    [PunRPC]
    private void FirstUserCompleted()
    {
        fieldCard.CreateDropAreas();
    }

    [PunRPC]
    private void FirstFieldCompleted()
    {
        userCard.SelectedUserCard();
    }

    [PunRPC]
    private void LetsCardCount() // �ڽ��� ī�� ���� ������Ʈ
    {
        turnChange.TurnEnd(); 
    }
}
