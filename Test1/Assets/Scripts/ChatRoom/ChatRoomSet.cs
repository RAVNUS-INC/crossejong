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

public class ChatRoomSet : MonoBehaviourPunCallbacks
{
    public UserProfileLoad UserProfileLoad;
    public ChatEditor ChatEditor;
    public ChatManager chatManager; 

    // �� �̸�, �����ο�/�ִ��ο�, ���̵�, ���ѽð�, ����Ϸ�޽���
    public Text txtRoomName, txtPlayerCount, txtDifficulty, txtTimelimit, Savetext; 
    // ���̵�, ���ѽð� ��ư �迭
    public Button[] DifButton, TimeButton;
    // ���常 ����� �� �ִ� �� �Ӽ� ���� ��ư, �����ư
    public Button RoomSetBtn, SaveBtn;
    //���常 ����� �� �ִ� �� �Ӽ� �г�
    public GameObject RoomSetPanel;
    // ���ŵ� ���̵�(�ʱ�, �߱�, ���)(���� ��)
    private string selectedDifficulty; 
    // ���̵�, ���ѽð� ���� �ε���, ���ŵ� ���ѽð�(15��, 30��, 45��)(���� ��)
    private int selectedDifficultyIndex, selectedTimeLimitIndex, selectedTimeLimit;

    private int myActorNum, myImgIndex, myMaster; //�� actornumber, �� ���� �ε���, �������� �ƴ���
    private string myDisplayName, myMesseages; //�� �̸�, ���� ���� �޽���
    private Dictionary<int, bool> playerReadyStates = new Dictionary<int, bool>(); // �÷��̾� �غ� ���� ����

    public InputField ChatField; //ä���Է�â
    public Button ReadyBtn; //�غ��ư



    void Awake()
    {
        // property�� �ִ� �� ���� �ҷ��� ������ ����(���̸��� ����)
        LoadRoomInfo();

        // ���� �ο� ������Ʈ
        PlayersUpdate();

        // ���̵�, ���ѽð� text ������Ʈ
        txtDifficulty.text = selectedDifficulty; //ex. �ʱ�
        txtTimelimit.text = selectedTimeLimit + "��"; //ex. 15��

        ChatField.text = ""; //ä���Է�â�� �׻� �������

        ReadyBtn.interactable = true; // ó������ �غ��ư Ȱ��ȭ

        //�� ���� �ҷ�����(�� �̸�, �� ������ȣ, ������ �ε���)
        var customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        myDisplayName = customProperties.ContainsKey("Displayname") ? (string)customProperties["Displayname"] : "Unknown";
        myActorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        myImgIndex = int.Parse(customProperties.ContainsKey("Imageindex") ? (string)customProperties["Imageindex"] : "Unknown");

        //���� ���� �˸���("~���� �����Ͽ����ϴ�.")
        photonView.RPC("EnterState", RpcTarget.All, myDisplayName, true);
        Debug.Log("���� ������ �˷Ƚ��ϴ�");
    }

    private void Start()
    {
        // ���� ���ο� ���� ��ư ó��
        RoomSetBtn.interactable = PhotonNetwork.IsMasterClient;

        // �� ��ư �迭�� ������ �߰�(Ŭ���� ���� ����)
        DifficultySet(DifButton);
        TimeLimitSet(TimeButton);

        //�� ���� ���� ��, �����ư ������ ������ �Լ�
        SaveBtn.onClick.AddListener(() =>
        {
            if (PhotonNetwork.InRoom)
            {
                Room room = PhotonNetwork.CurrentRoom;
                // ������ ǥ�õ� �������� key���� ���� value�� ������Ʈ
                var properties = new (string key, object value)[]
                {
                    ("difficulty", selectedDifficulty),
                    ("timeLimit", selectedTimeLimit),
                    ("DifficultyIndex", selectedDifficultyIndex),
                    ("TimeLimitIndex", selectedTimeLimitIndex)
                };
                foreach (var prop in properties)
                {
                    if (room.CustomProperties.ContainsKey(prop.key))
                    {
                        //������ �Ӽ����� �� ������Ʈ ����
                        SaveRoomProperties(prop.key, prop.value);
                    }
                }
            }
            // �ٲ� ������ ���� ���� ����
            UpdateButtonColors(DifButton, selectedDifficultyIndex);
            UpdateButtonColors(TimeButton, selectedTimeLimitIndex);

            // UI ���� (���� �޽��� ���)
            Savetext.text = "����Ǿ����ϴ�.";
        });
    }

