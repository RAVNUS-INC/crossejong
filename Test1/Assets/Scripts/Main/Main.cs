using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using static UnityEngine.EventSystems.EventTrigger;

//using UnityEditor.PackageManager;
//using UnityEditor.PackageManager.Requests;

using UnityEngine.SceneManagement;

// 메인에 존재하는 기능에 관한 스크립트
public class Main : MonoBehaviour
{

    //usersestmanager에서 가져온 변수들    
    private InputField inputField; //프로필 패널 안의 이름입력필드
    private Text SaveText; //프로필 패널 안의 저장메시지
    private Sprite[] ProfileImg; //프로필 이미지 배열

    // --------------메인에 보여질 오브젝트------------------
    public Text displayNameText; // DisplayName을 표시할 UI 텍스트
    public InputField profileInputField; //메인의 프로필 이름 입력란
    public Image centralImage;  // 메인 프로필 이미지
    public GameObject profilePanel; // 프로필 수정 패널

    // ---------------대시보드에 보여질 랭킹 오브젝트---------------
    public GameObject[] ranklist; //활성화/비활성화를 위한 오브젝트
    public Image[] userimage; //유저 이미지
    public Text[] username; //유저 이름
    public Text[] wordcount; //단어완성횟수


    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // 저장 키

    private void Awake()
    {
        // PlayFab에서 저장된 이미지 인덱스를 불러와 이미지 업데이트
        LoadProfileImageIndex();

        //유저 네임 불러와서 텍스트로 표시
        GetUserDisplayName();

        RankActiveFalse(); //순위 오브젝트 모두 비활성화
        GetLeaderBoard(); //순위 업데이트
    }

    void Start()
    {
        // UserSetManager 컴포넌트 참조
        UserSetManager userSetManager = FindObjectOfType<UserSetManager>();

        // UserSetManager에서 InputField를 가져옴
        inputField = userSetManager.inputText;
        SaveText = userSetManager.saveText;
        ProfileImg = userSetManager.profileImages; //프로필 이미지 배열을 가져옴


        profilePanel.SetActive(false);
        profileInputField.interactable = false; //프로필 이름 초기 비활성화
  
    }


    // 프로필 이미지 인덱스 불러오기 함수
    // PlayFab에서 저장된 이미지 인덱스를 불러오는 함수
    // -> 커스텀프로퍼티에 저장된 값을 불러오기
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
                centralImage.sprite = ProfileImg[index];
            }
            else
            {
                Debug.LogWarning("PROFILE_IMAGE_INDEX_KEY가 존재하지 않습니다. 기본 이미지로 설정합니다.");
                centralImage.sprite = ProfileImg[0]; ;  // 기본 이미지로 설정
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

    //성공적으로 DisplayName을 가져온 경우
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


    // 로그아웃 버튼을 누르면
    public void LogoutBtn()
    {
        // PlayFab 인증 정보 초기화
        PlayFabClientAPI.ForgetAllCredentials();

        // 서버와의 연결도 끊기
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect(); // 현재 연결 끊기
        }

        // 로그인 화면으로 이동
        SceneManager.LoadScene("Login");
        Debug.Log("로그아웃되었습니다. 인증 정보가 초기화되었습니다.");
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

    private void RankActiveFalse()
    {
        //모든 순위오브젝트 비활성화 시키기
        for (int i = 0; i < ranklist.Length; i++)
        {
            ranklist[i].SetActive(false);
        }
    }

    //유저가 업데이트 버튼을 누르면
    public void UpdateBtn()
    {
        //순위오브젝트 비활성화
        RankActiveFalse();
        //유저들 간의 순위를 재갱신
        GetLeaderBoard();

    }

    // 리더보드 리스트에 들어갈 정보 불러오기
    public void GetLeaderBoard()
    {
        // playfab에서 리더보드 정보 요청
        var request = new GetLeaderboardRequest 
        { StartPosition = 0, StatisticName = "WordCompletionCount", MaxResultsCount = 10, 
          ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true } };
        PlayFabClientAPI.GetLeaderboard(request, (result) =>
        {
            for (int i = 0; i < result.Leaderboard.Count; i++)
            {
                var curBoard = result.Leaderboard[i];
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

    // 특정 유저의 공개 데이터 요청 함수
    private void GetUserImageData(string playFabId, int index)
    {
        var userDataRequest = new GetUserDataRequest
        {
            PlayFabId = playFabId // 데이터를 가져올 유저의 PlayFabId
        };
        PlayFabClientAPI.GetUserData(userDataRequest, result =>
        {
            // PROFILE_IMAGE_INDEX_KEY가 존재하는지 확인
            if (result.Data.ContainsKey(PROFILE_IMAGE_INDEX_KEY))
            {
                // 저장된 인덱스 값 불러오기
                int imgindex = int.Parse(result.Data[PROFILE_IMAGE_INDEX_KEY].Value);
                // 인덱스 범위 체크 후 랭킹 유저 이미지 업데이트
                userimage[index].sprite = ProfileImg[imgindex];
            }
            else
            {
                Debug.LogWarning("PROFILE_IMAGE_INDEX_KEY가 존재하지 않습니다. 기본 이미지로 설정합니다.");
                userimage[index].sprite = ProfileImg[0];  // 기본 이미지로 설정
            }
        }, error =>
        {
            Debug.LogError($"유저 데이터 불러오기 실패: {error.GenerateErrorReport()}");
        });
    }

}
