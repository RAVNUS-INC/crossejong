using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StartCard : MonoBehaviour
{
    public Image cardImage; // 카드 이미지
    public Sprite backSprite; // 뒷면 이미지
    public Sprite[] frontSprites; // 앞면 이미지 배열
    public Sprite selectedFrontSprite; // 선택된 앞면
    public float flipDuration = 0.5f; // 뒤집기 시간

    // 특수 카드 이름 리스트
    public string[] specialCardNames = { "Asset9", "Asset10", "Asset11", "Asset65", "Asset66" };

    void Start()
    {
        ResetCard();
    }

    public void ResetCard()
    {
        cardImage.sprite = backSprite; // 뒷면 이미지로 초기화
        selectedFrontSprite = GetRandomFrontSprite(); // 무작위 앞면 설정
    }

    Sprite GetRandomFrontSprite()
    {
        // 특수 카드를 제외한 무작위 카드 선택
        Sprite randomSprite;
        do
        {
            randomSprite = frontSprites[Random.Range(0, frontSprites.Length)];
        } while (IsSpecialCard(randomSprite.name));

        return randomSprite;
    }

    bool IsSpecialCard(string spriteName)
    {
        // 특수 카드인지 확인
        foreach (string specialName in specialCardNames)
        {
            if (spriteName == specialName) return true;
        }
        return false;
    }

    public void FlipCard()
    {
        // DOTween 애니메이션으로 뒤집기
        transform.DOScaleX(0, flipDuration / 2).OnComplete(() =>
        {
            cardImage.sprite = selectedFrontSprite; // 앞면으로 변경
            transform.DOScaleX(1, flipDuration / 2);
        });
    }
}