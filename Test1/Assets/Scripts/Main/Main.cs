using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

// ���ο� �����ϴ� ��ɿ� ���� ��ũ��Ʈ
public class Main : MonoBehaviour
{
    private InputField inputField; //������ �г� ���� �̸��Է��ʵ�
    private Text SaveText; //������ �г� ���� ����޽���

    public Text displayNameText; // DisplayName�� ǥ���� UI �ؽ�Ʈ
    public InputField profileInputField; //������ ������ �̸� �Է¶�

    public Sprite[] profileImages; // 3���� �⺻ ���� �̹���
    public GameObject profilePanel; // ������ ���� �г�
    public Image centralImage;  // ������ �̹���

    //public GameObject friendPanel; // ģ�� �߰� �г�

    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // ���� Ű

    void Start()
    {
        // UserSetManager ������Ʈ ����
        UserSetManager userSetManager = FindObjectOfType<UserSetManager>();
        // UserSetManager���� InputField�� ������
        inputField = userSetManager.inputText;
        SaveText = userSetManager.saveText;

        // PlayFab���� ����� �̹��� �ε����� �ҷ��� �̹��� ������Ʈ
        LoadProfileImageIndex();

        GetUserDisplayName(); //���� ���� �ҷ��ͼ� �ؽ�Ʈ�� ǥ��

        profilePanel.SetActive(false);

        profileInputField.interactable = false; //������ �̸� �ʱ� ��Ȱ��ȭ
    }


    // ������ �̹��� �ε��� �ҷ����� �Լ�
    // PlayFab���� ����� �̹��� �ε����� �ҷ����� �Լ�
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
                UpdateProfileImage(index);
            }
            else
            {
                Debug.LogWarning("PROFILE_IMAGE_INDEX_KEY�� �������� �ʽ��ϴ�. �⺻ �̹����� �����մϴ�.");
                UpdateProfileImage(0);  // �⺻ �̹����� ����
            }
        }, error =>
        {
            Debug.LogError($"���� ������ �ҷ����� ����: {error.GenerateErrorReport()}");
        });
    }

    // �̹��� ������Ʈ �Լ�
    private void UpdateProfileImage(int index)
    {
        // �ε����� �ش��ϴ� �̹����� �߾� �̹��� ������Ʈ
        centralImage.sprite = profileImages[index];
    }

    // �̸� �ҷ�����
    // DisplayName �ҷ����� �Լ�
    public void GetUserDisplayName()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountInfoSuccess, OnGetAccountInfoFailure);
    }

    // ���������� DisplayName�� ������ ���
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
            displayNameText.text = "Welcome, Player!";
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

}
