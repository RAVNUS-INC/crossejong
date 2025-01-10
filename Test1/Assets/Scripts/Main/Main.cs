using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

public class Main : MonoBehaviour
{
    public MainSettingsPopup mainSettingsPopup; //MainSettingsPopup 스크립트 연결
    public MakeRoomPopup makeRoomPopup; //MakeRoomPopup 스크립트 연결
    public Text displayNameText; // DisplayName을 표시할 UI 텍스트
    public InputField profileInputField; //프로필 이름 입력란

    public Sprite[] profileImages; // 3가지 기본 제공 이미지
    public GameObject profilePanel; // 프로필 패널
    public Image centralImage;  // 프로필 이미지

    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // 저장 키

    void Start()
    {


        // PlayFab에서 저장된 이미지 인덱스를 불러와 이미지 업데이트
        LoadProfileImageIndex();

        GetUserDisplayName(); //유저 네임 불러와서 텍스트로 표시

        profilePanel.SetActive(false);

        profileInputField.interactable = false; //프로필 이름 초기 비활성화

        mainSettingsPopup.MainSettingsPopupf();
        makeRoomPopup.MakeRoomPopupf();
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
                UpdateProfileImage(index);
            }
            else
            {
                Debug.LogWarning("PROFILE_IMAGE_INDEX_KEY가 존재하지 않습니다. 기본 이미지로 설정합니다.");
                UpdateProfileImage(0);  // 기본 이미지로 설정
            }
        }, error =>
        {
            Debug.LogError($"유저 데이터 불러오기 실패: {error.GenerateErrorReport()}");
        });
    }

    // 이미지 업데이트 함수
    private void UpdateProfileImage(int index)
    {
        // 인덱스에 해당하는 이미지로 중앙 이미지 업데이트
        centralImage.sprite = profileImages[index];
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
            displayNameText.text = "Welcome, Player!";
        }
    }

    // DisplayName 가져오기에 실패한 경우
    private void OnGetAccountInfoFailure(PlayFabError error)
    {
        Debug.LogError($"DisplayName 가져오기 실패: {error.GenerateErrorReport()}");
    }

    public void ProfileBtn() //프로필 버튼 클릭하면 
    {
        profilePanel.SetActive(true); //프로필 패널 띄우기

    }

    public void ExitBtn() //프로필 패널의 닫기 버튼을 누르면
    {
        profilePanel.SetActive(false); //패널 비활성화

        //유저 프로필 이미지 재로드, 이름 재로드 텍스트 보여주기

        GetUserDisplayName();
        LoadProfileImageIndex();

    }

}
