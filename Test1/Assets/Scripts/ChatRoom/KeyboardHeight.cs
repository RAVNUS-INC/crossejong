using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardHeight : MonoBehaviour
{
    public RectTransform canvasRect; // Canvas의 RectTransform
    public RectTransform inputFieldParent; // InputField가 포함된 부모 객체 (예: Panel)

    private Vector2 originalPosition; // Canvas의 초기 위치를 저장
    void Start()
    {
        // Canvas의 초기 위치를 저장
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
        // 현재 키보드 높이를 가져옴
        float keyboardHeight = GetKeyboardHeight();

        if (keyboardHeight > 0)
        {
            // 키보드가 화면에 나타난 경우
            Vector3 inputWorldPosition = inputFieldParent.position;
            Vector3 inputScreenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, inputWorldPosition);

            // 키보드 위로 InputField가 보이도록 Canvas 위치를 조정
            if (inputScreenPosition.y < keyboardHeight)
            {
                float offset = keyboardHeight - inputScreenPosition.y; //+ 50f; // 여유 공간 추가
                canvasRect.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + offset);
            }
        }
        else
        {
            // 키보드가 닫히면 Canvas를 원래 위치로 복구
            canvasRect.anchoredPosition = originalPosition;
        }
    }

    float GetKeyboardHeight()
    {
        #if UNITY_ANDROID
            return TouchScreenKeyboard.area.height / Screen.dpi * 160f; // 안드로이드 키보드 높이 계산
        #elif UNITY_IOS
            return TouchScreenKeyboard.area.height; // iOS 키보드 높이
        #else
            return 0f; // 다른 플랫폼에서는 기본값
        #endif
    }
}
