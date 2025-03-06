using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfoManager : MonoBehaviour
{
    public static UserInfoManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(transform.parent.gameObject);
    }


    public string MyName; // 유저의 displayname
    public int MyActNum; // 유저의 액터넘버
    public int MyImageIndex; // 유저의 프로필 이미지 인덱스
    public Sprite[] profileImages; // 3가지 기본 제공 프로필 이미지

    // 저장 키
    public string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";

    //playerprefs에 저장할 내용들(Key)
    public string DISPLAYNAME_KEY = "DisplayName"; // 유저의 DisplayName
    public string IMAGEINDEX_KEY = "ImageIndex"; // 유저의 이미지 인덱스

}
