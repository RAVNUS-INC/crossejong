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
using TMPro;

public class ChatRoomSet : MonoBehaviourPunCallbacks
{
    public UserProfileLoad UserProfileLoad;
    public ChatManager chatManager;
    public ChangeLevel Changelevel;

    // �ν����Ϳ��� PhotonView�� �Ҵ�
    public PhotonView PV;

    // �� �̸�, �����ο�/�ִ��ο�, ���̵�, ���ѽð�, ����Ϸ�޽���
    public TMP_Text txtRoomName, txtPlayerCount, txtDifficulty, txtTimelimit, Savetext;
    // ���̵�, ���ѽð� ��ư �迭
    public Button[] DifButton, TimeButton;
    // ���常 ����� �� �ִ� �� �Ӽ� ���� ��ư, �����ư
    public Button RoomSetBtn, SaveBtn;
    //���常 ����� �� �ִ� �� �Ӽ� �г�
    public GameObject RoomSetPanel;
    // ���ŵ� ���̵�(�ʱ�, �߱�, ���)
    private string selectedDifficulty;
    // ���̵�, ���ѽð� ���� �ε���, ���ŵ� ���ѽð�(15��, 30��, 45��)
    private int selectedDifficultyIndex, selectedTimeLimitIndex, selectedTimeLimit;
    // ���� �� ���̵�, ���ѽð� ����
    private int beforeDifficultyIndex, beforeTimeLimitIndex;

    private int myActorNum, myImgIndex; //�� actornumber, �� ���� �ε���
    private string myDisplayName, myMesseages; //�� �̸�, ���� ���� �޽���
    private Dictionary<int, bool> playerReadyStates = new Dictionary<int, bool>(); // �÷��̾� �غ� ���� ����

    private const string DISPLAYNAME_KEY = "DisplayName"; // ������ DisplayName
    private const string IMAGEINDEX_KEY = "ImageIndex"; // ������ �̹��� �ε���

    public TMP_InputField ChatField; //ä���Է�â
    public Button ReadyBtn; //�غ��ư

    void Awake()
    {
        //�� ���� Playerprefs���� �ҷ�����
        myDisplayName = PlayerPrefs.GetString(DISPLAYNAME_KEY, "Guest"); //�̸�
        myImgIndex = PlayerPrefs.GetInt(IMAGEINDEX_KEY, 0); //�̹��� �ε���
        myActorNum = PhotonNetwork.LocalPlayer.ActorNumber; //���ͳѹ�

        // ���� ������ �ε�� �Ŀ� �޽��� ť �簳
        StartCoroutine(EnableMessageQueue());
    }

