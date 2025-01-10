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
    public static UserSetManager Instance { get; private set; }

    [SerializeField] public InputField inputText; //닉네임 입력(나중에 바꿀 수 있는 Displayname)
    [SerializeField] Button confirmButton; //제출(저장) 버튼
    [SerializeField] Text warningText; // 경고 메시지를 출력할 UI 텍스트
    [SerializeField] public Text saveText; // 저장완료 메시지를 출력할 UI 텍스트
    
    private const int MaxLength = 8; // 최대 입력 길이(변동가능)

    public Image centralImage; // 중앙에 표시되는 프로필이미지
    public Sprite[] profileImages; // 3가지 기본 제공 이미지
    private int currentIndex = 0; // 현재 선택된 이미지 인덱스
    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // 저장 키
    private string displayName; //디스플레이 이름(임시저장을 위한 변수)

    public GameObject profilePanel; //프로필 설정 패널(메인패널에서 미리 준비해야 작동)
    public GameObject usersetPanel; //유저 초기 설정 패널

    void Start()
    {
        if ((usersetPanel.activeSelf || profilePanel.activeSelf))
        {
            DisplayName(); // 기존 이름 정보 불러와 변수에 저장
            CheckAndSaveDefaultImageIndex(); // 게임 시작 시 저장된 이미지 인덱스를 불러와 변수에 저장 및 이미지 업데이트
        }

        confirmButton.interactable = false; // 기본적으로 버튼 비활성화
        warningText.text = ""; // 초기 경고 메시지 비우기
        saveText.text = ""; // 초기 저장 메시지 비우기

        //내용이 변경되었을때 규칙 검사
        inputText.onValueChanged.AddListener(ValidateNickname);

        //확인 버튼 누르면 이름 저장
        confirmButton.onClick.AddListener(OnClickSaveDisplayName);
    }

    // 이름 닉네임 관련 함수들
    // 설정한 이름 저장 함수
    public void DisplayName() //DisplayName: 고유하지 않음
    {
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request,
             result =>
             {
                 // displayName이 없는 경우 null을 반환
                 if (string.IsNullOrEmpty(result.AccountInfo.TitleInfo.DisplayName))
                 {
                     displayName = null; // displayName이 없으면 null 할당
                     Debug.Log("displayName이 없습니다.");
                 }
                 else
                 {
                     // displayName 값을 전역 변수에 저장
                     displayName = result.AccountInfo.TitleInfo.DisplayName;
                     Debug.Log($"불러온 displayName: {displayName}");
                 }
             },
            error =>
            {
                Debug.LogError($"유저 정보 불러오기 실패: {error.GenerateErrorReport()}");
            });
    }


    // 유저 프로필 이미지 관련 함수들
    // 저장된 이미지 인덱스를 불러오기

    private void CheckAndSaveDefaultImageIndex()
    {
        var request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request,
            result =>
            {
                // PROFILE_IMAGE_INDEX_KEY가 존재하는지 확인
                if (result.Data.ContainsKey(PROFILE_IMAGE_INDEX_KEY))
                {
                    Debug.Log("KEY가 존재합니다. 값을 불러옵니다.");
                    string value = result.Data[PROFILE_IMAGE_INDEX_KEY].Value;

                    if (!string.IsNullOrEmpty(value)) 
                    {
                        currentIndex = int.Parse(value); 
                    }
                    else 
                    {
                        Debug.LogWarning("KEY의 값이 비어 있습니다. 기본값(0)으로 저장합니다.");
                        currentIndex = 0; // 기본값 설정
                        SaveSelectedImageIndex(currentIndex); // PlayFab에 저장
                    }
                }
                else
                {
                    Debug.LogWarning("KEY가 존재하지 않습니다. 기본값(0)을 생성합니다.");
                    currentIndex = 0; // 기본값 설정
                    SaveSelectedImageIndex(currentIndex); // PlayFab에 새로운 키 생성 및 값 저장
                }

                UpdateCentralImage(); // 이미지 업데이트
            },
            error =>
            {
                Debug.LogError($"유저 데이터 불러오기 실패: {error.GenerateErrorReport()}");
            });
    }


    // 이미지 업데이트 함수
    private void UpdateCentralImage()
    {
        if (profileImages.Length > 0 && currentIndex >= 0 && currentIndex < profileImages.Length)
        {
            centralImage.sprite = profileImages[currentIndex];  // 인덱스에 해당하는 이미지로 업데이트
            SaveSelectedImageIndex(currentIndex);                // 선택된 이미지 인덱스 저장
        }
        else
        {
            Debug.LogWarning("Invalid profile image index.");
        }
    }

    // 선택된 이미지 인덱스를 저장
    private void SaveSelectedImageIndex(int index)
    {
        // 선택된 이미지 인덱스를 PlayFab 타이틀 데이터에 저장
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
        {
            { PROFILE_IMAGE_INDEX_KEY, index.ToString() }
        }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result =>
            {
                Debug.Log($"프로필 데이터 저장 성공: {index}");
            },
            error =>
            {
                Debug.LogError($"유저 데이터 저장 실패: {error.GenerateErrorReport()}");
            });
    }




    // 왼쪽 버튼 클릭 시 호출
    public void OnLeftButtonClicked()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = profileImages.Length - 1;  // 순환 (맨 처음으로 돌아감)
        UpdateCentralImage();  // 이미지 업데이트
    }

    // 오른쪽 버튼 클릭 시 호출
    public void OnRightButtonClicked()
    {
        currentIndex++;
        if (currentIndex >= profileImages.Length) currentIndex = 0;  // 순환 (맨 마지막에서 처음으로 돌아감)
        UpdateCentralImage();  // 이미지 업데이트
    }



    public void OnClickSaveDisplayName()
    {
        // displayName이 존재하는지 확인(첫 유저인지 아닌지를 확인)
        if (string.IsNullOrEmpty(displayName))
        {
            // displayName이 없을 경우 (첫 유저일 경우)
            Debug.Log("첫 displayName이 설정되었습니다.");
            SaveDisplayName(); //닉네임을 저장하고
            OnClickConnect(); //메인으로 서버접속을 요청한다
        }
        else //displayname이 이미 존재할 경우(재접속 유저일 경우)
        {
            // 전역 변수로 displayName에 저장
            Debug.Log("새로운 displayName이 설정되었습니다.");
            SaveDisplayName(); //기존의 이름을 새 이름으로 덮어씌워 저장한다
            saveText.text = "저장되었습니다"; //저장 메시지 알림
        }
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
        string inputname = input.Replace(" ", ""); //공백을 허용하지 않는다

        // 입력 값이 패턴에 맞지 않으면 수정
        if (!Regex.IsMatch(inputname, validPattern))
        {
            warningText.text = "한글과 숫자만 입력 가능합니다.";
            confirmButton.interactable = false; 
        }
        // 길이 제한 초과 검사
        else if (GetKoreanCharCount(inputname) > MaxLength) // 한글 자음, 모음을 포함한 최대 길이 검사
        {
            warningText.text = $"최대 {MaxLength}자까지만 입력 가능합니다.";
            confirmButton.interactable = false; 
        }
        else if (inputname.Length == 0) // 빈 문자열 검사
        {
            warningText.text = "닉네임을 입력해주세요.";
            confirmButton.interactable = false; 
        }
        else if (displayName == inputname && (inputText.isActiveAndEnabled)) //입력란이 기존 닉네임과 같으면서 활성화되어있는 경우
        {
            warningText.text = "기존 닉네임과 달라야 합니다.";
            confirmButton.interactable = false;
        }
        else
        {
            warningText.text = ""; // 규칙에 맞으면 경고 메시지 제거
            confirmButton.interactable = true; // 확인 버튼 활성화
        }
        // 입력란에 공백을 제거한 값 반영
        inputText.text = inputname;
    }


    // 한글 음절 자음, 모음을 포함하여 글자 수를 계산하는 함수
    private int GetKoreanCharCount(string input)
    {
        int count = 0;
        foreach (char c in input)
        {
            // 한글 음절인지 체크 (가-힣 범위)
            if (c >= '가' && c <= '힣')
            {
                count++;
            }
            // 한글 자음/모음인지 체크 (ㄱ-ㅎ, ㅏ-ㅣ 범위)
            else if ((c >= 'ㄱ' && c <= 'ㅎ') || (c >= 'ㅏ' && c <= 'ㅣ'))
            {
                count++;
            }
        }
        return count;
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


