using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.SceneManagement;
using System;

// ���ο� �����ϴ� ��ɿ� ���� ��ũ��Ʈ
public class Main : MonoBehaviour
{

    //usersetmanager���� ������ ������    
    private InputField inputField; //������ �г� ���� �̸��Է��ʵ�
    private Text SaveText; //������ �г� ���� ����޽���
    private Sprite[] ProfileImg; //������ �̹��� �迭
    private Image ProfileCenImg; //������ �г� �߽� �̹���

    // --------------���ο� ������ ������Ʈ------------------
    public Text displayNameText; // DisplayName�� ǥ���� UI �ؽ�Ʈ
    public Image centralImage;  // ���� ������ �̹���
    public GameObject profilePanel; // ������ ���� �г�

    // ---------------��ú��忡 ������ ��ŷ ������Ʈ---------------
    public GameObject[] ranklist; //Ȱ��ȭ/��Ȱ��ȭ�� ���� ������Ʈ
    public Image[] userimage; //���� �̹���
    public Text[] username; //���� �̸�
    public Text[] wordcount; //�ܾ�ϼ�Ƚ��

    public Text TestText, LogTestText; //���� �׽�Ʈ �ؽ�Ʈ ����, �α׾ƿ� �ؽ�Ʈ ����
    private const string DISPLAYNAME_KEY = "DisplayName"; // ������ DisplayName
    private const string IMAGEINDEX_KEY = "ImageIndex"; // ������ �̹��� �ε���
    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // ���� Ű

    private void Awake()
    {
        // UserSetManager ������Ʈ ����
        UserSetManager userSetManager = FindObjectOfType<UserSetManager>();

        // UserSetManager���� InputField�� ������
        inputField = userSetManager.inputText;
        SaveText = userSetManager.saveText;
        ProfileImg = userSetManager.profileImages; //������ �̹��� �迭
        ProfileCenImg = userSetManager.centralImage; //������ �г� �߽ɻ���
    }

    void Start()
    {
        if (PhotonNetwork.InLobby)
        {
            Debug.Log("���� �κ� ����.");
        }
        else
        {
            Debug.Log("���� �κ� ����.");
            PhotonNetwork.JoinLobby();  // �κ�� �̵�
        }

        profilePanel.SetActive(false); //������ �г� ��Ȱ��ȭ

        GetProfileImageIndex(); // PlayFab���� ����� �̹��� �ε����� �ҷ��� �̹��� ������Ʈ
        GetUserDisplayName(); //���� ���� �ҷ��ͼ� �ؽ�Ʈ�� ǥ��

        RankActiveFalse(); //���� ������Ʈ ��� ��Ȱ��ȭ
        GetLeaderBoard(); //���� ������Ʈ
    }


    // ������ �̹��� �ε��� �ҷ����� �Լ�
    private void GetProfileImageIndex()
    {
        int imageIndex = PlayerPrefs.GetInt(IMAGEINDEX_KEY, 0);
        centralImage.sprite = ProfileImg[imageIndex]; //���� ���� �̹��� ��ü
        ProfileCenImg.sprite = ProfileImg[imageIndex]; //������ �г� �߽� �̹��� ��ü

        TestText.text = "�̹��� �ε� �Ϸ�";
    }
    

    // DisplayName �ҷ����� �Լ�
    public void GetUserDisplayName()
    {
        string displayName = PlayerPrefs.GetString(DISPLAYNAME_KEY, "Guest");
        displayNameText.text = displayName; //���� �г� �̸�
        inputField.text = displayName; //������ �г� �̸�

        TestText.text = "DisplayName �ε� �Ϸ�";
    }

    //������ �г��� �ݱ� ��ư�� ������(�ݱ� ��ư�� �����ص�)
    public void ExitBtn() 
    {
        profilePanel.SetActive(false); //�г� ��Ȱ��ȭ
        inputField.interactable = false; //�̸��Է¶� ��Ȱ��ȭ
        SaveText.text = ""; //���� �޽��� �ʱ�ȭ

        //���� ������ �̹��� ��ε�, �̸� ��ε� �ؽ�Ʈ �����ֱ�
        GetUserDisplayName();
        GetProfileImageIndex();

    }

    // �α׾ƿ� ��ư�� ������->playfab ������ �α׾ƿ�
    public void LogoutBtn()
    {
        // �������� ���ᵵ ����
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect(); // ���� ���� ����
        }

