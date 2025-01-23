//using Photon.Pun;
//using PlayFab;
//using PlayFab.ClientModels;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SocialPlatforms;
//using UnityEngine.SocialPlatforms.Impl;
//using UnityEngine.UI;

//public class GPGS_Manager : MonoBehaviour
//{
//    public LoginManager LoginManager; // 로그인 매니저 스크립트에서 일부 변수 사용을 위해 선언(별도)

//    public Text UserID, Username, Messages; //텍스트 요소들
//    private GameObject PlayerSetPanel; //player초기 세팅 패널(참조)

//    private void Start()
//    {
//        PlayerSetPanel = LoginManager.GetPlayerSetPanel();
//    }


//    //구글 로그인 버튼->구글 연결 완료->플레이팹 로그인 시도->로그인 실패 시 회원가입 성공(계정 존재하면 플레이팹 로그인 성공)
//    public void GPGS_Login()
//    {
//        GPGSBinder.Inst.Login((success, localUser) =>
//        {
//            if (success)
//            {
//                Messages.text = "구글 로그인 성공";
//                UserID.text = localUser.id;
//                Username.text = localUser.userName;

//                var request = new LoginWithEmailAddressRequest
//                {
//                    Email = localUser.id + "@gmail.com",
//                    Password = localUser.id
//                };
//                PlayFabClientAPI.LoginWithEmailAddress(request,
//                   result =>
//                   {
//                       Messages.text = "플레이팹 로그인 성공";
//                       OnClickConnect(); //기존 회원이 플레이팹 로그인 성공 시, 메인으로 이동
//                   },
//                   error =>
//                   {
//                       if (error.Error == PlayFabErrorCode.AccountNotFound)
//                       {
//                           // 계정이 없으면 회원가입 호출
//                           GPGSRegister(localUser.id, localUser.userName, Messages);
//                           // 회원가입 완료 되었으면 플레이어 프로필 이름, 사진 설정
//                           Messages.text = ""; //그 전에 상태 메시지 비우기
//                           PlayerSetPanel.SetActive(true); //유저 초기 설정 패널 띄우기
//                       }
//                   });
//            }
//            else
//            {
//                Messages.text = "구글 로그인 실패";
//            }
//        });
//    }

//    //구글플레이를 통한 회원가입 시
//    public void GPGSRegister(string userID, string userName, Text Logtext)
//    {
//        var request = new RegisterPlayFabUserRequest { Email = userID + "@gmail.com", Password = userID, Username = userName };
//        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => Logtext.text = "플레이팹 회원가입 성공", (error) => Logtext.text = "플레이팹 회원가입 실패");
//    }

//    //로그아웃 버튼 누르면 로그아웃 실행
//    public void GPGS_Logout()
//    {
//        GPGSBinder.Inst.Logout();
//        UserID.text = "정보없음";
//        Username.text = "정보없음";
//        Messages.text = "구글 로그아웃 성공";
//    }

//    public void OnClickConnect()
//    {
//        // 마스터 서버 접속 요청
//        PhotonNetwork.ConnectUsingSettings();
//    }
//}
