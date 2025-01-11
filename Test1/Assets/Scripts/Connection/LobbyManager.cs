using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.PointerEventData;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

//�� ���� �� �� ������ ���� �ڵ�
public class LobbyManager : MonoBehaviourPunCallbacks
{
    // �� ���� ���� UI
    [SerializeField] InputField input_RoomName; //�� �̸�
    [SerializeField] Button[] btn_MaxPlayers; // �ִ��ο� ��ư
    [SerializeField] Button[] btn_Difficulty; // ���̵� ��ư
    [SerializeField] Button[] btn_TimeLimit; // ���ѽð� ��ư

    // �� ���� �� �̸� ��Ģ ���޽���
    [SerializeField] Text warningText;

    // �� ���� ��ư�� �� ���� ��ư, �� ����� ǥ���� ��ũ�Ѻ�
    [SerializeField] Button btn_CreateRoom; // �� ����� ��ư
    [SerializeField] Button btn_JoinRoom; // �� ���� ��ư
    [SerializeField] GameObject roomListItem; // �� ��� ������

    // �� ���� �� �ʿ��� ���� ����
    int selectedMaxPlayers = 0; // �ִ��ο�(2, 3, 4��)
    int selectedDifficulty = 0; // ���̵�(�ʱ�, �߱�, ���)
    int selectedTimeLimit = 0; // ī�� ������� ���ѽð�(15��, 30��, 45��)
    public Transform rtContent;
    private const int MaxLength = 12; // ���̸� �ִ� �Է� ����

    // �� ����� ������ �ִ� Dictionaly ����
    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();

    void Start()
    {
        SetDefaultSelection(btn_MaxPlayers, 0, out selectedMaxPlayers);
        SetDefaultSelection(btn_Difficulty, 0, out selectedDifficulty);
        SetDefaultSelection(btn_TimeLimit, 0, out selectedTimeLimit);

        // �� �̸� �Է� �ʵ� �ʱ�ȭ
        input_RoomName.text = ""; //�� �̸� �⺻ ���� ����
        btn_CreateRoom.interactable = false; // ó������ �� ���� ��ư ��Ȱ��ȭ
        warningText.text = ""; // �ʱ� ��� �޽��� ����

        input_RoomName.onValueChanged.AddListener(ValidateRoomName); //�� �̸� �ۼ��� ��, �� �̸� ��Ģ �˻�
        btn_CreateRoom.onClick.AddListener(OnClickCreateRoom); //�� ���� ��ư Ŭ�� ��, �� ���� ����
        btn_JoinRoom.onClick.AddListener(OnClickJoinRoom); //�� ���� ��ư Ŭ�� ��, �� ���� ����

        // MaxPlayers ��ư�� ������ �߰�
        for (int i = 0; i < btn_MaxPlayers.Length; i++)
        {
            int index = i; // Ŭ������ ���� ���� ���� ���
            btn_MaxPlayers[i].onClick.AddListener(() => OnMaxPlayersButtonClicked(index));
        }

        // Difficulty ��ư�� ������ �߰�
        for (int i = 0; i < btn_Difficulty.Length; i++)
        {
            int index = i;
            btn_Difficulty[i].onClick.AddListener(() => OnDifficultyButtonClicked(index));
        }

        // TimeLimit ��ư�� ������ �߰�
        for (int i = 0; i < btn_TimeLimit.Length; i++)
        {
            int index = i;
            btn_TimeLimit[i].onClick.AddListener(() => OnTimeLimitButtonClicked(index));
        }

        UpdateButtonColors(btn_MaxPlayers, -1); // �ʱ�ȭ
        UpdateButtonColors(btn_Difficulty, -1); // �ʱ�ȭ
        UpdateButtonColors(btn_TimeLimit, -1); // �ʱ�ȭ
    }


