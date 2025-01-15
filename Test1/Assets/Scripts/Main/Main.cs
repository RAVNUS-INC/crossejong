using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using System;

// 메인에 존재하는 기능에 관한 스크립트
public class Main : MonoBehaviour
{
    private InputField inputField; //프로필 패널 안의 이름입력필드
    private Text SaveText; //프로필 패널 안의 저장메시지

    public Text displayNameText; // DisplayName을 표시할 UI 텍스트
    public InputField profileInputField; //메인의 프로필 이름 입력란
    public Image centralImage;  // 메인 프로필 이미지

    public Sprite[] profileImages; // 3가지 기본 제공 이미지
    public GameObject profilePanel; // 프로필 수정 패널

    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // 저장 키

    private void Awake()
    {
        // PlayFab에서 저장된 이미지 인덱스를 불러와 이미지 업데이트
        LoadProfileImageIndex();

        //유저 네임 불러와서 텍스트로 표시
        GetUserDisplayName();


    }

    void Start()
    {
        // UserSetManager 컴포넌트 참조
        UserSetManager userSetManager = FindObjectOfType<UserSetManager>();

        // UserSetManager에서 InputField를 가져옴
        inputField = userSetManager.inputText;
        SaveText = userSetManager.saveText;
    
        profilePanel.SetActive(false);
        profileInputField.interactable = false; //프로필 이름 초기 비활성화
    }

    


    // 프로필 이미지 인덱스 불러오기 함수
    // PlayFab에서 저장된 이미지 인덱스를 불러오는 함수
    private void LoadProfileImageIndex()
    {
        var request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request, result =>
        {
            // PROFILE_IMAGE_INDEX_KEY가 존재하는지 확인
            if (result.Data.ContainsKey(PROFILE_IMAGE_INDEX_KEY))
            {
                // 저장된 인덱스 값 불러오기
                int index = int.Parse(result.Data[PROFILE_IMAGE_INDEX_KEY].Value);
                // 인덱스 범위 체크 후 이미지 업데이트
                centralImage.sprite = profileImages[index];
            }
            else
            {
                Debug.LogWarning("PROFILE_IMAGE_INDEX_KEY가 존재하지 않습니다. 기본 이미지로 설정합니다.");
                centralImage.sprite = profileImages[0]; ;  // 기본 이미지로 설정
            }
        }, error =>
        {
            Debug.LogError($"유저 데이터 불러오기 실패: {error.GenerateErrorReport()}");
        });
    }

    

    // 이름 불러오기
    // DisplayName 불러오기 함수
    public void GetUserDisplayName()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountInfoSuccess, OnGetAccountInfoFailure);
    }

    // 성공적으로 DisplayName을 가져온 경우
    private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        string displayName = result.AccountInfo.TitleInfo.DisplayName;
        profileInputField.text = displayName;

        if (!string.IsNullOrEmpty(displayName))
        {
            Debug.Log($"유저의 DisplayName: {displayName}");
            displayNameText.text = $"{displayName}";
        }
        else
        {
            Debug.Log("DisplayName이 설정되지 않았습니다.");
            displayNameText.text = "이름없음";
        }
    }

    // DisplayName 가져오기에 실패한 경우
    private void OnGetAccountInfoFailure(PlayFabError error)
    {
        Debug.LogError($"DisplayName 가져오기 실패: {error.GenerateErrorReport()}");
    }


    public void ExitBtn() //프로필 패널의 닫기 버튼을 누르면
    {
        profilePanel.SetActive(false); //패널 비활성화
        inputField.interactable = false; //이름입력란 비활성화
        SaveText.text = ""; //저장 메시지 초기화

        //유저 프로필 이미지 재로드, 이름 재로드 텍스트 보여주기
        GetUserDisplayName();
        LoadProfileImageIndex();

    }

}
