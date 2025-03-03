using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


// 방 목록 표시 형태(프리팹)
public class RoomListItem : MonoBehaviour
{
    public TMP_Text roomInfo;

    //클릭되었을때 호출되는 함수
    public Action<string> onDelegate;


    private void Awake()
    {
        // 프리팹 내 텍스트(자식요소) 연결
        roomInfo = GetComponentInChildren<TMP_Text>();
    }
    public void SetInfo(string roomName, int currPlayer, int maxPlayer, string difficulty, int timeLimit)
    {
        if (roomInfo == null)
        {
            Debug.LogError("roomInfo is not assigned!");
            return;
        }

        name = roomName;
        // 텍스트 업데이트: 첫 줄에 방 이름과 인원, 두 번째 줄에 난이도와 제한 시간
        roomInfo.text = $"{roomName} ({currPlayer}/{maxPlayer})\n" + $"{maxPlayer}명 / {difficulty} / {timeLimit}초";
        Debug.Log("Updated roomInfo.text: " + roomInfo.text);
    }

    public void OnClick()
    {
        //만약 onDelegate 에 무언가 들어있다면 실행
        if (onDelegate != null)
        {
            onDelegate(name);
            //Debug.Log("onDelegate executed for room: " + name);
        }
    }
}
