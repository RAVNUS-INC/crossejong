using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using ExitGames.Client.Photon;
using System;


public class ChatRoomSet : MonoBehaviourPunCallbacks
{
    private int selectedDifficulty; // ���ŵ� ���̵�(�ʱ�, �߱�, ���)(���� ��)
    private int selectedTimeLimit; //  ���ŵ� ���ѽð�(15��, 30��, 45��)(���� ��)

    // UI �ؽ�Ʈ ����
    public Text txtRoomName; // �� �̸�
    public Text txtPlayerCount; // �����ο�/�ִ��ο�
    public Text txtDandT; // ���̵��� ���ѽð�

    public Button[] DifButton; // ���̵� ��ư �迭
    public Button[] TimeButton; // ���ѽð� ��ư �迭

    GameObject LobbyManager;

    void Start()
    {

        UpdateRoomInfo(); // �� ���ڸ��� �� ���� ������Ʈ

    }


    // �÷��̾ �濡 ���ö�, ������ ������(�ǽð� �ο��� ������Ʈ)
    // 
    public void UpdateRoomInfo()

    { 
        if (PhotonNetwork.InRoom)
        {
            Room room = PhotonNetwork.CurrentRoom;

            // LobbyManager ������Ʈ�� ã��, �� ������Ʈ���� LobbyManager ��ũ��Ʈ�� ��������
            LobbyManager lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>(); //���� ��ȯ�Ǿ ��ü������ ����

            object ReDifficulty = room.CustomProperties["DifficultyIndex"];
            object ReselectedTimeLimit = room.CustomProperties["TimeLimitIndex"];

            // �������� �κ�Ŵ��� ������ �Ȱ���(�Լ� ����� ����)
            selectedDifficulty = (int)ReDifficulty; //int����ȯ
            selectedTimeLimit = (int)ReselectedTimeLimit; //int����ȯ

            // ó�� �����ߴ� ��ư��(���̵�, ���ѽð�)�� ���� �ٸ���(�ʱ⼳�� 1����) 
            lobbyManager.SetDefaultSelection(DifButton, selectedDifficulty);
            lobbyManager.SetDefaultSelection(TimeButton, selectedTimeLimit); 

            // �� ��ư �迭�� ������ �߰�(Ŭ���� ���� ����) -> OK
            lobbyManager.DifficultySet(DifButton);
            lobbyManager.TimeLimitSet(TimeButton);



            // �� �̸�
            txtRoomName.text = $"{room.Name}";
            
            // ���� �ο� / �ִ� �ο�
            txtPlayerCount.text = $"{room.PlayerCount}/{room.MaxPlayers}";

            // ���̵�(Custom Properties���� ��������)
            string difficulty = room.CustomProperties.ContainsKey("difficulty")
                                ? room.CustomProperties["difficulty"].ToString()
                                : "����";
            // ���� �ð�(Custom Properties���� ��������)
            string timeLimit = room.CustomProperties.ContainsKey("timeLimit")
                                ? room.CustomProperties["timeLimit"].ToString()
                                : "����";
            txtDandT.text = $"{difficulty} / {timeLimit}��";
        }
        else
        {
            Debug.LogWarning("���� �濡 ���ӵǾ� ���� ����!");
        }
    }


    // �÷��̾ �濡 ������ �� ȣ��
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateRoomInfo(); // �� ���� ������Ʈ
    }

    // �÷��̾ ���� ������ �� ȣ��
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateRoomInfo(); // �� ���� ������Ʈ
    }


    // �� ������ ��ư�� ȣ���ϴ� �޼���
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    // ���� ���������� ������ �� ȣ��Ǵ� �ݹ�
    public override void OnLeftRoom()
    {
        // �κ� �� �̸����� �̵�
        SceneManager.LoadScene("Main");
    }

    // ��Ʈ��ũ ������ �߻��ϰų� ���� ������ ������ ��
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError($"Disconnected from server: {cause}");
        // �ʿ��� ��� ������ ���� �߰�
    }
}
