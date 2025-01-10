using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NativeGalleryNamespace;


// ���� ������ ���� ȭ�鿡�� �۵��ϴ� �ڵ�(�г���, ���������ʻ��� ���� ����)
public class UserSetManager : MonoBehaviourPunCallbacks
{
    public static UserSetManager Instance { get; private set; }

    [SerializeField] InputField inputText; //�г��� �Է�(���߿� �ٲ� �� �ִ� Displayname)
    [SerializeField] Button confirmButton; //����(����) ��ư
    [SerializeField] Text warningText; // ��� �޽����� ����� UI �ؽ�Ʈ
    [SerializeField] Text saveText; // ����Ϸ� �޽����� ����� UI �ؽ�Ʈ
    
    private const int MaxLength = 8; // �ִ� �Է� ����(��������)

    public Image centralImage; // �߾ӿ� ǥ�õǴ� �������̹���
    public Sprite[] profileImages; // 3���� �⺻ ���� �̹���
    private int currentIndex = 0; // ���� ���õ� �̹��� �ε���
    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // ���� Ű
    private string displayName; //���÷��� �̸�

    public GameObject profilePanel; //������ ���� �г�(�����гο��� �̸� �غ��ؾ� �۵�)
    public GameObject usersetPanel; //���� �ʱ� ���� �г�

    void Start()
    {
        if ((usersetPanel.activeSelf || profilePanel.activeSelf))
        {
            DisplayName(); // ���� �̸� ���� �ҷ��� ������ ����
            CheckAndSaveDefaultImageIndex(); // ���� ���� �� ����� �̹��� �ε����� �ҷ��� ������ ���� �� �̹��� ������Ʈ
        }

        confirmButton.interactable = false; // �⺻������ ��ư ��Ȱ��ȭ
        warningText.text = ""; // �ʱ� ��� �޽��� ����
        saveText.text = ""; // �ʱ� ���� �޽��� ����

        //������ ����Ǿ����� ��Ģ �˻�
        inputText.onValueChanged.AddListener(ValidateNickname);

        //Ȯ�� ��ư ������ �̸� ����
        confirmButton.onClick.AddListener(OnClickSaveDisplayName);

    }

