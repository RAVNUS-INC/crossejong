using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainSettingsPopup : MonoBehaviour
{
    public GameObject settingsPopupPanel; // 설정 팝업 패널
    public Button openSettingsPopupButton; // 설정 팝업 열기 버튼
    public Button closeSettingsPopupButton; // 설정 팝업 닫기 버튼

    void Start()
    {

    }

    public void MainSettingsPopupf()
    {
        openSettingsPopupButton.onClick.AddListener(OpenSettingsPopup);  // 설정 팝업 열기 버튼에 이벤트 추가
        closeSettingsPopupButton.onClick.AddListener(CloseSettingsPopup);  // 설정 팝업 닫기 버튼에 이벤트 추가

        settingsPopupPanel.SetActive(false); // 설정 팝업 비활성화
        openSettingsPopupButton.gameObject.SetActive(true); // 설정 팝업 열기 버튼을 활성화
        closeSettingsPopupButton.gameObject.SetActive(false); // 설정 팝업 닫기 버튼을 비활성화
    }

    void OpenSettingsPopup() // 설정 팝업 열기 메서드
    {
        settingsPopupPanel.SetActive(true); // 팝업 활성화
        closeSettingsPopupButton.gameObject.SetActive(true); // 설정 팝업 닫기 버튼 활성화
    }

    void CloseSettingsPopup() // 설정 팝업 닫기 메서드
    {
        settingsPopupPanel.SetActive(false); // 팝업 비활성화
        closeSettingsPopupButton.gameObject.SetActive(false); // 설정 팝업 닫기 버튼 비활성화
    }
}
