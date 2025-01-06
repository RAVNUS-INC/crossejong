using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 초기 유저 설정 화면에서 로비로 넘어갈때 작동하는 코드
public class ConnectionManager : MonoBehaviourPunCallbacks
{
    private ConnectionManager s_instance;
    public ConnectionManager Instance { get { return s_instance; } }

    [SerializeField]
    InputField inputText;
    [SerializeField]
    Button inputButton;

    void Start()
    {
        //내용이 변경되었을때
        inputText.onValueChanged.AddListener(OnValueChanged);
        //내용을 제출했을때
        inputText.onSubmit.AddListener(OnSubmit);
        //커서가 다른곳을 누르면
        inputText.onEndEdit.AddListener(
            (string s) =>
            {
                Debug.Log("OnEndmit" + s);
            }
        );
        inputButton.onClick.AddListener(OnClickConnect);
    }

    void OnValueChanged(string s) // s는 문자열
    {
        inputButton.interactable = s.Length > 0; // input에 뭐라도 입력했으면 확인버튼 활성화
    }
    void OnSubmit(string s) // s는 문자열
    {
        Debug.Log("OnSubmit " + s); // 닉네임을 입력하고 제출했음을 알림
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("마스터 서버 접속 성공");

        //나의 이름을 포톤에 설정
        PhotonNetwork.NickName = inputText.text;
        //로비진입
        PhotonNetwork.JoinLobby();
    }
    //Lobby 진입에 성공했으면 호출되는 함수
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        //메인 씬으로 이동
        PhotonNetwork.LoadLevel("Main");

        print("로비 진입 성공");

    }
    public void OnClickConnect()
    {
        // 마스터 서버 접속 요청
        PhotonNetwork.ConnectUsingSettings();
    }
}