    // �̸� �г��� ���� �Լ���
    // ������ �̸� ���� �Լ�
    public void DisplayName() //DisplayName: �������� ����
    {
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request,
             result =>
             {
                 // displayName�� ���� ��� null�� ��ȯ
                 if (string.IsNullOrEmpty(result.AccountInfo.TitleInfo.DisplayName))
                 {
                     displayName = null; // displayName�� ������ null �Ҵ�
                     Debug.Log("displayName�� �����ϴ�.");
                 }
                 else
                 {
                     // displayName ���� ���� ������ ����
                     displayName = result.AccountInfo.TitleInfo.DisplayName;
                     Debug.Log($"�ҷ��� displayName: {displayName}");
                 }
             },
            error =>
            {
                Debug.LogError($"���� ���� �ҷ����� ����: {error.GenerateErrorReport()}");
            });
    }


    // ���� ������ �̹��� ���� �Լ���
    // ����� �̹��� �ε����� �ҷ�����

    private void CheckAndSaveDefaultImageIndex()
    {
        var request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request,
            result =>
            {
                // PROFILE_IMAGE_INDEX_KEY�� �����ϴ��� Ȯ��
                if (result.Data.ContainsKey(PROFILE_IMAGE_INDEX_KEY))
                {
                    Debug.Log("KEY�� �����մϴ�. ���� �ҷ��ɴϴ�.");
                    string value = result.Data[PROFILE_IMAGE_INDEX_KEY].Value;

                    if (!string.IsNullOrEmpty(value)) 
                    {
                        currentIndex = int.Parse(value); 
                    }
                    else 
                    {
                        Debug.LogWarning("KEY�� ���� ��� �ֽ��ϴ�. �⺻��(0)���� �����մϴ�.");
                        currentIndex = 0; // �⺻�� ����
                        SaveSelectedImageIndex(currentIndex); // PlayFab�� ����
                    }
                }
                else
                {
                    Debug.LogWarning("KEY�� �������� �ʽ��ϴ�. �⺻��(0)�� �����մϴ�.");
                    currentIndex = 0; // �⺻�� ����
                    SaveSelectedImageIndex(currentIndex); // PlayFab�� ���ο� Ű ���� �� �� ����
                }

                UpdateCentralImage(); // �̹��� ������Ʈ
            },
            error =>
            {
                Debug.LogError($"���� ������ �ҷ����� ����: {error.GenerateErrorReport()}");
            });
    }


    // �̹��� ������Ʈ �Լ�
    private void UpdateCentralImage()
    {
        if (profileImages.Length > 0 && currentIndex >= 0 && currentIndex < profileImages.Length)
        {
            centralImage.sprite = profileImages[currentIndex];  // �ε����� �ش��ϴ� �̹����� ������Ʈ
            SaveSelectedImageIndex(currentIndex);                // ���õ� �̹��� �ε��� ����
        }
        else
        {
            Debug.LogWarning("Invalid profile image index.");
        }
    }

    // ���õ� �̹��� �ε����� ����
    private void SaveSelectedImageIndex(int index)
    {
        // ���õ� �̹��� �ε����� PlayFab Ÿ��Ʋ �����Ϳ� ����
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
        {
            { PROFILE_IMAGE_INDEX_KEY, index.ToString() }
        }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result =>
            {
                Debug.Log($"������ ������ ���� ����: {index}");
            },
            error =>
            {
                Debug.LogError($"���� ������ ���� ����: {error.GenerateErrorReport()}");
            });
    }




    // ���� ��ư Ŭ�� �� ȣ��
    public void OnLeftButtonClicked()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = profileImages.Length - 1;  // ��ȯ (�� ó������ ���ư�)
        UpdateCentralImage();  // �̹��� ������Ʈ
    }

    // ������ ��ư Ŭ�� �� ȣ��
    public void OnRightButtonClicked()
    {
        currentIndex++;
        if (currentIndex >= profileImages.Length) currentIndex = 0;  // ��ȯ (�� ���������� ó������ ���ư�)
        UpdateCentralImage();  // �̹��� ������Ʈ
    }





    

    public void OnClickSaveDisplayName()
    {
        // displayName�� �����ϴ��� Ȯ��(ù �������� �ƴ����� Ȯ��)
        if (string.IsNullOrEmpty(displayName))
        {
            // displayName�� ���� ��� (ù ������ ���)
            Debug.Log("ù displayName�� �����Ǿ����ϴ�.");
            SaveDisplayName(); //�г����� �����ϰ�
            OnClickConnect(); //�������� ���������� ��û�Ѵ�
        }
        else //displayname�� �̹� ������ ���(������ ������ ���)
        {
            // ���� ������ displayName�� ����
            Debug.Log("���ο� displayName�� �����Ǿ����ϴ�.");
            SaveDisplayName(); //������ �̸��� �� �̸����� ����� �����Ѵ�
            saveText.text = "����Ǿ����ϴ�"; //���� �޽��� �˸�
        }
    }
    public void SaveDisplayName() //�ܼ��� �̸� �����ϱ�
    {
        string displayName = inputText.text.Trim();

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
           result =>
           {
               Debug.Log($"�г��� ���� ����: {result.DisplayName}");
           },

           error =>
           {
               Debug.LogError($"�г��� ���� ����: {error.GenerateErrorReport()}");
           });
    }


    //�̸� ���� ��ư Ŭ���� �� -> �̸� �Է� ��ǲ Ȱ��ȭ
    public void ChangeNameBtn() 
    {
        inputText.interactable = true; //�̸� ���� Ȱ��ȭ
    }


    //�̸� ���� ��Ģ
    public void ValidateNickname(string input)
    {
        /// �ѱ�(�ϼ���/����/����)�� ���ڸ� ����ϴ� ���Խ�
        string validPattern = @"^[��-�R��-����-��0-9]*$";

        // ���� ����
        input = input.Replace(" ", ""); //������ ������� �ʴ´�

        // �Է� ���� ���Ͽ� ���� ������ ����
        if (!Regex.IsMatch(input, validPattern))
        {
            warningText.text = "�ѱ۰� ���ڸ� �Է� �����մϴ�.";
            confirmButton.interactable = false; 
        }
        else if (input.Length > MaxLength) // ���� ���� �ʰ� �˻�
        {
            warningText.text = $"�ִ� {MaxLength}�ڱ����� �Է� �����մϴ�.";
            confirmButton.interactable = false; 
        }
        else if (input.Length == 0) // �� ���ڿ� �˻�
        {
            warningText.text = "�г����� �Է����ּ���.";
            confirmButton.interactable = false; 
        }
        else if (displayName == inputText.text && (inputText.isActiveAndEnabled)) //�Է¶��� ���� �г��Ӱ� �����鼭 Ȱ��ȭ�Ǿ��ִ� ���
        {
            warningText.text = "���� �г��Ӱ� �޶�� �մϴ�.";
            confirmButton.interactable = false;
        }
        else
        {
            warningText.text = ""; // ��Ģ�� ������ ��� �޽��� ����
            confirmButton.interactable = true; // Ȯ�� ��ư Ȱ��ȭ
        }

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

    private void OnDestroy()
    {
        // �̺�Ʈ ����
        inputText.onValueChanged.RemoveListener(ValidateNickname);
    }


}


