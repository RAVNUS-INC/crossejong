using Photon.Pun;
using Photon.Realtime; // AuthenticationValues �� ��Ÿ �ǽð� ��� ���
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


// ���� ������ ���� ȭ�鿡�� �۵��ϴ� �ڵ�(�г���, ���������ʻ��� ���� ����)
public class UserSetManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public InputField inputText; //�г��� �Է�(���߿� �ٲ� �� �ִ� Displayname)
    [SerializeField] Button confirmButton; //����(����) ��ư
    [SerializeField] Text warningText; // ��� �޽����� ����� UI �ؽ�Ʈ
    [SerializeField] public Text saveText; // ����Ϸ� �޽����� ����� UI �ؽ�Ʈ(profile panel���� ����)

    // displayname ����
    private const int MinLength = 3; // �ּ� �Է� ����(2���� �̻�)
    private const int MaxLength = 8; // �ִ� �Է� ����(��������)

    public Image centralImage; // �߾ӿ� ǥ�õǴ� �������̹���
    public Sprite[] profileImages; // 3���� �⺻ ���� �̹���

    private int currentIndex = 0; // ���� ���õ� �̹��� �ε���
    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // ���� Ű
    private string displayName; //���÷��� �̸�(�ӽ������� ���� ����)

    public GameObject profilePanel; //������ ���� �г�(�����гο��� �̸� �غ��ؾ� �۵�)
    public GameObject usersetPanel; //���� �ʱ� ���� �г�

    //playerprefs�� ������ �����(Key)
    private const string DISPLAYNAME_KEY = "DisplayName"; // ������ DisplayName
    private const string IMAGEINDEX_KEY = "ImageIndex"; // ������ �̹��� �ε���

    void Start()
    {
        // �ʱ� ���� ���� �г� OR ���� ���� ������ �г��� Ȱ��ȭ�Ǿ��ִٸ�
        if ((usersetPanel.activeSelf || profilePanel.activeSelf)) 
        {
            LoadDefaultDisplayName(); // ���� �̸� ���� �ҷ��� ������ ����
            LoadDefaultImageIndex(); // ���� �̹��� �ε����� �ҷ��� ������ ����, ������Ʈ
        }
        
        confirmButton.interactable = false; // �⺻������ ��ư ��Ȱ��ȭ
        warningText.text = ""; // �ʱ� ��� �޽��� ����
        saveText.text = ""; // �ʱ� ���� �޽��� ����

        //������ ����Ǿ����� ��Ģ �˻�
        inputText.onValueChanged.AddListener(ValidateNickname);

        //Ȯ�� ��ư ������ �̸� �� �̹��� ����(playfab �� playerprefs�� ������Ʈ)(+ �ܾ�ϼ�Ƚ�� ��ư�� ���� ����)
        confirmButton.onClick.AddListener(SaveDisplayName); //�̸� ���� 
        confirmButton.onClick.AddListener(SaveSelectedImageIndex); // �̹��� ����
    }

    
    private void LoadDefaultDisplayName() // ����� DisplayName �ε� �� ���� �Լ�(�����ϸ� �ش� ����, �������� ������ Guest)
    {
        if (PlayerPrefs.HasKey(DISPLAYNAME_KEY))
        {
            // Ű�� �����ϸ� ����� ���� �����´�
            string displayName = PlayerPrefs.GetString(DISPLAYNAME_KEY, "Guest");

            // �ҷ��� �̸��� ��ǲ���� �����ֱ�
            inputText.text = displayName;

            //������ �̸� �ʱ� ��Ȱ��ȭ
            inputText.interactable = false;
        }
        else
        {
            // Ű�� �������� ������ �⺻���� �����Ѵ�
            inputText.text = "";

            // �⺻���� playerprefs���� �ٷ� ����
            PlayerPrefs.SetString(DISPLAYNAME_KEY, "Guest"); PlayerPrefs.Save();
        }
    }

    private void LoadDefaultImageIndex() // ����� �̹��� �ε����� �ҷ�����(�����ϸ� �ش� ����, �������� ������ �⺻�� 0)
    {
        if (PlayerPrefs.HasKey(IMAGEINDEX_KEY))
        {
            // Ű�� �����ϸ� ����� ���� �����´�
            int Index = PlayerPrefs.GetInt(IMAGEINDEX_KEY, 0);

            //currentIndex �ε��� ������ �� ����
            currentIndex = Index;
        }
        else
        {
            // Ű�� �������� ������ �⺻���� �����Ѵ�
            currentIndex = 0;

            // �⺻���� playerprefs���� �ٷ� ����
            PlayerPrefs.SetInt(IMAGEINDEX_KEY, currentIndex); PlayerPrefs.Save();
        }
        centralImage.sprite = profileImages[currentIndex];  // �̹��� ������Ʈ
    }

    public void SaveDisplayName() //DisplayName�� playfab�� playerprefs�� ����
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
                Debug.Log($"[Playfab] �г��� ���� ����: {result.DisplayName}");
            },
            error =>
            {
                Debug.LogError($"[Playfab] �г��� ���� ����: {error.GenerateErrorReport()}");
            });

        //����� �̹��� �ε����� playerprefs�� ����(���� ������ ��� �����, �ű� ������ ���� �߰��ϴ� ��Ȳ)
        UpdateDisplayName(displayName);
        Debug.Log($"[playerprefs] Displayname: {displayName}�� �����߽��ϴ�");

        if (usersetPanel.activeSelf)
        {
            // �������� ���������� ��û
            OnClickConnect();
        }
        if (profilePanel.activeSelf)
        {
            // ���� �޽��� �˸�
            saveText.text = "����Ǿ����ϴ�";
        }
    }

    private void SaveSelectedImageIndex() // ���õ� �̹��� �ε����� ������ playfab�� ����, playerprefs ����
    {
        string ImageIndex = currentIndex.ToString(); //int�� -> ���ڿ��� ��ȯ

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
        {
            { PROFILE_IMAGE_INDEX_KEY, ImageIndex}
        },
            Permission = UserDataPermission.Public // �����͸� ���� ���·� ����
        };

        //playfab�� ����
        PlayFabClientAPI.UpdateUserData(request,
            result =>
            {
                Debug.Log($"[Playfab] �������̹��� ������ ���� ����: {ImageIndex}");
            },
            error =>
            {
                Debug.LogError($"[Playfab] ���� ������ ���� ����: {error.GenerateErrorReport()}");
            });

        //���ڿ� -> int������ ��ȯ
        int RImageIndex = int.Parse(ImageIndex); 

        //����� �̹��� �ε����� playerprefs�� ����(���� ������ ��� �����, �ű� ������ ���� �߰��ϴ� ��Ȳ)
        UpdateImageIndex(RImageIndex); 
        Debug.Log($"[playerprefs] Imageindex: {currentIndex}�� �����߽��ϴ�");
    }

    void UpdateDisplayName(string name) //���ο� �̸� ����
    {
        PlayerPrefs.SetString(DISPLAYNAME_KEY, name); // ���ο� �� ����
        PlayerPrefs.Save(); // ���� ����
    }

    void UpdateImageIndex(int newIndex) //���ο� �ε��� ����
    {
        PlayerPrefs.SetInt(IMAGEINDEX_KEY, newIndex); // ���ο� �� ����
        PlayerPrefs.Save(); // ���� ����
    }
    
    public void OnLeftButtonClicked() // ���� ��ư Ŭ�� �� ȣ��
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = profileImages.Length - 1;  // ��ȯ (�� ó������ ���ư�)
        centralImage.sprite = profileImages[currentIndex];  // �ε����� �ش��ϴ� �̹����� ������Ʈ
    }

    public void OnRightButtonClicked() // ������ ��ư Ŭ�� �� ȣ��
    {
        currentIndex = (currentIndex + 1) % profileImages.Length;
        centralImage.sprite = profileImages[currentIndex];  // �ε����� �ش��ϴ� �̹����� ������Ʈ
    }

    public void ChangeNameBtn() //�̸� ���� ��ư Ŭ���� �� -> �̸� �Է� ��ǲ Ȱ��ȭ
    {
        inputText.interactable = true; //�̸� ���� Ȱ��ȭ
    }

    public void ValidateNickname(string input) //�̸� ���� ��Ģ(displayname)
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

    private int GetKoreanCharCount(string input) // �ѱ� ���� ����, ������ �����Ͽ� ���� ���� ����ϴ� �Լ�
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


    public override void OnConnectedToMaster() //������ ���� ���� �Ǹ�
    {
        base.OnConnectedToMaster();
        Debug.Log("������ ���� ���� ����");

        //���� �̸��� ���濡 ����
        PhotonNetwork.NickName = inputText.text;

        //�κ�����
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby() //Lobby ���Կ� ���������� ȣ��Ǵ� �Լ�
    {
        base.OnJoinedLobby();

        //���� ������ �̵�
        PhotonNetwork.LoadLevel("Main");

        print("�κ� ���� ����");

    }
    public void OnClickConnect() // ������ ���� ���� ��û(OkBtn�� ����)
    {
        // ������ ���� ���� ��û
        PhotonNetwork.ConnectUsingSettings();

        //�ε��� ui �ִϸ��̼� �����ֱ�
        LoadingSceneController.Instance.LoadScene("Main");
    }
    private void OnDestroy() // �̺�Ʈ ����
    {
        inputText.onValueChanged.RemoveListener(ValidateNickname);
    }


}


