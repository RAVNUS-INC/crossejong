using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// �� ��� ǥ�� ����(������)
public class RoomListItem : MonoBehaviour
{
    public Text roomInfo;

    //Ŭ���Ǿ����� ȣ��Ǵ� �Լ�
    public Action<string> onDelegate;

    public void SetInfo(string roomName, int currPlayer, int maxPlayer, string difficulty, int timeLimit)
    {
        name = roomName;

        // �ؽ�Ʈ ������Ʈ: ù �ٿ� �� �̸��� �ο�, �� ��° �ٿ� ���̵��� ���� �ð�
        roomInfo.text = $"{roomName} ({currPlayer}/{maxPlayer})\n" + $"{maxPlayer}�� / {difficulty} / {timeLimit}��";
    }

    public void OnClick()
    {
        //���� onDelegate �� ���� ����ִٸ� ����
        if (onDelegate != null)
        {
            onDelegate(name);
        }
    }
}
