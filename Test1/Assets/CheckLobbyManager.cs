using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckLobbyManager : MonoBehaviour
{
    public static CheckLobbyManager instance;

    public string Beforescene = "Default"; //로비 재접속이 이후부턴 수행되도록


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않음
    }
}
