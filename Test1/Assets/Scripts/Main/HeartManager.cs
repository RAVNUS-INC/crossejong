using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartManager : MonoBehaviour
{
    public static HeartManager Instance;

    public Image[] heartImages; // 하트 이미지 배열
    private int currentHearts = 5; // 현재 하트 수
    //private Coroutine rechargeCoroutine; // 재충전 코루틴

    private void Awake()
    {
        if (PlayerPrefs.HasKey("HeartAmount"))
        {
            currentHearts = PlayerPrefs.GetInt("HeartAmount"); //playerprefs에 저장된 하트값이 있으면 불러오기
            if (currentHearts < 0)
            {
                currentHearts = 0;
            }
        }
        else //저장된 값이 없으면 기본값 5로 설정
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

    //        UpdateHeartUI(currentHearts); //하트를 사용하면 UI 변화 반영

    //        //if (rechargeCoroutine == null) // 재충전이 진행 중이 아닐 때만 시작
    //        //{
    //        //    rechargeCoroutine = StartCoroutine(RechargeHearts());
    //        //}
    //        //Debug.Log($"하트 사용 후 저장된 개수: {currentHearts}");
    //    }
    //}

    public void UpdateHeartUI(int count)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            Color color = heartImages[i].color;
            color.a = (i < count) ? 1f : 0.3f; // 하트가 있는 경우 불투명(1), 없는 경우 투명도 30%(0.3)
            heartImages[i].color = color;
        }
    }


    //private IEnumerator RechargeHearts()
    //{
    //    while (currentHearts < heartImages.Length)
    //    {
    //        yield return new WaitForSeconds(180f); // 180초 대기 (재충전 간격)
    //        currentHearts++;
    //        UpdateHeartUI(currentHearts);
    //    }
    //    rechargeCoroutine = null; // 재충전 완료 후 코루틴 종료
    //}
}
