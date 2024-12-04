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
    [SerializeField] InputField input_MaxPlayer;
    [SerializeField] Button btn_CreateRoom;
    [SerializeField] Button btn_JoinRoom;
    [SerializeField] GameObject roomListItem;
    public Transform rtContent;

    // �� ����� ������ �ִ� Dictionaly ����
    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();

    void Start()
    {
        input_RoomName.onValueChanged.AddListener(OnNameValueChanged);
        input_MaxPlayer.onValueChanged.AddListener(OnPlayerValueChange);
        btn_CreateRoom.onClick.AddListener(OnClickCreateRoom);
        btn_JoinRoom.onClick.AddListener(OnClickJoinRoom);
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
        if (input_RoomName.text == "")
            btn_CreateRoom.interactable = false;
    }
    void OnPlayerValueChange(string s)
    {
        btn_CreateRoom.interactable = s.Length > 0;
        if (input_MaxPlayer.text == "")
            btn_CreateRoom.interactable = false;
    }

    // ���� ��ư Ŭ���� ȣ��Ǵ� �Լ�
    public void OnClickCreateRoom()
    {
        //�� �ɼ�
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = int.Parse(input_MaxPlayer.text);

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

        PhotonNetwork.LoadLevel("Scene C");
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

        PhotonNetwork.LoadLevel("Scene C");
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
        options.MaxPlayers = int.Parse(input_MaxPlayer.text);

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
}
