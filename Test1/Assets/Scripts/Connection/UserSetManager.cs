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
    
    private const int MaxLength = 8; // 최대 입력 길이(변동가능)

    public Image centralImage; // 중앙에 표시되는 프로필이미지
    public Sprite[] profileImages; // 3가지 기본 제공 이미지
    private int currentIndex = 0; // 현재 선택된 이미지 인덱스


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
        confirmButton.onClick.AddListener(SaveProfileImageToPlayFab);

        LoadSavedImage();  // 게임 시작 시 저장된 이미지 인덱스를 불러오기

    }


    public void SaveProfileImageToPlayFab()
    {
        // PlayerPrefs에서 저장된 이미지 인덱스를 가져오기
        if (PlayerPrefs.HasKey("ProfileImageIndex"))
        {
            int profileImageIndex = PlayerPrefs.GetInt("ProfileImageIndex");

            // PlayFab에 저장할 데이터를 준비
            var request = new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
            {
                { "ProfileImageIndex", profileImageIndex.ToString() }
            }
            };

            // PlayFab에 데이터 업데이트 요청
            PlayFabClientAPI.UpdateUserData(request,
                result => {
                    Debug.Log("Successfully saved ProfileImageIndex to PlayFab.");
                },
                error => {
                    Debug.LogError("Error saving ProfileImageIndex to PlayFab: " + error.GenerateErrorReport());
                });
        }
        else
        {
            Debug.LogWarning("ProfileImageIndex not found in PlayerPrefs.");
        }
    }

    // 좌측 화살표 클릭 시
    public void OnLeftArrowClicked()
    {
        currentIndex = (currentIndex - 1 + profileImages.Length) % profileImages.Length;
        UpdateCentralImage();
    }

    // 우측 화살표 클릭 시
    public void OnRightArrowClicked()
    {
        currentIndex = (currentIndex + 1) % profileImages.Length;
        UpdateCentralImage();
    }

    // 중앙 이미지를 업데이트
    private void UpdateCentralImage()
    {
        if (centralImage == null)
        {
            Debug.LogError("centralImage is not initialized!");
            return;
        }

        if (profileImages == null || profileImages.Length == 0)
        {
            Debug.LogError("profileImages array is not initialized or is empty!");
            return;
        }

        // currentIndex가 profileImages 배열의 유효한 범위 내에 있는지 확인
        if (currentIndex < 0 || currentIndex >= profileImages.Length)
        {
            Debug.LogError("currentIndex is out of range!");
            return;
        }

        // 이미지 업데이트
        centralImage.sprite = profileImages[currentIndex];

        // 선택한 이미지 인덱스를 저장
        SaveSelectedImageIndex(currentIndex);
    }

    // 이미지 인덱스를 PlayerPrefs에 저장
    private void SaveSelectedImageIndex(int index)
    {
        PlayerPrefs.SetInt("ProfileImageIndex", index);
        PlayerPrefs.Save();
    }

    // 시작 시 저장된 이미지 인덱스를 불러오기
    public void LoadSavedImage()
    {
        if (PlayerPrefs.HasKey("ProfileImageIndex"))
        {
            currentIndex = PlayerPrefs.GetInt("ProfileImageIndex");
            UpdateCentralImage();
        }
        else
        {
            // 기본 이미지 설정 (첫 시작 시 인덱스 0, 한번 변경했다면 변경한 이미지에서 시작)
            UpdateCentralImage();
        }
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


