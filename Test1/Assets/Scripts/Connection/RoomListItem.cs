using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 방 목록 표시 형태(프리팹)
public class RoomListItem : MonoBehaviour
{
    public Text roomInfo;

    //클릭되었을때 호출되는 함수
    public Action<string> onDelegate;

    public void SetInfo(string roomName, int currPlayer, int maxPlayer, string difficulty, int timeLimit)
    {
        name = roomName;
        // 텍스트 업데이트: 첫 줄에 방 이름과 인원, 두 번째 줄에 난이도와 제한 시간
        roomInfo.text = $"{roomName} ({currPlayer}/{maxPlayer})\n" + $"{maxPlayer}명 / {difficulty} / {timeLimit}초";
    }

    public void OnClick()
    {
        //만약 onDelegate 에 무언가 들어있다면 실행
        if (onDelegate != null)
        {
            onDelegate(name);
        }
        ////InputRoomName 찾아오기
        //GameObject go =GameObject.Find("InputRoomName");
        ////찾아온 게임오브젝트에서 InputField 컴포넌트 가져오기
        //InputField inputField = go.GetComponent<InputField>();
        ////가져온 컴포넌트에서 text 값을 나의 이름으로 셋팅하기
        //inputField.text = name;
    }
}
