using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ù ���� ������ ���� ȭ�鿡�� �۵��ϴ� �ڵ�(�г���, ���������ʻ��� ����)
public class UserSetManager : MonoBehaviourPunCallbacks
{
    private UserSetManager s_instance;
    public UserSetManager Instance { get { return s_instance; } }

    [SerializeField]
    InputField inputText; //�г��� �Է�
    [SerializeField]
    Button confirmButton; //���� ��ư
    [SerializeField]
    Text warningText; // ��� �޽����� ����� UI �ؽ�Ʈ

    private const int MaxLength = 5; // �ִ� �Է� ����

    void Start()
    {
        confirmButton.interactable = false; // �⺻������ ��ư ��Ȱ��ȭ
        warningText.text = ""; // �ʱ� ��� �޽��� ����

        //������ ����Ǿ�����
        inputText.onValueChanged.AddListener(ValidateNickname);

        //������ ����������
        inputText.onSubmit.AddListener(OnSubmit);

        ////Ŀ���� �ٸ����� ������
        //inputText.onEndEdit.AddListener(
        //    (string s) =>
        //    {
        //        Debug.Log("OnEndmit" + s);
        //    }
        //);

        confirmButton.onClick.AddListener(OnClickConnect);
    }

    private void ValidateNickname(string input)
    {
        /// �ѱ�(�ϼ���/����/����)�� ���ڸ� ����ϴ� ���Խ�
        string validPattern = @"^[��-�R��-����-��0-9]*$";

        // ���� ����
        input = input.Replace(" ", "");

        // �Է� ���� ���Ͽ� ���� ������ ����
        if (!Regex.IsMatch(input, validPattern))
        {
            warningText.text = "�ѱ۰� ���ڸ� �Է� �����մϴ�.";
            confirmButton.interactable = false; // Ȯ�� ��ư ��Ȱ��ȭ
        }
        else if (input.Length > MaxLength) // ���� ���� �ʰ� �˻�
        {
            warningText.text = $"�ִ� {MaxLength}�ڱ����� �Է� �����մϴ�.";
            confirmButton.interactable = false; // Ȯ�� ��ư ��Ȱ��ȭ
        }
        else if (input.Length == 0) // �� ���ڿ� �˻�
        {
            warningText.text = "�г����� �Է����ּ���.";
            confirmButton.interactable = false; // Ȯ�� ��ư ��Ȱ��ȭ
        }
        else
        {
            warningText.text = ""; // ��Ģ�� ������ ��� �޽��� ����
            confirmButton.interactable = true; // Ȯ�� ��ư Ȱ��ȭ
        }

    }

    private void OnDestroy()
    {
        // �̺�Ʈ ����
        inputText.onValueChanged.RemoveListener(ValidateNickname);
    }

    void OnSubmit(string s) // s�� ���ڿ�
    {
        Debug.Log("OnSubmit " + s); // �г����� �Է��ϰ� ���������� �˸�
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

    //Lobby ���Կ� ���������� ȣ��Ǵ� �Լ�
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        //���� ������ �̵�
        PhotonNetwork.LoadLevel("Main");

        print("�κ� ���� ����");

    }
    public void OnClickConnect()
    {
        // ������ ���� ���� ��û
        PhotonNetwork.ConnectUsingSettings();
    }
}
