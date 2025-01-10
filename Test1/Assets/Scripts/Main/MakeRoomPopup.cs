using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MakeRoomPopup : MonoBehaviour
{
    public Button makeRoomButton; // 방 만들기 
    public Button joinRoomButton; // 방 참여하기 
    public GameObject makeRoomPanel; // 방 관련 팝업 판넬

    public GameObject roomSetPanel; // 방 정보 설정 
    public Button goToRoomButton; // 설정 완료된 방 만들기 

    public GameObject joinRoomPanel; // 다른 방에 참여
    public Button joinButton; // 선택한 방에 참여 

    public Button ExitButton; // 닫기 버튼

    public void MakeRoomPopupf()
    {
        makeRoomButton.onClick.AddListener(MakeRoomSetting); // 방 만들기 버튼 클릭 -> 방 설정 팝업
        joinRoomButton.onClick.AddListener(JoinRoomList); // 방 참여하기 버튼 클릭 -> 방목록 판넬 이동
        ExitButton.onClick.AddListener(ClosePopup); // 닫기 버튼 누르면 닫기
    }

    void MakeRoomSetting() // 방 설정 팝업 열기 메서드
    {
        makeRoomPanel.gameObject.SetActive(true); // 방 관련 팝업 판넬 활성화
        roomSetPanel.gameObject.SetActive(true); // 방 설정 패널 활성화
        joinRoomPanel.gameObject.SetActive(false); // 방 참여 패널 비활성화
    }

    void JoinRoomList() //방 목록 열기 메서드
    {
        makeRoomPanel.gameObject.SetActive(true); // 방 관련 팝업 판넬 활성화
        joinRoomPanel.gameObject.SetActive(true); // 방 접속 패널 활성화
        roomSetPanel.gameObject.SetActive(false); // 방 참여 패널 활성화
    }
    void ClosePopup()
    {
        makeRoomPanel.gameObject.SetActive(false);
    }


    void MoveToPlayRoom()
    {
        SceneManager.LoadScene("PlayRoom");
    }

   

}
