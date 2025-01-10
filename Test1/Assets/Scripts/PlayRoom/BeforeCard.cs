using UnityEngine;
using UnityEngine.UI;

public class BeforeCard : MonoBehaviour
{
    public Sprite frontSprite; // ī�� �ո� �̹��� (Sprite)
    public Sprite backSprite;  // ī�� �޸� �̹��� (Sprite)

    public Image frontImage; // UI�� �ո� Image
    public Image backImage;  // UI�� �޸� Image

    private bool isFrontVisible = false; // ī�� �ո�/�޸� ����

    // InitializeCard �޼��忡�� �̹����� �ʱ�ȭ�ϰ� �⺻������ �޸��� ���̵��� ����
    public void InitializeCard(Sprite backSprite, Sprite frontSprite)
    {
        this.backSprite = backSprite;
        this.frontSprite = frontSprite;

        // �޸� �̹����� �⺻���� ����
        backImage.sprite = this.backSprite;
        frontImage.sprite = this.frontSprite;

        // ó������ �޸鸸 ���̵��� ����
        ShowBack();
    }

    // �ո��� ���̵��� ����
    public void ShowFront()
    {
        isFrontVisible = true;
        backImage.gameObject.SetActive(false);
        frontImage.gameObject.SetActive(true);
    }

    // �޸��� ���̵��� ����
    public void ShowBack()
    {
        isFrontVisible = false;
        backImage.gameObject.SetActive(true);
        frontImage.gameObject.SetActive(false);
    }

    // ī�带 ������ �޼���
    public void FlipCard()
    {
        if (isFrontVisible)
        {
            ShowBack();
        }
        else
        {
            ShowFront();
        }
    }
}