    private void ValidateRoomName(string input) //�� �̸��� ��Ģ�� ���� �ڵ�
    {

        // �ѱ�(�ϼ���/����/����)�� ���ڸ� ����ϴ� ���Խ�
        string validPattern = @"^[��-�R��-����-��0-9\s]*$";

        // �Է� ���� ���Ͽ� ���� ������ ����
        if (!Regex.IsMatch(input, validPattern))
        {
            warningText.text = "�ѱ�, ����, ���鸸 �Է� �����մϴ�.";
            btn_CreateRoom.interactable = false; // �� ���� ��ư ��Ȱ��ȭ
        }
        else if (input.Length > MaxLength) // ���� ���� �ʰ� �˻�
        {
            warningText.text = $"�ִ� {MaxLength}�ڱ����� �Է� �����մϴ�.";
            btn_CreateRoom.interactable = false; // �� ���� ��ư ��Ȱ��ȭ
        }
        else if (input.Length == 0) // �� ���ڿ� �˻�
        {
            warningText.text = "�� �̸��� �Է����ּ���.";
            btn_CreateRoom.interactable = false; // �� ���� ��ư ��Ȱ��ȭ
        }
        else
        {
            warningText.text = ""; // ��Ģ�� ������ ��� �޽��� ����
            btn_CreateRoom.interactable = true; // �� ���� ��ư Ȱ��ȭ
        }
    }
    private void OnDestroy()
    {
        // �̺�Ʈ ����
        input_RoomName.onValueChanged.RemoveListener(ValidateRoomName);
    }

    

    //�� ����� ��ȭ�� ���� �� ȣ��Ǵ� �Լ�(���� �⺻ ����)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);  // �⺻ �޼��� ȣ��

        //Content�� �ڽ����� �پ��ִ� Item�� �� ����
        DeleteRoomListItem();

        //dicRoomInfo ������ roomList�� �̿��ؼ� ����
        UpdateRoomListItem(roomList);