    public void LoadRoomInfo()
    {
        //���� �� ���� �ҷ�����(customProperties�κ���)
        if (PhotonNetwork.InRoom)
        {
            Room room = PhotonNetwork.CurrentRoom;
            selectedDifficultyIndex = (int)room.CustomProperties["DifficultyIndex"]; //���̵� �ε����� �ҷ�����
            selectedTimeLimitIndex = (int)room.CustomProperties["TimeLimitIndex"]; //���ѽð� �ε����� �ҷ�����
            selectedDifficulty = (string)room.CustomProperties["difficulty"]; //���̵��� �ҷ�����
            selectedTimeLimit = (int)room.CustomProperties["timeLimit"]; //���ѽð��� �ҷ�����

            // �� �̸�
            txtRoomName.text = $"{room.Name}";
        }
    }

    // ������ �� �Ӽ� ���� �г� ���� ��ư�� �������� -> ��ư�� ��ġ�� ���� �Ӽ��� �°� �ʱ�ȭ
    public void RoomSetPanelOpenBtn()
    {
        LoadRoomInfo();
        UnityEngine.Debug.Log("���̵�: " + selectedDifficulty);
        UnityEngine.Debug.Log("�ð�: " + selectedTimeLimit); 

        // ó�� �����ߴ� ��ư��(���̵�, ���ѽð�)�� ���� �ٸ���(�ٲ� �������� �����) 
        UpdateButtonColors(DifButton, selectedDifficultyIndex);
        UpdateButtonColors(TimeButton, selectedTimeLimitIndex);
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
        UpdateButtonColors(difBtn, index); //���� ������Ʈ
        UnityEngine.Debug.Log("Selected Difficulty: " + selectedDifficulty); //�޽��� ���
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
        UpdateButtonColors(TimBtn, index); //���� ������Ʈ
        UnityEngine.Debug.Log("Selected Time Limit: " + selectedTimeLimit);
    }


    // ������ ��ư�� ������ ��ĥ�ϴ� �Լ�
    private void UpdateButtonColors(Button[] buttons, int selectedIndex)
    {
        // �ʱ�ȭ �۾� �� ��ư ���� ǥ���ϱ� ���� ���̴� �ݺ���
        for (int i = 0; i < buttons.Length; i++)
        {
            ColorBlock colorBlockbg = buttons[i].colors; //colorBlock�� ���� ���� �Ѱ��ֱ�
            colorBlockbg.normalColor = Color.white; // �⺻ ���� ȭ��Ʈ
            colorBlockbg.normalColor = (i == selectedIndex) ? Color.yellow : Color.white; //���� �ε����� ������ �����
            colorBlockbg.selectedColor = Color.yellow;
            buttons[i].colors = colorBlockbg; //��ư�� ���� ������Ʈ
        }
    }

    // --------------------------- �� �Ӽ� ������Ʈ �� ------------------------
    public void UpdateRoomUI(string key, object value) // UI ������Ʈ �Լ�(���̵�,�ð�)
    {
        switch (key)
        {
            case "difficulty": //���̵� ������Ʈ
                txtDifficulty.text = (string)value;
                break;

            case "timeLimit": //���ѽð� ������Ʈ
                txtTimelimit.text = $"{value}��";
                break;

            case "DifficultyIndex": //�ε��� ������Ʈ
            case "TimeLimitIndex":
                Debug.Log($"�ε��� �Ӽ� �ݿ���: {key} = {value}");
                break;
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


    // ���� �ƴ� ���ο� �÷��̾ ������ ���
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // ������ �ƴ� �÷��̾�� ��ư ��Ȱ��ȭ
        RoomSetBtn.interactable = PhotonNetwork.IsMasterClient;

        //���� ���� �ο� ������Ʈ
        PlayersUpdate();
        UnityEngine.Debug.Log("���ο� �÷��̾� ����");
    }

    // �÷��̾ ���� ������ �� ȣ��Ǵ� �ݹ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // ������ �ƴ� �÷��̾�� ��ư ��Ȱ��ȭ
        RoomSetBtn.interactable = PhotonNetwork.IsMasterClient;

        //���� ���� �ο� ������Ʈ
        PlayersUpdate();
        UnityEngine.Debug.Log("�ٸ� �÷��̾� �� ����");
    }


