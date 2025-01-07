using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine;

public class PlayFabManager : MonoBehaviour
{
    public InputField EmailInput, PasswordInput, UsernameInput;

    public void LoginBtn()
    {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => print("�α��� ����"), (error) => print("�α��� ����"));
    }

    public void RegisterBtn()
    {
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UsernameInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => print("ȸ������ ����"), (error) => print("ȸ������ ����"));
    }


}
