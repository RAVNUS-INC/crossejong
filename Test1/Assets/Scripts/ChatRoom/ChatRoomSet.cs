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
    private int selectedDifficultyIndex; // ���̵� ���� �ε���
    private int selectedTimeLimitIndex; // ���ѽð� �ε���

    // UI �ؽ�Ʈ ����
    public Text txtRoomName; // �� �̸�
    public Text txtPlayerCount; // �����ο�/�ִ��ο�
    public Text txtDandT; // ���̵��� ���ѽð�

    public Button[] DifButton; // ���̵� ��ư �迭
    public Button[] TimeButton; // ���ѽð� ��ư �迭

    //GameObject LobbyManager;
    

    void Awake()
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
            //LobbyManager lobbyManager = GameObject.Find("LobbyManager_Clone").GetComponent<LobbyManager>(); //���� ��ȯ�Ǿ ��ü������ ����

            object ReDifficulty = room.CustomProperties["DifficultyIndex"];
            object ReselectedTimeLimit = room.CustomProperties["TimeLimitIndex"];

            // �������� �κ�Ŵ��� ������ �Ȱ���(�Լ� ����� ����)
            selectedDifficulty = (int)ReDifficulty; //int����ȯ
            selectedTimeLimit = (int)ReselectedTimeLimit; //int����ȯ

            // ó�� �����ߴ� ��ư��(���̵�, ���ѽð�)�� ���� �ٸ���(�ʱ⼳�� 1����) 
            SetDefaultSelection(DifButton, selectedDifficulty);
            SetDefaultSelection(TimeButton, selectedTimeLimit); 

            // �� ��ư �迭�� ������ �߰�(Ŭ���� ���� ����) -> OK
            DifficultySet(DifButton);
            TimeLimitSet(TimeButton);


            // OK (1/13)
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

    public void DifficultySet(Button[] buttons)
    {
        // Difficulty ��ư�� ������ �߰�
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnDifficultyButtonClicked(index, buttons));
        }
    }

    public void TimeLimitSet(Button[] buttons)
    {
        // TimeLimit ��ư�� ������ �߰�
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnTimeLimitButtonClicked(index, buttons));
        }
    }

    public void OnDifficultyButtonClicked(int index, Button[] difBtn)
    {
        switch (index) // 0: �ʱ�, 1: �߱�, 2: ���
        {
            case 0:
                selectedDifficulty = 2;
                break;
            case 1:
                selectedDifficulty = 3;
                break;
            case 2:
                selectedDifficulty = 4;
                break;
        }
        selectedDifficultyIndex = index;
        string difficultyText = GetDifficultyText(selectedDifficulty); // selectedDifficulty ���� ������� ���� ���ڿ��� ��ȯ
        UpdateButtonColors(difBtn, index); //���� ������Ʈ
        UnityEngine.Debug.Log("Selected Difficulty: " + difficultyText); //�޽��� ���
    }

    // selectedDifficulty�� ���� 2, 3, 4�� �� ���� "�ʱ�", "�߱�", "���"�̶�� ���ڿ��� ���
    public string GetDifficultyText(int difficulty)
    {
        switch (difficulty)
        {
            case 2:
                return "�ʱ�";
            case 3:
                return "�߱�";
            case 4:
                return "���";
            default:
                return "�� �� ����"; // �ٸ� ���� ��� �⺻ �� ��ȯ
        }
    }

    public void OnTimeLimitButtonClicked(int index, Button[] TimBtn)
    {
        switch (index) // 0: 15��, 1: 30��, 2: 45��
        {
            case 0:
                selectedTimeLimit = 15;
                break;
            case 1:
                selectedTimeLimit = 30;
                break;
            case 2:
                selectedTimeLimit = 45;
                break;

        }
        selectedTimeLimitIndex = index;
        UpdateButtonColors(TimBtn, index);
        UnityEngine.Debug.Log("Selected Time Limit: " + selectedTimeLimit);

    }
    private void SetDefaultSelection(Button[] buttons, int defaultIndex)
    {

        for (int i = 0; i < buttons.Length; i++) //���� �ݺ�(�� ��ư �迭 ����)
        {
            int index = i;

            ColorBlock colorBlock = buttons[i].colors;
            colorBlock.normalColor = Color.white; // �⺻ ���� ȭ��Ʈ
            colorBlock.selectedColor = Color.yellow; //���õ� ���� �����
            buttons[i].colors = colorBlock;

            //buttons[i].onClick.AddListener(() =>
            //{
            //    UpdateButtonColors(buttons, index); //��ư ���� ����
            //});
        }
        UpdateButtonColors(buttons, defaultIndex);  //�⺻�� ��ư ������ ���������
    }

    // ��ư�� ������ ��ĥ�ϴ� �Լ�
    private void UpdateButtonColors(Button[] buttons, int selectedIndex)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            ColorBlock colorBlock = buttons[i].colors; //colorBlock�� ���� ���� �Ѱ��ֱ�

            if (i == selectedIndex) //���� ������ �ε����� i���� ������
            {
                colorBlock.normalColor = Color.yellow; //�����
            }
            else //���� ������ �ε����� i���� �ٸ���
            {
                colorBlock.normalColor = Color.white; //�Ͼ��
            }
            buttons[i].colors = colorBlock; //��ư�� ���� ������Ʈ
        }
    }

    private void PlayersUpdate() //���� �� ������Ʈ
    {
        if (PhotonNetwork.InRoom)
        {
            Room room = PhotonNetwork.CurrentRoom;
            // ���� �ο� / �ִ� �ο�
            txtPlayerCount.text = $"{room.PlayerCount}/{room.MaxPlayers}";
        }
    }

    // �÷��̾ �濡 ������ �� ȣ��
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayersUpdate(); //���� �� ������Ʈ
    }

    // �÷��̾ ���� ������ �� ȣ��
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayersUpdate(); //���� �� ������Ʈ
    }


    // ���� ������
    // LobbyManager��ũ��Ʈ�� ��� ���� �� �ʱ�ȭ
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

        // LobbyManager ������Ʈ�� ã��, �� ������Ʈ���� LobbyManager ��ũ��Ʈ�� ��������
        //LobbyManager lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>(); //���� ��ȯ�Ǿ ��ü������ ����
        // �������� �ٽ� ���ư��⿡, ���� ���� �� ���� �����ϴ� �Լ��� �����ϰ� ����
    }

    // ��Ʈ��ũ ������ �߻��ϰų� ���� ������ ������ ��
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError($"Disconnected from server: {cause}");
        // �ʿ��� ��� ������ ���� �߰�
    }
}
