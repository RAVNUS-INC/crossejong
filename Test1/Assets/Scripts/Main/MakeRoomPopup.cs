using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MakeRoomPopup : MonoBehaviour
{
    public GameObject makeRoomPopupPanel; // 방만들기 팝업 패널
    public Button openMakeRoomPopupButton; // 방만들기 팝업 열기 버튼
    public Button closeMakeRoomPopupButton; // 방만들기 팝업 닫기 버튼
    public Button makeRoomButton; // 방 만들기 버튼
    public Button joinRoomButton; // 방 참여하기 버튼

    void Start()
    {

    }

    public void OpenMakeRoomPopup() // 방만들기 팝업 열기 메서드
    {
        makeRoomPopupPanel.SetActive(true); // 팝업 활성화
        closeMakeRoomPopupButton.gameObject.SetActive(true); // 방만들기 팝업 닫기 버튼 활성화
        makeRoomButton.gameObject.SetActive(true); // 방 만들기 버튼 활성화
        joinRoomButton.gameObject.SetActive(true); // 방 참여하기 버튼 활성화
    }

    public void CloseMakeRoomPopup() // 방만들기 팝업 닫기 메서드
    {
        makeRoomPopupPanel.SetActive(false); // 팝업 비활성화
        closeMakeRoomPopupButton.gameObject.SetActive(false); // 방만들기 팝업 닫기 버튼 비활성화
        makeRoomButton.gameObject.SetActive(false); // 방 만들기 버튼 비활성화
        joinRoomButton.gameObject.SetActive(false); // 방 참여하기 버튼 비활성화
    }
}
