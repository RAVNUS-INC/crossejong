using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Specialized;
using TMPro;

public class ChatManager : MonoBehaviour
{
    public UserProfileLoad UserProfileLoad;

    public GameObject YellowArea, WhiteArea, DateArea;
    public RectTransform ContentRect;
    public Scrollbar scrollBar;
    AreaScript LastArea;

    public void Chat(bool isSend, string text, string user, int? imgindex)
    {
        if (text.Trim() == "") return;

        bool isBottom = scrollBar.value <= 0.00001f;

        //보내는 사람은 노랑, 받는 사람은 흰색영역을 크게 만들고 텍스트 대입
        AreaScript Area = Instantiate(isSend ? YellowArea : WhiteArea).GetComponent<AreaScript>();
        Area.transform.SetParent(ContentRect.transform, false);
        Area.BoxRect.sizeDelta = new Vector2(600, Area.BoxRect.sizeDelta.y);
        Area.TextRect.GetComponent<TMP_Text>().text = text;
        Fit(Area.BoxRect);


        //두 줄 이상이면 크기를 줄여가면서, 한 줄이 아래로 내려가면 바로 전 크기를 대입
        float X = Area.TextRect.sizeDelta.x + 42;
        float Y = Area.TextRect.sizeDelta .y;
        if (Y > 49)
        {
            for (int i = 0; i < 200; i++)
            {
                Area.BoxRect.sizeDelta = new Vector2(X - i * 2, Area.BoxRect.sizeDelta.y);
                Fit(Area.BoxRect);

                if (Y != Area.TextRect.sizeDelta.y) { Area.BoxRect.sizeDelta = new Vector2(X - (i * 2) + 2, Y); break; }
            }
        }

        else Area.BoxRect.sizeDelta = new Vector2(X, Y);

        //현재 것에 분까지 나오는 날짜와 유저이름을 대입
        DateTime t = DateTime.Now;
        Area.Time = t.ToString("yyyy-MM-dd-HH-mm");
        Area.User = user; //이름 대입

        if (imgindex != null)
        {
            Area.UserImage.sprite = UserProfileLoad.profileImages[imgindex.Value]; //이미지 교체
        }
        

        //현재 것은 항상 새로운 시간 대입
        int hour = t.Hour;
        if (t.Hour == 0) hour = 12;
        else if (t.Hour > 12) hour -= 12;
        Area.TimeText.text = (t.Hour > 12 ? "오후" : "오전") + hour + ":" + t.Minute.ToString("D2");

        //이전 것과 같으면 이전 시간, 꼬리 없애기
        bool isSame = LastArea != null && LastArea.Time == Area.Time && LastArea.User == Area.User;
        if (isSame) LastArea.TimeText.text = "";
        Area.Tail.SetActive(!isSame);

        //타인이 이전 것과 같으며 이미지, 이름 없애기
        if (!isSend)
        {
            Area.UserImage.gameObject.SetActive(!isSame);
            Area.UserText.gameObject.SetActive(!isSame);
            Area.UserText.text = Area.User;
        }

        Fit(Area.BoxRect);
        Fit(Area.AreaRect);
        Fit(ContentRect);
        LastArea = Area;

        //스크롤바가 위로 올라간 상태에서 메시지를 받으면 맨 아래로 내리지 않음
        if (isSend && !isBottom) return;
        Invoke("ScrollDelay", 0.03f);
    }

    // 플레이어가 입장/퇴장했을 때 알림 띄우기
    public void DisplayUserMessage(string userName, bool isEntering)
    {
        // 새로운 메시지 영역을 생성
        Transform userMessageArea = Instantiate(DateArea).transform;

        // 부모 객체(ContentRect)의 자식으로 설정
        userMessageArea.SetParent(ContentRect.transform, false);

        // 메시지 설정: 입장/퇴장 여부에 따라 다르게 표시
        string message = isEntering ? $"{userName}님이 입장하셨습니다." : $"{userName}님이 퇴장하셨습니다.";

        // 메시지 텍스트를 설정
        userMessageArea.GetComponent<AreaScript>().DateText.text = message;
    }

    void Fit(RectTransform Rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);

    void ScrollDelay() => scrollBar.value = 0;


}
