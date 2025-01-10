using UnityEngine;
using UnityEngine.UI;

public class BeforeCard : MonoBehaviour
{
    public Sprite frontSprite; // 카드 앞면 이미지 (Sprite)
    public Sprite backSprite;  // 카드 뒷면 이미지 (Sprite)

    public Image frontImage; // UI의 앞면 Image
    public Image backImage;  // UI의 뒷면 Image

    private bool isFrontVisible = false; // 카드 앞면/뒷면 상태

    // InitializeCard 메서드에서 이미지를 초기화하고 기본적으로 뒷면을 보이도록 설정
    public void InitializeCard(Sprite backSprite, Sprite frontSprite)
    {
        this.backSprite = backSprite;
        this.frontSprite = frontSprite;

        // 뒷면 이미지를 기본으로 설정
        backImage.sprite = this.backSprite;
        frontImage.sprite = this.frontSprite;

        // 처음에는 뒷면만 보이도록 설정
        ShowBack();
    }

    // 앞면을 보이도록 설정
    public void ShowFront()
    {
        isFrontVisible = true;
        backImage.gameObject.SetActive(false);
        frontImage.gameObject.SetActive(true);
    }

    // 뒷면을 보이도록 설정
    public void ShowBack()
    {
        isFrontVisible = false;
        backImage.gameObject.SetActive(true);
        frontImage.gameObject.SetActive(false);
    }

    // 카드를 뒤집는 메서드
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
