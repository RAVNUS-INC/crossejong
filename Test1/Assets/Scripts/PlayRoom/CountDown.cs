using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스
using System.Collections;

public class Countdown : MonoBehaviour
{
    public TMP_Text countDownText; // TextMeshPro 사용
    public StartCard startCard; // StartCard 참조
    public UserCard userCard;  // UserCard 참조
    public float startDelay = 1f; // 시작 딜레이

    private void Start()
    {
        StartCoroutine(CountDownRoutine());
    }

    private IEnumerator CountDownRoutine()
    {
        int count = 3;

        // 카운트다운 표시
        while (count > 0)
        {
            countDownText.text = count.ToString(); // TMP_Text로 설정
            yield return new WaitForSeconds(1f);
            count--;
        }

        // "시작!" 표시
        countDownText.text = "Start!"; // TMP_Text로 설정
        yield return new WaitForSeconds(startDelay);

        countDownText.gameObject.SetActive(false); // 카운트다운 텍스트 숨김

        // 카드 뒤집기 실행
        if (startCard != null)
        {
            //startCard.FlipCard();
        }

        // UserCard의 카드 생성 실행
        if (userCard != null)
        {
            userCard.CreateCards();
        }
    }
}
