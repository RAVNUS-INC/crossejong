using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MakeRoomPopup : MonoBehaviour
{
    public GameObject makeRoomPopupPanel; // 방만들기 팝업 패널
    public Button openMakeRoomPopupButton; // 방만들기 팝업 열기 버튼
    public Button closeMakeRoomPopupButton; // 방만들기 팝업 닫기 버튼
    public Button makeRoomButton; // 방 만들기 버튼
    public Button joinRoomButton; // 방 참여하기 버튼
    public Button roomSetButton; // 방 설정 버튼
    public Button goToRoomButton; // 설정 완료된 방 만들기 버튼

    void Start()
    {

    }

    public void MakeRoomPopupf()
    {
        openMakeRoomPopupButton.onClick.AddListener(OpenMakeRoomPopup);  // 방만들기 팝업 열기 버튼에 이벤트 추가
        closeMakeRoomPopupButton.onClick.AddListener(CloseMakeRoomPopup);  // 방만들기 팝업 닫기 버튼에 이벤트 추가

        makeRoomPopupPanel.SetActive(false); // 방만들기 팝업 비활성화
        openMakeRoomPopupButton.gameObject.SetActive(true); // 방만들기 팝업 열기 버튼을 활성화
        closeMakeRoomPopupButton.gameObject.SetActive(false); // 방만들기 팝업 닫기 버튼을 비활성화

        makeRoomButton.onClick.AddListener(MakeRoomSetting); // 방 만들기 버튼 클릭 이벤트에 방 설정 버튼 팝업 추가
        joinRoomButton.onClick.AddListener(MoveToPlayRoom); // 방 참여하기 버튼 클릭 이벤트에 플레이방 이동 메서드 추가
    }

    void OpenMakeRoomPopup() // 방만들기 팝업 열기 메서드
    {
        makeRoomPopupPanel.SetActive(true); // 팝업 활성화
        closeMakeRoomPopupButton.gameObject.SetActive(true); // 방만들기 팝업 닫기 버튼 활성화
        makeRoomButton.gameObject.SetActive(true); // 방 만들기 버튼 활성화
        joinRoomButton.gameObject.SetActive(true); // 방 참여하기 버튼 활성화
        roomSetButton.gameObject.SetActive(false); // 방 설정 팝업 버튼을 비활성화
    }

    void CloseMakeRoomPopup() // 방만들기 팝업 닫기 메서드
    {
        makeRoomPopupPanel.SetActive(false); // 팝업 비활성화
        closeMakeRoomPopupButton.gameObject.SetActive(false); // 방만들기 팝업 닫기 버튼 비활성화
        makeRoomButton.gameObject.SetActive(false); // 방 만들기 버튼 비활성화
        joinRoomButton.gameObject.SetActive(false); // 방 참여하기 버튼 비활성화
        roomSetButton.gameObject.SetActive(false); // 방 설정 팝업 버튼을 비활성화
    }

    void MakeRoomSetting() // 방 설정 팝업 열기 메서드
    {
        closeMakeRoomPopupButton.gameObject.SetActive(true); // 방만들기 팝업 닫기 버튼 활성화
        makeRoomButton.gameObject.SetActive(false); // 방 만들기 버튼 비활성화
        joinRoomButton.gameObject.SetActive(false); // 방 참여하기 버튼 비활성화
        roomSetButton.gameObject.SetActive(true); // 방 설정 버튼 활성화

        goToRoomButton.onClick.AddListener(MoveToMakeRoom); // 설정완료 후 방 만들기 버튼 클릭 이벤트에 플레이방 이동 메서드 추가
    }

     void MoveToPlayRoom()
    {
        SceneManager.LoadScene("PlayRoom");
    }

    void MoveToMakeRoom()
    {
        SceneManager.LoadScene("MakeRoom");
    }

}
