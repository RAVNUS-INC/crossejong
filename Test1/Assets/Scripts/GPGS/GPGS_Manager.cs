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
//    public LoginManager LoginManager; // �α��� �Ŵ��� ��ũ��Ʈ���� �Ϻ� ���� ����� ���� ����(����)

//    public Text UserID, Username, Messages; //�ؽ�Ʈ ��ҵ�
//    private GameObject PlayerSetPanel; //player�ʱ� ���� �г�(����)

//    private void Start()
//    {
//        PlayerSetPanel = LoginManager.GetPlayerSetPanel();
//    }


//    //���� �α��� ��ư->���� ���� �Ϸ�->�÷����� �α��� �õ�->�α��� ���� �� ȸ������ ����(���� �����ϸ� �÷����� �α��� ����)
//    public void GPGS_Login()
//    {
//        GPGSBinder.Inst.Login((success, localUser) =>
//        {
//            if (success)
//            {
//                Messages.text = "���� �α��� ����";
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
//                       Messages.text = "�÷����� �α��� ����";
//                       OnClickConnect(); //���� ȸ���� �÷����� �α��� ���� ��, �������� �̵�
//                   },
//                   error =>
//                   {
//                       if (error.Error == PlayFabErrorCode.AccountNotFound)
//                       {
//                           // ������ ������ ȸ������ ȣ��
//                           GPGSRegister(localUser.id, localUser.userName, Messages);
//                           // ȸ������ �Ϸ� �Ǿ����� �÷��̾� ������ �̸�, ���� ����
//                           Messages.text = ""; //�� ���� ���� �޽��� ����
//                           PlayerSetPanel.SetActive(true); //���� �ʱ� ���� �г� ����
//                       }
//                   });
//            }
//            else
//            {
//                Messages.text = "���� �α��� ����";
//            }
//        });
//    }

//    //�����÷��̸� ���� ȸ������ ��
//    public void GPGSRegister(string userID, string userName, Text Logtext)
//    {
//        var request = new RegisterPlayFabUserRequest { Email = userID + "@gmail.com", Password = userID, Username = userName };
//        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => Logtext.text = "�÷����� ȸ������ ����", (error) => Logtext.text = "�÷����� ȸ������ ����");
//    }

//    //�α׾ƿ� ��ư ������ �α׾ƿ� ����
//    public void GPGS_Logout()
//    {
//        GPGSBinder.Inst.Logout();
//        UserID.text = "��������";
//        Username.text = "��������";
//        Messages.text = "���� �α׾ƿ� ����";
//    }

//    public void OnClickConnect()
//    {
//        // ������ ���� ���� ��û
//        PhotonNetwork.ConnectUsingSettings();
//    }
//}
