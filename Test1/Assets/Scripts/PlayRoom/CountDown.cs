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

    private void Start()
    {
        //startGameButton.onClick.AddListener(StartCountDown);

        //���� ���� �� �ּ� ����------------------------------------
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

    //���� ���� �� �ּ� ����------------------------------------
    private void StartGame()
    {
        //���� ���� �� �ּ� ����------------------------------------
        //���常 ���� ù��° ī�带 ����
        if (PhotonNetwork.IsMasterClient)
        {
            userCard.FirstUserCardArea();
            
        }
        // userCard.FirstUserCardArea();
        fieldCard.CreateDropAreas();

        if (PhotonNetwork.IsMasterClient)
        {
            fieldCard.FirstFieldCard();
        }
        // fieldCard.FirstFieldCard();
        userCard.SelectedUserCard();

    }
}
