using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public Sprite cardImage; // ī�� �ո� �̹���
    public Sprite backImage; // ī�� �޸� �̹���
    private bool isFaceUp = false; // ī���� ���� ����

    private SpriteRenderer spriteRenderer; // SpriteRenderer ����

    private void Start()
    {
        // ī���� �ʱ� �̹��� ����
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = cardImage; // ó������ �ո����� �ʱ�ȭ
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFaceUp) // ī�尡 �ո��� �� Ŭ���ϸ� ������
        {
            StartCoroutine(FlipAnimation(backImage)); // �޸����� ������ �ִϸ��̼�
        }
        else // ī�尡 �޸��� �� Ŭ���ϸ� �ո����� ���ư���
        {
            StartCoroutine(FlipAnimation(cardImage)); // �ո����� ������ �ִϸ��̼�
        }
    }

    private System.Collections.IEnumerator FlipAnimation(Sprite targetImage)
    {
        float duration = 0.5f; // �ִϸ��̼� ���� �ð�
        Vector3 originalScale = transform.localScale;
        Vector3 flippedScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z); // X�� ����

        // ī�� ������ �ִϸ��̼�
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration; // ���� ����
            transform.localScale = Vector3.Lerp(originalScale, flippedScale, t); // ũ�� ����
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ��������Ʈ ����
        spriteRenderer.sprite = targetImage; // Ŭ���� ���� ��������Ʈ ����
        isFaceUp = !isFaceUp; // ���� ����

        // �ٽ� ���� ���·� ����
        elapsed = 0f; // �ð� �ʱ�ȭ
        while (elapsed < duration)
        {
            float t = elapsed / duration; // ���� ����
            transform.localScale = Vector3.Lerp(flippedScale, originalScale, t); // ũ�� ����
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ���� ���� ����
        transform.localScale = originalScale; // ���� ũ��� ����
    }
}
