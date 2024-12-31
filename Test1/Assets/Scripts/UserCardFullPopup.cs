using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UserCardFullPopup : MonoBehaviour
{
    public GameObject userCardFullPanel; // 보유카드 전체보기 팝업 패널
    public Button openUserCardFullPopupButton; // 보유카드 전체보기 팝업 열기 버튼
    public Button closeUserCardFullPopupButton; // 보유카드 전체보기 팝업 닫기 버튼

    void Start()
    {

    }

    public void UserCardFullPopupf()
    {
        openUserCardFullPopupButton.onClick.AddListener(OpenUserCardFullPopup);  // 보유카드 전체보기 팝업 열기 버튼에 이벤트 추가
        closeUserCardFullPopupButton.onClick.AddListener(CloseUserCardFullPopup);  // 보유카드 전체보기 팝업 닫기 버튼에 이벤트 추가

        userCardFullPanel.SetActive(false); // 보유카드 전체보기 팝업 비활성화
        openUserCardFullPopupButton.gameObject.SetActive(true); // 보유카드 전체보기 팝업 열기 버튼을 활성화
        closeUserCardFullPopupButton.gameObject.SetActive(false); // 보유카드 전체보기 팝업 닫기 버튼을 비활성화
    }

    void OpenUserCardFullPopup() // 보유카드 전체보기 팝업 열기 메서드
    {
        userCardFullPanel.SetActive(true); // 보유카드 전체보기 팝업 활성화
        closeUserCardFullPopupButton.gameObject.SetActive(true); // 보유카드 전체보기 팝업 닫기 버튼 활성화
    }

    void CloseUserCardFullPopup() // 보유카드 전체보기 팝업 닫기 메서드
    {
        userCardFullPanel.SetActive(false); // 보유카드 전체보기 팝업 비활성화
        closeUserCardFullPopupButton.gameObject.SetActive(false); // 보유카드 전체보기 팝업 닫기 버튼 비활성화
    }

}