    // ���� ������
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            //���� ������ ��ο��� �˸���
            photonView.RPC("EnterState", RpcTarget.All, myDisplayName, false);

            //�� �ڽ��� players����Ʈ���� ����(�� �����ڿ��Ե� ���� ����)
            UserProfileLoad.photonView.RPC("RemoveUserInfo", RpcTarget.AllBuffered, myActorNum);

            //������
            PhotonNetwork.LeaveRoom();

            //�ε��� ui �ִϸ��̼� �����ֱ�
            LoadingSceneController.Instance.LoadScene("Main");
        }
    }

    // ���� ���������� ������ �� ȣ��Ǵ� �ݹ�
    public override void OnLeftRoom()
    {
        Debug.Log("���� ���������� �����߽��ϴ�.");

        // �κ� �� �̸����� �̵�
        //SceneManager.LoadScene("Main");
    }

    // ä�� ���۹�ư�� ���� ������ ���(�޽��� ���� ����)
    public void SendMyMessage()
    {
        if (ChatField.text.Trim() != "")
        {
            //ä��â�� �Է��� ������ �޽��� ������ ����
            myMesseages = ChatField.text;

            //���� ������ �ٸ� �����鿡�� �� ä�� ����
            photonView.RPC("SendChat", RpcTarget.Others, false, myMesseages, myDisplayName, myImgIndex);
            Debug.Log($"�� ä�ð� ������ �ٸ� �������� �����߽��ϴ�");

            //�� ä�ÿ� �� �޽��� ������Ʈ
            chatManager.Chat(true, myMesseages, "��", null);

            //������ ������ ��, ä�� ��ǲ�ʵ� ����
            ChatField.text = "";
        }
    }

    //�غ� ��ư�� ���� ����(�غ� ���� �˸��� ����, ������ �̵����� ����)
    public void UserReadyState()
    {
        ReadyBtn.interactable = false; // ��ư �ѹ� �������� �������� ��Ȱ��ȭ(�غ� ��� �Ұ���)

        //���忡�Ը� ���� �غ� ���� ����
        photonView.RPC("IsReady", RpcTarget.MasterClient, myDisplayName, myActorNum);
        Debug.Log("���忡�� �غ� �Ϸ� ���¸� �˷Ƚ��ϴ�.");

        //������ ��쿡�� �Ʒ��� �ڵ带 ����
        if (!PhotonNetwork.IsMasterClient) return;

        // �÷��̾�� �� �� ���̶� �غ����� �ʾҴٸ� ����
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!playerReadyStates.ContainsKey(player.ActorNumber) || !playerReadyStates[player.ActorNumber])
                return; 
        }
        //��� �غ� ���� ���
        Debug.Log("��� �÷��̾� �غ�. �÷��̹����� �̵��մϴ�.");
        // �÷��̷� ������ �̵�(���� �濡�� �غ��ư�� ���� ��� �÷��̾ ���Ͽ�)
        photonView.RPC("ChangeScene", RpcTarget.All, "PlayRoom");
    }


    [PunRPC]
    //ä���� ��ο��� ������ ui������Ʈ���� �ѹ��� ����ȭ
    void SendChat(bool who, string chat, string senderName, int index)
    {
        //ȭ�鿡 ��ǳ�� ����(��: true, ����: false), index�� �������̹���
        chatManager.Chat(who, chat, senderName, index);
        Debug.Log("�������� ä���� �����߽��ϴ�");
    }

    [PunRPC]
    //������ ���� ���� �޽��� �˸���
    void EnterState(string enteruserName, bool isbool)
    {
        // ���� ����/���������� �˸��� �޽��� ����
        chatManager.DisplayUserMessage(enteruserName, isbool);
        Debug.Log(isbool ? $"{enteruserName}�� �����߽��ϴ�" : $"{enteruserName}�� �����߽��ϴ�");
    }

    [PunRPC]
    //��� �������� �غ� ���� �˸���
    void IsReady(string userName, int userNum)
    {
        playerReadyStates[userNum] = true; // ������ �غ� ���� ����
    }

    [PunRPC]
    //��� ������ �غ��ϸ� �÷��̷����� �̵�
    void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
