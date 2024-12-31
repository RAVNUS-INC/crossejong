using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartManager : MonoBehaviour
{
    public Image[] heartImages; // 하트 이미지 배열
    private int currentHearts = 5; // 현재 하트 수
    private Coroutine rechargeCoroutine; // 재충전 코루틴

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
            if (rechargeCoroutine == null) // 재충전이 진행 중이 아닐 때만 시작
            {
                rechargeCoroutine = StartCoroutine(RechargeHearts());
            }
        }
    }

    private void UpdateHeartUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = (i < currentHearts); // 하트를 표시하거나 숨김
        }
    }

    private IEnumerator RechargeHearts()
    {
        while (currentHearts < heartImages.Length)
        {
            yield return new WaitForSeconds(180f); // 180초 대기 (재충전 간격)
            currentHearts++;
            UpdateHeartUI();
        }
        rechargeCoroutine = null; // 재충전 완료 후 코루틴 종료
    }
}
