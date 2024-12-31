using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class Main : MonoBehaviour
{
    public MainSettingsPopup mainSettingsPopup; //MainSettingsPopup 스크립트 연결
    public MakeRoomPopup makeRoomPopup; //MakeRoomPopup 스크립트 연결

    void Start()
    {
        mainSettingsPopup.MainSettingsPopupf();
        makeRoomPopup.MakeRoomPopupf();
    }
    void Update()
    {
        
    }
}
