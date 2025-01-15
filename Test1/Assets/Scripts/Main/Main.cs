using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using System;

// ���ο� �����ϴ� ��ɿ� ���� ��ũ��Ʈ
public class Main : MonoBehaviour
{
    private InputField inputField; //������ �г� ���� �̸��Է��ʵ�
    private Text SaveText; //������ �г� ���� ����޽���

    public Text displayNameText; // DisplayName�� ǥ���� UI �ؽ�Ʈ
    public InputField profileInputField; //������ ������ �̸� �Է¶�
    public Image centralImage;  // ���� ������ �̹���

    public Sprite[] profileImages; // 3���� �⺻ ���� �̹���
    public GameObject profilePanel; // ������ ���� �г�

    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // ���� Ű

    private void Awake()
    {
        // PlayFab���� ����� �̹��� �ε����� �ҷ��� �̹��� ������Ʈ
        LoadProfileImageIndex();

        //���� ���� �ҷ��ͼ� �ؽ�Ʈ�� ǥ��
        GetUserDisplayName();


    }

    void Start()
    {
        // UserSetManager ������Ʈ ����
        UserSetManager userSetManager = FindObjectOfType<UserSetManager>();

        // UserSetManager���� InputField�� ������
        inputField = userSetManager.inputText;
        SaveText = userSetManager.saveText;
    
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
                centralImage.sprite = profileImages[index];
            }
            else
            {
                Debug.LogWarning("PROFILE_IMAGE_INDEX_KEY�� �������� �ʽ��ϴ�. �⺻ �̹����� �����մϴ�.");
                centralImage.sprite = profileImages[0]; ;  // �⺻ �̹����� ����
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

}
