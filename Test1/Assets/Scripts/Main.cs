using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public MainSettingsPopup mainSettingsPopup; //MainSettingsPopup 스크립트 연결
    public MakeRoomPopup makeRoomPopup; //MakeRoomPopup 스크립트 연결

    void SetSettingsPopup()
    {
        mainSettingsPopup.openSettingsPopupButton.onClick.AddListener(mainSettingsPopup.OpenSettingsPopup);  // 설정 팝업 열기 버튼에 이벤트 추가
        mainSettingsPopup.closeSettingsPopupButton.onClick.AddListener(mainSettingsPopup.CloseSettingsPopup);  // 설정 팝업 닫기 버튼에 이벤트 추가

        mainSettingsPopup.settingsPopupPanel.SetActive(false); // 설정 팝업 비활성화
        mainSettingsPopup.openSettingsPopupButton.gameObject.SetActive(true); // 설정 팝업 열기 버튼을 활성화
        mainSettingsPopup.closeSettingsPopupButton.gameObject.SetActive(false); // 설정 팝업 닫기 버튼을 비활성화
    }

    void MakeRoomPopup()
    {
        makeRoomPopup.openMakeRoomPopupButton.onClick.AddListener(makeRoomPopup.OpenMakeRoomPopup);  // 방만들기 팝업 열기 버튼에 이벤트 추가
        makeRoomPopup.closeMakeRoomPopupButton.onClick.AddListener(makeRoomPopup.CloseMakeRoomPopup);  // 방만들기 팝업 닫기 버튼에 이벤트 추가

        makeRoomPopup.makeRoomPopupPanel.SetActive(false); // 방만들기 팝업 비활성화
        makeRoomPopup.openMakeRoomPopupButton.gameObject.SetActive(true); // 방만들기 팝업 열기 버튼을 활성화
        makeRoomPopup.closeMakeRoomPopupButton.gameObject.SetActive(false); // 방만들기 팝업 닫기 버튼을 비활성화

        makeRoomPopup.makeRoomButton.onClick.AddListener(MoveToPlayRoom); // 방 만들기 버튼 클릭 이벤트에 플레이방 이동 메서드 추가
        makeRoomPopup.joinRoomButton.onClick.AddListener(MoveToPlayRoom); // 방 참여하기 버튼 클릭 이벤트에 플레이방 이동 메서드 추가
    }


    void MoveToPlayRoom()
    {
        SceneManager.LoadScene("PlayRoom");
    }


    void Start()
    {
        SetSettingsPopup();
        MakeRoomPopup();
    }


    void Update()
    {
        
    }
}
