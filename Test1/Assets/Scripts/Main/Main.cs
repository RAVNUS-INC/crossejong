using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

// 메인에 존재하는 기능에 관한 스크립트
public class Main : MonoBehaviour
{
    //usersetmanager에서 가져온 변수들    
    private TMP_InputField inputField; //프로필 패널 안의 이름입력필드
    private TMP_Text SaveText; //프로필 패널 안의 저장메시지
    private Image ProfileCenImg; //프로필 패널 중심 이미지
    private Button SaveBtn; //프로필 패널 정보 변경 후 저장 버튼

    // --------------메인에 보여질 오브젝트------------------
    public TMP_Text displayNameText, myRankText; // DisplayName, 순위를 표시할 UI 텍스트
    public Image centralImage;  // 메인 프로필 이미지
    public GameObject profilePanel; // 프로필 수정 패널

    // ---------------대시보드에 보여질 랭킹 오브젝트---------------
    public GameObject[] ranklist; //활성화/비활성화를 위한 오브젝트
    public Image[] userimage; //유저 이미지
    public TMP_Text[] username; //유저 이름
    public TMP_Text[] wordcount; //단어완성횟수

    public Text TestText, LogTestText; //빌드 테스트 텍스트 상태, 로그아웃 텍스트 상태

    private void Awake()
    {
        // UserSetManager 컴포넌트 참조
        UserSetManager userSetManager = FindObjectOfType<UserSetManager>();

        // UserSetManager에서 InputField를 가져옴
        inputField = userSetManager.inputText;
        SaveText = userSetManager.saveText;
        ProfileCenImg = userSetManager.centralImage; //프로필 패널 중심사진
        SaveBtn = userSetManager.confirmButton; //프로필 패널 저장버튼
    }

    void Start()
    {
        if (PhotonNetwork.InLobby)
        {
            Debug.Log("현재 로비에 있음.");
        }
        else
        {
            Debug.Log("현재 로비에 없음.");
            PhotonNetwork.JoinLobby();  // 로비로 이동
        }

        profilePanel.SetActive(false); //프로필 패널 비활성화

        GetProfileImageIndex(); // PlayFab에서 저장된 이미지 인덱스를 불러와 이미지 업데이트
        GetUserDisplayName(); //유저 네임 불러와서 텍스트로 표시
        //UpdateBtn(); //리더보드 업데이트
    }


    // 프로필 이미지 인덱스 불러오기 함수
    private void GetProfileImageIndex()
    {
        centralImage.sprite = UserInfoManager.instance.profileImages[UserInfoManager.instance.MyImageIndex]; //메인 유저 이미지 교체
        ProfileCenImg.sprite = UserInfoManager.instance.profileImages[UserInfoManager.instance.MyImageIndex]; //프로필 패널 중심 이미지 교체

        TestText.text = "이미지 로딩 완료";
    }
    

    // DisplayName 불러오기 함수
    public void GetUserDisplayName()
    {
        displayNameText.text = UserInfoManager.instance.MyName; //메인 패널 이름
        inputField.text = UserInfoManager.instance.MyName; //프로필 패널 이름

        TestText.text = "DisplayName 로딩 완료";
    }

    //프로필 패널의 닫기 버튼을 누르면(닫기 버튼에 연결해둠)
    public void ExitBtn() 
    {
        profilePanel.SetActive(false); //패널 비활성화
        inputField.interactable = false; //이름입력란 비활성화
        SaveText.text = ""; //저장 메시지 초기화
        SaveBtn.interactable = false; //저장버튼 비활성화

        //유저 프로필 이미지 재로드, 이름 재로드 텍스트 보여주기
        GetUserDisplayName();
        GetProfileImageIndex();

    }

    // 로그아웃 버튼을 누르면->playfab 인증을 로그아웃
    public void LogoutBtn()
    {
        // 서버와의 연결도 끊기
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect(); // 현재 연결 끊기
        }

        // 안드로이드 기기 ID도 연동 해제
        PlayFabClientAPI.UnlinkAndroidDeviceID(new UnlinkAndroidDeviceIDRequest(), result =>
        {
            Debug.Log("기기 ID 연동 해제 성공");

            // PlayFab 인증 정보 초기화
            PlayFabClientAPI.ForgetAllCredentials();
            LogTestText.text = "기기 및 playfab 해제";

            //로딩바 ui 애니메이션 보여주기(Login씬으로 이동)
            LoadingSceneController.Instance.LoadScene("Login");
        },
        error =>
        {
            Debug.LogError("기기 ID 연동 해제 실패: " + error.GenerateErrorReport());
            LogTestText.text = "기기 ID 연동 해제 실패";
        });

        

