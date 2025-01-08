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


// 유저 프로필 설정 화면에서 작동하는 코드(닉네임, 유저프로필사진 변경 저장)
public class UserSetManager : MonoBehaviourPunCallbacks
{
    private UserSetManager s_instance;
    public UserSetManager Instance { get { return s_instance; } }

    [SerializeField] InputField inputText; //닉네임 입력(나중에 바꿀 수 있는 Displayname)
    [SerializeField] Button confirmButton; //제출(저장) 버튼
    [SerializeField] Text warningText; // 경고 메시지를 출력할 UI 텍스트
    [SerializeField] Text saveText; // 저장완료 메시지를 출력할 UI 텍스트

    [SerializeField] private RawImage rawImage;  // 선택된 이미지를 표시할 UI
    [SerializeField] private GameObject imagePrefab; // 프로필사진 프리팹 (UI > Image 형태)
    [SerializeField] private Transform canvasTransform; // Canvas의 Transform

    private const int MaxLength = 8; // 최대 입력 길이(변동가능)

    void Awake()
    {
        // 싱글톤 인스턴스를 설정
        if (s_instance == null)
        {
            s_instance = this;
        }
        else if (s_instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); // 씬이 변경되더라도 객체가 파괴되지 않도록 설정
    }

    void Start()
    {
        confirmButton.interactable = false; // 기본적으로 버튼 비활성화
        warningText.text = ""; // 초기 경고 메시지 비우기
        saveText.text = ""; // 초기 저장 메시지 비우기

        //내용이 변경되었을때 규칙 검사
        inputText.onValueChanged.AddListener(ValidateNickname);

        //확인 버튼 누르면 이름 저장
        confirmButton.onClick.AddListener(DisplayName);

    }

    //이름 변경 버튼 클릭할 때 -> 이름 입력 인풋 활성화
    public void ChangeNameBtn() 
    {
        inputText.interactable = true; //이름 변경 활성화
    }
     
    //이름 설정 규칙
    public void ValidateNickname(string input)
    {
        /// 한글(완성형/자음/모음)과 숫자만 허용하는 정규식
        string validPattern = @"^[가-힣ㄱ-ㅎㅏ-ㅣ0-9]*$";

        // 공백 제거
        input = input.Replace(" ", ""); //공백을 허용하지 않는다

        // 입력 값이 패턴에 맞지 않으면 수정
        if (!Regex.IsMatch(input, validPattern))
        {
            warningText.text = "한글과 숫자만 입력 가능합니다.";
            confirmButton.interactable = false; 
        }
        else if (input.Length > MaxLength) // 길이 제한 초과 검사
        {
            warningText.text = $"최대 {MaxLength}자까지만 입력 가능합니다.";
            confirmButton.interactable = false; 
        }
        else if (input.Length == 0) // 빈 문자열 검사
        {
            warningText.text = "닉네임을 입력해주세요.";
            confirmButton.interactable = false; 
        }
        else
        {
            warningText.text = ""; // 규칙에 맞으면 경고 메시지 제거
            confirmButton.interactable = true; // 확인 버튼 활성화
        }

    }

    //설정한 이름 저장
    public void DisplayName() //DisplayName: 고유하지 않음
    {
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request,
             result =>
             {
                 // displayName이 존재하는지 확인(첫 유저인지 아닌지를 확인)
                 if (string.IsNullOrEmpty(result.AccountInfo.TitleInfo.DisplayName))
                 {
                     // displayName이 없을 경우 (첫 유저일 경우)
                     Debug.Log("displayName이 설정되지 않았습니다. 설정 중...");
                     SaveDisplayName(); //닉네임을 저장하고
                     OnClickConnect(); //메인으로 서버접속을 요청한다
                 }
                 else //displayname이 이미 존재할 경우(재접속 유저일 경우)
                 {
                     SaveDisplayName(); //기존의 이름을 새 이름으로 덮어씌워 저장한다
                     saveText.text = "저장되었습니다"; //저장 메시지 알림
                 }
             },
            error =>
            {
                Debug.LogError($"유저 정보 불러오기 실패: {error.GenerateErrorReport()}");
            });
    }

    public void SaveDisplayName() //단순히 이름 저장하기
    {
        string displayName = inputText.text.Trim();

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
           result =>
           {
               Debug.Log($"닉네임 저장 성공: {result.DisplayName}");
           },

           error =>
           {
               Debug.LogError($"닉네임 저장 실패: {error.GenerateErrorReport()}");
           });
    }

    // 갤러리에서 이미지 선택
    public void OpenGallery()
    {
        if (NativeGallery.IsMediaPickerBusy())
            return;

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                StartCoroutine(LoadImage(path)); // 이미지를 로드하여 표시
            }
        });

        Debug.Log("Gallery permission: " + permission);
    }

    // 이미지 로드 및 적용
    private IEnumerator LoadImage(string path)
    {
        // 갤러리에서 선택한 이미지를 Texture2D로 로드
        Texture2D texture = NativeGallery.LoadImageAtPath(path);
        if (texture == null)
        {
            Debug.LogError("Failed to load image.");
            yield break;
        }

        // 선택된 이미지를 RawImage에 적용 (Texture2D)
        if (rawImage != null)
        {
            rawImage.texture = texture;

            // RawImage의 RectTransform을 사용하여 1:1 비율로 유지
            float aspectRatio = (float)texture.width / texture.height;
            if (aspectRatio > 1)
            {
                rawImage.rectTransform.sizeDelta = new Vector2(texture.width, texture.width); // 너비 기준으로 크기 설정
            }
            else
            {
                rawImage.rectTransform.sizeDelta = new Vector2(texture.height, texture.height); // 높이 기준으로 크기 설정
            }
        }

        // 이미지 프리팹에 적용
        if (imagePrefab != null)
        {
            GameObject newImageObject = Instantiate(imagePrefab, canvasTransform);
            Image imageComponent = newImageObject.GetComponent<Image>();
            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            imageComponent.sprite = newSprite;

            // 이미지 비율 유지
            imageComponent.preserveAspect = true; // 비율을 유지하도록 설정

            // 프리팹의 크기 설정 (이미지 비율에 맞게)
            float imageAspectRatio = (float)texture.width / texture.height;
            if (imageAspectRatio > 1)
            {
                newImageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.width, texture.width); // 너비 기준
            }
            else
            {
                newImageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.height, texture.height); // 높이 기준
            }
        }

        Debug.Log("Image successfully applied!");
    }




    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("마스터 서버 접속 성공");

        //나의 이름을 포톤에 설정
        PhotonNetwork.NickName = inputText.text;

        //로비진입
        PhotonNetwork.JoinLobby();
    }

    //Lobby 진입에 성공했으면 호출되는 함수
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        //메인 씬으로 이동
        PhotonNetwork.LoadLevel("Main");

        print("로비 진입 성공");

    }
    public void OnClickConnect()
    {
        // 마스터 서버 접속 요청
        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnDestroy()
    {
        // 이벤트 해제
        inputText.onValueChanged.RemoveListener(ValidateNickname);
    }

    
}