        //dicRoom�� ������� roomListItem�� ������
        CreateRoomListItem();

    }

    // �� ����� �������� ��
    void SelectRoomItem(string roomName, GameObject button)
    {
        input_RoomName.text = roomName;
        // ������ ���õ� ��ư�� ������ �ʱ�ȭ
        if (roomListItem != null)
        {
            var prevImage = roomListItem.GetComponent<Image>();
            if (prevImage != null)
            {
                prevImage.color = Color.white; // �⺻ �������� ����
            }
        }

        // ���� ���õ� ��ư�� ������ ��������� ����
        roomListItem = button;
        var currentImage = roomListItem.GetComponent<Image>();
        if (currentImage != null)
        {
            currentImage.color = Color.yellow; // ��������� ����
        }

        // �� �̸��� �Է� �ʵ忡 ����
        input_RoomName.text = roomName;
    }



    void DeleteRoomListItem() // ���� �� ���� ������ ��
    {

        foreach (Transform tr in rtContent)
        {
            Destroy(tr.gameObject);
        }
    }


    // ��ũ�� �信 �������� �� ����� ���� �� ��
    void UpdateRoomListItem(List<RoomInfo> roomList) 
    {
        foreach (RoomInfo info in roomList)
        {
            // dicRoomInfo�� info �� ���̸����� �Ǿ��ִ� key���� �����ϴ°�
            if (dicRoomInfo.ContainsKey(info.Name))
            {
                // �̹� ������ ���̶��?
                if (info.RemovedFromList)
                {
                    dicRoomInfo.Remove(info.Name); // �� ������ ����
                    continue;
                }
            }
            dicRoomInfo[info.Name] = info; // �� ������ �߰�, ������Ʈ
        }
    }


    // ������ �� ����� ��ũ�� �信 ������ ��
    void CreateRoomListItem() 
    {
        foreach (RoomInfo info in dicRoomInfo.Values)
        {
            //�� ���� ������ ���ÿ� ScrollView-> Content�� �ڽ����� ����
            GameObject go = Instantiate(roomListItem, rtContent); //����: ������, ������ ��

            //������ item���� RoomListItem ������Ʈ�� �����´�.
            RoomListItem item = go.GetComponent<RoomListItem>();

            // ���̵��� ���� �ð��� Ŀ���� ������Ƽ���� ������
            string difficulty = info.CustomProperties.ContainsKey("difficulty") ?
                                info.CustomProperties["difficulty"].ToString() :
                                "����";
            int timeLimit = info.CustomProperties.ContainsKey("timeLimit") ?
                            Convert.ToInt32(info.CustomProperties["timeLimit"]) :
                            0;

            /// ������ ������Ʈ�� ������ �ִ� SetInfo �Լ� ����
            item.SetInfo(info.Name, info.PlayerCount, info.MaxPlayers, difficulty, timeLimit);

            // item Ŭ���Ǿ��� �� ȣ��Ǵ� �Լ� ���
            item.onDelegate = (roomName) =>
            {
                // SelectRoomItem�� �ٷ� ȣ��
                SelectRoomItem(roomName, go); // roomName�� ���� ��ư(GameObject)�� ���� -> ���õ� ������ ������ ����ǵ���
            };
        }
    }


    // �� ���� �� ��
    public void OnClickCreateRoom()
    {
        //�� �ɼ�
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)selectedMaxPlayers;
        string difficultyText = GetDifficultyText(selectedDifficulty);

        //Ŀ���� �� ������Ƽ ����(�߿�)
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {"maxPlayers", selectedMaxPlayers},   // �ִ� �÷��̾� �� ����
            {"difficulty", difficultyText},
            {"timeLimit", selectedTimeLimit}
        };

        //�κ񿡼��� ������ ������Ƽ ����
        options.CustomRoomPropertiesForLobby = new string[] { "maxPlayers", "difficulty", "timeLimit" };

        //�� ��Ͽ� ���̰� �Ұ��ΰ�?
        options.IsVisible = true;

        //�� ����
        PhotonNetwork.CreateRoom(input_RoomName.text, options);
    }


    public override void OnCreatedRoom() // �� ������ �������� ��
    {
        base.OnCreatedRoom();

        UnityEngine.Debug.Log("�� ���� ����");

        PhotonNetwork.LoadLevel("MakeRoom");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) //�� ������ �������� ��
    {
        base.OnCreateRoomFailed(returnCode, message);
        UnityEngine.Debug.Log("�� ���� ����" + message);
    }

    public void OnClickJoinRoom() // �� ����
    {
        PhotonNetwork.JoinRoom(input_RoomName.text);

    }

    public override void OnJoinedRoom() // �� ���忡 �������� ��
    {
        base.OnJoinedRoom();

        UnityEngine.Debug.Log("�� ���� ����");

        PhotonNetwork.LoadLevel("MakeRoom");
    }

    public override void OnJoinRoomFailed(short returnCode, string message) // �� ���忡 �������� ��
    {
        base.OnJoinRoomFailed(returnCode, message);
        UnityEngine.Debug.Log("�� ���� ����" + message);
    }



    // �� ������ ���� �ɼ� ���� �� �̷����� ui�� ��ȭ�� index ������Ʈ�� ���� �ڵ�
    void OnMaxPlayersButtonClicked(int index)
    {
        switch (index) // 2, 3, 4 �÷��̾� �ɼ�
        {
            case 0:
                selectedMaxPlayers = 2;
                break;
            case 1:
                selectedMaxPlayers = 3;
                break;
            case 2:
                selectedMaxPlayers = 4;
                break; 
        }
        UpdateButtonColors(btn_MaxPlayers, index);
        UnityEngine.Debug.Log("Selected Max Players: " + selectedMaxPlayers);
    }

    void OnDifficultyButtonClicked(int index)
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
        // selectedDifficulty ���� ������� ���� ���ڿ��� ��ȯ
        string difficultyText = GetDifficultyText(selectedDifficulty);
        UpdateButtonColors(btn_Difficulty, index);
        UnityEngine.Debug.Log("Selected Difficulty: " + difficultyText);
    }

    // selectedDifficulty�� ���� 2, 3, 4�� �� ���� "�ʱ�", "�߱�", "���"�̶�� ���ڿ��� ���
    string GetDifficultyText(int difficulty) 
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

    void OnTimeLimitButtonClicked(int index)
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
        UpdateButtonColors(btn_TimeLimit, index);
        UnityEngine.Debug.Log("Selected Time Limit: " + selectedTimeLimit);
    }

    private void SetDefaultSelection(Button[] buttons, int defaultIndex, out int selectedValue)
    {
        selectedValue = defaultIndex;

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;

            ColorBlock colorBlock = buttons[i].colors;
            colorBlock.normalColor = Color.white; // �⺻ ���� ȭ��Ʈ
            colorBlock.selectedColor = Color.yellow; //���õ� ���� ���̾�
            buttons[i].colors = colorBlock;

            buttons[i].onClick.AddListener(() =>
            {
                UpdateButtonColors(buttons, index); //��ư ���� ����
            });
        }

        UpdateButtonColors(buttons, defaultIndex);
    }

    // ��ư �迭�� ���� ������Ʈ �Լ�
    void UpdateButtonColors(Button[] buttons, int selectedIndex)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            ColorBlock colorBlock = buttons[i].colors;
            if (i == selectedIndex)
            {
                colorBlock.normalColor = Color.yellow;
            }
            else
            {
                colorBlock.normalColor = Color.white;
            }
            buttons[i].colors = colorBlock;
        }
    }
}
