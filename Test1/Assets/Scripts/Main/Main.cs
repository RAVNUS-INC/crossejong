using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

public class Main : MonoBehaviour
{
    public MainSettingsPopup mainSettingsPopup; //MainSettingsPopup ��ũ��Ʈ ����
    public MakeRoomPopup makeRoomPopup; //MakeRoomPopup ��ũ��Ʈ ����
    public Text displayNameText; // DisplayName�� ǥ���� UI �ؽ�Ʈ

    public GameObject profilePanel; // ������ �г�
    public InputField profileInputField; //������ �̸� �Է¶�
    [SerializeField] private Image centralImage;  // �̹��� ������Ʈ
    [SerializeField] private Sprite[] profileImages;  // �̹��� �迭


    void Start()
    {
        // ����� ������ �̹��� �ε����� �ҷ��ɴϴ�.
        int savedIndex = PlayerPrefs.GetInt("ProfileImageIndex", 0);  // �⺻�� 0 (ù ��° �̹���)

        // �ε����� ��ȿ���� Ȯ���ϰ� �̹��� ������Ʈ
        if (savedIndex >= 0 && savedIndex < profileImages.Length)
        {
            centralImage.sprite = profileImages[savedIndex];  // �ش� �ε����� �´� �̹����� ����
        }
        else
        {
            Debug.LogError("Invalid ProfileImageIndex.");
        }

        GetUserDisplayName(); //���� ���� �ҷ��ͼ� �ؽ�Ʈ�� ǥ��

        profilePanel.SetActive(false);
        profileInputField.interactable = false; //������ �̸� �ʱ� ��Ȱ��ȭ

        mainSettingsPopup.MainSettingsPopupf();
        makeRoomPopup.MakeRoomPopupf();
    }


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

    public void ProfileBtn() //������ ��ư Ŭ���ϸ� 
    {
        profilePanel.SetActive(true); //������ �г� ����

    }

    

}
