using System.Collections.Generic;
using UnityEngine;
//사본
public class ChangeLevel : MonoBehaviour
{   //일시적 변수들 (objectmanger에 나중에 값 전달)
    public List<string> cardFrontRed;
    public List<string> cardFrontSpecial;
    public List<string> cardFrontBlack;

    public void ChangeLevelLow()
    {
        cardFrontRed = new List<string>
       {"ㄱ", "ㅇ", "ㅎ" };

        cardFrontBlack = new List<string>
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
        Debug.Log(string.Join(", ", cardFrontBlack));

        cardFrontSpecial = new List<string>
       {"C", "B"};
    }

    public void ChangeLevelMiddle()
    {
        cardFrontRed = new List<string>
       {"ㄱ", "ㅇ", "ㅎ" };

        cardFrontBlack = new List<string>
       {"가", "경", "고", "공", "관", "구", "국", "기",
        "대", "도", "동",
        "라", "로", "리",
        "무",
        "법", "보", "부", "비",
        "사", "산", "상", "서", "성", "소", "수", "스", "시", "신",
        "아", "안", "용", "원", "위", "유", "의", "이", "인", "일",
        "자", "장", "전", "정", "제", "조", "주", "지",
        "해", "화" };
        Debug.Log(string.Join(", ", cardFrontBlack));

        cardFrontSpecial = new List<string>
       {"C", "B"};
    }

    public void ChangeLevelHigh()
    {
        cardFrontRed = new List<string>
       {"ㄱ", "ㅇ", "ㅎ" };

        cardFrontBlack = new List<string>
       {"가", "강", "경", "고", "교", "구", "국", "군", "기", "김",
        "대", "도", "동",
        "리",
        "무", "문",
        "박", "보", "부",
        "사", "산", "상", "서", "선", "성", "수", "시", "신",
        "안", "영", "원", "유", "이", "인", "일",
        "자", "장", "재", "전", "정", "제", "조", "주", "지", "진",
        "천",
        "학", "호", "화"};
        Debug.Log(string.Join(", ", cardFrontBlack));

        cardFrontSpecial = new List<string>
       {"C", "B"};
    }
}
