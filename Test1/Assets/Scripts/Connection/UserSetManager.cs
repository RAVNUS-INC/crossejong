using Photon.Pun;
using Photon.Realtime; // AuthenticationValues �� ��Ÿ �ǽð� ��� ���
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// ���� ������ ���� ȭ�鿡�� �۵��ϴ� �ڵ�(�г���, ���������ʻ��� ���� ����)
public class UserSetManager : MonoBehaviourPunCallbacks
{
    public static UserSetManager Instance { get; private set; }

    [SerializeField] public InputField inputText; //�г��� �Է�(���߿� �ٲ� �� �ִ� Displayname)
    [SerializeField] Button confirmButton; //����(����) ��ư
    [SerializeField] Text warningText; // ��� �޽����� ����� UI �ؽ�Ʈ
    [SerializeField] public Text saveText; // ����Ϸ� �޽����� ����� UI �ؽ�Ʈ

    // -----displayname ����-----
    private const int MinLength = 3; // �ּ� �Է� ����(2���� �̻�)
    private const int MaxLength = 8; // �ִ� �Է� ����(��������)

    public Image centralImage; // �߾ӿ� ǥ�õǴ� �������̹���
    public Sprite[] profileImages; // 3���� �⺻ ���� �̹���

    private int currentIndex = 0; // ���� ���õ� �̹��� �ε���
    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // ���� Ű
    private string displayName; //���÷��� �̸�(�ӽ������� ���� ����)

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
        confirmButton.onClick.AddListener(OnClickSaveDisplayName); //�ʱ� �̸� ���� (+ �ܾ�ϼ�Ƚ�� ��ư�� ���� ����)
    }


    // �̸� �г��� ���� �Լ���
    // ������ �̸� ���� �Լ�
    public void DisplayName() //DisplayName: �������� ����
    {
        var request = new GetAccountInfoRequest(); //�̸� ���� �ҷ�����
        PlayFabClientAPI.GetAccountInfo(request,
             result =>
             {
                 // ���� displayName�� ���� ��� null�� ��ȯ
                 if (string.IsNullOrEmpty(result.AccountInfo.TitleInfo.DisplayName))
                 {
                     displayName = null; // displayName�� ������ null �Ҵ�
                     Debug.Log("displayName�� �����ϴ�.");
                 }
                 else // ���� ���� �����ϴ� ���
                 {
                     // displayName ���� ���� ������ ����
                     displayName = result.AccountInfo.TitleInfo.DisplayName;

                     SaveCustomProperty("Displayname", displayName); //�������гο� ������Ʈ-�ش� �̸��� Ŀ����������Ƽ�� ����
                     Debug.Log($"displayName: {displayName}�� ������Ƽ�� �����߽��ϴ�");
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
                    Debug.Log("�������̹��� KEY�� �����մϴ�. ���� �ҷ��ɴϴ�.");
                    string value = result.Data[PROFILE_IMAGE_INDEX_KEY].Value;

                    if (!string.IsNullOrEmpty(value))  //���� �����Ѵٸ�
                    {
                        currentIndex = int.Parse(value);
                        
                    }
                    else 
                    {
                        Debug.LogWarning("KEY�� ���� ��� �ֽ��ϴ�. �⺻��(0)���� �����մϴ�.");
                        currentIndex = 0; // �⺻�� ����
                    }
                }
                else
                {
                    Debug.LogWarning("KEY�� �������� �ʽ��ϴ�. �⺻��(0)�� �����մϴ�.");
                    currentIndex = 0; // �⺻�� ����
                }

                UpdateCentralImage(); // �̹��� ������Ʈ �� �ε��� ����, ������Ƽ�� ����
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
        },
            Permission = UserDataPermission.Public // �����͸� ���� ���·� ����
        };

        //playfab�� ����
        PlayFabClientAPI.UpdateUserData(request,
            result =>
            {
                Debug.Log($"�������̹��� ������ ���� ����: {index}");
            },
            error =>
            {
                Debug.LogError($"���� ������ ���� ����: {error.GenerateErrorReport()}");
            });
        //����� �̹��� �ε����� Ŀ����������Ƽ�� ����(�����)
        SaveCustomProperty("Imageindex", index.ToString()); 
        Debug.Log($"���õ� Imageindex: {currentIndex}�� ������Ƽ�� �����߽��ϴ�");
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
            SaveDisplayName(); //�г����� ����(������Ƽ���� ����)
            Debug.Log("ù displayName�� �����Ǿ����ϴ�.");
            OnClickConnect(); //�������� ���������� ��û�Ѵ�
        }
        else //displayname�� �̹� ������ ���(������ ������ ���)
        {
            // ���� ������ displayName�� ����
            SaveDisplayName(); //������ �̸��� �� �̸����� ����� �����Ѵ�
            Debug.Log("���ο� displayName�� �����Ǿ����ϴ�.");
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
        
        //playfab�� ����
        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
           result =>
           {
               Debug.Log($"�г��� ���� ����: {result.DisplayName}");
           },

           error =>
           {
               Debug.LogError($"�г��� ���� ����: {error.GenerateErrorReport()}");
           });
        SaveCustomProperty("Displayname", displayName); //����� �̸��� Ŀ����������Ƽ�� ����(or �����)
        Debug.Log($"Displayname: {displayName}�� ������Ƽ�� �����߽��ϴ�");
    }

    // --------------Ŀ����������Ƽ�� �̸�, �ε��� ���� -----------------
    void SaveCustomProperty(string key, string value)
    {
        // 1. �ؽ����̺� ����
        Hashtable customProperties = new Hashtable();

        // 2. Ű�� ���� �߰�
        customProperties[key] = value;

        // 3. Ŀ���� ������Ƽ�� ���� (�÷��̾��� Ŀ���� ������Ƽ�� ����)
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }


    //�̸� ���� ��ư Ŭ���� �� -> �̸� �Է� ��ǲ Ȱ��ȭ
    public void ChangeNameBtn() 
    {
        inputText.interactable = true; //�̸� ���� Ȱ��ȭ
    }


    //�̸� ���� ��Ģ(displayname)
    public void ValidateNickname(string input)
    {
        /// �ѱ�(�ϼ���/����/����)�� ���ڸ� ����ϴ� ���Խ�
        string validPattern = @"^[��-�R��-����-��0-9]*$";

        // ���� ����
        string inputname = input.Replace(" ", ""); //������ ������� �ʴ´�

        // �Է� ���� ���Ͽ� ���� ������ ����
        if (!Regex.IsMatch(inputname, validPattern))
        {
            warningText.text = "�ѱ۰� ���ڸ� �Է� �����մϴ�.";
            confirmButton.interactable = false; 
        }
        // ���� ���� �ʰ� �˻�
        else if (GetKoreanCharCount(inputname) > MaxLength) // �ѱ� ����, ������ ������ �ִ� ���� �˻�
        {
            warningText.text = $"�ִ� {MaxLength}�ڱ����� �Է� �����մϴ�.";
            confirmButton.interactable = false; 
        }
        // �ּ� ���� ���� �˻�
        else if (GetKoreanCharCount(inputname) < MinLength) // �ѱ� ����, ������ ������ �ִ� ���� �˻�
        {
            warningText.text = $"�ּ� {MinLength}�� �̻��̾�� �մϴ�."; //3�� �̻��̾�� ��
            confirmButton.interactable = false;
        }
        else if (inputname.Length == 0) // �� ���ڿ� �˻�
        {
            warningText.text = "�г����� �Է����ּ���.";
            confirmButton.interactable = false; 
        }
        else if (displayName == inputname && (inputText.isActiveAndEnabled)) //�Է¶��� ���� �г��Ӱ� �����鼭 Ȱ��ȭ�Ǿ��ִ� ���
        {
            warningText.text = "���� �г��Ӱ� �޶�� �մϴ�.";
            confirmButton.interactable = false;
        }
        else
        {
            warningText.text = ""; // ��Ģ�� ������ ��� �޽��� ����
            confirmButton.interactable = true; // Ȯ�� ��ư Ȱ��ȭ
        }
        // �Է¶��� ������ ������ �� �ݿ�
        inputText.text = inputname;
    }


    // �ѱ� ���� ����, ������ �����Ͽ� ���� ���� ����ϴ� �Լ�
    private int GetKoreanCharCount(string input)
    {
        int count = 0;
        foreach (char c in input)
        {
            // �ѱ� �������� üũ (��-�R ����)
            if (c >= '��' && c <= '�R')
            {
                count++;
            }
            // �ѱ� ����/�������� üũ (��-��, ��-�� ����)
            else if ((c >= '��' && c <= '��') || (c >= '��' && c <= '��'))
            {
                count++;
            }
            // ����(0-9)���� üũ
            else if (c >= '0' && c <= '9')
            {
                count++;
            }
        }
        return count;
    }


    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("������ ���� ���� ����");

        //���� �̸��� ���濡 ����
        PhotonNetwork.NickName = inputText.text;

        // ���� ���� ID ���
        //Debug.Log($"Photon UserId: {PhotonNetwork.AuthValues.UserId}");

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


