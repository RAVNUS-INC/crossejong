using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class Main : MonoBehaviour
{
    public MainSettingsPopup mainSettingsPopup; //MainSettingsPopup ��ũ��Ʈ ����
    public MakeRoomPopup makeRoomPopup; //MakeRoomPopup ��ũ��Ʈ ����

    void Start()
    {
        mainSettingsPopup.MainSettingsPopupf();
        makeRoomPopup.MakeRoomPopupf();
    }
    void Update()
    {
        
    }
}
