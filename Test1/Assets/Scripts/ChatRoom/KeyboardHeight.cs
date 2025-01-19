using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardHeight : MonoBehaviour
{
    public RectTransform canvasRect; // Canvas�� RectTransform
    public RectTransform inputFieldParent; // InputField�� ���Ե� �θ� ��ü (��: Panel)

    private Vector2 originalPosition; // Canvas�� �ʱ� ��ġ�� ����
    void Start()
    {
        // Canvas�� �ʱ� ��ġ�� ����
        originalPosition = canvasRect.anchoredPosition;
    }

    void Update()
    {
        #if UNITY_ANDROID || UNITY_IOS
        AdjustCanvasForKeyboard();
        #endif
    }

    private void AdjustCanvasForKeyboard()
    {
        // ���� Ű���� ���̸� ������
        float keyboardHeight = GetKeyboardHeight();

        if (keyboardHeight > 0)
        {
            // Ű���尡 ȭ�鿡 ��Ÿ�� ���
            Vector3 inputWorldPosition = inputFieldParent.position;
            Vector3 inputScreenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, inputWorldPosition);

            // Ű���� ���� InputField�� ���̵��� Canvas ��ġ�� ����
            if (inputScreenPosition.y < keyboardHeight)
            {
                float offset = keyboardHeight - inputScreenPosition.y; //+ 50f; // ���� ���� �߰�
                canvasRect.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + offset);
            }
        }
        else
        {
            // Ű���尡 ������ Canvas�� ���� ��ġ�� ����
            canvasRect.anchoredPosition = originalPosition;
        }
    }

    float GetKeyboardHeight()
    {
        #if UNITY_ANDROID
            return TouchScreenKeyboard.area.height / Screen.dpi * 160f; // �ȵ���̵� Ű���� ���� ���
        #elif UNITY_IOS
            return TouchScreenKeyboard.area.height; // iOS Ű���� ����
        #else
            return 0f; // �ٸ� �÷��������� �⺻��
        #endif
    }
}
