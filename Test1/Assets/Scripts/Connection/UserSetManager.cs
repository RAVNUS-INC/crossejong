using Photon.Pun;
using Photon.Realtime; // AuthenticationValues 및 기타 실시간 기능 사용
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// 유저 프로필 설정 화면에서 작동하는 코드(닉네임, 유저프로필사진 변경 저장)
public class UserSetManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public TMP_InputField inputText; //닉네임 입력(나중에 바꿀 수 있는 Displayname)
    [SerializeField] public Button confirmButton; //제출(저장) 버튼
    [SerializeField] TMP_Text warningText; // 경고 메시지를 출력할 UI 텍스트
    [SerializeField] public TMP_Text saveText; // 저장완료 메시지를 출력할 UI 텍스트(profile panel에만 존재)

    // displayname 조건
    private const int MinLength = 3; // 최소 입력 길이(2글자 이상)
    private const int MaxLength = 8; // 최대 입력 길이(변동가능)

    public Image centralImage; // 중앙에 표시되는 프로필이미지
    private int currentIndex = 0; // 현재 선택된 이미지 인덱스

    public GameObject profilePanel; //프로필 설정 패널(메인패널에서 미리 준비해야 작동)
    public GameObject usersetPanel; //유저 초기 설정 패널

    

    void Start()
    {
        // 초기 유저 세팅 패널 OR 기존 유저 프로필 패널이 활성화되어있다면
        if ((usersetPanel.activeSelf || profilePanel.activeSelf)) 
        {
            LoadDefaultDisplayName(); // 기존 이름 정보 불러와 변수에 저장
            LoadDefaultImageIndex(); // 기존 이미지 인덱스를 불러와 변수에 저장, 업데이트
        }
        
        confirmButton.interactable = false; // 기본적으로 버튼 비활성화
        warningText.text = ""; // 초기 경고 메시지 비우기
        saveText.text = ""; // 초기 저장 메시지 비우기

        //내용이 변경되었을때 규칙 검사
        inputText.onValueChanged.AddListener(ValidateNickname);

        //확인 버튼 누르면 이름 및 이미지 저장(playfab 및 playerprefs에 업데이트)(+ 단어완성횟수 버튼에 직접 연결)
        confirmButton.onClick.AddListener(SaveDisplayName); //이름 저장 
        confirmButton.onClick.AddListener(SaveSelectedImageIndex); // 이미지 저장
        confirmButton.onClick.AddListener(() => { inputText.interactable=false; }); // 인풋필드 선택 비활성화
    }

    
    private void LoadDefaultDisplayName() // 저장된 DisplayName 로드 및 저장 함수(존재하면 해당 값을, 존재하지 않으면 Guest)
    {
        if (PlayerPrefs.HasKey(UserInfoManager.DISPLAYNAME_KEY))
        {
            // 키가 존재하면 저장된 값을 가져온다
            UserInfoManager.instance.MyName = PlayerPrefs.GetString(UserInfoManager.DISPLAYNAME_KEY, "Guest");

            // 불러온 이름을 인풋란에 보여주기
            inputText.text = UserInfoManager.instance.MyName;

            //프로필 이름 초기 비활성화
            inputText.interactable = false;
        }
        else
        {
            // 키가 존재하지 않으면 기본값을 설정한다
            inputText.text = "";

            // 기본값을 playerprefs에도 바로 저장
            PlayerPrefs.SetString(UserInfoManager.DISPLAYNAME_KEY, "Guest"); PlayerPrefs.Save();
        }
    }

    private void LoadDefaultImageIndex() // 저장된 이미지 인덱스를 불러오기(존재하면 해당 값을, 존재하지 않으면 기본값 0)
    {
        if (PlayerPrefs.HasKey(UserInfoManager.IMAGEINDEX_KEY))
        {
            // 키가 존재하면 저장된 값을 가져온다
            UserInfoManager.instance.MyImageIndex = PlayerPrefs.GetInt(UserInfoManager.IMAGEINDEX_KEY, 0);

            //currentIndex 인덱스 변수에 값 저장
            currentIndex = UserInfoManager.instance.MyImageIndex;
        }
        else
        {
            // 키가 존재하지 않으면 기본값을 설정한다
            currentIndex = 0;

            // 기본값을 playerprefs에도 바로 저장
            PlayerPrefs.SetInt(UserInfoManager.IMAGEINDEX_KEY, currentIndex); PlayerPrefs.Save();
        }
        centralImage.sprite = UserInfoManager.instance.profileImages[currentIndex];  // 이미지 업데이트
    }

    public void SaveDisplayName() //DisplayName을 playfab과 playerprefs에 저장
    {
        if (UserInfoManager.instance.MyName == inputText.text)
        {
            return;
        }

        UserInfoManager.instance.MyName = inputText.text.Trim();

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = UserInfoManager.instance.MyName
        };

        //playfab에 저장
        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            result =>
            {
                Debug.Log($"[Playfab] 닉네임 저장 성공: {result.DisplayName}");
            },
            error =>
            {
                Debug.LogError($"[Playfab] 닉네임 저장 실패: {error.GenerateErrorReport()}");
            });

        //변경된 이미지 인덱스를 playerprefs에 저장(기존 유저의 경우 덮어쓰기, 신규 유저는 새로 추가하는 상황)
        UpdateDisplayName(UserInfoManager.instance.MyName);
        Debug.Log($"[playerprefs] Displayname: {UserInfoManager.instance.MyName}을 저장했습니다");

        if (usersetPanel.activeSelf)
        {
            // 메인으로 서버접속을 요청
            OnClickConnect();
        }
        if (profilePanel.activeSelf)
        {
            // 저장 메시지 알림
            saveText.text = "저장되었습니다";
        }
    }

    private void SaveSelectedImageIndex() // 선택된 이미지 인덱스를 저장해 playfab에 전송, playerprefs 업뎃
    {
        if (UserInfoManager.instance.MyImageIndex == currentIndex)
        {
            return;
        }

        UserInfoManager.instance.MyImageIndex = currentIndex; // int형

        string ImageIndex = UserInfoManager.instance.MyImageIndex.ToString(); // 문자열로 변환

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
        {
            { UserInfoManager.PROFILE_IMAGE_INDEX_KEY, ImageIndex}
        },
            Permission = UserDataPermission.Public // 데이터를 공개 상태로 저장
        };

        // playfab에 저장
        PlayFabClientAPI.UpdateUserData(request,
            result =>
            {
                Debug.Log($"[Playfab] 프로필이미지 데이터 저장 성공: {ImageIndex}");
            },
            error =>
            {
                Debug.LogError($"[Playfab] 유저 이미지 저장 실패: {error.GenerateErrorReport()}");
            });

        // 변경된 이미지 인덱스를 playerprefs에 저장(기존 유저의 경우 덮어쓰기, 신규 유저는 새로 추가하는 상황)
        UpdateImageIndex(UserInfoManager.instance.MyImageIndex); 
        Debug.Log($"[playerprefs] Imageindex: {UserInfoManager.instance.MyImageIndex}을 저장했습니다");
    }

    void UpdateDisplayName(string name) //새로운 이름 저장
    {
        PlayerPrefs.SetString(UserInfoManager.DISPLAYNAME_KEY, name); // 새로운 값 저장
        PlayerPrefs.Save(); // 저장 유지
    }

    void UpdateImageIndex(int newIndex) //새로운 인덱스 저장
    {
        PlayerPrefs.SetInt(UserInfoManager.IMAGEINDEX_KEY, newIndex); // 새로운 값 저장
        PlayerPrefs.Save(); // 저장 유지
    }
    
    public void OnLeftButtonClicked() // 왼쪽 버튼 클릭 시 호출
    {
        currentIndex = (currentIndex - 1 + UserInfoManager.instance.profileImages.Length) % UserInfoManager.instance.profileImages.Length;
        centralImage.sprite = UserInfoManager.instance.profileImages[currentIndex];  // 인덱스에 해당하는 이미지로 업데이트

        if (UserInfoManager.instance.MyImageIndex == currentIndex) //만약 현재 인덱스 이미지와 기존 이미지 인덱스가 같다면
        {
            confirmButton.interactable = false; //저장버튼 비활성화
        }
        else
        {
            confirmButton.interactable = true;
        }
    }

    public void OnRightButtonClicked() // 오른쪽 버튼 클릭 시 호출
    {
        currentIndex = (currentIndex + 1) % UserInfoManager.instance.profileImages.Length;
        centralImage.sprite = UserInfoManager.instance.profileImages[currentIndex];  // 인덱스에 해당하는 이미지로 업데이트

        if (UserInfoManager.instance.MyImageIndex == currentIndex) //만약 현재 인덱스 이미지와 기존 이미지 인덱스가 같다면
        {
            confirmButton.interactable = false; //저장버튼 비활성화
        }
        else
        {
            confirmButton.interactable = true;
        }
    }

    public void ChangeNameBtn() //이름 변경 버튼 클릭할 때 -> 이름 입력 인풋 활성화
    {
        inputText.interactable = true; //이름 변경 활성화
    }

    public void ValidateNickname(string input) //이름 설정 규칙(displayname)
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
        // 최소 길이 충족 검사
        else if (GetKoreanCharCount(inputname) < MinLength) // 한글 자음, 모음을 포함한 최대 길이 검사
        {
            warningText.text = $"최소 {MinLength}자 이상이어야 합니다."; //3자 이상이어야 함
            confirmButton.interactable = false;
        }
        else if (inputname.Length == 0) // 빈 문자열 검사
        {
            warningText.text = "닉네임을 입력해주세요.";
            confirmButton.interactable = false; 
        }
        else if (UserInfoManager.instance.MyName == inputname && (inputText.isActiveAndEnabled)) //입력란이 기존 닉네임과 같으면서 활성화되어있는 경우
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

    private int GetKoreanCharCount(string input) // 한글 음절 자음, 모음을 포함하여 글자 수를 계산하는 함수
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
            // 숫자(0-9)인지 체크
            else if (c >= '0' && c <= '9')
            {
                count++;
            }
        }
        return count;
    }

    public override void OnConnectedToMaster() //마스터 서버 접속 되면
    {
        Debug.Log("마스터 서버 접속 성공");

        //나의 이름을 포톤에 설정
        PhotonNetwork.NickName = inputText.text;

        //로비진입
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby() //Lobby 진입에 성공했으면 호출되는 함수
    {
        Debug.Log("로비 진입 성공");
    }
    public void OnClickConnect() // 마스터 서버 접속 요청(OkBtn에 연결)
    {
        // 마스터 서버 접속 요청
        PhotonNetwork.ConnectUsingSettings();

        //로딩바 ui 애니메이션 보여주기
        LoadingSceneController.Instance.LoadScene("Main");
    }
    private void OnDestroy() // 이벤트 해제
    {
        inputText.onValueChanged.RemoveListener(ValidateNickname);
    }


}


