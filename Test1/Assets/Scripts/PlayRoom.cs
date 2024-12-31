using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class PlayRoom : MonoBehaviour
{
    public UserCardFullPopup userCardFullPopup; //UserCardFullPopup 스크립트 연결

    void Start()
    {
        userCardFullPopup.UserCardFullPopupf();
    }


    void Update()
    {

    }
}
