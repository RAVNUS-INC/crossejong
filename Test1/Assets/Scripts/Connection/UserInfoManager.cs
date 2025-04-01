using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfoManager : MonoBehaviour
{
    public static UserInfoManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 변경에도 살아있게 유지
        }
        else
        {
            Destroy(gameObject); // 이미 instance가 존재하면 파괴
        }
    }


    public string MyName; // 유저의 displayname
    public int MyActNum; // 유저의 액터넘버
    public int MyImageIndex = -1; // 유저의 프로필 이미지 인덱스
    public int MyBirthYear; // 유저의 출생연도
    public Sprite[] profileImages; // 3가지 기본 제공 프로필 이미지

    // playfab에 저장할 변수 이름
    public const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";
    public const string BIRTH_INDEX_KEY = "BirthIndex";

    //playerprefs에 저장할 내용들(Key)
    public const string DISPLAYNAME_KEY = "DisplayName"; // 유저의 DisplayName
    public const string IMAGEINDEX_KEY = "ImageIndex"; // 유저의 이미지 인덱스
    public const string BIRTHYEAR_KEY = "BirthYear"; // 유저의 출생연도

}
