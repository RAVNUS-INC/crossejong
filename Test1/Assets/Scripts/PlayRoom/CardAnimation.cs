using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;

public class CardAnimation : MonoBehaviourPun
{
    public Button getCardButton;

    public static CardAnimation instance;

    public CanvasGroup imageCanvasGroup;  // 투명도 제어
    public RectTransform imageTransform; // 이동할 이미지의 RectTransform

    public Transform addStartPos; // 카드 추가 시작 위치
    public Transform RollFinPos; // 나 - 롤백 종료 위치
    //public Transform[] AllPlayerPos; // 플레이어 프로필 위치
    public Transform FieldCenPos; // 보드 중앙 - 롤백 시작 위치
    public Transform playerPos1;
    public Transform playerPos2;
    public Transform playerPos3;
    public Transform playerPos4;
    public Transform playerPos5;

    Vector3 addStartPosV;
    Vector3 RollFinPosV;
    Vector3 FieldCenPosV;
    Vector3 playerPos1V;
    Vector3 playerPos2V;
    Vector3 playerPos3V;
    Vector3 playerPos4V;
    Vector3 playerPos5V;

    public List<Vector3> playerPosList;

    //private Vector3 ZeroPosition = new Vector3(-396, -532, 0); // 처음 위치(카드 추가 버튼)
    //private Vector3 RollPosition = new Vector3(0, 110, 0); // 롤백 초기 위치(보드판의 중앙)

    //// 자신의 actnum+1한 값에 따라 자신의 UI가 위치한 좌표로 카드 이동
    //// 5가지 케이스의 좌표
    //private Vector3[] targetPositions = new Vector3[]
    //{
    //    new Vector3(-396, 936, 0), // 목표 위치-플레이어1t
    //    new Vector3(-228, 936, 0), // 목표 위치-플레이어2t
    //    new Vector3(-56, 936, 0), // 목표 위치-플레이어3t
    //    new Vector3(112, 936, 0), // 목표 위치-플레이어4t
    //    new Vector3(287, 936, 0) // 목표 위치-플레이어5t
    //};

    private float duration = 0.6f; // 이동 시간
    private float fadeDuration = 0.3f; // 투명도 애니메이션 시간

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        addStartPosV = addStartPos.position;
        RollFinPosV = RollFinPos.position;
        FieldCenPosV = FieldCenPos.position;
        playerPos1V = playerPos1.transform.position;
        playerPos2V = playerPos2.transform.position;
        playerPos3V = playerPos3.transform.position;
        playerPos4V = playerPos4.transform.position;
        playerPos5V = playerPos5.transform.position;

        playerPosList = new List<Vector3>()
        {
            playerPos1V, playerPos2V, playerPos3V, playerPos4V, playerPos5V
        };

        //Debug.Log($"추가시작위치: {addStartPosV}");
        //Debug.Log($"롤백끝위치: {RollFinPosV}");
        //Debug.Log($"카드중앙위치: {FieldCenPosV}");
        //Debug.Log($"카드위치: {imageTransform.anchoredPosition}");

        // 초기 상태 비활성화와 투명도 0으로 설정
        imageCanvasGroup.alpha = 0;
        imageTransform.gameObject.SetActive(false);
    }

    [PunRPC]
    public void AddCardAnimation(int userActNum) //누군가 카드를 먹으면 해당 유저를 향한 애니메이션을 취함  ok
    {
        addStartPosV = getCardButton.transform.position;

        // 이미지를 활성화하고 애니메이션 시작
        imageTransform.gameObject.SetActive(true);
        imageTransform.position = addStartPosV; // 위치 초기화

        // 이동과 투명도 증가 (0 → 1)
        imageCanvasGroup.DOFade(1, fadeDuration);

        // 게임 UI내에서의 인덱스 번호가 i와 같다면
        // 이동 애니메이션 수행
        imageTransform.DOAnchorPos(playerPosList[userActNum], duration) // 앵커포지션
            .SetEase(Ease.OutQuad);

        // 이동이 끝날 때 투명도 감소 (1 → 0)
        imageCanvasGroup.DOFade(0, duration).SetDelay(duration - fadeDuration).OnComplete(() =>
        {
            imageCanvasGroup.gameObject.SetActive(false);  // 비활성화
            imageTransform.position = addStartPosV; // 위치 초기화
        });

    }

    [PunRPC]
    public void RollBackCardAnimation(int userActNum) //누군가 카드를 롤백하면 유저에게 되돌아가는 애니메이션을 취함 
    {
        FieldCenPosV = ObjectManager.instance.endDragPosition;

        // 이미지를 활성화하고 애니메이션 시작
        imageTransform.gameObject.SetActive(true);
        imageTransform.position = FieldCenPosV; // 위치 초기화

        // 이동과 투명도 증가 (0 → 1)
        imageCanvasGroup.DOFade(1, fadeDuration);

        // 게임 UI내에서의 인덱스 번호가 i와 같다면
        // 이동 애니메이션 수행
        imageTransform.DOAnchorPos(playerPosList[userActNum], duration) // 앵커포지션
            .SetEase(Ease.OutQuad);

        // 이동이 끝날 때 투명도 감소 (1 → 0)
        imageCanvasGroup.DOFade(0, duration).SetDelay(duration - fadeDuration).OnComplete(() =>
        {
            imageCanvasGroup.gameObject.SetActive(false);  // 비활성화
            imageTransform.position = FieldCenPosV; // 위치 초기화
        });

    }

    public void RollBackCardAnimationUser()
    {
        FieldCenPosV = ObjectManager.instance.endDragPosition;
        RollFinPosV = ObjectManager.instance.startDragPosition;

        // 이미지를 활성화하고 애니메이션 시작
        imageTransform.gameObject.SetActive(true);
        imageTransform.position = FieldCenPosV; // 위치 초기화

        // 이동과 투명도 증가 (0 → 1)
        imageCanvasGroup.DOFade(1, fadeDuration);

        // 게임 UI내에서의 인덱스 번호가 i와 같다면
        // 이동 애니메이션 수행
        imageTransform.DOAnchorPos(RollFinPosV, duration)
            .SetEase(Ease.OutQuad);

        // 이동이 끝날 때 투명도 감소 (1 → 0)
        imageCanvasGroup.DOFade(0, duration).SetDelay(duration - fadeDuration).OnComplete(() =>
        {
            imageCanvasGroup.gameObject.SetActive(false);  // 비활성화
            imageTransform.position = FieldCenPosV; // 위치 초기화
        });
    }

}
