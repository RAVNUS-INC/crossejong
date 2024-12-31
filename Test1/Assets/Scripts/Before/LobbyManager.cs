using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] InputField input_RoomName;
    [SerializeField] Button[] btn_MaxPlayers; // 3���� ��ư�� �迭�� ����
    [SerializeField] Button[] btn_Difficulty;
    [SerializeField] Button[] btn_TimeLimit;

    [SerializeField] Button btn_CreateRoom; // �游��� ��ư
    [SerializeField] Button btn_JoinRoom; // �� ���� ��ư
    [SerializeField] GameObject roomListItem; // �� ����� �����ִ� ��ũ�Ѻ�

    int selectedMaxPlayers = 0; // ���õ� MaxPlayers ���� ����
    int selectedDifficulty = 0;
    int selectedTimeLimit = 0;
    public Transform rtContent;

    // �� ����� ������ �ִ� Dictionaly ����
    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();

    void Start()
    {
        input_RoomName.onValueChanged.AddListener(OnNameValueChanged);
        btn_CreateRoom.onClick.AddListener(OnClickCreateRoom);
        btn_JoinRoom.onClick.AddListener(OnClickJoinRoom);

        // MaxPlayers ��ư�� ������ �߰�
        for (int i = 0; i < btn_MaxPlayers.Length; i++)
        {
            int index = i; // Ŭ������ ���� ���� ���� ���
            btn_MaxPlayers[i].onClick.AddListener(() => OnMaxPlayersButtonClicked(index));
        }

        for (int i = 0; i < btn_Difficulty.Length; i++)
        {
            int index = i;
            btn_Difficulty[i].onClick.AddListener(() => OnDifficultyButtonClicked(index));
        }

        for (int i = 0; i < btn_TimeLimit.Length; i++)
        {
            int index = i;
            btn_TimeLimit[i].onClick.AddListener(() => OnTimeLimitButtonClicked(index));
        }
    }
    //�� ����� ��ȭ�� ���� �� ȣ��Ǵ� �Լ�
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        //Content�� �ڽ����� �پ��ִ� Item�� �� ����
        DeleteRoomListItem();
        //dicRoomInfo ������ roomList�� �̿��ؼ� ����
        UpdateRoomListItem(roomList);
        //dicRoom�� ������� roomListItem�� ������
        CreateRoomListItem();

    }
    void SelectRoomItem(string roomName)
    {
        input_RoomName.text = roomName;
    }
    void DeleteRoomListItem()
    {

        foreach (Transform tr in rtContent)
        {
            Destroy(tr.gameObject);
        }
    }
    void UpdateRoomListItem(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            //dicRoomInfo�� info �� ���̸����� �Ǿ��ִ� key���� �����ϴ°�
            if (dicRoomInfo.ContainsKey(info.Name))
            {
                //���࿡ ���� �����Ǿ�����?
                if (info.RemovedFromList)
                {
                    dicRoomInfo.Remove(info.Name); //����
                    continue;
                }
            }
            dicRoomInfo[info.Name] = info; //�߰�
        }
    }
    void CreateRoomListItem()
    {
        foreach (RoomInfo info in dicRoomInfo.Values)
        {
            //�� ���� ������ ���ÿ� ScrollView-> Content�� �ڽ����� ����
            GameObject go = Instantiate(roomListItem, rtContent);
            //������ item���� RoomListItem ������Ʈ�� �����´�.
            RoomListItem item = go.GetComponent<RoomListItem>();
            //������ ������Ʈ�� ������ �ִ� SetInfo �Լ� ����
            item.SetInfo(info.Name, info.PlayerCount, info.MaxPlayers);
            //item Ŭ���Ǿ��� �� ȣ��Ǵ� �Լ� ���
            item.onDelegate = SelectRoomItem;
        }
    }
    void OnNameValueChanged(string s)
    {
        btn_JoinRoom.interactable = s.Length > 0;
        UpdateCreateButtonInteractable();
    }
 

    // ���� ��ư Ŭ���� ȣ��Ǵ� �Լ�
    public void OnClickCreateRoom()
    {
        //�� �ɼ�
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)selectedMaxPlayers;
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {"difficulty", selectedDifficulty},
            {"timeLimit", selectedTimeLimit}
        };
        options.CustomRoomPropertiesForLobby = new string[] { "difficulty", "timeLimit" };

        //�� ��Ͽ� ���̰� �Ұ��ΰ�?
        options.IsVisible = true;

        //�濡 ���� ���� ����
        //options.IsOpen = true;

        //�� ����
        PhotonNetwork.CreateRoom(input_RoomName.text, options);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("�� ���� ����" + message);
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        Debug.Log("�� ���� ����");

        PhotonNetwork.LoadLevel("MakeRoom");
    }
    public void OnClickJoinRoom()
    {
        // �� ����
        PhotonNetwork.JoinRoom(input_RoomName.text);

    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("�� ���� ����");

        PhotonNetwork.LoadLevel("MakeRoom");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("�� ���� ����" + message);
    }
    void JoinOrCreateRoom()
    {
        //�� �ɼ�
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)selectedMaxPlayers;
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {"difficulty", selectedDifficulty},
            {"timeLimit", selectedTimeLimit}
        };

        options.CustomRoomPropertiesForLobby = new string[] { "difficulty", "timeLimit" };

        //�� ��Ͽ� ���̰� �Ұ��ΰ�?
        options.IsVisible = true;

        //�濡 ���� ���� ����
        options.IsOpen = true;
        PhotonNetwork.JoinOrCreateRoom(input_RoomName.text, options, TypedLobby.Default);
    }
    void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    } // ���� ��ư Ŭ���� ȣ��Ǵ� �Լ�
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
    }

    void OnMaxPlayersButtonClicked(int index)
    {
        selectedMaxPlayers = (index + 2); // 2, 3, 4 �÷��̾� �ɼ�
        UpdateMaxPlayersButtonUI();
        UpdateCreateButtonInteractable();
    }

    void OnDifficultyButtonClicked(int index)
    {
        selectedDifficulty = index; // 0: �ʱ�, 1: �߱�, 2: ���
        UpdateButtonUI(btn_Difficulty, index);
        UpdateCreateButtonInteractable();
    }

    void OnTimeLimitButtonClicked(int index)
    {
        selectedTimeLimit = (index + 1) * 15; // 15, 30, 45�� �ɼ�
        UpdateButtonUI(btn_TimeLimit, index);
        UpdateCreateButtonInteractable();
    }

    void UpdateMaxPlayersButtonUI()
    {
        for (int i = 0; i < btn_MaxPlayers.Length; i++)
        {
            btn_MaxPlayers[i].interactable = (i + 2 != selectedMaxPlayers);
        }
    }
    void UpdateButtonUI(Button[] buttons, int selectedIndex)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = (i != selectedIndex);
        }
    }

    void UpdateCreateButtonInteractable()
    {
        btn_CreateRoom.interactable = (input_RoomName.text.Length > 0 &&
                                       selectedMaxPlayers > 0 &&
                                       selectedDifficulty >= 0 &&
                                       selectedTimeLimit > 0);
    }


}
