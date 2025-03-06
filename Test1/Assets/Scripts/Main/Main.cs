using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

// ���ο� �����ϴ� ��ɿ� ���� ��ũ��Ʈ
public class Main : MonoBehaviour
{
    //usersetmanager���� ������ ������    
    private TMP_InputField inputField; //������ �г� ���� �̸��Է��ʵ�
    private TMP_Text SaveText; //������ �г� ���� ����޽���
    private Image ProfileCenImg; //������ �г� �߽� �̹���
    private Button SaveBtn; //������ �г� ���� ���� �� ���� ��ư

    // --------------���ο� ������ ������Ʈ------------------
    public TMP_Text displayNameText, myRankText; // DisplayName, ������ ǥ���� UI �ؽ�Ʈ
    public Image centralImage;  // ���� ������ �̹���
    public GameObject profilePanel; // ������ ���� �г�

    // ---------------��ú��忡 ������ ��ŷ ������Ʈ---------------
    public GameObject[] ranklist; //Ȱ��ȭ/��Ȱ��ȭ�� ���� ������Ʈ
    public Image[] userimage; //���� �̹���
    public TMP_Text[] username; //���� �̸�
    public TMP_Text[] wordcount; //�ܾ�ϼ�Ƚ��

    public Text TestText, LogTestText; //���� �׽�Ʈ �ؽ�Ʈ ����, �α׾ƿ� �ؽ�Ʈ ����

    private void Awake()
    {
        // UserSetManager ������Ʈ ����
        UserSetManager userSetManager = FindObjectOfType<UserSetManager>();

        // UserSetManager���� InputField�� ������
        inputField = userSetManager.inputText;
        SaveText = userSetManager.saveText;
        ProfileCenImg = userSetManager.centralImage; //������ �г� �߽ɻ���
        SaveBtn = userSetManager.confirmButton; //������ �г� �����ư
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
        //UpdateBtn(); //�������� ������Ʈ
    }


    // ������ �̹��� �ε��� �ҷ����� �Լ�
    private void GetProfileImageIndex()
    {
        centralImage.sprite = UserInfoManager.instance.profileImages[UserInfoManager.instance.MyImageIndex]; //���� ���� �̹��� ��ü
        ProfileCenImg.sprite = UserInfoManager.instance.profileImages[UserInfoManager.instance.MyImageIndex]; //������ �г� �߽� �̹��� ��ü

        TestText.text = "�̹��� �ε� �Ϸ�";
    }
    

    // DisplayName �ҷ����� �Լ�
    public void GetUserDisplayName()
    {
        displayNameText.text = UserInfoManager.instance.MyName; //���� �г� �̸�
        inputField.text = UserInfoManager.instance.MyName; //������ �г� �̸�

        TestText.text = "DisplayName �ε� �Ϸ�";
    }

    //������ �г��� �ݱ� ��ư�� ������(�ݱ� ��ư�� �����ص�)
    public void ExitBtn() 
    {
        profilePanel.SetActive(false); //�г� ��Ȱ��ȭ
        inputField.interactable = false; //�̸��Է¶� ��Ȱ��ȭ
        SaveText.text = ""; //���� �޽��� �ʱ�ȭ
        SaveBtn.interactable = false; //�����ư ��Ȱ��ȭ

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

    // �������� ����Ʈ�� �� ���� �ҷ�����(�ε������� ���� ����)
    public void GetLeaderBoard()
    {
        // playfab���� �������� ���� ��û
        var request = new GetLeaderboardRequest 
        { StartPosition = 0, StatisticName = "WordCompletionCount", MaxResultsCount = 10, 
          ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true } };
        PlayFabClientAPI.GetLeaderboard(request, (result) =>
        {
            string myPlayFabId = PlayFabSettings.staticPlayer.PlayFabId; // ���� �α����� ������ ID

            for (int i = 0; i < result.Leaderboard.Count; i++)
            {
                var curBoard = result.Leaderboard[i];

                // ���� �α����� ������ ������ ���� ������
                if (curBoard.PlayFabId == myPlayFabId)
                {
                    int actualRank = curBoard.Position + 1; // 0���� 1���� ��ȯ
                    myRankText.text = $"���� {actualRank}��";
                    Debug.Log($"���� {curBoard.Position + 1}��:");
                }
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
            if (result.Data.ContainsKey(UserInfoManager.instance.PROFILE_IMAGE_INDEX_KEY))
            {
                // ����� �ε��� �� �ҷ�����
                int imgindex;
                if (int.TryParse(result.Data[UserInfoManager.instance.PROFILE_IMAGE_INDEX_KEY].Value, out imgindex))
                {
                    
                    // �ε��� ���� üũ �� ��ŷ ���� �̹��� ������Ʈ
                    if (imgindex >= 0 && imgindex < UserInfoManager.instance.profileImages.Length)
                    {
                        userimage[index].sprite = UserInfoManager.instance.profileImages[imgindex];
                    }
                    else
                    {
                        Debug.LogWarning("��ȿ���� ���� �̹��� �ε����Դϴ�. �⺻ �̹����� �����մϴ�.");
                        userimage[index].color = Color.white;  // �⺻ �̹����� ����
                    }  
                }
                else
                {
                    Debug.LogWarning("�̹��� �ε��� ��ȯ ����. �⺻ �̹����� �����մϴ�.");
                    userimage[index].color = Color.white;  // �⺻ �̹����� ����
                }
            }
            else
            {
                Debug.LogWarning("�̹��� key�� �������� �ʽ��ϴ�.");
            }
        }, error =>
        {
            Debug.LogError($"���� ������ �ҷ����� ����: {error.GenerateErrorReport()}");
        });
    }


}
