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
    public GameObject[] panels; // 4���� �г� �迭
    public Button leftArrow;  // ���� ȭ��ǥ ��ư
    public Button rightArrow; // ������ ȭ��ǥ ��ư
    public TMP_Text PageIndex; // ���� �������� ��Ÿ���� �ؽ�Ʈ

    public RectTransform ArrowTransform, ArrowTransformSecond, ArrowTransformFourth; // ������ ȭ��ǥ 1, 2�� RectTransform

    public RectTransform MoveUserCard, WrongUserCard, RandomCard, RollUserCard; // ������ ī���� RectTransform

    public CanvasGroup CenterCard, UserCardListPanel, UserCardListPanelSecond, UserCardListPanelThree; // ������� �� ����ī�� �г� 1, 2

    public CanvasGroup DropCardBtn, RollBackBtn, FullBtn, FullViewArea; // ī������̰� �г� ����� ��, ī�峻��Ϸ��ư�� ������ ��Ÿ��

    public CanvasGroup InputWord; // �Ϸ��ư ������ �׼� �� ��ǲ�� ��Ÿ��

    public CanvasGroup AlarmMsg; // ī�带 �߸� �θ� �˸��޽��� ����

    public CanvasGroup Timer; // Ÿ�̸� ��ü�� �ð��� �ٵǸ� �����

    public CanvasGroup AddCardBtn, RandomCardFade; // ī�� �Դ� �ִϸ��̼� ǥ��

    private float fadeDuration = 1.5f; // ���� �ִϸ��̼� �ð�

    private Vector3 ZeroPosition = new Vector3(620, -832, 0); // ������ ī���� �ʱ� ��ġ
    private Vector3 TargetPosition = new Vector3(441, -537, 0); // ������ ī���� ���� ��ġ

    private Vector3 ZeroPositionWrong = new Vector3(280, -28, 0); // ������ wrongī���� �ʱ� ��ġ
    private Vector3 TargetPositionWrong = new Vector3(440, 267, 0); // ������ wrongī���� ���� ��ġ

    private Vector3 ZeroPositionRandom = new Vector3(100, 228, 0); // ������ randomī���� �ʱ� ��ġ
    private Vector3 TargetPositionRandom = new Vector3(365, -28, 0); // ������ randomī���� ���� ��ġ

    private Vector3 ZeroPositionRoll = new Vector3(450, -831, 0); // ������ rollī���� �ʱ� ��ġ
    private Vector3 TargetPositionRoll = new Vector3(440, -537, 0); // ������ rollī���� ���� ��ġ

    private Vector3 ZeroPositionFourthArrow = new Vector3(624, -511, 0); // ������ ȭ��ǥ4�� �ʱ� ��ġ

    public int currentIndex = 0; // ���� �г� �ε���

    public Image backgroundImage;   // Ÿ�̸� ��� �̹���
    public TextMeshProUGUI timerText;          // �ð� �ؽ�Ʈ
    public float timerDuration = 5f; // Ÿ�̸� �� �ð�


    private Tween[] tweens = new Tween[4]; // 4���� Ʈ���� ������ �迭

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

    public void ResetPageIndex() //�г� �ݱ� ��ư�� ����
    {
        currentIndex = 0;

        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == currentIndex);
        }

        PageIndex.text = $"{currentIndex + 1}/{panels.Length}";

        // ù ��° �гο����� ���� ȭ��ǥ ��Ȱ��ȭ, ������ �гο����� ������ ȭ��ǥ ��Ȱ��ȭ
        leftArrow.interactable = currentIndex > 0;
        rightArrow.interactable = currentIndex < panels.Length - 1;

        StopAndDestroyTweens();
    }

    public void UpdatePanel()
    {

        // ��� �г� ��Ȱ��ȭ �� ���� �ε����� �гθ� Ȱ��ȭ
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

                        // ��ġ �ʱ�ȭ
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

                        // ��ġ �ʱ�ȭ
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
                break; // �ϳ��� �гθ� Ȱ��ȭ�� ������ �����ϰ�, ã���� ���� ����
            }
        }

        // ù ��° �гο����� ���� ȭ��ǥ ��Ȱ��ȭ, ������ �гο����� ������ ȭ��ǥ ��Ȱ��ȭ
        leftArrow.interactable = currentIndex > 0;
        rightArrow.interactable = currentIndex < panels.Length - 1;

    }

    void StopAndDestroyTweens()
    {
        for (int i = 0; i < tweens.Length; i++)
        {
            if (i == currentIndex) continue; // currentIndex�� ��ġ�ϴ� ��� �ǳʶ�

            if (tweens[i] != null && tweens[i].IsActive())
            {
                tweens[i].Kill(); // Ʈ�� ���� �� ����
                tweens[i] = null; // ���� ���� (�޸� ����)
                //Debug.Log($"{i}��° Ʈ�� ���� �� ���� �Ϸ�");
            }
        }
    }

    public void MoveAction1() //������1�� Ȱ��ȭ�Ǿ����� �� ����
    {
        // Sequence ����
        Sequence mySequence = DOTween.Sequence();

        // CenterCard�� ���̵� �� �ִϸ��̼�
        mySequence.Append(CenterCard.DOFade(1, 1f));

        // UserCardListPanel�� ���̵� �� �ִϸ��̼�
        mySequence.Append(UserCardListPanel.DOFade(1, fadeDuration));

        // ArrowTransform Ȱ��ȭ �� �ִϸ��̼�
        mySequence.AppendCallback(() =>
        {
            ArrowTransform.gameObject.SetActive(true); // ȭ��ǥ Ȱ��ȭ
        })
        .AppendCallback(() =>
        {
            // ���� ��ġ ����
            Vector2 startPos = ArrowTransform.anchoredPosition;
            Vector2 targetPos = new Vector2(startPos.x - 500f, startPos.y); // ��ǥ ����

            // ��ǥ �������� �ִϸ��̼� ���� (�������� �̵�)
            ArrowTransform.DOAnchorPos(targetPos, 1f) // ��ǥ �������� �̵�
                .SetEase(Ease.InOutQuad)  // �ε巯�� ���Ӱ� ����
                .OnComplete(() =>
                {
                    // ��ǥ ������ �����ϸ� �ٽ� ���� �������� �̵�
                    ArrowTransform.DOAnchorPos(startPos, 0.7f)  // �ٽ� ���� �������� �̵�
                        .SetEase(Ease.InOutQuad) // �ε巯�� ���Ӱ� ����
                        .OnComplete(() =>
                        {
                            // ���Ʒ� �ݺ� �ִϸ��̼�
                            ArrowTransform.DOAnchorPosY(startPos.y + 20f, 0.5f)
                                .SetEase(Ease.InOutSine) // �ε巯�� ���� & ����
                                .SetLoops(4, LoopType.Yoyo); // ���Ʒ� �ݺ�
                        });
                });
        });

        // ������ ������ Sequence�� tweens[0]�� ����
        tweens[0] = mySequence;
    }

    public void MoveAction2() // ������2�� Ȱ��ȭ�Ǿ����� �� ����
    {
        // Sequence ����
        Sequence mySequence = DOTween.Sequence();

        // �̵� �ִϸ��̼�
        mySequence.Append(MoveUserCard.DOAnchorPos(TargetPosition, 0.5f).SetEase(Ease.OutQuad));

        // ����ī�帮��Ʈ ���� ���� (1 -> 0)
        mySequence.Append(UserCardListPanelSecond.DOFade(0, fadeDuration));

        // ī�峻��Ϸ��ư ���� �� �ִϸ��̼� ����
        mySequence.Append(DropCardBtn.DOFade(1, fadeDuration));

        // ȭ��ǥ �ִϸ��̼� ���� (4�� �ݺ�)
        mySequence.AppendCallback(() =>
        {
            ArrowTransformSecond.gameObject.SetActive(true);  // SetActive ȣ���� �ݹ鿡�� ó��
        });
        mySequence.Append(ArrowTransformSecond.DOAnchorPosY(ArrowTransformSecond.anchoredPosition.y + 20f, 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(4, LoopType.Yoyo));

        // ȭ��ǥ �ִϸ��̼� ���� �� InputWord�� ���� ����
        mySequence.AppendCallback(() =>
        {
            ArrowTransformSecond.gameObject.SetActive(false);
            InputWord.DOFade(1, fadeDuration);
        });

        // Sequence�� tweens[1]�� ����
        tweens[1] = mySequence;
    }

    public void MoveAction3() // ������3�� Ȱ��ȭ�Ǿ����� �� ����
    {
        // Sequence ����
        Sequence mySequence = DOTween.Sequence();

        // �̵� �ִϸ��̼�
        mySequence.Append(WrongUserCard.DOAnchorPos(TargetPositionWrong, 0.5f).SetEase(Ease.OutQuad));

        // ����ī�帮��Ʈ ���� ���� (1 -> 0)
        mySequence.Append(UserCardListPanelThree.DOFade(0, fadeDuration));

        // "�������� �ʴ� �ܾ��Դϴ�" �����޽��� fade
        mySequence.Append(AlarmMsg.DOFade(1, fadeDuration));
        mySequence.Append(AlarmMsg.DOFade(0, fadeDuration));

        // �߸��� ī��� ����Ʈ�� ���ƿ���
        mySequence.Append(WrongUserCard.DOAnchorPos(ZeroPositionWrong, 0.5f).SetEase(Ease.OutQuad));
        mySequence.Append(UserCardListPanelThree.DOFade(1, fadeDuration));

        // Ÿ�̸� 3�� �帣�� �ִϸ��̼� ����
        mySequence.Append(StartTimer());

        // ī��Ա� ��ư����, ī�� �� �� �������� �̵�
        mySequence.Append(AddCardBtn.DOFade(1, fadeDuration));
        mySequence.Append(RandomCardFade.DOFade(1, 0.7f));

        mySequence.Append(RandomCard.DOAnchorPos(TargetPositionRandom, 0.5f).SetEase(Ease.OutQuad));
        mySequence.Append(RandomCardFade.DOFade(0, 1f));

        // Sequence�� tweens[1]�� ����
        tweens[2] = mySequence;
    }

    Sequence StartTimer()
    {
        // 5�� ���� Ÿ�̸� ȿ��
        Sequence timerSequence = DOTween.Sequence();

        timerSequence.Append(Timer.DOFade(1, 1f));

        // �ð� �ݴ� �������� fillAmount�� 1���� 0���� ����
        timerSequence.Join(backgroundImage.DOFillAmount(0, timerDuration).SetEase(Ease.Linear));

        // �ð� �ؽ�Ʈ ��ȭ: 5 -> 4 -> 3 -> 2 -> 1 (���ÿ� ����)
        timerSequence.Join(DOTween.To(() => timerText.text, x => timerText.text = x, "5", timerDuration / 5).SetEase(Ease.Linear));
        timerSequence.Join(DOTween.To(() => timerText.text, x => timerText.text = x, "4", timerDuration / 5).SetEase(Ease.Linear).SetDelay(0.8f));
        timerSequence.Join(DOTween.To(() => timerText.text, x => timerText.text = x, "3", timerDuration / 5).SetEase(Ease.Linear).SetDelay(1f));
        timerSequence.Join(DOTween.To(() => timerText.text, x => timerText.text = x, "2", timerDuration / 5).SetEase(Ease.Linear).SetDelay(1f));
        timerSequence.Join(DOTween.To(() => timerText.text, x => timerText.text = x, "1", timerDuration / 5).SetEase(Ease.Linear).SetDelay(1f));

        // Ÿ�̸� ������ �� �̺�Ʈ
        timerSequence.OnKill(() =>
        {
            Timer.DOFade(0, fadeDuration);  // Ÿ�̸� ���� �� ���̵� �ƿ�
        });

        // Ÿ�̸� ����
        return timerSequence;
    }

    public void MoveAction4() // ������4�� Ȱ��ȭ�Ǿ����� �� ����
    {
        // Sequence ����
        Sequence mySequence = DOTween.Sequence();

        // �̵� �ִϸ��̼�
        mySequence.Append(RollUserCard.DOAnchorPos(TargetPositionRoll, 0.5f).SetEase(Ease.OutQuad));

        // ArrowTransform Ȱ��ȭ �� �ִϸ��̼�
        mySequence.AppendCallback(() =>
        {
            ArrowTransformFourth.gameObject.SetActive(true); // ȭ��ǥ Ȱ��ȭ
        });

        // ù ��° ���Ʒ� �ݺ� �ִϸ��̼�
        mySequence.Append(ArrowTransformFourth.DOAnchorPosY(ArrowTransformFourth.anchoredPosition.y + 20f, 0.5f)
            .SetEase(Ease.InOutSine) // �ε巯�� ���� & ����
            .SetLoops(5, LoopType.Yoyo) // ���Ʒ� �ݺ�
            .OnComplete(() =>
            {
                ArrowTransformFourth.gameObject.SetActive(false); // �ִϸ��̼� �Ϸ� �� ȭ��ǥ ��Ȱ��ȭ
                ArrowTransformFourth.anchoredPosition = ZeroPositionFourthArrow; //��ġ �ʱ�ȭ

                // �߸��� ī��� ����Ʈ�� ���ƿ���, �ѹ� �������
                RollUserCard.DOAnchorPos(ZeroPositionRoll, 0.5f).SetEase(Ease.OutQuad).SetDelay(fadeDuration);
                RollBackBtn.DOFade(0, fadeDuration).SetDelay(2f); // �ѹ� ��ư �������
            }));

        // �� ��° Ʈ���� ���� �߰�
        mySequence.Append(FullBtn.DOFade(1, fadeDuration).SetDelay(3f)); // 2���� ���� �߰�

        mySequence.AppendCallback(() =>
        {
            ArrowTransformFourth.gameObject.SetActive(true); // ȭ��ǥ Ȱ��ȭ
        });

        // �� ��° ���Ʒ� �ݺ� �ִϸ��̼�
        mySequence.Append(ArrowTransformFourth.DOAnchorPosY(ArrowTransformFourth.anchoredPosition.y + 20f, 0.5f)
            .SetEase(Ease.InOutSine) // �ε巯�� ���� & ����
            .SetLoops(5, LoopType.Yoyo) // ���Ʒ� �ݺ�
            .OnComplete(() =>
            {
                ArrowTransformFourth.gameObject.SetActive(false); // �ִϸ��̼� �Ϸ� �� ȭ��ǥ ��Ȱ��ȭ
                ArrowTransformFourth.anchoredPosition = ZeroPositionFourthArrow; //��ġ �ʱ�ȭ

                FullViewArea.DOFade(1, 0.5f).SetDelay(0.5f); // Ǯ�� �����ֱ�
            }));

        // Sequence�� tweens[3]�� ����
        tweens[3] = mySequence;
    }


}
