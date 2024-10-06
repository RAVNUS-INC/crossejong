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

    // 방 목록을 가지고 있는 Dictionaly 변수
    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();

    void Start()
    {
        input_RoomName.onValueChanged.AddListener(OnNameValueChanged);
        input_MaxPlayer.onValueChanged.AddListener(OnPlayerValueChange);
        btn_CreateRoom.onClick.AddListener(OnClickCreateRoom);
        btn_JoinRoom.onClick.AddListener(OnClickJoinRoom);
    }
    //방 목록의 변화가 있을 때 호출되는 함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        //Content에 자식으로 붙어있는 Item을 다 삭제
        DeleteRoomListItem();
        //dicRoomInfo 변수를 roomList를 이용해서 갱신
        UpdateRoomListItem(roomList);
        //dicRoom을 기반으로 roomListItem을 만들자
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
            //dicRoomInfo에 info 의 방이름으로 되어있는 key값이 존재하는가
            if (dicRoomInfo.ContainsKey(info.Name))
            {
                //만약에 방이 삭제되었으면?
                if (info.RemovedFromList)
                {
                    dicRoomInfo.Remove(info.Name); //삭제
                    continue;
                }
            }
            dicRoomInfo[info.Name] = info; //추가
        }
    }
    void CreateRoomListItem()
    {
        foreach (RoomInfo info in dicRoomInfo.Values)
        {
            //방 정보 생성과 동시에 ScrollView-> Content의 자식으로 하자
            GameObject go = Instantiate(roomListItem, rtContent);
            //생성된 item에서 RoomListItem 컴포넌트를 가져온다.
            RoomListItem item = go.GetComponent<RoomListItem>();
            //가져온 컴포넌트가 가지고 있는 SetInfo 함수 실행
            item.SetInfo(info.Name, info.PlayerCount, info.MaxPlayers);
            //item 클릭되었을 때 호출되는 함수 등록
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

    // 생성 버튼 클릭시 호출되는 함수
    public void OnClickCreateRoom()
    {
        //방 옵션
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = int.Parse(input_MaxPlayer.text);

        //방 목록에 보이게 할것인가?
        options.IsVisible = true;

        //방에 참여 가능 여부
        //options.IsOpen = true;

        //방 생성
        PhotonNetwork.CreateRoom(input_RoomName.text, options);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("방 생성 실패" + message);
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        Debug.Log("방 생성 성공");

        PhotonNetwork.LoadLevel("Scene C");
    }
    public void OnClickJoinRoom()
    {
        // 방 참여
        PhotonNetwork.JoinRoom(input_RoomName.text);

    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("방 입장 성공");

        PhotonNetwork.LoadLevel("Scene C");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("방 입장 실패" + message);
    }
    void JoinOrCreateRoom()
    {
        //방 옵션
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = int.Parse(input_MaxPlayer.text);

        //방 목록에 보이게 할것인가?
        options.IsVisible = true;

        //방에 참여 가능 여부
        options.IsOpen = true;
        PhotonNetwork.JoinOrCreateRoom(input_RoomName.text, options, TypedLobby.Default);
    }
    void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    } // 참여 버튼 클릭시 호출되는 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
    }
}
