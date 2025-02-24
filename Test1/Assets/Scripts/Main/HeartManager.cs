using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartManager : MonoBehaviour
{
    public static HeartManager Instance;

    public Image[] heartImages; // ��Ʈ �̹��� �迭
    private int currentHearts = 5; // ���� ��Ʈ ��
    //private Coroutine rechargeCoroutine; // ������ �ڷ�ƾ

    private void Awake()
    {
        if (PlayerPrefs.HasKey("HeartAmount"))
        {
            currentHearts = PlayerPrefs.GetInt("HeartAmount"); //playerprefs�� ����� ��Ʈ���� ������ �ҷ�����
            if (currentHearts < 0)
            {
                currentHearts = 0;
            }
        }
        else //����� ���� ������ �⺻�� 5�� ����
        {
            currentHearts = 5;
        }

        if (Instance == null) Instance = this;
    }

    void Start()
    {
        UpdateHeartUI(currentHearts);
    }

    //public void UseHeart()
    //{
    //    if (currentHearts > 0)
    //    {
    //        currentHearts--;

    //        UpdateHeartUI(currentHearts); //��Ʈ�� ����ϸ� UI ��ȭ �ݿ�

    //        //if (rechargeCoroutine == null) // �������� ���� ���� �ƴ� ���� ����
    //        //{
    //        //    rechargeCoroutine = StartCoroutine(RechargeHearts());
    //        //}
    //        //Debug.Log($"��Ʈ ��� �� ����� ����: {currentHearts}");
    //    }
    //}

    public void UpdateHeartUI(int count)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            Color color = heartImages[i].color;
            color.a = (i < count) ? 1f : 0.3f; // ��Ʈ�� �ִ� ��� ������(1), ���� ��� ���� 30%(0.3)
            heartImages[i].color = color;
        }
    }


    //private IEnumerator RechargeHearts()
    //{
    //    while (currentHearts < heartImages.Length)
    //    {
    //        yield return new WaitForSeconds(180f); // 180�� ��� (������ ����)
    //        currentHearts++;
    //        UpdateHeartUI(currentHearts);
    //    }
    //    rechargeCoroutine = null; // ������ �Ϸ� �� �ڷ�ƾ ����
    //}
}
