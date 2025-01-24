using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using static UnityEngine.EventSystems.EventTrigger;

//using UnityEditor.PackageManager;
//using UnityEditor.PackageManager.Requests;

using UnityEngine.SceneManagement;

// ���ο� �����ϴ� ��ɿ� ���� ��ũ��Ʈ
public class Main : MonoBehaviour
{

    //usersestmanager���� ������ ������    
    private InputField inputField; //������ �г� ���� �̸��Է��ʵ�
    private Text SaveText; //������ �г� ���� ����޽���
    private Sprite[] ProfileImg; //������ �̹��� �迭

    // --------------���ο� ������ ������Ʈ------------------
    public Text displayNameText; // DisplayName�� ǥ���� UI �ؽ�Ʈ
    public InputField profileInputField; //������ ������ �̸� �Է¶�
    public Image centralImage;  // ���� ������ �̹���
    public GameObject profilePanel; // ������ ���� �г�

    // ---------------��ú��忡 ������ ��ŷ ������Ʈ---------------
    public GameObject[] ranklist; //Ȱ��ȭ/��Ȱ��ȭ�� ���� ������Ʈ
    public Image[] userimage; //���� �̹���
    public Text[] username; //���� �̸�
    public Text[] wordcount; //�ܾ�ϼ�Ƚ��


    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // ���� Ű

    private void Awake()
    {
        // PlayFab���� ����� �̹��� �ε����� �ҷ��� �̹��� ������Ʈ
        LoadProfileImageIndex();

        //���� ���� �ҷ��ͼ� �ؽ�Ʈ�� ǥ��
        GetUserDisplayName();

        RankActiveFalse(); //���� ������Ʈ ��� ��Ȱ��ȭ
        GetLeaderBoard(); //���� ������Ʈ
    }

    void Start()
    {
        // UserSetManager ������Ʈ ����
        UserSetManager userSetManager = FindObjectOfType<UserSetManager>();

        // UserSetManager���� InputField�� ������
        inputField = userSetManager.inputText;
        SaveText = userSetManager.saveText;
        ProfileImg = userSetManager.profileImages; //������ �̹��� �迭�� ������


        profilePanel.SetActive(false);
        profileInputField.interactable = false; //������ �̸� �ʱ� ��Ȱ��ȭ
  
    }


    // ������ �̹��� �ε��� �ҷ����� �Լ�
    // PlayFab���� ����� �̹��� �ε����� �ҷ����� �Լ�
    // -> Ŀ����������Ƽ�� ����� ���� �ҷ�����
    private void LoadProfileImageIndex()
    {
        var request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request, result =>
        {
            // PROFILE_IMAGE_INDEX_KEY�� �����ϴ��� Ȯ��
            if (result.Data.ContainsKey(PROFILE_IMAGE_INDEX_KEY))
            {
                // ����� �ε��� �� �ҷ�����
                int index = int.Parse(result.Data[PROFILE_IMAGE_INDEX_KEY].Value);
                // �ε��� ���� üũ �� �̹��� ������Ʈ
                centralImage.sprite = ProfileImg[index];
            }
            else
            {
                Debug.LogWarning("PROFILE_IMAGE_INDEX_KEY�� �������� �ʽ��ϴ�. �⺻ �̹����� �����մϴ�.");
                centralImage.sprite = ProfileImg[0]; ;  // �⺻ �̹����� ����
            }
        }, error =>
        {
            Debug.LogError($"���� ������ �ҷ����� ����: {error.GenerateErrorReport()}");
        });
    }
    

    // �̸� �ҷ�����
    // DisplayName �ҷ����� �Լ�
    public void GetUserDisplayName()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountInfoSuccess, OnGetAccountInfoFailure);
    }

    //���������� DisplayName�� ������ ���
    private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        string displayName = result.AccountInfo.TitleInfo.DisplayName;
        profileInputField.text = displayName;

        if (!string.IsNullOrEmpty(displayName))
        {
            Debug.Log($"������ DisplayName: {displayName}");
            displayNameText.text = $"{displayName}";
        }
        else
        {
            Debug.Log("DisplayName�� �������� �ʾҽ��ϴ�.");
            displayNameText.text = "�̸�����";
        }
    }

    // DisplayName �������⿡ ������ ���
    private void OnGetAccountInfoFailure(PlayFabError error)
    {
        Debug.LogError($"DisplayName �������� ����: {error.GenerateErrorReport()}");
    }


    public void ExitBtn() //������ �г��� �ݱ� ��ư�� ������
    {
        profilePanel.SetActive(false); //�г� ��Ȱ��ȭ
        inputField.interactable = false; //�̸��Է¶� ��Ȱ��ȭ
        SaveText.text = ""; //���� �޽��� �ʱ�ȭ

        //���� ������ �̹��� ��ε�, �̸� ��ε� �ؽ�Ʈ �����ֱ�
        GetUserDisplayName();
        LoadProfileImageIndex();

    }


    // �α׾ƿ� ��ư�� ������
    public void LogoutBtn()
    {
        // PlayFab ���� ���� �ʱ�ȭ
        PlayFabClientAPI.ForgetAllCredentials();

        // �������� ���ᵵ ����
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect(); // ���� ���� ����
        }

        // �α��� ȭ������ �̵�
        SceneManager.LoadScene("Login");
        Debug.Log("�α׾ƿ��Ǿ����ϴ�. ���� ������ �ʱ�ȭ�Ǿ����ϴ�.");
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

    private void RankActiveFalse()
    {
        //��� ����������Ʈ ��Ȱ��ȭ ��Ű��
        for (int i = 0; i < ranklist.Length; i++)
        {
            ranklist[i].SetActive(false);
        }
    }

    //������ ������Ʈ ��ư�� ������
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

    // Ư�� ������ ���� ������ ��û �Լ�
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
