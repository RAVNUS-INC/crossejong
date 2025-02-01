using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;


public class ChatEditor : MonoBehaviourPunCallbacks
{
    public ChatManager chatManager; //chatmanager스크립트 참조

    private int myActorNum, myImgIndex; //내 actornumber, 내 사진 인덱스
    private string myDisplayName, myMesseages; //내 이름, 내가 보낸 메시지
    private Dictionary<int, bool> playerReadyStates = new Dictionary<int, bool>(); // 플레이어 준비 상태 저장

    public InputField ChatField; //채팅입력창
    public Button ReadyBtn; //준비버튼

    void Awake()//초기화
    {
        ChatField.text = ""; //채팅입력창은 항상 비워놓기

        ReadyBtn.interactable = true; // 처음에는 준비버튼 활성화

        //내 정보 불러오기(내 이름, 내 고유번호, 프로필 인덱스)
        var customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        myDisplayName = customProperties.ContainsKey("Displayname") ? (string)customProperties["Displayname"] : "Unknown";
        myActorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        myImgIndex = int.Parse(customProperties.ContainsKey("Imageindex") ? (string)customProperties["Imageindex"] : "Unknown");

        //나의 입장 알리기("~님이 입장하였습니다.")
        photonView.RPC("EnterState", RpcTarget.All, myDisplayName, true);
        Debug.Log("나의 입장을 알렸습니다");
    }


    //유저들의 준비하기 유무 수신 함수
    //public void UserReadyState() //준비하기 버튼 누르면 실행->한번 누르면 버튼 이제 비활성화
    //{
    //    ReadyBtn.interactable = false; // 버튼 한번 눌렀으면 다음부턴 비활성화(준비 취소 불가능)

    //    if (!PhotonNetwork.InRoom) return;

    //    // 데이터 묶기(액터넘버, 내이름)
    //    object[] data = new object[] { myActorNum, myDisplayName };

    //    // 방장에게 이벤트 브로드캐스트
    //    PhotonNetwork.RaiseEvent(SendReadyEventCode, data, new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }, SendOptions.SendReliable);
    //    Debug.Log("방장에게 준비 완료 상태를 알렸습니다.");
    //}

    //유저의 입/퇴장을 알리는 함수
    //public void UserEnterState(bool isbool)
    //{
    //    // 내가 입장/퇴장했음을 알리는 메시지 띄우기
    //    chatManager.DisplayUserMessage(myDisplayName, isbool);

    //    // 데이터 묶기(액터넘버, 내이름)
    //    object[] data = new object[] { myActorNum, myDisplayName, isbool};

    //    // 방에 있는 모든 유저에게 이벤트 브로드캐스트
    //    PhotonNetwork.RaiseEvent(SendUserEventCode, data, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    //    Debug.Log(isbool ? "나의 입장을 알렸습니다" : "나의 퇴장을 알렸습니다");
    //}

    //전송버튼 누르면 메시지 전달
    public void SendMyMessage() 
    {
        if (ChatField.text.Trim() != "")
        {
            //채팅창에 입력한 내용을 메시지 변수에 저장
            myMesseages = ChatField.text;

            //나를 제외한 다른 유저들에게 내 채팅 전달
            photonView.RPC("SendChat", RpcTarget.Others, false, myMesseages, myDisplayName, myImgIndex);
            Debug.Log($"내 채팅과 정보를 다른 유저에게 전달했습니다");

            //내 채팅에 내 메시지 업데이트
            chatManager.Chat(true, myMesseages, "나", null);

            //내용을 전달한 뒤, 채팅 인풋필드 비우기
            ChatField.text = "";
        }      
    }

    //public void OnEvent(EventData photonEvent)
    //{
    //    object[] data = (object[])photonEvent.CustomData; //데이터파싱
    //    //공통
    //    if (data != null)
    //    {
    //        int actornumber = (int)data[0];
    //        string senderName = (string)data[1];

    //        switch (photonEvent.Code)
    //        {
    //            //case SendChatEventCode:
    //            //    int Index = (int)data[2]; //보낸 유저의 인덱스 받기
    //            //    string message = (string)data[3]; //보낸 유저의 메시지 내용 받기
    //            //    Debug.Log($"상대방의 정보를 받았습니다");

    //            //    if (actornumber != PhotonNetwork.LocalPlayer.ActorNumber) // 자신의 ActorNumber와 다르다면(다른 유저가 보낸 이벤트라면)
    //            //    {
    //            //        //화면에 보낸사람 말풍선 띄우기
    //            //        chatManager.Chat(false, message, senderName, Index);
    //            //    }
    //            //    break;

    //            //case SendUserEventCode:
    //            //    bool isEnter = (bool)data[2]; //보낸 유저가 입장인지 퇴장인지 구분

    //            //    if (actornumber != PhotonNetwork.LocalPlayer.ActorNumber) // 자신의 ActorNumber와 다르다면(다른 유저가 보낸 이벤트라면)
    //            //    {
    //            //        chatManager.DisplayUserMessage(senderName, isEnter);
    //            //    }
    //            //    break;

    //            case SendReadyEventCode: //준비 버튼 이벤트를 받았을 때(방장만이 받을 수 있음)
    //                playerReadyStates[actornumber] = true; // 준비 상태 저장

    //                if (!PhotonNetwork.IsMasterClient) return; // 방장만 다음의 코드를 수행

    //                foreach (Player player in PhotonNetwork.PlayerList)
    //                {
    //                    if (!playerReadyStates.ContainsKey(player.ActorNumber) || !playerReadyStates[player.ActorNumber])
    //                        return; // 한 명이라도 준비하지 않았다면 종료
    //                }
    //                //모두 준비 했을 경우
    //                Debug.Log("모든 플레이어 준비. 씬을 이동합니다.");
    //                // 플레이룸 씬으로 이동
    //                //PhotonNetwork.LoadLevel("PlayRoom"); // 방장만 씬을 이동해도, photon에서 모든 플레이어들의 씬 동기화를 보장해줌
    //                break;
    //        }
    //    }
    //}

    //private void OnEnable()
    //{
    //    PhotonNetwork.AddCallbackTarget(this);
    //}

    //private void OnDisable()
    //{
    //    PhotonNetwork.RemoveCallbackTarget(this);
    //}

    [PunRPC]
    //채팅을 모두에게 보내고 ui업데이트까지 한번에 동기화
    void SendChat(bool who, string chat, string senderName, int index)
    {
        //화면에 말풍선 띄우기(나: true, 상대방: false), index는 프로필이미지
        chatManager.Chat(who, chat, senderName, index);
        Debug.Log("누군가의 채팅이 도착했습니다");
    }

    [PunRPC]
    //유저의 입장 퇴장 메시지 알리미
    void EnterState(string enteruserName, bool isbool)
    {
        // 내가 입장/퇴장했음을 알리는 메시지 띄우기
        chatManager.DisplayUserMessage(enteruserName, isbool);
    }



}


