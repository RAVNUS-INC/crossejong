using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


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
        //������ ����Ǿ�����
        inputText.onValueChanged.AddListener(OnValueChanged);
        //������ ����������
        inputText.onSubmit.AddListener(OnSubmit);
        //Ŀ���� �ٸ����� ������
        inputText.onEndEdit.AddListener(
            (string s) =>
            {
                Debug.Log("OnEndmit" + s);
            }
        );
        inputButton.onClick.AddListener(OnClickConnect);
    }

    void OnValueChanged(string s)
    {
        inputButton.interactable = s.Length > 0;
    }
    void OnSubmit(string s)
    {
        Debug.Log("OnSubmit " + s);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("������ ���� ���� ����");

        //���� �̸��� ���濡 ����
        PhotonNetwork.NickName = inputText.text;
        //�κ�����
        PhotonNetwork.JoinLobby();
    }
    //Lobby ������ ���������� ȣ��Ǵ� �Լ�
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        //���� ������ �̵�
        PhotonNetwork.LoadLevel("Main");

        print("���� ���� ����");

    }
    public void OnClickConnect()
    {
        // ������ ���� ���� ��û
        PhotonNetwork.ConnectUsingSettings();
    }
}
