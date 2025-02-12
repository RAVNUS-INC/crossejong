using UnityEngine;
using TMPro; // TextMeshPro�� ����ϱ� ���� ���ӽ����̽�
using System.Collections;
using UnityEngine.UI;
using Photon.Pun;

public class Countdown : //MonoBehaviour

    //-----------(���� ���� �� �ּ�����)------------
    MonoBehaviourPun

{
    public TMP_Text countDownText; // TextMeshPro ���
    public UserCard userCard;  // UserCard ����
    public float startDelay = 1f; // ���� ������
    public Button startGameButton; //���� ���۹�ư
    public FieldCard fieldCard;

    //private void Start()
    //{
    //    startGameButton.onClick.AddListener(StartCountDown);
    //}

    //���� ���� �� �ּ� ����------------------------------------
    private void Start()
    {
        // ���常 ���۹�ư Ȱ��ȭ, ���� ����
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.onClick.AddListener(() =>
            photonView.RPC("StartCountDown", RpcTarget.All));
        }
    }

    //���� ���� �� �ּ� ����------------------------------------
    [PunRPC]
    private void StartCountDown()
    {
        StartCoroutine(CountDownRoutine(1));
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

    //private void StartGame()
    //{
    //    userCard.FirstUserCardArea();
    //    fieldCard.CreateDropAreas();
    //    fieldCard.FirstFieldCard();
    //    userCard.SelectedUserCard();
    //}


    //���� ���� �� �ּ� ����------------------------------------
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
    }

    [PunRPC]
    private void FirstUserCompleted()
    {
        Debug.Log("FirstUserCompleted �Ϸ�, ���� ����");
        fieldCard.CreateDropAreas();
    }

    [PunRPC]
    private void FirstFieldCompleted()
    {
        Debug.Log("FirstFieldCompleted �Ϸ�, ���� ����");
        userCard.SelectedUserCard();
    }

    //���� ���� �� �ּ� ����------------------------------------
}
