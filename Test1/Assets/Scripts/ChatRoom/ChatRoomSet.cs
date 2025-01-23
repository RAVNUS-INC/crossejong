using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using ExitGames.Client.Photon;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;
using Button = UnityEngine.UI.Button;
using PlayFab.ClientModels;
using PlayFab;
using System.Reflection;
using Photon.Pun.Demo.PunBasics;
//using Unity.VisualScripting.Dependencies.Sqlite;


public class ChatRoomSet : MonoBehaviourPunCallbacks
{
    //[SerializeField] private UserProfileLoad UserProfileLoad; // Inspector���� �巡���Ͽ� ����
    public UserProfileLoad UserProfileLoad;
    
    private string selectedDifficulty; // ���ŵ� ���̵�(�ʱ�, �߱�, ���)(���� ��)
    private int selectedTimeLimit; //  ���ŵ� ���ѽð�(15��, 30��, 45��)(���� ��)

    private int selectedDifficultyIndex; // ���̵� ���� �ε���
    private int selectedTimeLimitIndex; // ���ѽð� �ε���


    // UI �ؽ�Ʈ ����
    public Text txtRoomName; // �� �̸�
    public Text txtPlayerCount; // �����ο�/�ִ��ο�

    public Text txtDifficulty; // ���̵�
    public Text txtTimelimit; // ���ѽð�

    public Button[] DifButton; // ���̵� ��ư �迭
    public Button[] TimeButton; // ���ѽð� ��ư �迭

    public Button RoomSetBtn; // ���常 ����� �� �ִ� �� �Ӽ� ���� ��ư
    public GameObject RoomSetPanel; //���常 ����� �� �ִ� �� �Ӽ� �г�

    public Button SaveBtn; //�����ư
    public Text Savetext; //����Ϸ�޽���

    
    void Awake()
    {
        
    }

    private void Start()
    {
        UserProfileLoad = GameObject.FindObjectOfType<UserProfileLoad>();  // ���� �ִ� PlayerManager�� ã��

        UpdateRoomInfo(); // �� ���ڸ��� �� ���� ������Ʈ

        //�����ư ������ ������ �Լ�
        SaveBtn.onClick.AddListener(() =>
        {
            if (PhotonNetwork.InRoom)
            {
                Room room = PhotonNetwork.CurrentRoom;
                if (room.CustomProperties.ContainsKey("difficulty"))
                {
                    //string difficultyText = GetDifficultyText(selectedDifficulty); //���̵� ���ڿ� ��ȯ
                    UpdateRoomUIBtn("DifficultyIndex", selectedDifficultyIndex); //�ε����� ����
                    UpdateRoomUIBtn("difficulty", selectedDifficulty); //���̵� ����(���� �ݿ��� �ؽ�Ʈ)

                    UnityEngine.Debug.Log("difficulty: " + selectedDifficulty);
                }
                if (room.CustomProperties.ContainsKey("timeLimit"))
                {
                    UpdateRoomUIBtn("TimeLimitIndex", selectedTimeLimitIndex); //�ε����� ����
                    UpdateRoomUIBtn("timeLimit", selectedTimeLimit); //�ð� ����(���� �ݿ��� ��)

                    UnityEngine.Debug.Log("timeLimit: " + selectedTimeLimit);
                }

            }
            // UI ���� (���� �޽��� ���)
            Savetext.text = "����Ǿ����ϴ�.";
        });

        // ���� ���ο� ���� ��ư ó��
        if (PhotonNetwork.IsMasterClient)
        {
            RoomSetBtn.interactable = true;  // �����̸� ��ư Ȱ��ȭ
        }
        else
        {
            RoomSetBtn.interactable = false;  // ������ �ƴϸ� ��ư ��Ȱ��ȭ
        }


    }


    // ������ �� �Ӽ� ���� �г� ���� ��ư�� �������� -> ��ư�� ��ġ ���� �Ӽ��� �°� �ʱ�ȭ
    public void RoomSetPanelOpenBtn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            RoomSetPanel.SetActive(true);  // �����̸� �г� ����

