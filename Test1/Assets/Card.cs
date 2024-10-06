using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public Sprite cardImage; // 카드 앞면 이미지
    public Sprite backImage; // 카드 뒷면 이미지
    private bool isFaceUp = false; // 카드의 현재 상태

    private SpriteRenderer spriteRenderer; // SpriteRenderer 참조

    private void Start()
    {
        // 카드의 초기 이미지 설정
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = cardImage; // 처음에는 앞면으로 초기화
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFaceUp) // 카드가 앞면일 때 클릭하면 뒤집기
        {
            StartCoroutine(FlipAnimation(backImage)); // 뒷면으로 뒤집기 애니메이션
        }
        else // 카드가 뒷면일 때 클릭하면 앞면으로 돌아가기
        {
            StartCoroutine(FlipAnimation(cardImage)); // 앞면으로 뒤집기 애니메이션
        }
    }

    private System.Collections.IEnumerator FlipAnimation(Sprite targetImage)
    {
        float duration = 0.5f; // 애니메이션 지속 시간
        Vector3 originalScale = transform.localScale;
        Vector3 flippedScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z); // X축 반전

        // 카드 뒤집기 애니메이션
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration; // 진행 비율
            transform.localScale = Vector3.Lerp(originalScale, flippedScale, t); // 크기 보간
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 스프라이트 변경
        spriteRenderer.sprite = targetImage; // 클릭에 따라 스프라이트 변경
        isFaceUp = !isFaceUp; // 상태 변경

        // 다시 원래 상태로 복귀
        elapsed = 0f; // 시간 초기화
        while (elapsed < duration)
        {
            float t = elapsed / duration; // 진행 비율
            transform.localScale = Vector3.Lerp(flippedScale, originalScale, t); // 크기 보간
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 최종 상태 설정
        transform.localScale = originalScale; // 원래 크기로 복귀
    }
}
