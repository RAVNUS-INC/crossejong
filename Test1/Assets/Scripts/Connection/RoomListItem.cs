using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


// �� ��� ǥ�� ����(������)
public class RoomListItem : MonoBehaviour
{
    public TMP_Text roomInfo;

    //Ŭ���Ǿ����� ȣ��Ǵ� �Լ�
    public Action<string> onDelegate;


    private void Awake()
    {
        // ������ �� �ؽ�Ʈ(�ڽĿ��) ����
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
        // �ؽ�Ʈ ������Ʈ: ù �ٿ� �� �̸��� �ο�, �� ��° �ٿ� ���̵��� ���� �ð�
        roomInfo.text = $"{roomName} ({currPlayer}/{maxPlayer})\n" + $"{maxPlayer}�� / {difficulty} / {timeLimit}��";
        Debug.Log("Updated roomInfo.text: " + roomInfo.text);
    }

    public void OnClick()
    {
        //���� onDelegate �� ���� ����ִٸ� ����
        if (onDelegate != null)
        {
            onDelegate(name);
            //Debug.Log("onDelegate executed for room: " + name);
        }
    }
}
