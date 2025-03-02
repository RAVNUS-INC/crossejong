using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResult : MonoBehaviourPunCallbacks
{
    public GameObject ResultPanel; // ���̰�� �ǳ�
    public GameObject[] ResultUserList; // ���̰�� �������� ����Ʈ
    public Image[] ResultUserImg; // ���̰�� �������� �����ʻ���
    public TMP_Text[] ResultUserName, ResultUserTime; // ���̰�� �������� �г���, ī�� ���� �ð�

    void Start()
    {
        
    }

    // sortedplayers����Ʈ�� �ִ� �������� �������� ���â ���� ǥ��(������ ������ �÷����� ������)

    // ī�带 ���� ���� ������ ������ �ð��� ������������ ������ ����Ʈ�κ��� ��� ǥ��

    // 2���� ��� -> �Ѹ��� ī�� �� ���� -> ��� �ٷ� ǥ��
    // 3�� �̻��� ��� -> �Ѹ��� ī�� �� ���� -> ������ �����鳢�� ���� ���� -> ��� ǥ��
    // ������ ī�带 �����ϸ� ���忡�� �˸��� �ָ� ��. ������ ���� �����ϴ� ������ �� ī�� ������ �������� 1���� ������ ��ζ�� ���� ���â�� ��쵵�� ��ο��� ��û.


    // �����θ��� ī�� ��� �������� ī�� ����(0��) �������� �ڷ�ƾ �Լ� �ð� ���� ����

    // ��� Ȯ�� ��ư ������ �������� ���ư�����
    // Ȯ�� ��ư�� ������ �ʾƵ� 15�ʵڿ� �������� �ڵ����� �̵��ϵ��� �˸��޽��� �����ϱ�

    public void OnConfirmButton() // ���� ��� Ȯ�� ��ư�� ������ �� -> ���� �̵�
    {
        if (PhotonNetwork.InRoom)
        {

            Debug.Log($"Ȯ�� ��ư Ŭ��. �������� �̵��մϴ�.");

            //�ε��� ui �ִϸ��̼� �����ֱ�
            LoadingSceneController.Instance.LoadScene("Main");

            // �� ������
            PhotonNetwork.LeaveRoom();

            // ���� �̵� �� �ٽ� ����� �õ� -> makeroom �� ��ȯ ���� �߻�
        }
    }

    public override void OnLeftRoom() // ���� ���������� ������ ��
    {
        // ������ ���� ������ ������ ��µ� �޽���
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("�� �� �ڵ� ���� Ȯ��");
        }
    }
}
