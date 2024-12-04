using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Image cardImage; // Image Ÿ���� ����
    public Sprite frontImage; // ī�� �ո� �̹���
    public Sprite backImage; // ī�� �޸� �̹���
    private bool isFaceUp = false; // ī���� ���� ����

    private void Start()
    {
        // ������ �� �޸� �̹����� ����
        cardImage.sprite = backImage;
    }

    void OnMouseDown() // ���콺 Ŭ�� �� ȣ��˴ϴ�.
    {
        if (isFaceUp)
        {
            // ī�尡 �⺻ �̹����� �� �޸� �̹����� ��ȯ
            cardImage.sprite = backImage;
        }
        else
        {
            // ī�尡 �޸� �̹����� �� �⺻ �̹����� ��ȯ
            cardImage.sprite = frontImage;
        }

        // ���¸� ����
        isFaceUp = !isFaceUp;
    }
}