        // �ȵ���̵� ��� ID�� ���� ����
        PlayFabClientAPI.UnlinkAndroidDeviceID(new UnlinkAndroidDeviceIDRequest(), result =>
        {
            Debug.Log("��� ID ���� ���� ����");

            // PlayFab ���� ���� �ʱ�ȭ
            PlayFabClientAPI.ForgetAllCredentials();
            LogTestText.text = "��� �� playfab ����";

            //�ε��� ui �ִϸ��̼� �����ֱ�(Login������ �̵�)
            LoadingSceneController.Instance.LoadScene("Login");
        },
        error =>
        {
            Debug.LogError("��� ID ���� ���� ����: " + error.GenerateErrorReport());
            LogTestText.text = "��� ID ���� ���� ����";
        });

        

        //Debug.Log("�α׾ƿ��Ǿ����ϴ�. ���� ������ �ʱ�ȭ�Ǿ����ϴ�.");
    }

    // ���� ���� ��ư�� ������
    public void ExitGame()
    {
        Debug.Log("���� ����"); // Unity �����Ϳ��� ����� �޽��� Ȯ��
        Application.Quit(); // ������ ���� ����

        //Debug.Log("���� ����");
        //#if UNITY_EDITOR
        //        UnityEditor.EditorApplication.isPlaying = false; // ������ ����
        //#else
        //    Application.Quit(); // ����� ���� ����
        //#endif
    }

    //��� ����������Ʈ ��Ȱ��ȭ ��Ű��
    private void RankActiveFalse()
    {
        for (int i = 0; i < ranklist.Length; i++)
        {
            ranklist[i].SetActive(false);
        }
    }

    //������ �������� ������Ʈ ��ư�� ������
    public void UpdateBtn()
    {
        //����������Ʈ ��Ȱ��ȭ
        RankActiveFalse();
        //������ ���� ������ �簻��
        GetLeaderBoard();

    }

    // �������� ����Ʈ�� �� ���� �ҷ�����
    public void GetLeaderBoard()
    {
        // playfab���� �������� ���� ��û
        var request = new GetLeaderboardRequest 
        { StartPosition = 0, StatisticName = "WordCompletionCount", MaxResultsCount = 10, 
          ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true } };
        PlayFabClientAPI.GetLeaderboard(request, (result) =>
        {
            for (int i = 0; i < result.Leaderboard.Count; i++)
            {
                var curBoard = result.Leaderboard[i];
                //������, ������ ���� ������Ʈ Ȱ��ȭ
                ranklist[i].SetActive(true);
                //���� �ܾ�ϼ�Ƚ�� ������Ʈ
                wordcount[i].text = "�� " + curBoard.StatValue.ToString() + "ȸ";
                //���� �̸� ������Ʈ
                username[i].text = curBoard.DisplayName;
                //���� �̹��� �ε����� ��û �� ������Ʈ
                GetUserImageData(curBoard.PlayFabId, i);
            }
        },
        (Error) => print("�������� �ҷ����� ����"));
    }

    // Ư�� ������ ���� ������ ��û �Լ�(playfab���κ���)
    private void GetUserImageData(string playFabId, int index)
    {
        var userDataRequest = new GetUserDataRequest
        {
            PlayFabId = playFabId // �����͸� ������ ������ PlayFabId
        };
        PlayFabClientAPI.GetUserData(userDataRequest, result =>
        {
            // PROFILE_IMAGE_INDEX_KEY�� �����ϴ��� Ȯ��
            if (result.Data.ContainsKey(PROFILE_IMAGE_INDEX_KEY))
            {
                // ����� �ε��� �� �ҷ�����
                int imgindex = int.Parse(result.Data[PROFILE_IMAGE_INDEX_KEY].Value);
                // �ε��� ���� üũ �� ��ŷ ���� �̹��� ������Ʈ
                userimage[index].sprite = ProfileImg[imgindex];
            }
            else
            {
                Debug.LogWarning("PROFILE_IMAGE_INDEX_KEY�� �������� �ʽ��ϴ�. �⺻ �̹����� �����մϴ�.");
                userimage[index].sprite = ProfileImg[0];  // �⺻ �̹����� ����
            }
        }, error =>
        {
            Debug.LogError($"���� ������ �ҷ����� ����: {error.GenerateErrorReport()}");
        });
    }

}
