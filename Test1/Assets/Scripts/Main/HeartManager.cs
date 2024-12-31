using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartManager : MonoBehaviour
{
    public Image[] heartImages; // ��Ʈ �̹��� �迭
    private int currentHearts = 5; // ���� ��Ʈ ��
    private Coroutine rechargeCoroutine; // ������ �ڷ�ƾ

    void Start()
    {
        UpdateHeartUI();
    }

    public void UseHeart()
    {
        if (currentHearts > 0)
        {
            currentHearts--;
            UpdateHeartUI();
            if (rechargeCoroutine == null) // �������� ���� ���� �ƴ� ���� ����
            {
                rechargeCoroutine = StartCoroutine(RechargeHearts());
            }
        }
    }

    private void UpdateHeartUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = (i < currentHearts); // ��Ʈ�� ǥ���ϰų� ����
        }
    }

    private IEnumerator RechargeHearts()
    {
        while (currentHearts < heartImages.Length)
        {
            yield return new WaitForSeconds(180f); // 180�� ��� (������ ����)
            currentHearts++;
            UpdateHeartUI();
        }
        rechargeCoroutine = null; // ������ �Ϸ� �� �ڷ�ƾ ����
    }
}
