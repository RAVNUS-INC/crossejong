using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;
using TMPro;

public class CardAnimation : MonoBehaviourPun
{
    public static CardAnimation instance;

    public CanvasGroup imageCanvasGroup;  // ���� ����
    public RectTransform imageTransform; // �̵��� �̹����� RectTransform

    private Vector3 ZeroPosition = new Vector3(-396, -532, 0); // ó�� ��ġ(ī�� �߰� ��ư)
    private Vector3 RollPosition = new Vector3(0, 110, 0); // �ѹ� �ʱ� ��ġ(�������� �߾�)

    // �ڽ��� actnum+1�� ���� ���� �ڽ��� UI�� ��ġ�� ��ǥ�� ī�� �̵�
    // 5���� ���̽��� ��ǥ
    private Vector3[] targetPositions = new Vector3[]
    {
        new Vector3(-396, 936, 0), // ��ǥ ��ġ-�÷��̾�1t
        new Vector3(-228, 936, 0), // ��ǥ ��ġ-�÷��̾�2t
        new Vector3(-56, 936, 0), // ��ǥ ��ġ-�÷��̾�3t
        new Vector3(112, 936, 0), // ��ǥ ��ġ-�÷��̾�4t
        new Vector3(287, 936, 0) // ��ǥ ��ġ-�÷��̾�5t
    };

    private float duration = 0.6f; // �̵� �ð�
    private float fadeDuration = 0.3f; // ���� �ִϸ��̼� �ð�

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // �ʱ� ���� ��Ȱ��ȭ�� ���� 0���� ����
        imageCanvasGroup.alpha = 0;
        imageTransform.gameObject.SetActive(false);
    }

    [PunRPC]
    public void AddCardAnimation(int userActNum) //������ ī�带 ������ ������ �ش� ������ ���� ������ �ִϸ��̼��� ����
    {
        // �̹����� Ȱ��ȭ�ϰ� �ִϸ��̼� ����
        imageTransform.gameObject.SetActive(true);
        imageTransform.anchoredPosition = ZeroPosition; // ��ġ �ʱ�ȭ

        // �̵��� ���� ���� (0 �� 1)
        imageCanvasGroup.DOFade(1, fadeDuration);

        // ���� UI�������� �ε��� ��ȣ�� i�� ���ٸ�
        // �̵� �ִϸ��̼� ����
        imageTransform.DOAnchorPos(targetPositions[userActNum], duration)
            .SetEase(Ease.OutQuad);

        // �̵��� ���� �� ���� ���� (1 �� 0)
        imageCanvasGroup.DOFade(0, duration).SetDelay(duration - fadeDuration).OnComplete(() =>
        {
            imageCanvasGroup.gameObject.SetActive(false);  // ��Ȱ��ȭ
            imageTransform.anchoredPosition = ZeroPosition; // ��ġ �ʱ�ȭ
        });

    }

    [PunRPC]
    public void RollBackCardAnimation(int userActNum) //������ ī�带 �ѹ��ϸ� �������� �ǵ��ư��� �ִϸ��̼��� ����
    {
        // �̹����� Ȱ��ȭ�ϰ� �ִϸ��̼� ����
        imageTransform.gameObject.SetActive(true);
        imageTransform.anchoredPosition = RollPosition; // ��ġ �ʱ�ȭ

        // �̵��� ���� ���� (0 �� 1)
        imageCanvasGroup.DOFade(1, fadeDuration);

        // ���� UI�������� �ε��� ��ȣ�� i�� ���ٸ�
        // �̵� �ִϸ��̼� ����
        imageTransform.DOAnchorPos(targetPositions[userActNum], duration)
            .SetEase(Ease.OutQuad);

        // �̵��� ���� �� ���� ���� (1 �� 0)
        imageCanvasGroup.DOFade(0, duration).SetDelay(duration - fadeDuration).OnComplete(() =>
        {
            imageCanvasGroup.gameObject.SetActive(false);  // ��Ȱ��ȭ
            imageTransform.anchoredPosition = ZeroPosition; // ��ġ �ʱ�ȭ
        });

    }

}
