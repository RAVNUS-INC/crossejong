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


    public string MyName; // ������ displayname
    public int MyActNum; // ������ ���ͳѹ�
    public int MyImageIndex; // ������ ������ �̹��� �ε���
    public Sprite[] profileImages; // 3���� �⺻ ���� ������ �̹���

    // ���� Ű
    public string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";

    //playerprefs�� ������ �����(Key)
    public string DISPLAYNAME_KEY = "DisplayName"; // ������ DisplayName
    public string IMAGEINDEX_KEY = "ImageIndex"; // ������ �̹��� �ε���

}
