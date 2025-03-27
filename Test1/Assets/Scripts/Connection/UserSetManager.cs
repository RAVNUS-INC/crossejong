using Photon.Pun;
using Photon.Realtime; // AuthenticationValues �� ��Ÿ �ǽð� ��� ���
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// ���� ������ ���� ȭ�鿡�� �۵��ϴ� �ڵ�(�г���, ���������ʻ��� ���� ����)
public class UserSetManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public TMP_InputField inputText; //�г��� �Է�(���߿� �ٲ� �� �ִ� Displayname)
    [SerializeField] public Button confirmButton; //����(����) ��ư
    [SerializeField] TMP_Text warningText; // ��� �޽����� ����� UI �ؽ�Ʈ
    [SerializeField] public TMP_Text saveText; // ����Ϸ� �޽����� ����� UI �ؽ�Ʈ(profile panel���� ����)

    // displayname ����
    private const int MinLength = 3; // �ּ� �Է� ����(2���� �̻�)
    private const int MaxLength = 8; // �ִ� �Է� ����(��������)

    public Image centralImage; // �߾ӿ� ǥ�õǴ� �������̹���
    private int currentIndex = 0; // ���� ���õ� �̹��� �ε���

    public GameObject profilePanel; //������ ���� �г�(�����гο��� �̸� �غ��ؾ� �۵�)
    public GameObject usersetPanel; //���� �ʱ� ���� �г�

    

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
        confirmButton.onClick.AddListener(() => { inputText.interactable=false; }); // ��ǲ�ʵ� ���� ��Ȱ��ȭ
    }

    
    private void LoadDefaultDisplayName() // ����� DisplayName �ε� �� ���� �Լ�(�����ϸ� �ش� ����, �������� ������ Guest)
    {
        if (PlayerPrefs.HasKey(UserInfoManager.DISPLAYNAME_KEY))
        {
            // Ű�� �����ϸ� ����� ���� �����´�
            UserInfoManager.instance.MyName = PlayerPrefs.GetString(UserInfoManager.DISPLAYNAME_KEY, "Guest");

            // �ҷ��� �̸��� ��ǲ���� �����ֱ�
            inputText.text = UserInfoManager.instance.MyName;

            //������ �̸� �ʱ� ��Ȱ��ȭ
            inputText.interactable = false;
        }
        else
        {
            // Ű�� �������� ������ �⺻���� �����Ѵ�
            inputText.text = "";

            // �⺻���� playerprefs���� �ٷ� ����
            PlayerPrefs.SetString(UserInfoManager.DISPLAYNAME_KEY, "Guest"); PlayerPrefs.Save();
        }
    }

    private void LoadDefaultImageIndex() // ����� �̹��� �ε����� �ҷ�����(�����ϸ� �ش� ����, �������� ������ �⺻�� 0)
    {
        if (PlayerPrefs.HasKey(UserInfoManager.IMAGEINDEX_KEY))
        {
            // Ű�� �����ϸ� ����� ���� �����´�
            UserInfoManager.instance.MyImageIndex = PlayerPrefs.GetInt(UserInfoManager.IMAGEINDEX_KEY, 0);

            //currentIndex �ε��� ������ �� ����
            currentIndex = UserInfoManager.instance.MyImageIndex;
        }
        else
        {
            // Ű�� �������� ������ �⺻���� �����Ѵ�
            currentIndex = 0;

            // �⺻���� playerprefs���� �ٷ� ����
            PlayerPrefs.SetInt(UserInfoManager.IMAGEINDEX_KEY, currentIndex); PlayerPrefs.Save();
        }
        centralImage.sprite = UserInfoManager.instance.profileImages[currentIndex];  // �̹��� ������Ʈ
    }

    public void SaveDisplayName() //DisplayName�� playfab�� playerprefs�� ����
    {
        if (UserInfoManager.instance.MyName == inputText.text)
        {
            return;
        }

        UserInfoManager.instance.MyName = inputText.text.Trim();

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = UserInfoManager.instance.MyName
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
        UpdateDisplayName(UserInfoManager.instance.MyName);
        Debug.Log($"[playerprefs] Displayname: {UserInfoManager.instance.MyName}�� �����߽��ϴ�");

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
        if (UserInfoManager.instance.MyImageIndex == currentIndex)
        {
            return;
        }

        UserInfoManager.instance.MyImageIndex = currentIndex; // int��

        string ImageIndex = UserInfoManager.instance.MyImageIndex.ToString(); // ���ڿ��� ��ȯ

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
        {
            { UserInfoManager.PROFILE_IMAGE_INDEX_KEY, ImageIndex}
        },
            Permission = UserDataPermission.Public // �����͸� ���� ���·� ����
        };

        // playfab�� ����
        PlayFabClientAPI.UpdateUserData(request,
            result =>
            {
                Debug.Log($"[Playfab] �������̹��� ������ ���� ����: {ImageIndex}");
            },
            error =>
            {
                Debug.LogError($"[Playfab] ���� �̹��� ���� ����: {error.GenerateErrorReport()}");
            });

        // ����� �̹��� �ε����� playerprefs�� ����(���� ������ ��� �����, �ű� ������ ���� �߰��ϴ� ��Ȳ)
        UpdateImageIndex(UserInfoManager.instance.MyImageIndex); 
        Debug.Log($"[playerprefs] Imageindex: {UserInfoManager.instance.MyImageIndex}�� �����߽��ϴ�");
    }

    void UpdateDisplayName(string name) //���ο� �̸� ����
    {
        PlayerPrefs.SetString(UserInfoManager.DISPLAYNAME_KEY, name); // ���ο� �� ����
        PlayerPrefs.Save(); // ���� ����
    }

    void UpdateImageIndex(int newIndex) //���ο� �ε��� ����
    {
        PlayerPrefs.SetInt(UserInfoManager.IMAGEINDEX_KEY, newIndex); // ���ο� �� ����
        PlayerPrefs.Save(); // ���� ����
    }
    
    public void OnLeftButtonClicked() // ���� ��ư Ŭ�� �� ȣ��
    {
        currentIndex = (currentIndex - 1 + UserInfoManager.instance.profileImages.Length) % UserInfoManager.instance.profileImages.Length;
        centralImage.sprite = UserInfoManager.instance.profileImages[currentIndex];  // �ε����� �ش��ϴ� �̹����� ������Ʈ

        if (UserInfoManager.instance.MyImageIndex == currentIndex) //���� ���� �ε��� �̹����� ���� �̹��� �ε����� ���ٸ�
        {
            confirmButton.interactable = false; //�����ư ��Ȱ��ȭ
        }
        else
        {
            confirmButton.interactable = true;
        }
    }

    public void OnRightButtonClicked() // ������ ��ư Ŭ�� �� ȣ��
    {
        currentIndex = (currentIndex + 1) % UserInfoManager.instance.profileImages.Length;
        centralImage.sprite = UserInfoManager.instance.profileImages[currentIndex];  // �ε����� �ش��ϴ� �̹����� ������Ʈ

        if (UserInfoManager.instance.MyImageIndex == currentIndex) //���� ���� �ε��� �̹����� ���� �̹��� �ε����� ���ٸ�
        {
            confirmButton.interactable = false; //�����ư ��Ȱ��ȭ
        }
        else
        {
            confirmButton.interactable = true;
        }
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
        else if (UserInfoManager.instance.MyName == inputname && (inputText.isActiveAndEnabled)) //�Է¶��� ���� �г��Ӱ� �����鼭 Ȱ��ȭ�Ǿ��ִ� ���
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
        Debug.Log("������ ���� ���� ����");

        //���� �̸��� ���濡 ����
        PhotonNetwork.NickName = inputText.text;

        //�κ�����
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby() //Lobby ���Կ� ���������� ȣ��Ǵ� �Լ�
    {
        Debug.Log("�κ� ���� ����");
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


