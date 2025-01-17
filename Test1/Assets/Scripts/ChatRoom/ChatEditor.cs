using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.GraphicsBuffer;

//[CustomEditor(typeof(ChatManager))]
//public class ChatEditor : Editor
//{
//    ChatManager chatManager;
//    string text;


//    void OnEnable()
//    {
//        chatManager = target as ChatManager;
//    }


//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        EditorGUILayout.BeginHorizontal();
//        text = EditorGUILayout.TextArea(text);

//        if (GUILayout.Button("보내기", GUILayout.Width(60)) && text.Trim() != "")
//        {
//            chatManager.Chat(true, text, "나", null);
//            text = "";
//            GUI.FocusControl(null);
//        }

//        if (GUILayout.Button("받기", GUILayout.Width(60)) && text.Trim() != "")
//        {
//            chatManager.Chat(false, text, "타인", null);
//            text = "";
//            GUI.FocusControl(null);
//        }

//        EditorGUILayout.EndHorizontal();
//    }

//}

public class ChatEditor : MonoBehaviour, IOnEventCallback
{
    public ChatManager chatManager;

    private const byte SendChatEventCode = 1; // 이벤트 코드 정의
    public InputField ChatField; //채팅입력창

    void Awake()
    {
        ChatField.text = ""; //채팅입력창은 항상 비워놓기
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void SendMyMessage() //전송버튼 누르면 실행
    {
        if (ChatField.text.Trim() != "")
        {
            // 커스텀 프로퍼티에서 나의 이름, 인덱스 값을 가져옴
            var customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            string displayname = customProperties.ContainsKey("Displayname") ? (string)customProperties["Displayname"] : "Unknown";
            string imgindex = customProperties.ContainsKey("Imageindex") ? (string)customProperties["Imageindex"] : "Unknown";

            //채팅창에 입력한 내용을 메시지 변수에 저장
            string message = ChatField.text;
            //화면에 나 말풍선 띄우기(보내기)
            chatManager.Chat(true, message, "나", null);

            //int형식으로 변환하기
            int index = int.Parse(imgindex);
            //유저의 actornumber(자동할당) 변수에 저장
            int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber; 

            // 메시지와 함께 보낼 데이터 패키지
            object[] data = new object[] { myActorNumber, displayname, index, message };

            // 방에 있는 모든 유저에게 이벤트 브로드캐스트
            PhotonNetwork.RaiseEvent(SendChatEventCode, data, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            Debug.Log($"내 메시지와 정보를 전달했습니다");

            //내용을 전달한 뒤, 채팅 인풋필드 비우기
            ChatField.text = "";
        }
        
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == SendChatEventCode)
        {
            // 수신된 데이터 파싱
            object[] data = (object[])photonEvent.CustomData;
            int actornumber = (int)data[0]; //보낸 유저의 actornumber 받기
            string senderName = (string)data[1]; //보낸 유저의 이름 받기
            int Index = (int)data[2]; //보낸 유저의 인덱스 받기
            string message = (string)data[3]; //보낸 유저의 메시지 내용 받기
            Debug.Log($"상대방의 정보를 받았습니다");

            if (actornumber != PhotonNetwork.LocalPlayer.ActorNumber) // 자신의 ActorNumber와 다르다면(다른 유저가 보낸 이벤트라면)
            {
                //화면에 보낸사람 말풍선 띄우기
                chatManager.Chat(false, message, senderName, Index);
            }
                
        }
    }











}


