using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OptionPopup : MonoBehaviour
{
    public GameObject optionPopupPanel; // 옵션 팝업 패널
    public Button openOptionPopupButton; // 옵션 팝업 열기 버튼
    public Button closeOptionPopupButton; // 옵션 팝업 닫기 버튼
    public Button surrenderButton; // 항복 확인 버튼

    void Start()
    {

    }

    public void OpenPopup() // 옵션 팝업 열기 메서드
    {
        optionPopupPanel.SetActive(true); // 팝업 활성화
        openOptionPopupButton.gameObject.SetActive(false); // 옵션 팝업 열기 버튼 비활성화
        closeOptionPopupButton.gameObject.SetActive(true); // 옵션 팝업 닫기 버튼 활성화
        surrenderButton.gameObject.SetActive(true); // 항복 확인 버튼 활성화
    }

    public void ClosePopup() // 옵션 팝업 닫기 메서드
    {
        optionPopupPanel.SetActive(false); // 팝업 비활성화
        openOptionPopupButton.gameObject.SetActive(true); // 옵션 팝업 열기 버튼 활성화
        closeOptionPopupButton.gameObject.SetActive(false); // 옵션 팝업 닫기 버튼 비활성화
        surrenderButton.gameObject.SetActive(false); // 항복 확인 버튼 비활성화
    }
}
