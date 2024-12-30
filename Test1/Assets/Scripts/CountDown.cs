using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CountDown: MonoBehaviour
{
    public Text countDownText;  // 카운트다운 텍스트
    public GameObject startCard;  // 시작 카드
    public float flipDuration = 0.5f;  // 시작 카드 뒤집기 시간

    void IEnumerator CountDownRoutine()
    {
        startCard.gameObject.SetActive(false);  // 시작 카드 비활성화

        int countDown = 3;

        while (countDown > 0)  // 카운트다운 실행
        {
            countDownText.text = countDown.ToString();  // 텍스트 업데이트
            yield return new WaitForSeconds(1f);  // 1초 대기
            countDown--;
        }

        // 카운트다운 완료 후
        countDown.text = "시작!";  // 최종 메세지
        yield return new WaitForSeconds(0.5f);  // 0.5초 대기

        countDownText.gameObject.SetActive(false);  // 카운트다운 텍스트 비활성화

        startCard.gameObject.SetActive(true);  // 시작 카드 활성화
        FlipCard();  // 카드 뒤집기
    }

    void FlipCard()
    {
        if (startCard == null) return;

        // DOTween을 사용한 뒤집기 애니메이션
        // x축 기준으로 90도 회전 후, 이미지 교체 후 다시 0도로 복귀
        startCard.transform
            .DOScaleX(0, flipDuration / 2)
            .OnComplete(() =>
            {
                // 이미지 교체 또는 상태 변경
                // startCard.GetComponent<Image>().sprite = 다른 이미지;
                startCard.transform.DOScaleX(1, flipDuration / 2);
            });
    }

    void Start()
    {
        StartCoroutine(CountDownRoutine());
    }

    void Update()
    {
        
    }
}
