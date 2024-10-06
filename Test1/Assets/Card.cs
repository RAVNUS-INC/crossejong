using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Image cardImage; // Image 타입의 변수
    public Sprite frontImage; // 카드 앞면 이미지
    public Sprite backImage; // 카드 뒷면 이미지
    private bool isFaceUp = false; // 카드의 현재 상태

    private void Start()
    {
        // 시작할 때 뒷면 이미지로 설정
        cardImage.sprite = backImage;
    }

    void OnMouseDown() // 마우스 클릭 시 호출됩니다.
    {
        if (isFaceUp)
        {
            // 카드가 기본 이미지일 때 뒷면 이미지로 전환
            cardImage.sprite = backImage;
        }
        else
        {
            // 카드가 뒷면 이미지일 때 기본 이미지로 전환
            cardImage.sprite = frontImage;
        }

        // 상태를 반전
        isFaceUp = !isFaceUp;
    }
}
