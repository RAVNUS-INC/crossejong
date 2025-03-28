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
            DontDestroyOnLoad(gameObject); // �� ���濡�� ����ְ� ����
        }
        else
        {
            Destroy(gameObject); // �̹� instance�� �����ϸ� �ı�
        }
    }


    public string MyName; // ������ displayname
    public int MyActNum; // ������ ���ͳѹ�
    public int MyImageIndex = -1; // ������ ������ �̹��� �ε���
    public int MyBirthYear; // ������ �������
    public Sprite[] profileImages; // 3���� �⺻ ���� ������ �̹���

    // playfab�� ������ ���� �̸�
    public const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";
    public const string BIRTH_INDEX_KEY = "BirthIndex";

    //playerprefs�� ������ �����(Key)
    public const string DISPLAYNAME_KEY = "DisplayName"; // ������ DisplayName
    public const string IMAGEINDEX_KEY = "ImageIndex"; // ������ �̹��� �ε���
    public const string BIRTHYEAR_KEY = "BirthYear"; // ������ �������

}
