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
    public Text UserID, Username, UserEmail, Messages; //�ؽ�Ʈ ��ҵ�

    void Start()
    {
        
    }


    //���� �α��� ��ư->���� ���� �Ϸ�->�÷����� �α��� �õ�->�α��� ���� �� ȸ������ ����(���� �����ϸ� �÷����� �α��� ����)
    public void GPGS_Login()
    {
        GPGSBinder.Inst.Login((success, localUser) =>
        {
            if (success)
            {
                Messages.text = "���� �α��� ����";
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
                       Messages.text = "�÷����� �α��� ����";
                   },
                   error =>
                   {
                       if (error.Error == PlayFabErrorCode.AccountNotFound)
                       {
                           // ������ ������ ȸ������ ȣ��
                           GPGSRegisterBtn(localUser.id, localUser.userName, Messages);
                       }
                   });
            }
            else
            {
                Messages.text = "���� �α��� ����";
            }
        });
    }

    //�����÷��̸� ���� ȸ������ ��
    public void GPGSRegisterBtn(string userID, string userName, Text Logtext)
    {
        var request = new RegisterPlayFabUserRequest { Email = userID + "@gmail.com", Password = userID, Username = userName };
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => Logtext.text = "�÷����� ȸ������ ����" + userName, (error) => Logtext.text = "�÷����� ȸ������ ����");
    }

    //�α׾ƿ� ��ư ������ �α׾ƿ� ����
    public void GPGS_Logout()
    {
        GPGSBinder.Inst.Logout();
        UserID.text = "��������";
        Username.text = "��������";
        Messages.text = "���� �α׾ƿ� ����";
    }

}
