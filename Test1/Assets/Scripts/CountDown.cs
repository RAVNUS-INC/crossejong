using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CountDown: MonoBehaviour
{
    public Text countDownText;  // ī��Ʈ�ٿ� �ؽ�Ʈ
    public GameObject startCard;  // ���� ī��
    public float flipDuration = 0.5f;  // ���� ī�� ������ �ð�

    void IEnumerator CountDownRoutine()
    {
        startCard.gameObject.SetActive(false);  // ���� ī�� ��Ȱ��ȭ

        int countDown = 3;

        while (countDown > 0)  // ī��Ʈ�ٿ� ����
        {
            countDownText.text = countDown.ToString();  // �ؽ�Ʈ ������Ʈ
            yield return new WaitForSeconds(1f);  // 1�� ���
            countDown--;
        }

        // ī��Ʈ�ٿ� �Ϸ� ��
        countDown.text = "����!";  // ���� �޼���
        yield return new WaitForSeconds(0.5f);  // 0.5�� ���

        countDownText.gameObject.SetActive(false);  // ī��Ʈ�ٿ� �ؽ�Ʈ ��Ȱ��ȭ

        startCard.gameObject.SetActive(true);  // ���� ī�� Ȱ��ȭ
        FlipCard();  // ī�� ������
    }

    void FlipCard()
    {
        if (startCard == null) return;

        // DOTween�� ����� ������ �ִϸ��̼�
        // x�� �������� 90�� ȸ�� ��, �̹��� ��ü �� �ٽ� 0���� ����
        startCard.transform
            .DOScaleX(0, flipDuration / 2)
            .OnComplete(() =>
            {
                // �̹��� ��ü �Ǵ� ���� ����
                // startCard.GetComponent<Image>().sprite = �ٸ� �̹���;
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
