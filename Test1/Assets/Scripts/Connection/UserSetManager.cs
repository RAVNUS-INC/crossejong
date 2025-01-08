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

    [SerializeField] private RawImage rawImage;  // ���õ� �̹����� ǥ���� UI
    [SerializeField] private GameObject imagePrefab; // �����ʻ��� ������ (UI > Image ����)
    [SerializeField] private Transform canvasTransform; // Canvas�� Transform

    private const int MaxLength = 8; // �ִ� �Է� ����(��������)

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

    // ���������� �̹��� ����
    public void OpenGallery()
    {
        if (NativeGallery.IsMediaPickerBusy())
            return;

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                StartCoroutine(LoadImage(path)); // �̹����� �ε��Ͽ� ǥ��
            }
        });

        Debug.Log("Gallery permission: " + permission);
    }

    // �̹��� �ε� �� ����
    private IEnumerator LoadImage(string path)
    {
        // ���������� ������ �̹����� Texture2D�� �ε�
        Texture2D texture = NativeGallery.LoadImageAtPath(path);
        if (texture == null)
        {
            Debug.LogError("Failed to load image.");
            yield break;
        }

        // ���õ� �̹����� RawImage�� ���� (Texture2D)
        if (rawImage != null)
        {
            rawImage.texture = texture;

            // RawImage�� RectTransform�� ����Ͽ� 1:1 ������ ����
            float aspectRatio = (float)texture.width / texture.height;
            if (aspectRatio > 1)
            {
                rawImage.rectTransform.sizeDelta = new Vector2(texture.width, texture.width); // �ʺ� �������� ũ�� ����
            }
            else
            {
                rawImage.rectTransform.sizeDelta = new Vector2(texture.height, texture.height); // ���� �������� ũ�� ����
            }
        }

        // �̹��� �����տ� ����
        if (imagePrefab != null)
        {
            GameObject newImageObject = Instantiate(imagePrefab, canvasTransform);
            Image imageComponent = newImageObject.GetComponent<Image>();
            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            imageComponent.sprite = newSprite;

            // �̹��� ���� ����
            imageComponent.preserveAspect = true; // ������ �����ϵ��� ����

            // �������� ũ�� ���� (�̹��� ������ �°�)
            float imageAspectRatio = (float)texture.width / texture.height;
            if (imageAspectRatio > 1)
            {
                newImageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.width, texture.width); // �ʺ� ����
            }
            else
            {
                newImageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.height, texture.height); // ���� ����
            }
        }

        Debug.Log("Image successfully applied!");
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
