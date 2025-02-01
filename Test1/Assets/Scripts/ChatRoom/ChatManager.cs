using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Specialized;

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

        //������ ����� ���, �޴� ����� ��������� ũ�� ����� �ؽ�Ʈ ����
        AreaScript Area = Instantiate(isSend ? YellowArea : WhiteArea).GetComponent<AreaScript>();
        Area.transform.SetParent(ContentRect.transform, false);
        Area.BoxRect.sizeDelta = new Vector2(600, Area.BoxRect.sizeDelta.y);
        Area.TextRect.GetComponent<Text>().text = text;
        Fit(Area.BoxRect);


        //�� �� �̻��̸� ũ�⸦ �ٿ����鼭, �� ���� �Ʒ��� �������� �ٷ� �� ũ�⸦ ����
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

        //���� �Ϳ� �б��� ������ ��¥�� �����̸��� ����
        DateTime t = DateTime.Now;
        Area.Time = t.ToString("yyyy-MM-dd-HH-mm");
        Area.User = user; //�̸� ����

        if (imgindex != null)
        {
            Area.UserImage.sprite = UserProfileLoad.profileImages[imgindex.Value]; //�̹��� ��ü
        }
        

        //���� ���� �׻� ���ο� �ð� ����
        int hour = t.Hour;
        if (t.Hour == 0) hour = 12;
        else if (t.Hour > 12) hour -= 12;
        Area.TimeText.text = (t.Hour > 12 ? "����" : "����") + hour + ":" + t.Minute.ToString("D2");

        //���� �Ͱ� ������ ���� �ð�, ���� ���ֱ�
        bool isSame = LastArea != null && LastArea.Time == Area.Time && LastArea.User == Area.User;
        if (isSame) LastArea.TimeText.text = "";
        Area.Tail.SetActive(!isSame);

        //Ÿ���� ���� �Ͱ� ������ �̹���, �̸� ���ֱ�
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

        //��ũ�ѹٰ� ���� �ö� ���¿��� �޽����� ������ �� �Ʒ��� ������ ����
        if (isSend && !isBottom) return;
        Invoke("ScrollDelay", 0.03f);
    }

    // �÷��̾ ����/�������� �� �˸� ����
    public void DisplayUserMessage(string userName, bool isEntering)
    {
        // ���ο� �޽��� ������ ����
        Transform userMessageArea = Instantiate(DateArea).transform;

        // �θ� ��ü(ContentRect)�� �ڽ����� ����
        userMessageArea.SetParent(ContentRect.transform, false);

        // �޽��� ����: ����/���� ���ο� ���� �ٸ��� ǥ��
        string message = isEntering ? $"{userName}���� �����ϼ̽��ϴ�." : $"{userName}���� �����ϼ̽��ϴ�.";

        // �޽��� �ؽ�Ʈ�� ����
        userMessageArea.GetComponent<AreaScript>().DateText.text = message;
    }

    void Fit(RectTransform Rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);

    void ScrollDelay() => scrollBar.value = 0;


}
