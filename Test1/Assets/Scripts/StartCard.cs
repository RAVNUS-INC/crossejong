using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StartCard : MonoBehaviour
{
    public Image cardImage; // ī�� �̹���
    public Sprite backSprite; // �޸� �̹���
    public Sprite[] frontSprites; // �ո� �̹��� �迭
    public Sprite selectedFrontSprite; // ���õ� �ո�
    public float flipDuration = 0.5f; // ������ �ð�

    // Ư�� ī�� �̸� ����Ʈ
    public string[] specialCardNames = { "Asset9", "Asset10", "Asset11", "Asset65", "Asset66" };

    void Start()
    {
        ResetCard();
    }

    public void ResetCard()
    {
        cardImage.sprite = backSprite; // �޸� �̹����� �ʱ�ȭ
        selectedFrontSprite = GetRandomFrontSprite(); // ������ �ո� ����
    }

    Sprite GetRandomFrontSprite()
    {
        // Ư�� ī�带 ������ ������ ī�� ����
        Sprite randomSprite;
        do
        {
            randomSprite = frontSprites[Random.Range(0, frontSprites.Length)];
        } while (IsSpecialCard(randomSprite.name));

        return randomSprite;
    }

    bool IsSpecialCard(string spriteName)
    {
        // Ư�� ī������ Ȯ��
        foreach (string specialName in specialCardNames)
        {
            if (spriteName == specialName) return true;
        }
        return false;
    }

    public void FlipCard()
    {
        // DOTween �ִϸ��̼����� ������
        transform.DOScaleX(0, flipDuration / 2).OnComplete(() =>
        {
            cardImage.sprite = selectedFrontSprite; // �ո����� ����
            transform.DOScaleX(1, flipDuration / 2);
        });
    }
}