        //Debug.Log("로그아웃되었습니다. 인증 정보가 초기화되었습니다.");
    }

    // 게임 종료 버튼을 누르면
    public void ExitGame()
    {
        Debug.Log("게임 종료"); // Unity 에디터에서 디버그 메시지 확인
        Application.Quit(); // 실제로 게임 종료

        //Debug.Log("게임 종료");
        //#if UNITY_EDITOR
        //        UnityEditor.EditorApplication.isPlaying = false; // 에디터 종료
        //#else
        //    Application.Quit(); // 빌드된 게임 종료
        //#endif
    }

    //모든 순위오브젝트 비활성화 시키기
    private void RankActiveFalse()
    {
        for (int i = 0; i < ranklist.Length; i++)
        {
            ranklist[i].SetActive(false);
        }
    }

    //유저가 리더보드 업데이트 버튼을 누르면
    public void UpdateBtn()
    {
        //순위오브젝트 비활성화
        RankActiveFalse();
        //유저들 간의 순위를 재갱신
        GetLeaderBoard();
    }

    // 리더보드 리스트에 들어갈 정보 불러오기(로딩과정이 눈에 보임)
    public void GetLeaderBoard()
    {
        // playfab에서 리더보드 정보 요청
        var request = new GetLeaderboardRequest 
        { StartPosition = 0, StatisticName = "WordCompletionCount", MaxResultsCount = 10, 
          ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true } };
        PlayFabClientAPI.GetLeaderboard(request, (result) =>
        {
            string myPlayFabId = PlayFabSettings.staticPlayer.PlayFabId; // 현재 로그인한 유저의 ID

            for (int i = 0; i < result.Leaderboard.Count; i++)
            {
                var curBoard = result.Leaderboard[i];

                // 현재 로그인한 유저의 정보만 따로 가져옴
                if (curBoard.PlayFabId == myPlayFabId)
                {
                    int actualRank = curBoard.Position + 1; // 0위는 1위로 변환
                    myRankText.text = $"현재 {actualRank}위";
                    Debug.Log($"현재 {curBoard.Position + 1}위:");
                }
                //유저수, 순위에 따른 오브젝트 활성화
                ranklist[i].SetActive(true);
                //유저 단어완성횟수 업데이트
                wordcount[i].text = "총 " + curBoard.StatValue.ToString() + "회";
                //유저 이름 업데이트
                username[i].text = curBoard.DisplayName;
                //유저 이미지 인덱스를 요청 및 업데이트
                GetUserImageData(curBoard.PlayFabId, i);
            }
        },
        (Error) => print("리더보드 불러오기 실패"));
    }

    // 특정 유저의 공개 데이터 요청 함수(playfab으로부터)
    private void GetUserImageData(string playFabId, int index)
    {
        var userDataRequest = new GetUserDataRequest
        {
            PlayFabId = playFabId // 데이터를 가져올 유저의 PlayFabId
        };
        PlayFabClientAPI.GetUserData(userDataRequest, result =>
        {
            // PROFILE_IMAGE_INDEX_KEY가 존재하는지 확인
            if (result.Data.ContainsKey(UserInfoManager.instance.PROFILE_IMAGE_INDEX_KEY))
            {
                // 저장된 인덱스 값 불러오기
                int imgindex;
                if (int.TryParse(result.Data[UserInfoManager.instance.PROFILE_IMAGE_INDEX_KEY].Value, out imgindex))
                {
                    
                    // 인덱스 범위 체크 후 랭킹 유저 이미지 업데이트
                    if (imgindex >= 0 && imgindex < UserInfoManager.instance.profileImages.Length)
                    {
                        userimage[index].sprite = UserInfoManager.instance.profileImages[imgindex];
                    }
                    else
                    {
                        Debug.LogWarning("유효하지 않은 이미지 인덱스입니다. 기본 이미지로 설정합니다.");
                        userimage[index].color = Color.white;  // 기본 이미지로 설정
                    }  
                }
                else
                {
                    Debug.LogWarning("이미지 인덱스 변환 실패. 기본 이미지로 설정합니다.");
                    userimage[index].color = Color.white;  // 기본 이미지로 설정
                }
            }
            else
            {
                Debug.LogWarning("이미지 key가 존재하지 않습니다.");
            }
        }, error =>
        {
            Debug.LogError($"유저 데이터 불러오기 실패: {error.GenerateErrorReport()}");
        });
    }


}