    private IEnumerator EnableMessageQueue()
    {

        // �� �ε� �� �����̸� �߰��Ͽ� �޽��� ť �簳
        yield return new WaitForSeconds(0.1f); // �� �ε� ������

        PhotonNetwork.IsMessageQueueRunning = true;

        Debug.Log("�޽��� ť �簳 �Ϸ�");

        // property�� �ִ� �� ���� �ҷ��� ������ ����(���̸��� ����)
        LoadRoomInfo();

        // ���� �ο� ������Ʈ
        PlayersUpdate();

        //���� ���� �˸���
        PV.RPC("EnterState", RpcTarget.All, myDisplayName, true);

        // ���̵�, ���ѽð� text ������Ʈ
        txtDifficulty.text = selectedDifficulty; //ex. �ʱ�
        txtTimelimit.text = selectedTimeLimit + "��"; //ex. 15��
        ChatField.text = ""; //ä���Է�â�� �׻� �������
        ReadyBtn.interactable = true; // ó������ �غ��ư Ȱ��ȭ

        // ������ ���� �߰��� ���忡�� ���� - userProfileLoad �� �Լ� ����
        UserProfileLoad.PV.RPC("RequestAddPlayerInfo", RpcTarget.MasterClient, myDisplayName, myImgIndex, myActorNum);

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
                    ("TimeLimitIndex", selectedTimeLimitIndex),
                    ("DifficultyContents", Changelevel.cardFrontBlack.ToArray()) // ���̵� ī�� ����(��,��,��..), ������ ���� �迭�� ���º���
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

    public void LoadRoomInfo() //���� �� ���� �ҷ�����(customProperties�κ���)
    {
        if ((PhotonNetwork.InRoom) && (PhotonNetwork.IsMessageQueueRunning))
        {
            Room room = PhotonNetwork.CurrentRoom;
            selectedDifficultyIndex = (int)room.CustomProperties["DifficultyIndex"]; //���̵� �ε����� �ҷ�����
            selectedTimeLimitIndex = (int)room.CustomProperties["TimeLimitIndex"]; //���ѽð� �ε����� �ҷ�����
            selectedDifficulty = (string)room.CustomProperties["difficulty"]; //���̵��� �ҷ�����
            selectedTimeLimit = (int)room.CustomProperties["timeLimit"]; //���ѽð��� �ҷ�����

            // ���� �� �� ������ ������ ����
            beforeDifficultyIndex = selectedDifficultyIndex;
            beforeTimeLimitIndex = selectedTimeLimitIndex;

            // �� �̸�
            txtRoomName.text = $"{room.Name}";
        }
    }
    public void RoomSetPanelOpenBtn() // ������ �� �Ӽ� ���� �г� ���� ��ư�� �������� -> ��ư�� ��ġ�� ���� �Ӽ��� �°� �ʱ�ȭ
    {
        // �� �Ӽ� �ٽ� ������Ʈ
        LoadRoomInfo();

        // ó�� �����ߴ� ��ư��(���̵�, ���ѽð�)�� ���� �ٸ���(�ٲ� �������� �����) 
        UpdateButtonColors(DifButton, selectedDifficultyIndex);
        UpdateButtonColors(TimeButton, selectedTimeLimitIndex);

        // ���� ��ư ��Ȱ��ȭ
        SaveBtn.interactable = false;

        // ����޽��� �ʱ�ȭ
        Savetext.text = ""; 
    }
    private void PlayersUpdate()  //�����ο��� �ִ��ο� �ؽ�Ʈ ���� ������Ʈ
    {
        if ((PhotonNetwork.InRoom) && (PhotonNetwork.IsMessageQueueRunning))
        {
            Room room = PhotonNetwork.CurrentRoom;
            // ���� �ο� / �ִ� �ο�
            txtPlayerCount.text = $"{room.PlayerCount}/{room.MaxPlayers}";
        }
    }
    public void DifficultySet(Button[] buttons) //���̵� ��ư ����
    {
        // Difficulty ��ư�� ������ �߰�
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnDifficultyButtonClicked(index, buttons));
        }
    }
    public void TimeLimitSet(Button[] buttons) //���ѽð� ��ư ����
    {
        // TimeLimit ��ư�� ������ �߰�
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnTimeLimitButtonClicked(index, buttons));
        }
    }
    public void OnDifficultyButtonClicked(int index, Button[] difBtn) //���̵� ��ư Ŭ�� �� �� ������Ʈ
    {
        switch (index) // 0: �ʱ�, 1: �߱�, 2: ���
        {
            case 0:
                selectedDifficulty = "�ʱ�";
                // �����͸� �ʱ� ���̵��� ������Ʈ
                Changelevel.ChangeLevelLow();
                break;
            case 1:
                selectedDifficulty = "�߱�";
                // �����͸� �߱� ���̵��� ������Ʈ
                Changelevel.ChangeLevelMiddle();
                break;
            case 2:
                selectedDifficulty = "���";
                // �����͸� ��� ���̵��� ������Ʈ
                Changelevel.ChangeLevelHigh();
                break;
        }
        selectedDifficultyIndex = index;
        UpdateButtonColors(difBtn, index); //���� ������Ʈ
        UnityEngine.Debug.Log("Selected Difficulty: " + selectedDifficulty); //�޽��� ���
    }
    public void OnTimeLimitButtonClicked(int index, Button[] TimBtn) //���ѽð� ��ư Ŭ�� �� �� ������Ʈ
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
    private void UpdateButtonColors(Button[] buttons, int selectedIndex) // ������ ��ư�� ��ĥ
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
        if (selectedDifficultyIndex == beforeDifficultyIndex && selectedTimeLimitIndex == beforeTimeLimitIndex) //������ �������� ��� ���ٸ�
        {
            SaveBtn.interactable = false; //�����ư ��Ȱ��ȭ
        }
        else
        {
            SaveBtn.interactable = true; //�����ư Ȱ��ȭ
        }
    }
    public void UpdateRoomUI(string key, object value) // UI �ؽ�Ʈ ������Ʈ(���̵�,�ð�)
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
    private void SaveRoomProperties(string key, object value) // �� �Ӽ��� ������ ������Ʈ�ϴ� �Լ�
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
    public override void OnPlayerEnteredRoom(Player newPlayer) // ���� �ƴ� ���ο� �÷��̾ ������ ���
    {
        // ������ �ƴ� �÷��̾�� ��ư ��Ȱ��ȭ
        RoomSetBtn.interactable = PhotonNetwork.IsMasterClient;

        //���� ���� �ο� ������Ʈ
        PlayersUpdate();
        UnityEngine.Debug.Log("���ο� �÷��̾� ����");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer) // �÷��̾ ���� ������ ��
    {
        // ������ �ƴ� �÷��̾�� ��ư ��Ȱ��ȭ
        RoomSetBtn.interactable = PhotonNetwork.IsMasterClient;

        //���� ���� �ο� ������Ʈ
        PlayersUpdate();
        UnityEngine.Debug.Log("�ٸ� �÷��̾� �� ����");
    }
 
    public void LeaveRoom() // ���� ������
    {
        if (PhotonNetwork.InRoom)
        {
            //���� ������ ��ο��� �˸���
            PV.RPC("EnterState", RpcTarget.All, myDisplayName, false);

            // ������ ���� ���� ��û�� ���忡�� ����
            UserProfileLoad.PV.RPC("RequestRemoveUserInfo", RpcTarget.MasterClient, myActorNum);

            //�ε��� ui �ִϸ��̼� �����ֱ�
            LoadingSceneController.Instance.LoadScene("Main");

            //������
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom() // ���� ���������� ������ �� ȣ��Ǵ� �ݹ�
    {
        Debug.Log("���� ���������� �����߽��ϴ�.");
    }

    public void SendMyMessage() // �޽��� ���� (ä������ ��ư�� ����)
    {
        if (ChatField.text.Trim() != "")
        {
            //ä��â�� �Է��� ������ �޽��� ������ ����
            myMesseages = ChatField.text;

            //���� ������ �ٸ� �����鿡�� �� ä�� ����
            PV.RPC("SendChat", RpcTarget.Others, false, myMesseages, myDisplayName, myImgIndex);
            Debug.Log($"�� ä�ð� ������ �ٸ� �������� �����߽��ϴ�");

            //�� ä�ÿ� �� �޽��� ������Ʈ
            chatManager.Chat(true, myMesseages, "��", null);

            //������ ������ ��, ä�� ��ǲ�ʵ� ����
            ChatField.text = "";
        }
    }

    public void UserReadyState() //�غ� ��ư�� ���� ����(�غ� ���� �˸��� ����, ������ �̵����� ����)
    {
        // �׽�Ʈ �ÿ��� �ּ�ó��, ���� ���� �� �ּ������!
        //if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        //{
        //    Debug.Log("�濡 1�� ���ϸ� �����ϹǷ� �������� ����.");
        //    return;
        //}

        ReadyBtn.interactable = false; // ��ư �ѹ� �������� �������� ��Ȱ��ȭ(�غ� ��� �Ұ���)

        //���忡�Ը� ���� �غ� ���� ����
        PV.RPC("IsReady", RpcTarget.MasterClient, myDisplayName, myActorNum);
        Debug.Log("���忡�� �غ� �Ϸ� ���¸� �˷Ƚ��ϴ�.");

    }
    
    [PunRPC]
    void SendChat(bool who, string chat, string senderName, int index) //ä���� ��ο��� ������ ui������Ʈ���� �ѹ��� ����ȭ
    {
        //ȭ�鿡 ��ǳ�� ����(��: true, ����: false), index�� �������̹���
        chatManager.Chat(who, chat, senderName, index);
        Debug.Log("�������� ä���� �����߽��ϴ�");
    }

    [PunRPC]
    private void EnterState(string enteruserName, bool isbool) //������ ���� ���� �޽��� �˸���
    {
        // ���� ����/���������� �˸��� �޽��� ����
        chatManager.DisplayUserMessage(enteruserName, isbool);
        Debug.Log(isbool ? $"{enteruserName}�� �����߽��ϴ�" : $"{enteruserName}�� �����߽��ϴ�");
    }

    [PunRPC]
    void IsReady(string userName, int userNum) //��� �������� �غ� ���� �˸���
    {
        playerReadyStates[userNum] = true; // ������ �غ� ���� ����

        //������ ��쿡�� �Ʒ��� �ڵ带 ����
        if (!PhotonNetwork.IsMasterClient) return;

        // �� ����� �ܺο��� ������ �ʵ��� �ϱ� - �̹� ������ ���۵� ���̹Ƿ�
        PhotonNetwork.CurrentRoom.IsVisible = false; // �� ��Ͽ��� ����

        //�÷��̾�� �� �� ���̶� �غ����� �ʾҴٸ� ����
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!playerReadyStates.ContainsKey(player.ActorNumber) || !playerReadyStates[player.ActorNumber])
                return;
        }

        //�÷��̷� ������ �̵�(���� �濡�� �غ��ư�� ���� ��� �÷��̾ ���Ͽ�)
        PV.RPC("ChangeScene", RpcTarget.All, "PlayRoom");

        //PhotonNetwork.LoadLevel("PlayRoom");
    }

    [PunRPC]
    void ChangeScene(string sceneName) //��� ������ �غ��ϸ� �÷��̷����� �̵�
    {
        //�ε��� ui �ִϸ��̼� �����ֱ�
        LoadingSceneController.Instance.LoadScene($"{sceneName}");
    }
}



