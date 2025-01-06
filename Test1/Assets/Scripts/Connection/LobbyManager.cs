using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.PointerEventData;
using Button = UnityEngine.UI.Button;

//�� ���� �� �� ������ ���� �ڵ�
public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] InputField input_RoomName;
    [SerializeField] Button[] btn_MaxPlayers; // 3���� ��ư�� �迭�� ����
    [SerializeField] Button[] btn_Difficulty;
    [SerializeField] Button[] btn_TimeLimit;

    [SerializeField] Text warningText; // ��� �޽����� ����� UI �ؽ�Ʈ
    [SerializeField] Button btn_CreateRoom; // �� ����� ��ư
    [SerializeField] Button btn_JoinRoom; // �� ���� ��ư
    [SerializeField] GameObject roomListItem; // �� ����� �����ִ� ��ũ�Ѻ�

    int selectedMaxPlayers = 0; // �ִ��ο�(2, 3, 4��)
    int selectedDifficulty = 0; // ���̵�(�ʱ�, �߱�, ���)
    int selectedTimeLimit = 0; // ī�� ������� ���ѽð�(15��, 30��, 45��)
    public Transform rtContent;
    private const int MaxLength = 12; // ���̸� �ִ� �Է� ����

    // �� ����� ������ �ִ� Dictionaly ����
    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();

    void Start()
    {
        // �⺻�� ����: ��� ��ư�� ù ��° �׸��� �⺻ �����ϵ��� ����
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

        // TimeLimi ��ư�� ������ �߰�
        for (int i = 0; i < btn_TimeLimit.Length; i++)
        {
            int index = i;
            btn_TimeLimit[i].onClick.AddListener(() => OnTimeLimitButtonClicked(index));
        }
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
    
    private void SetDefaultSelection(Button[] buttons, int defaultIndex, out int selectedValue) // ��ư �迭�� �⺻�� ���� �Լ�
    {
        // ���õ� ���� ��¿����� ��ȯ
        selectedValue = defaultIndex;

        // ��� ��ư�� ���� �ʱ�ȭ
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Ŭ���� ���� �ذ��� ���� ���� ����

            // ��� ��ư�� ���� �⺻�� ����
            ColorBlock colorBlock = buttons[i].colors;
            colorBlock.normalColor = Color.white; // �⺻ ����: ȭ��Ʈ
            colorBlock.selectedColor = Color.yellow; // ���õ� ��ư ����: ���
            buttons[i].colors = colorBlock;

            // ��ư Ŭ�� �̺�Ʈ ����
            buttons[i].onClick.AddListener(() =>
            {
                UpdateButtonColors(buttons, index); // ��ư ���� ����
            });
        }

        // �⺻��(ù ��° ��ư) ����
        UpdateButtonColors(buttons, defaultIndex);
    }

    // ��ư �迭�� ���� ���� �Լ�
    private void UpdateButtonColors(Button[] buttons, int selectedIndex) 
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            ColorBlock colorBlock = buttons[i].colors;
            if (i == selectedIndex)
            {
                colorBlock.normalColor = Color.yellow; // ���õ� ��ư
            }
            else
            {
                colorBlock.normalColor = Color.white; // ���õ��� ���� ��ư
            }
            buttons[i].colors = colorBlock;
        }
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

    // ���� �������� ��
    void SelectRoomItem(string roomName)
    {
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

            //������ ������Ʈ�� ������ �ִ� SetInfo �Լ� ����
            item.SetInfo(info.Name, info.PlayerCount, info.MaxPlayers);

            //item Ŭ���Ǿ��� �� ȣ��Ǵ� �Լ� ���
            item.onDelegate = SelectRoomItem;
        }
    }


    // �� ���� �� ��
    public void OnClickCreateRoom()
    {
        //�� �ɼ�
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)selectedMaxPlayers;

        //Ŀ���� �� ������Ƽ ����(�߿�)
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {"maxPlayers", selectedMaxPlayers},   // �ִ� �÷��̾� �� ����
            {"difficulty", selectedDifficulty},
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
        selectedMaxPlayers = (index + 2); // 2, 3, 4 �÷��̾� �ɼ�
        UpdateButtonColors(btn_MaxPlayers, index);
        UnityEngine.Debug.Log("Selected Max Players: " + selectedMaxPlayers);
    }

    void OnDifficultyButtonClicked(int index)
    {
        selectedDifficulty = index; // 0: �ʱ�, 1: �߱�, 2: ���
        UpdateButtonColors(btn_Difficulty, index);
        UnityEngine.Debug.Log("Selected Difficulty: " + selectedDifficulty);
    }

    void OnTimeLimitButtonClicked(int index)
    {
        selectedTimeLimit = (index + 1) * 15; // 15, 30, 45�� �ɼ�
        UpdateButtonColors(btn_TimeLimit, index);
        UnityEngine.Debug.Log("Selected Time Limit: " + selectedTimeLimit);
    }

}
