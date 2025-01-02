using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Countdown : MonoBehaviour
{
    public Text countDownText; // ī��Ʈ�ٿ� �ؽ�Ʈ
    public StartCard startCard; // StartCard ����
    public UserCard userCard;  // UserCard ����
    public float startDelay = 1f; // ���� ������

    private void Start()
    {
        StartCoroutine(CountDownRoutine());
    }

    private IEnumerator CountDownRoutine()
    {
        int count = 3;

        // ī��Ʈ�ٿ� ǥ��
        while (count > 0)
        {
            countDownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }

        // "����!" ǥ��
        countDownText.text = "����!";
        yield return new WaitForSeconds(startDelay);

        countDownText.gameObject.SetActive(false); // ī��Ʈ�ٿ� �ؽ�Ʈ ����

        // ī�� ������ ����
        if (startCard != null)
        {
            //startCard.FlipCard();
        }

        // UserCard�� ī�� ���� ����
        if (userCard != null)
        {
            userCard.CreateCards();
        }
    }
}