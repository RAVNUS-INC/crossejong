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

    private UserSetManager s_instance;
    public UserSetManager Instance { get { return s_instance; } }

    [SerializeField] InputField inputText; //�г��� �Է�(���߿� �ٲ� �� �ִ� Displayname)
    [SerializeField] Button confirmButton; //����(����) ��ư
    [SerializeField] Text warningText; // ��� �޽����� ����� UI �ؽ�Ʈ
    [SerializeField] Text saveText; // ����Ϸ� �޽����� ����� UI �ؽ�Ʈ
    
    private const int MaxLength = 8; // �ִ� �Է� ����(��������)

    public Image centralImage; // �߾ӿ� ǥ�õǴ� �������̹���
    public Sprite[] profileImages; // 3���� �⺻ ���� �̹���
    private int currentIndex = 0; // ���� ���õ� �̹��� �ε���


    void Awake()
    {
        // �̱��� �ν��Ͻ��� ����
        if (s_instance == null)
        {
            s_instance = this;
        }
        else if (s_instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); // ���� ����Ǵ��� ��ü�� �ı����� �ʵ��� ����
    }

    void Start()
    {
        confirmButton.interactable = false; // �⺻������ ��ư ��Ȱ��ȭ
        warningText.text = ""; // �ʱ� ��� �޽��� ����
        saveText.text = ""; // �ʱ� ���� �޽��� ����

        //������ ����Ǿ����� ��Ģ �˻�
        inputText.onValueChanged.AddListener(ValidateNickname);

        //Ȯ�� ��ư ������ �̸� ����
        confirmButton.onClick.AddListener(DisplayName);
        confirmButton.onClick.AddListener(SaveProfileImageToPlayFab);

        LoadSavedImage();  // ���� ���� �� ����� �̹��� �ε����� �ҷ�����

    }


    public void SaveProfileImageToPlayFab()
    {
        // PlayerPrefs���� ����� �̹��� �ε����� ��������
        if (PlayerPrefs.HasKey("ProfileImageIndex"))
        {
            int profileImageIndex = PlayerPrefs.GetInt("ProfileImageIndex");

            // PlayFab�� ������ �����͸� �غ�
            var request = new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
            {
                { "ProfileImageIndex", profileImageIndex.ToString() }
            }
            };

            // PlayFab�� ������ ������Ʈ ��û
            PlayFabClientAPI.UpdateUserData(request,
                result => {
                    Debug.Log("Successfully saved ProfileImageIndex to PlayFab.");
                },
                error => {
                    Debug.LogError("Error saving ProfileImageIndex to PlayFab: " + error.GenerateErrorReport());
                });
        }
        else
        {
            Debug.LogWarning("ProfileImageIndex not found in PlayerPrefs.");
        }
    }

    // ���� ȭ��ǥ Ŭ�� ��
    public void OnLeftArrowClicked()
    {
        currentIndex = (currentIndex - 1 + profileImages.Length) % profileImages.Length;
        UpdateCentralImage();
    }

    // ���� ȭ��ǥ Ŭ�� ��
    public void OnRightArrowClicked()
    {
        currentIndex = (currentIndex + 1) % profileImages.Length;
        UpdateCentralImage();
    }

    // �߾� �̹����� ������Ʈ
    private void UpdateCentralImage()
    {
        if (centralImage == null)
        {
            Debug.LogError("centralImage is not initialized!");
            return;
        }

        if (profileImages == null || profileImages.Length == 0)
        {
            Debug.LogError("profileImages array is not initialized or is empty!");
            return;
        }

        // currentIndex�� profileImages �迭�� ��ȿ�� ���� ���� �ִ��� Ȯ��
        if (currentIndex < 0 || currentIndex >= profileImages.Length)
        {
            Debug.LogError("currentIndex is out of range!");
            return;
        }

        // �̹��� ������Ʈ
        centralImage.sprite = profileImages[currentIndex];

        // ������ �̹��� �ε����� ����
        SaveSelectedImageIndex(currentIndex);
    }

    // �̹��� �ε����� PlayerPrefs�� ����
    private void SaveSelectedImageIndex(int index)
    {
        PlayerPrefs.SetInt("ProfileImageIndex", index);
        PlayerPrefs.Save();
    }

    // ���� �� ����� �̹��� �ε����� �ҷ�����
    public void LoadSavedImage()
    {
        if (PlayerPrefs.HasKey("ProfileImageIndex"))
        {
            currentIndex = PlayerPrefs.GetInt("ProfileImageIndex");
            UpdateCentralImage();
        }
        else
        {
            // �⺻ �̹��� ���� (ù ���� �� �ε��� 0, �ѹ� �����ߴٸ� ������ �̹������� ����)
            UpdateCentralImage();
        }
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
        else
        {
            warningText.text = ""; // ��Ģ�� ������ ��� �޽��� ����
            confirmButton.interactable = true; // Ȯ�� ��ư Ȱ��ȭ
        }

    }

    //������ �̸� ����
    public void DisplayName() //DisplayName: �������� ����
    {
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request,
             result =>
             {
                 // displayName�� �����ϴ��� Ȯ��(ù �������� �ƴ����� Ȯ��)
                 if (string.IsNullOrEmpty(result.AccountInfo.TitleInfo.DisplayName))
                 {
                     // displayName�� ���� ��� (ù ������ ���)
                     Debug.Log("displayName�� �������� �ʾҽ��ϴ�. ���� ��...");
                     SaveDisplayName(); //�г����� �����ϰ�
                     OnClickConnect(); //�������� ���������� ��û�Ѵ�
                 }
                 else //displayname�� �̹� ������ ���(������ ������ ���)
                 {
                     SaveDisplayName(); //������ �̸��� �� �̸����� ����� �����Ѵ�
                     saveText.text = "����Ǿ����ϴ�"; //���� �޽��� �˸�
                 }
             },
            error =>
            {
                Debug.LogError($"���� ���� �ҷ����� ����: {error.GenerateErrorReport()}");
            });
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


