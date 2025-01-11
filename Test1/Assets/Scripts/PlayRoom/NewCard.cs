using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCard : MonoBehaviour
{
    public List<string> cardFrontRed = new List<string>
       {"ㄱ", "ㅇ", "ㅎ" };

    public List<string> cardFrontBlack = new List<string>
       {"가", "거", "고", "구", "그", "금", "기",
        "나",
        "다", "대", "도", "동", "드",
        "라", "로", "리",
        "마",
        "보", "부", "비",
        "사", "상", "생", "소", "수", "스", "시", "식",
        "아", "안", "어", "오", "요", "우", "음", "이", "인", "일",
        "자", "장", "전", "정", "제", "주", "지", "진",
        "하", "한", "해" };

    public List<string> cardFrontSpecial = new List<string>
       {"컬러", "흑백"};

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
