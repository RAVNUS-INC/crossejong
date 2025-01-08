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

    public GameObject profilePanel; // 프로필 패널
    public InputField profileInputField; //프로필 이름 입력란
    [SerializeField] private Image centralImage;  // 이미지 컴포넌트
    [SerializeField] private Sprite[] profileImages;  // 이미지 배열


    void Start()
    {
        // 저장된 프로필 이미지 인덱스를 불러옵니다.
        int savedIndex = PlayerPrefs.GetInt("ProfileImageIndex", 0);  // 기본값 0 (첫 번째 이미지)

        // 인덱스가 유효한지 확인하고 이미지 업데이트
        if (savedIndex >= 0 && savedIndex < profileImages.Length)
        {
            centralImage.sprite = profileImages[savedIndex];  // 해당 인덱스에 맞는 이미지로 설정
        }
        else
        {
            Debug.LogError("Invalid ProfileImageIndex.");
        }

        GetUserDisplayName(); //유저 네임 불러와서 텍스트로 표시

        profilePanel.SetActive(false);
        profileInputField.interactable = false; //프로필 이름 초기 비활성화

        mainSettingsPopup.MainSettingsPopupf();
        makeRoomPopup.MakeRoomPopupf();
    }


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

    

}
