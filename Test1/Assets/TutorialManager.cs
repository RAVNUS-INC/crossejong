using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] panels; // 4개의 패널 배열
    public Button leftArrow;  // 왼쪽 화살표 버튼
    public Button rightArrow; // 오른쪽 화살표 버튼
    public TMP_Text PageIndex; // 현재 페이지를 나타내는 텍스트

    public RectTransform ArrowTransform, ArrowTransformSecond, ArrowTransformFourth; // 움직일 화살표 1, 2의 RectTransform

    public RectTransform MoveUserCard, WrongUserCard, RandomCard, RollUserCard; // 움직일 카드의 RectTransform

    public CanvasGroup CenterCard, UserCardListPanel, UserCardListPanelSecond, UserCardListPanelThree; // 사라져야 할 유저카드 패널 1, 2

    public CanvasGroup DropCardBtn, RollBackBtn, FullBtn, FullViewArea; // 카드움직이고 패널 사라진 뒤, 카드내기완료버튼이 서서히 나타남

    public CanvasGroup InputWord; // 완료버튼 누르는 액션 뒤 인풋이 나타남

    public CanvasGroup AlarmMsg; // 카드를 잘못 두면 알림메시지 등장

    public CanvasGroup Timer; // 타이머 객체는 시간이 다되면 사라짐

    public CanvasGroup AddCardBtn, RandomCardFade; // 카드 먹는 애니메이션 표현

    private float fadeDuration = 1.5f; // 투명도 애니메이션 시간

    private Vector3 ZeroPosition = new Vector3(620, -832, 0); // 움직일 카드의 초기 위치
    private Vector3 TargetPosition = new Vector3(441, -537, 0); // 움직일 카드의 최종 위치

    private Vector3 ZeroPositionWrong = new Vector3(280, -28, 0); // 움직일 wrong카드의 초기 위치
    private Vector3 TargetPositionWrong = new Vector3(440, 267, 0); // 움직일 wrong카드의 최종 위치

    private Vector3 ZeroPositionRandom = new Vector3(100, 228, 0); // 움직일 random카드의 초기 위치
    private Vector3 TargetPositionRandom = new Vector3(365, -28, 0); // 움직일 random카드의 최종 위치

    private Vector3 ZeroPositionRoll = new Vector3(450, -831, 0); // 움직일 roll카드의 초기 위치
    private Vector3 TargetPositionRoll = new Vector3(440, -537, 0); // 움직일 roll카드의 최종 위치

    private Vector3 ZeroPositionFourthArrow = new Vector3(624, -511, 0); // 움직일 화살표4의 초기 위치

    public int currentIndex = 0; // 현재 패널 인덱스

    public Image backgroundImage;   // 타이머 배경 이미지
    public TextMeshProUGUI timerText;          // 시간 텍스트
    public float timerDuration = 5f; // 타이머 총 시간


    private Tween[] tweens = new Tween[4]; // 4개의 트윈을 저장할 배열

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        leftArrow.onClick.AddListener(Previous);
        rightArrow.onClick.AddListener(Next);

        PageIndex.text = $"{currentIndex + 1}/{panels.Length}";
    }

    void Previous()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            PageIndex.text = $"{currentIndex+1}/{panels.Length}";
            
            UpdatePanel();
        }
    }

    void Next()
    {
        if (currentIndex < panels.Length - 1)
        {
            currentIndex++;
            PageIndex.text = $"{currentIndex+1}/{panels.Length}";
            
            UpdatePanel();
        }
    }

    public void ResetPageIndex() //패널 닫기 버튼에 연결
    {
        currentIndex = 0;

        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == currentIndex);
        }

        PageIndex.text = $"{currentIndex + 1}/{panels.Length}";

        // 첫 번째 패널에서는 왼쪽 화살표 비활성화, 마지막 패널에서는 오른쪽 화살표 비활성화
        leftArrow.interactable = currentIndex > 0;
        rightArrow.interactable = currentIndex < panels.Length - 1;

        StopAndDestroyTweens();
    }

    public void UpdatePanel()
    {

        // 모든 패널 비활성화 후 현재 인덱스의 패널만 활성화
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == currentIndex);
        }

        StopAndDestroyTweens();

        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].activeSelf)
            {
                switch (i)
                {
                    case 2:
                        CenterCard.alpha = 0;
                        UserCardListPanel.alpha = 0;
                        ArrowTransform.gameObject.SetActive(false);
                        MoveAction1();
                        break;

                    case 3:
                        InputWord.alpha = 0;
                        UserCardListPanelSecond.alpha = 1;
                        UserCardListPanelSecond.gameObject.SetActive(true);
                        DropCardBtn.alpha = 0;

                        // 위치 초기화
                        MoveUserCard.anchoredPosition = ZeroPosition;

                        MoveAction2();
                        break;

                    case 4:

                        UserCardListPanelThree.alpha = 1;
                        WrongUserCard.anchoredPosition = ZeroPositionWrong;
                        AlarmMsg.alpha = 0;
                        AddCardBtn.alpha = 0;
                        RandomCardFade.alpha = 0;
                        backgroundImage.fillAmount = 1;

                        Timer.alpha = 0;
                        timerText.alpha = 1;
                        timerText.text = "5";

                        // 위치 초기화
                        WrongUserCard.anchoredPosition = ZeroPositionWrong;
                        RandomCard.anchoredPosition = ZeroPositionRandom;

                        MoveAction3();
                        break;

                    case 5:

                        RollUserCard.anchoredPosition = ZeroPositionRoll;
                        ArrowTransformFourth.gameObject.SetActive(false);
                        RollBackBtn.alpha = 1;
                        FullBtn.alpha = 0;
                        FullViewArea.alpha = 0;

                        MoveAction4();
                        break;

                    default:
                        break;
                }
                break; // 하나의 패널만 활성화될 것으로 가정하고, 찾으면 루프 종료
            }
        }

        // 첫 번째 패널에서는 왼쪽 화살표 비활성화, 마지막 패널에서는 오른쪽 화살표 비활성화
        leftArrow.interactable = currentIndex > 0;
        rightArrow.interactable = currentIndex < panels.Length - 1;

    }

    void StopAndDestroyTweens()
    {
        for (int i = 0; i < tweens.Length; i++)
        {
            if (i == currentIndex) continue; // currentIndex와 일치하는 경우 건너뜀

            if (tweens[i] != null && tweens[i].IsActive())
            {
                tweens[i].Kill(); // 트윈 중지 및 제거
                tweens[i] = null; // 참조 제거 (메모리 관리)
                //Debug.Log($"{i}번째 트윈 중지 및 제거 완료");
            }
        }
    }

    public void MoveAction1() //페이지1이 활성화되어있을 때 수행
    {
        // Sequence 생성
        Sequence mySequence = DOTween.Sequence();

        // CenterCard의 페이드 인 애니메이션
        mySequence.Append(CenterCard.DOFade(1, 1f));

        // UserCardListPanel의 페이드 인 애니메이션
        mySequence.Append(UserCardListPanel.DOFade(1, fadeDuration));

        // ArrowTransform 활성화 후 애니메이션
        mySequence.AppendCallback(() =>
        {
            ArrowTransform.gameObject.SetActive(true); // 화살표 활성화
        })
        .AppendCallback(() =>
        {
            // 시작 위치 저장
            Vector2 startPos = ArrowTransform.anchoredPosition;
            Vector2 targetPos = new Vector2(startPos.x - 500f, startPos.y); // 목표 지점

            // 목표 지점으로 애니메이션 수행 (좌측으로 이동)
            ArrowTransform.DOAnchorPos(targetPos, 1f) // 목표 지점으로 이동
                .SetEase(Ease.InOutQuad)  // 부드러운 가속과 감속
                .OnComplete(() =>
                {
                    // 목표 지점에 도달하면 다시 시작 지점으로 이동
                    ArrowTransform.DOAnchorPos(startPos, 0.7f)  // 다시 시작 지점으로 이동
                        .SetEase(Ease.InOutQuad) // 부드러운 가속과 감속
                        .OnComplete(() =>
                        {
                            // 위아래 반복 애니메이션
                            ArrowTransform.DOAnchorPosY(startPos.y + 20f, 0.5f)
                                .SetEase(Ease.InOutSine) // 부드러운 가속 & 감속
                                .SetLoops(4, LoopType.Yoyo); // 위아래 반복
                        });
                });
        });

        // 위에서 생성한 Sequence를 tweens[0]에 저장
        tweens[0] = mySequence;
    }

    public void MoveAction2() // 페이지2가 활성화되어있을 때 수행
    {
        // Sequence 생성
        Sequence mySequence = DOTween.Sequence();

        // 이동 애니메이션
        mySequence.Append(MoveUserCard.DOAnchorPos(TargetPosition, 0.5f).SetEase(Ease.OutQuad));

        // 유저카드리스트 투명도 감소 (1 -> 0)
        mySequence.Append(UserCardListPanelSecond.DOFade(0, fadeDuration));

        // 카드내기완료버튼 등장 후 애니메이션 실행
        mySequence.Append(DropCardBtn.DOFade(1, fadeDuration));

        // 화살표 애니메이션 실행 (4번 반복)
        mySequence.AppendCallback(() =>
        {
            ArrowTransformSecond.gameObject.SetActive(true);  // SetActive 호출은 콜백에서 처리
        });
        mySequence.Append(ArrowTransformSecond.DOAnchorPosY(ArrowTransformSecond.anchoredPosition.y + 20f, 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(4, LoopType.Yoyo));

        // 화살표 애니메이션 끝난 후 InputWord의 투명도 증가
        mySequence.AppendCallback(() =>
        {
            ArrowTransformSecond.gameObject.SetActive(false);
            InputWord.DOFade(1, fadeDuration);
        });

        // Sequence를 tweens[1]에 저장
        tweens[1] = mySequence;
    }

    public void MoveAction3() // 페이지3가 활성화되어있을 때 수행
    {
        // Sequence 생성
        Sequence mySequence = DOTween.Sequence();

        // 이동 애니메이션
        mySequence.Append(WrongUserCard.DOAnchorPos(TargetPositionWrong, 0.5f).SetEase(Ease.OutQuad));

        // 유저카드리스트 투명도 감소 (1 -> 0)
        mySequence.Append(UserCardListPanelThree.DOFade(0, fadeDuration));

        // "존재하지 않는 단어입니다" 오류메시지 fade
        mySequence.Append(AlarmMsg.DOFade(1, fadeDuration));
        mySequence.Append(AlarmMsg.DOFade(0, fadeDuration));

        // 잘못낸 카드는 리스트로 돌아오기
        mySequence.Append(WrongUserCard.DOAnchorPos(ZeroPositionWrong, 0.5f).SetEase(Ease.OutQuad));
        mySequence.Append(UserCardListPanelThree.DOFade(1, fadeDuration));

        // 타이머 3초 흐르는 애니메이션 수행
        mySequence.Append(StartTimer());

        // 카드먹기 버튼등장, 카드 한 장 유저에게 이동
        mySequence.Append(AddCardBtn.DOFade(1, fadeDuration));
        mySequence.Append(RandomCardFade.DOFade(1, 0.7f));

        mySequence.Append(RandomCard.DOAnchorPos(TargetPositionRandom, 0.5f).SetEase(Ease.OutQuad));
        mySequence.Append(RandomCardFade.DOFade(0, 1f));

        // Sequence를 tweens[1]에 저장
        tweens[2] = mySequence;
    }

    Sequence StartTimer()
    {
        // 5초 동안 타이머 효과
        Sequence timerSequence = DOTween.Sequence();

        timerSequence.Append(Timer.DOFade(1, 1f));

        // 시계 반대 방향으로 fillAmount가 1에서 0으로 감소
        timerSequence.Join(backgroundImage.DOFillAmount(0, timerDuration).SetEase(Ease.Linear));

        // 시간 텍스트 변화: 5 -> 4 -> 3 -> 2 -> 1 (동시에 진행)
        timerSequence.Join(DOTween.To(() => timerText.text, x => timerText.text = x, "5", timerDuration / 5).SetEase(Ease.Linear));
        timerSequence.Join(DOTween.To(() => timerText.text, x => timerText.text = x, "4", timerDuration / 5).SetEase(Ease.Linear).SetDelay(0.8f));
        timerSequence.Join(DOTween.To(() => timerText.text, x => timerText.text = x, "3", timerDuration / 5).SetEase(Ease.Linear).SetDelay(1f));
        timerSequence.Join(DOTween.To(() => timerText.text, x => timerText.text = x, "2", timerDuration / 5).SetEase(Ease.Linear).SetDelay(1f));
        timerSequence.Join(DOTween.To(() => timerText.text, x => timerText.text = x, "1", timerDuration / 5).SetEase(Ease.Linear).SetDelay(1f));

        // 타이머 끝났을 때 이벤트
        timerSequence.OnKill(() =>
        {
            Timer.DOFade(0, fadeDuration);  // 타이머 종료 시 페이드 아웃
        });

        // 타이머 실행
        return timerSequence;
    }

    public void MoveAction4() // 페이지4가 활성화되어있을 때 수행
    {
        // Sequence 생성
        Sequence mySequence = DOTween.Sequence();

        // 이동 애니메이션
        mySequence.Append(RollUserCard.DOAnchorPos(TargetPositionRoll, 0.5f).SetEase(Ease.OutQuad));

        // ArrowTransform 활성화 후 애니메이션
        mySequence.AppendCallback(() =>
        {
            ArrowTransformFourth.gameObject.SetActive(true); // 화살표 활성화
        });

        // 첫 번째 위아래 반복 애니메이션
        mySequence.Append(ArrowTransformFourth.DOAnchorPosY(ArrowTransformFourth.anchoredPosition.y + 20f, 0.5f)
            .SetEase(Ease.InOutSine) // 부드러운 가속 & 감속
            .SetLoops(5, LoopType.Yoyo) // 위아래 반복
            .OnComplete(() =>
            {
                ArrowTransformFourth.gameObject.SetActive(false); // 애니메이션 완료 후 화살표 비활성화
                ArrowTransformFourth.anchoredPosition = ZeroPositionFourthArrow; //위치 초기화

                // 잘못낸 카드는 리스트로 돌아오기, 롤백 사라지기
                RollUserCard.DOAnchorPos(ZeroPositionRoll, 0.5f).SetEase(Ease.OutQuad).SetDelay(fadeDuration);
                RollBackBtn.DOFade(0, fadeDuration).SetDelay(2f); // 롤백 버튼 사라지기
            }));

        // 두 번째 트윈에 지연 추가
        mySequence.Append(FullBtn.DOFade(1, fadeDuration).SetDelay(3f)); // 2초의 지연 추가

        mySequence.AppendCallback(() =>
        {
            ArrowTransformFourth.gameObject.SetActive(true); // 화살표 활성화
        });

        // 두 번째 위아래 반복 애니메이션
        mySequence.Append(ArrowTransformFourth.DOAnchorPosY(ArrowTransformFourth.anchoredPosition.y + 20f, 0.5f)
            .SetEase(Ease.InOutSine) // 부드러운 가속 & 감속
            .SetLoops(5, LoopType.Yoyo) // 위아래 반복
            .OnComplete(() =>
            {
                ArrowTransformFourth.gameObject.SetActive(false); // 애니메이션 완료 후 화살표 비활성화
                ArrowTransformFourth.anchoredPosition = ZeroPositionFourthArrow; //위치 초기화

                FullViewArea.DOFade(1, 0.5f).SetDelay(0.5f); // 풀뷰 보여주기
            }));

        // Sequence를 tweens[3]에 저장
        tweens[3] = mySequence;
    }


}
