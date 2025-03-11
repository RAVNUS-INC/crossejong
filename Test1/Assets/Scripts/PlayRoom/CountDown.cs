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
    private bool isCountingDown = false;

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

        if (isCountingDown) return; // �̹� ���� ���̸� �ߺ� ���� ����
        isCountingDown = true;

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

        if (PhotonNetwork.IsMasterClient)
        {
            StartGame(); // �����̶�� �� �Լ� ����
        }

        isCountingDown = false; // Ÿ�̸� ���� ��, �ٽ� ȣ�� �����ϵ��� ����
    }

    private void StartGame() // ���常 ����
    {
        userCard.FirstUserCardArea(); // ������ ī�带 ���徿 �̾� �÷��̾�鿡�� ������

        fieldCard.FirstFieldCard(); // ������ ù ī�� �̾� ��ο��� ���� �߰� ��û

        // ���� ��� ���� ī�� ���� ���� ��û
        photonView.RPC("LetsCardCount", RpcTarget.All);

        // ������� ù ī��Ʈ �ٿ� ����
        turnMananger.AfterCountdown();
    }


    [PunRPC]
    private void LetsCardCount() // �ڽ��� ī�� ���� ������Ʈ
    {
        turnChange.TurnEnd(); 
    }
}
