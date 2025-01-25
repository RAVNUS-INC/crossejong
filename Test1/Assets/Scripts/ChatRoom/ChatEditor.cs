using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using UnityEditor.VersionControl;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;
//using static UnityEngine.GraphicsBuffer;
using System;
//using UnityEditor.XR;
//using System.Reflection;


public class ChatEditor : MonoBehaviour, IOnEventCallback
{
    public ChatManager chatManager; //chatmanager스크립트 참조

    private const byte SendChatEventCode = 1; // 이벤트 코드 정의(메시지 전송)
    private const byte SendUserEventCode = 0; // 이벤트 코드 정의(입장/퇴장)
    private int myActorNum, myImgIndex; //내 actornumber, 내 사진 인덱스
    private string myDisplayName, myMesseages; //내 이름, 내가 보낸 메시지
    public InputField ChatField; //채팅입력창

    void Awake()//초기화
    {
        ChatField.text = ""; //채팅입력창은 항상 비워놓기

        //내 정보 불러오기
        var customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        myDisplayName = customProperties.ContainsKey("Displayname") ? (string)customProperties["Displayname"] : "Unknown";
        myActorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        myImgIndex = int.Parse(customProperties.ContainsKey("Imageindex") ? (string)customProperties["Imageindex"] : "Unknown");

        UserEnterState(true); //나의 입장 알리기
    }

    //유저의 입/퇴장을 알리는 함수
    public void UserEnterState(bool isbool)
    {
        // 내가 입장/퇴장했음을 알리는 메시지 띄우기
        chatManager.DisplayUserMessage(myDisplayName, isbool);

        // 데이터 묶기(액터넘버, 내이름)
        object[] data = new object[] { myActorNum, myDisplayName, isbool};

        // 방에 있는 모든 유저에게 이벤트 브로드캐스트
        PhotonNetwork.RaiseEvent(SendUserEventCode, data, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        Debug.Log(isbool ? "나의 입장을 알렸습니다" : "나의 퇴장을 알렸습니다");
    }

    public void SendMyMessage() //전송버튼 누르면 실행
    {
        if (ChatField.text.Trim() != "")
        {
            //채팅창에 입력한 내용을 메시지 변수에 저장
            myMesseages = ChatField.text;
            //화면에 나 말풍선 띄우기(보내기)
            chatManager.Chat(true, myMesseages, "나", null);

            // 메시지와 함께 보낼 데이터 패키지
            object[] data = new object[] { myActorNum, myDisplayName, myImgIndex, myMesseages };
            // 방에 있는 모든 유저에게 이벤트 브로드캐스트
            PhotonNetwork.RaiseEvent(SendChatEventCode, data, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            Debug.Log($"내 메시지와 정보를 전달했습니다");

            //내용을 전달한 뒤, 채팅 인풋필드 비우기
            ChatField.text = "";
        }
        
    }

    public void OnEvent(EventData photonEvent)
    {

        object[] data = (object[])photonEvent.CustomData; //데이터파싱
        //공통
        if (data != null)
        {
            int actornumber = (int)data[0];
            string senderName = (string)data[1];

            switch (photonEvent.Code)
            {
                case SendChatEventCode:
                    int Index = (int)data[2]; //보낸 유저의 인덱스 받기
                    string message = (string)data[3]; //보낸 유저의 메시지 내용 받기
                    Debug.Log($"상대방의 정보를 받았습니다");

                    if (actornumber != PhotonNetwork.LocalPlayer.ActorNumber) // 자신의 ActorNumber와 다르다면(다른 유저가 보낸 이벤트라면)
                    {
                        //화면에 보낸사람 말풍선 띄우기
                        chatManager.Chat(false, message, senderName, Index);
                    }
                    break;

                case SendUserEventCode:
                    bool isEnter = (bool)data[2]; //보낸 유저가 입장인지 퇴장인지 구분

                    if (actornumber != PhotonNetwork.LocalPlayer.ActorNumber) // 자신의 ActorNumber와 다르다면(다른 유저가 보낸 이벤트라면)
                    {
                        chatManager.DisplayUserMessage(senderName, isEnter);
                    }
                    break;
            }
        }

    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}


