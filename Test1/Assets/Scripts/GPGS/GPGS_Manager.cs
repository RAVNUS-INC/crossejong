using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GPGS_Manager : MonoBehaviour
{
    public Text UserID, Username, UserEmail, Messages; //텍스트 요소들

    void Start()
    {
        
    }


    //구글 로그인 버튼->구글 연결 완료->플레이팹 로그인 시도->로그인 실패 시 회원가입 성공(계정 존재하면 플레이팹 로그인 성공)
    public void GPGS_Login()
    {
        GPGSBinder.Inst.Login((success, localUser) =>
        {
            if (success)
            {
                Messages.text = "구글 로그인 성공";
                UserID.text = localUser.id;
                Username.text = localUser.userName;

                var request = new LoginWithEmailAddressRequest
                {
                    Email = localUser.id + "@gmail.com",
                    Password = localUser.id
                };
                PlayFabClientAPI.LoginWithEmailAddress(request,
                   result =>
                   {
                       Messages.text = "플레이팹 로그인 성공";
                   },
                   error =>
                   {
                       if (error.Error == PlayFabErrorCode.AccountNotFound)
                       {
                           // 계정이 없으면 회원가입 호출
                           GPGSRegisterBtn(localUser.id, localUser.userName, Messages);
                       }
                   });
            }
            else
            {
                Messages.text = "구글 로그인 실패";
            }
        });
    }

    //구글플레이를 통한 회원가입 시
    public void GPGSRegisterBtn(string userID, string userName, Text Logtext)
    {
        var request = new RegisterPlayFabUserRequest { Email = userID + "@gmail.com", Password = userID, Username = userName };
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => Logtext.text = "플레이팹 회원가입 성공" + userName, (error) => Logtext.text = "플레이팹 회원가입 실패");
    }

    //로그아웃 버튼 누르면 로그아웃 실행
    public void GPGS_Logout()
    {
        GPGSBinder.Inst.Logout();
        UserID.text = "정보없음";
        Username.text = "정보없음";
        Messages.text = "구글 로그아웃 성공";
    }

}
