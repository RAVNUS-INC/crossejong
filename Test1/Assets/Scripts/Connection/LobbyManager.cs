using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.UIElements;
//using static UnityEditor.Progress;
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

    // �� ���� �� �ɼǵ�
    private int selectedMaxPlayers; // �ִ��ο�(2, 3, 4��)
    private int selectedDifficulty; // ���̵�(�ʱ�, �߱�, ���)
    private int selectedTimeLimit; // ī�� ������� ���ѽð�(15��, 30��, 45��)

    private int selectedMaxPlayersIndex; // �ο� ���� �ε���
    private int selectedDifficultyIndex; // ���̵� ���� �ε���
    private int selectedTimeLimitIndex; // ���ѽð� �ε���

    public Transform rtContent; // ������ ����
    private const int MaxLength = 12; // ���̸� �ִ� �Է� ����
    // �� ����� ������ �ִ� Dictionaly ����
    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();


    //public static LobbyManager Instance { get; private set; } //�� ��ȯ�ÿ��� ������ �����ְ�


    private void Awake() //�ʿ�
    {
        //if (Instance == null)
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject); // ������Ʈ�� �� ��ȯ �� �������� ����
        //}
        //else
        //{
        //    Destroy(gameObject); // �ߺ��� �ν��Ͻ� ����
        //}
        ResetRoomSetPanel(); // ù ���� ���� �� ���� ����

    }


    private void OnDestroy()
    {

        //// input_RoomName�� null�� �ƴ� ��쿡�� RemoveListener�� ȣ��
        //if (input_RoomName != null)
        //{
        //    input_RoomName.onValueChanged.RemoveListener(ValidateRoomName);
        //}
    }
    


    void Start() 
    {
        
    }


    // �� ���� �� ���� �ɼ� ��ư�� ���̸� ��Ģ�� ���� �ʱ�ȭ
    public void ResetRoomSetPanel()
    {
        // �⺻ ��ư ������ (0,0,0) ��������� ǥ��
        SetDefaultSelection(btn_MaxPlayers, 0);
        SetDefaultSelection(btn_Difficulty, 0);
        SetDefaultSelection(btn_TimeLimit, 0);

        // �ɼ� ��ư �����ϸ� ��������� �ٲ�� �ϴ� �ڵ�
        MaxPlayerSet(btn_MaxPlayers);
        DifficultySet(btn_Difficulty);
        TimeLimitSet(btn_TimeLimit);

        // �� �̸� �Է� �ʵ� �ʱ�ȭ
        input_RoomName.text = ""; //�� �̸� �⺻ ���� ����
        btn_CreateRoom.interactable = false; // ó������ �� ���� ��ư ��Ȱ��ȭ
        warningText.text = ""; // �ʱ� ��� �޽��� ����
        input_RoomName.onValueChanged.AddListener(ValidateRoomName); //�� �̸� �ۼ��� ��, �� �̸� ��Ģ �˻�
    }

    private void MaxPlayerSet(Button[] buttons)
    {
        // MaxPlayers ��ư�� ������ �߰�
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnMaxPlayersButtonClicked(index,buttons)); //������ ��ư�� ���� ����
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
        if (info.RemovedFromList)
        {
            dicRoomInfo.Remove(info.Name); // �� ���� ����
        }
        else
        {
            dicRoomInfo[info.Name] = info; // �� ���� �߰� �Ǵ� ������Ʈ
        }
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

            // ������ ������Ʈ�� ������ �ִ� SetInfo �Լ� ����(��� ���� ����)
            item.SetInfo(info.Name, info.PlayerCount, info.MaxPlayers, difficulty, timeLimit);  //���̵��� ���ѽð��� custom properties�� ����, �������� photon���� �⺻����

            // item Ŭ���Ǿ��� �� ȣ��Ǵ� �Լ� ���
            item.onDelegate = (roomName) =>
            {
                // SelectRoomItem�� �ٷ� ȣ��
                SelectRoomItem(roomName, go); // roomName�� ���� ��ư(GameObject)�� ���� -> ���õ� ������ ������ ����ǵ���
            };
        }
    }


    // �� ���� �� ��(���������Һ�)
    public void OnClickCreateRoom()
    {
        //�� �ɼ�
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)selectedMaxPlayers;
        string difficultyText = GetDifficultyText(selectedDifficulty);

        //Ŀ���� �� ������Ƽ ����(�߿�)
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            //{"PlayersIndex", selectedMaxPlayersIndex},  // �÷��̾� index

            {"DifficultyIndex", selectedDifficultyIndex},  // ���̵� index
            {"TimeLimitIndex", selectedTimeLimitIndex},  // ���ѽð� index

            {"difficultyInt", selectedDifficulty},  // ���̵� int��(2,3,4)

            {"timeLimit", selectedTimeLimit},  // ���ѽð� int��(15,30,45)
            {"difficulty", difficultyText}   // ���̵� str��(�ʱ�,�߱�,���)

        };

        //�κ񿡵� ���̰� �� ���ΰ�?(��Ͽ�)->�ǵ帮�� X
        options.CustomRoomPropertiesForLobby = new string[] { "difficulty", "timeLimit" };

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
    void OnMaxPlayersButtonClicked(int index, Button[] PlayBtn)
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
        UpdateButtonColors(PlayBtn, index);
        UnityEngine.Debug.Log("Selected Max Players: " + selectedMaxPlayers); //�޽��� ���
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
}