            if (PhotonNetwork.InRoom)
            {
                Room room = PhotonNetwork.CurrentRoom;

                object ReDifficulty = room.CustomProperties["DifficultyIndex"]; //���̵� �ε����� �ҷ�����
                object ReselectedTimeLimit = room.CustomProperties["TimeLimitIndex"]; //���ѽð� �ε����� �ҷ�����
                object Difficulty = room.CustomProperties["difficulty"]; //���̵��� �ҷ�����
                object TimeLimit = room.CustomProperties["timeLimit"]; //���ѽð��� �ҷ�����

                // �������� �κ�Ŵ��� ������ �Ȱ���(�Լ� ����� ����)
                selectedDifficultyIndex = (int)ReDifficulty; //int����ȯ
                selectedTimeLimitIndex = (int)ReselectedTimeLimit; //int����ȯ
                selectedDifficulty = (string)Difficulty; //���ڿ� ��ȯ
                selectedTimeLimit= (int)TimeLimit; //int����ȯ

                // ó�� �����ߴ� ��ư��(���̵�, ���ѽð�)�� ���� �ٸ���(�ٲ� ������� �ٲ� �������� �����) 
                SetDefaultSelection(DifButton, selectedDifficultyIndex);
                SetDefaultSelection(TimeButton, selectedTimeLimitIndex);
            }
        }
        else
        {
            Debug.LogWarning("���常 ���� �г��� �� �� �ֽ��ϴ�.");
        }
    }


    // �÷��̾ �濡 ���ö�, ������ ������(�ǽð� �ο��� ������Ʈ)
    public void UpdateRoomInfo()

    {   
        if (PhotonNetwork.InRoom)
        {
            Room room = PhotonNetwork.CurrentRoom;

            // -------------������Ƽ�κ��� �ʱ��ư ���� ǥ��-------------
            object ReDifficulty = room.CustomProperties["DifficultyIndex"];
            object ReselectedTimeLimit = room.CustomProperties["TimeLimitIndex"];

            // �������� �κ�Ŵ��� ������ �Ȱ���(�Լ� ����� ����)
            selectedDifficultyIndex = (int)ReDifficulty; //int����ȯ
            selectedTimeLimitIndex = (int)ReselectedTimeLimit; //int����ȯ

            // ó�� �����ߴ� ��ư��(���̵�, ���ѽð�)�� ���� �ٸ���(�ʱ⼳�� 1����) 
            SetDefaultSelection(DifButton, selectedDifficultyIndex);
            SetDefaultSelection(TimeButton, selectedTimeLimitIndex);


            // -------------��ư Ŭ�� �� ���� ����(�̰Ŵ� ������Ƽ ����X, ���� ������ ��������� �ʴ´�)-------------
            // �� ��ư �迭�� ������ �߰�(Ŭ���� ���� ����) -> OK
            DifficultySet(DifButton);
            TimeLimitSet(TimeButton);


            // -------------������Ƽ�κ��� ������ ���� Text�� ���� ǥ��/������Ʈ-------------
            // �� �̸�
            txtRoomName.text = $"{room.Name}";
            
            // ���� �ο� / �ִ� �ο�
            txtPlayerCount.text = $"{room.PlayerCount}/{room.MaxPlayers}";

            // ���̵�(Custom Properties���� ��������)
            string difficulty = room.CustomProperties.ContainsKey("difficulty")
                                ? room.CustomProperties["difficulty"].ToString(): "����";

            // ���� �ð�(Custom Properties���� ��������)
            string timeLimit = room.CustomProperties.ContainsKey("timeLimit")
                                ? room.CustomProperties["timeLimit"].ToString(): "����";

            // ���̵�, ���ѽð� text ������Ʈ
            txtDifficulty.text = $"{difficulty}";
            txtTimelimit.text = $"{timeLimit}��";
        }
        else
        {
            Debug.LogWarning("���� �濡 ���ӵǾ� ���� ����!");
        }
    }

    // --------------------------- �� �Ӽ� ��ư Ŭ�� �� ------------------------
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
                selectedDifficulty = "�ʱ�";
                break;
            case 1:
                selectedDifficulty = "�߱�";
                break;
            case 2:
                selectedDifficulty = "���";
                break;
        }
        selectedDifficultyIndex = index;
        //string difficultyText = GetDifficultyText(selectedDifficulty); // selectedDifficulty ���� ������� ���� ���ڿ��� ��ȯ
        UpdateButtonColors(difBtn, index); //���� ������Ʈ
        UnityEngine.Debug.Log("Selected Difficulty: " + selectedDifficulty); //�޽��� ���
    }

    // selectedDifficulty�� ���� 2, 3, 4�� �� ���� "�ʱ�", "�߱�", "���"�̶�� ���ڿ��� ���
    //public string GetDifficultyText(int difficulty)
    //{
    //    switch (difficulty)
    //    {
    //        case 2:
    //            return "�ʱ�";
    //        case 3:
    //            return "�߱�";
    //        case 4:
    //            return "���";
    //        default:
    //            return "�� �� ����"; // �ٸ� ���� ��� �⺻ �� ��ȯ
    //    }
    //}

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


    // --------------------------- �� �Ӽ� ������Ʈ �� ------------------------
    public void UpdateRoomUI(string key, object value) // UI ������Ʈ �Լ�(���̵�,�ð�)
    {
        if (key == "difficulty")
        {
            txtDifficulty.text = (string)value;
        }
        if (key == "timeLimit")
        {
            txtTimelimit.text = $"{value}��";
        }
        if (key == "DifficultyIndex")
        {
            Debug.Log($"�Ӽ� ������Ʈ �ݿ���: {key} = {value}");
        }
        if (key == "TimeLimitIndex")
        {
            Debug.Log($"�Ӽ� ������Ʈ �ݿ���: {key} = {value}");
        }
    }


    // �� �Ӽ��� ������ ������Ʈ�ϴ� �Լ� (UI ������ ���� ����)
    private void SaveRoomProperties(string key, object value)
    {
        // ������ �Ӽ� ����
        Hashtable propertiesToUpdate = new Hashtable
        {
            { key, value }
        };

        // Photon�� ���� �� �Ӽ� ������Ʈ
        PhotonNetwork.CurrentRoom.SetCustomProperties(propertiesToUpdate);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        foreach (DictionaryEntry entry in propertiesThatChanged)
        {
            string key = entry.Key.ToString();
            object value = entry.Value;

            Debug.Log($"�Ӽ� ������Ʈ �ݿ���: {key} = {value}");

            // ����� UI ����
            UpdateRoomUI(key, value);
        }
    }

    // ���� ��ư Ŭ�� �� ȣ��Ǵ� �Լ� (UI �޽��� �� �ؽ�Ʈ ����)
    public void UpdateRoomUIBtn(string key, object value)
    {
        // �� �Ӽ� ������ ������Ʈ
        SaveRoomProperties(key, value);
    }


    //�����ο��� �ִ��ο� �ؽ�Ʈ ���� ������Ʈ
    private void PlayersUpdate()
    {
        if (PhotonNetwork.InRoom)
        {
            Room room = PhotonNetwork.CurrentRoom;
            // ���� �ο� / �ִ� �ο�
            txtPlayerCount.text = $"{room.PlayerCount}/{room.MaxPlayers}";
        }
    }



    // ���� �ƴ� ���ο� �÷��̾ ������ ���
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // �������� �ƴ��� ������ �� �ʿ䰡 ����(�ε���0�� �׻� ���� �ƴϰٴ°�?)
        PlayersUpdate(); //���� �� ���� �ο� ������Ʈ

        // ������ �ƴ� �÷��̾�� UI �г� ��Ȱ��ȭ
        if (!PhotonNetwork.IsMasterClient)
        {
            RoomSetBtn.interactable = false;
            RoomSetPanel.SetActive(false);
        }
        UnityEngine.Debug.Log("���� �ƴ� ���ο� �÷��̾� ����");
        UserProfileLoad.players.Clear();  // ���� ����Ʈ�� ��� �׸� ����
        UserProfileLoad.SendPlayerInfoToOthers();
        UserProfileLoad.UpdateMyInfo();
    }

    // �÷��̾ ���� ������ �� ȣ��Ǵ� �ݹ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);  // �θ� Ŭ���� �޼��� ȣ��(���� ����)


        // �������� �ƴ��� ������ �ʿ䰡 �ִ�

        PlayersUpdate(); //���� �� ���� �ο� ������Ʈ

        // ������ �� �÷��̾�� UI �г� ��Ȱ��ȭ
        if (PhotonNetwork.IsMasterClient)
        {
            RoomSetBtn.interactable = true;
            RoomSetPanel.SetActive(false);
        }

        UnityEngine.Debug.Log("�ٸ� �÷��̾� �� ����");
        UserProfileLoad.players.Clear();  // ���� ����Ʈ�� ��� �׸� ����
        UserProfileLoad.SendPlayerInfoToOthers();
        UserProfileLoad.UpdateMyInfo();
       
    }


    // ���� ������
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
