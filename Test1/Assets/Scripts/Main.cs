using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public MainSettingsPopup mainSettingsPopup; //MainSettingsPopup 스크립트 연결
    public Button gameStartButton; // 게임 시작 버튼

    void SetSettingsPopup()
    {
        mainSettingsPopup.openSettingsPopupButton.onClick.AddListener(mainSettingsPopup.OpenSettingsPopup);  // 설정 팝업 열기 버튼에 이벤트 추가
        mainSettingsPopup.closeSettingsPopupButton.onClick.AddListener(mainSettingsPopup.CloseSettingsPopup);  // 설정 팝업 닫기 버튼에 이벤트 추가

        mainSettingsPopup.settingsPopupPanel.SetActive(false); // 설정 팝업 비활성화
        mainSettingsPopup.openSettingsPopupButton.gameObject.SetActive(true); // 설정 팝업 열기 버튼을 활성화
        mainSettingsPopup.closeSettingsPopupButton.gameObject.SetActive(false); // 설정 팝업 닫기 버튼을 비활성화
    }

    void Start()
    {
        SetSettingsPopup();
        gameStartButton.onClick.AddListener(MoveToPlayRoom); // 게임 시작 버튼 클릭 이벤트에 플레이방 이동 메서드 추가
    }

    void MoveToPlayRoom()
    {
        SceneManager.LoadScene("PlayRoom");
    }

    void Update()
    {
        
    }
}
