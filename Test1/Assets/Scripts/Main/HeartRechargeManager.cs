using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeartRechargeManager : MonoBehaviour
{
    public int m_HeartAmount = 5; //보유 하트 개수
    private DateTime m_AppQuitTime = new DateTime(1970, 1, 1).ToLocalTime(); //앱이 종료된 시간을 저장
    private int m_TimerQuitTime = 0; //종료 시점에서 타이머 시간 저장

    private const int MAX_HEART = 5; //하트 최대값
    public int HeartRechargeInterval = 180; //하트 충전 간격(단위:초)
    private Coroutine m_RechargeTimerCoroutine = null; //하트 충전 타이머를 위한 코루틴 변수
    private int m_RechargeRemainTime = 0; //다음 하트 충전까지 남은 시간을 저장

    public TMP_Text timerText;  // Unity UI Text 컴포넌트 참조

    //Unity 오브젝트가 초기화될 때 호출(초기화 수행)
    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        LoadHeartInfo(); //남은 하트 개수 불러오기
        LoadAppQuitTime(); //종료 시간 불러오기
        LoadTimerQuitTime(); //하트 타이머 불러오기
        SetRechargeScheduler(); //남은 시간 계산 및 업데이트
    }
    //게임 초기화, 중간 이탈, 중간 복귀 시 실행되는 함수
    public void OnApplicationFocus(bool value)
    {
        Debug.Log("게임 접속상태: " + value);
        if (value) //앱에 다시 돌아오면 하트 정보와 앱 종료시간을 로드하고 충전 스케쥴러를 설정
        {
            LoadHeartInfo();
            LoadAppQuitTime();
            LoadTimerQuitTime();
            SetRechargeScheduler();
        }
        else //그게 아니라면 현재 하트 정보와 앱 종료 시간, 하트 타이머를 저장
        {
            SaveHeartInfo();
            SaveAppQuitTime();
            SaveTimerQuitTime();
        }
    }
    //게임 종료시 실행되는 함수
    public void OnApplicationQuit()
    {
        Debug.Log("게임이 종료되었습니다");
        SaveHeartInfo(); 
        SaveAppQuitTime();
        SaveTimerQuitTime();
    } //하트 정보와 종료 시간을 저장

    //하트 사용 버튼을 클릭했을 때 실행되는 함수
    public void OnClickUseHeart()
    {
        if (m_HeartAmount > 0) //하트 개수가 1개라도 있을때 클릭 시
        {
            Debug.Log("하트를 사용했습니다");
            UseHeart();
        }
    } 

    //초기화를 위한 함수
    public void Init()
    {
        //PlayerPrefs.DeleteKey("HeartAmount"); // "HeartAmount" 키에 해당하는 데이터 삭제
        m_HeartAmount = 5;
        m_TimerQuitTime = 0;
        m_AppQuitTime = new DateTime(1970, 1, 1).ToLocalTime();
     
    }//하트 개수, 충전까지 남은 시간, 앱 종료 시간을 초기화, 디버그 출력

    //저장된 하트 정보를 로드하는 함수
    public bool LoadHeartInfo()
    {
        Debug.Log("하트 정보를 불러옵니다");
        bool result = false;
        try
        {
            if (PlayerPrefs.HasKey("HeartAmount"))
            {
                m_HeartAmount = PlayerPrefs.GetInt("HeartAmount"); //playerprefs에 저장된 하트값이 있으면 불러오기
                if (m_HeartAmount < 0)
                {
                    m_HeartAmount = 0;
                    //하트 사용 버튼 비활성화 필요
                }
            }
            else //저장된 값이 없으면 기본값 5로 설정
            {
                m_HeartAmount = MAX_HEART;
            }
            Debug.Log("불러온 보유 하트 수: " + m_HeartAmount);
            //ui업데이트 필요
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("하트 정보 불러오기 실패 (" + e.Message + ")");
        }
        return result;
    } //PlayerPrefs에서 HeartAmount 키로 저장된 값을 불러오고, 값이 없다면 최대 하트 개수로 설정


    //현재 하트 정보를 저장하는 함수
    public bool SaveHeartInfo()
    {
        Debug.Log("현재 하트 개수를 저장합니다");
        bool result = false;
        try
        {
            PlayerPrefs.SetInt("HeartAmount", m_HeartAmount);
            PlayerPrefs.Save();
            //현재 하트 개수를 PlayerPrefs에 저장
            Debug.Log("현재 저장된 하트 개수: " + m_HeartAmount);
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("하트 정보 저장 실패 (" + e.Message + ")");
        }
        return result;
    }
    
    //저장된 앱 종료 시간을 로드하는 함수
    public bool LoadAppQuitTime()
    {
        Debug.Log("마지막 종료 시간을 불러옵니다");
        bool result = false;
        try
        {   
            if (PlayerPrefs.HasKey("AppQuitTime"))
            {
                Debug.Log("저장된 시간이 존재합니다");
                var appQuitTime = string.Empty;
                appQuitTime = PlayerPrefs.GetString("AppQuitTime");
                m_AppQuitTime = DateTime.FromBinary(Convert.ToInt64(appQuitTime));
            } //PlayerPrefs에서 AppQuitTime키로 저장된 값을 불러와 DateTime 객체로 변환

            Debug.Log(string.Format("불러온 마지막 저장 시간 : {0}", m_AppQuitTime.ToString()));
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("종료 시간 불러오기 실패 (" + e.Message + ")");
        }
        return result;
    }

    //현재 시간을 앱 종료 시간으로 저장하는 함수
    public bool SaveAppQuitTime()
    {
        Debug.Log("현재 종료 시간을 저장합니다");
        bool result = false;
        try
        {
            var appQuitTime = DateTime.Now.ToLocalTime().ToBinary().ToString();
            PlayerPrefs.SetString("AppQuitTime", appQuitTime);
            PlayerPrefs.Save();
            Debug.Log("현재 종료 시간 : " + DateTime.Now.ToLocalTime().ToString());
            result = true;
        } //현재 시간을 이진 문자열로 변환해 PlayerPrefs에 저장
        catch (System.Exception e)
        {
            Debug.LogError("종료 시간 저장 실패 (" + e.Message + ")");
        }
        return result;
    }

    //저장된 타이머 종료 시간을 로드하는 함수
    public void LoadTimerQuitTime()
    {
        Debug.Log("마지막 타이머 시간을 불러옵니다");
        try
        {
            if (PlayerPrefs.HasKey("TimerQuitTime"))
            {
                m_TimerQuitTime = PlayerPrefs.GetInt("TimerQuitTime");
                Debug.Log("불러온 하트 타이머 시간: " + m_TimerQuitTime + "초");
            }
            else
            {
                m_TimerQuitTime = 0; // 기본 충전 시간으로 설정 (예: 180초)
                Debug.Log("저장된 하트 타이머가 없어 기본값(" + m_TimerQuitTime + "초)으로 설정");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("하트 타이머 불러오기 실패 (" + e.Message + ")");
        }
    }

    //타이머 시간을 저장하는 함수
    public void SaveTimerQuitTime()
    {
        try
        {
            PlayerPrefs.SetInt("TimerQuitTime", m_RechargeRemainTime); // 남은 시간(초 단위) 저장
            PlayerPrefs.Save();
            Debug.Log("하트 충전 타이머 저장: " + m_RechargeRemainTime + "초");
        }
        catch (System.Exception e)
        {
            Debug.LogError("하트 충전 타이머 저장 실패 (" + e.Message + ")");
        }
    }

    //하트 충전 스케쥴러를 저장하는 함수
    public void SetRechargeScheduler(Action onFinish = null)
    {
        // 하트 개수가 5개 이상일 경우 과정을 실행하지 않음
        if (m_HeartAmount >= 5)
        {
            Debug.Log("현재 하트 개수가 5개 이상이므로 충전 스케줄러를 실행하지 않습니다.");
            return; // 5개 이상이면 함수 종료
        }

        if (m_RechargeTimerCoroutine != null)
        {
            StopCoroutine(m_RechargeTimerCoroutine);
        } //이미 실행중인 타이머 코루틴이 있다면 중지

        var timeDifferenceInSec = (int)((DateTime.Now.ToLocalTime() - m_AppQuitTime).TotalSeconds);
        Debug.Log("앱 종료 후 경과 시간 : " + timeDifferenceInSec + "초"); //앱 종료 시간부터 현재까지 시간 차이를 초 단위로 계산

        // 종료 시점에서 남은 충전 시간을 경과 시간과 비교하여 조정
        if (timeDifferenceInSec < m_TimerQuitTime)
        {
            m_TimerQuitTime -= timeDifferenceInSec; // 남은 시간에서 경과 시간만큼 차감
        }
        else
        {
            // 초과된 경우, 추가할 하트 개수 계산
            int extraTime = timeDifferenceInSec - m_TimerQuitTime;
            var heartToAdd = 1 + (extraTime / HeartRechargeInterval); // 기본적으로 1개 추가, 초과 시간 반영
            m_TimerQuitTime = HeartRechargeInterval - (extraTime % HeartRechargeInterval); // 새로운 타이머로 설정
            m_HeartAmount += heartToAdd;
        }

        // 분과 초 계산
        int minutes = m_TimerQuitTime / 60;
        int seconds = m_TimerQuitTime % 60;
        string timeText = string.Format("{0:D2}:{1:D2}", minutes, seconds);
        Debug.Log("다음 하트 충전까지: " + "(" + timeText+ ")");
        
        // 하트 개수가 최대치를 초과하지 않도록 조정
        if (m_HeartAmount >= MAX_HEART)
        {
            m_HeartAmount = MAX_HEART;
            HeartManager.Instance.UpdateHeartUI(m_HeartAmount); // UI 업데이트
            timerText.text = "충전 완료"; // text 업데이트
        }
        else
        {
            timerText.text = timeText; // UI 업데이트
            // 충전 타이머 코루틴 실행
            m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(m_TimerQuitTime, onFinish));
        }
        Debug.Log("현재 하트 개수: " + m_HeartAmount);
    }

    //하트를 1개 증가시키고, 최대치에 도달했는지 확인. 최대치에 도달하지 않았다면 다음 충전 타이머 시작
    private IEnumerator DoRechargeTimer(int reaminTime, Action onFinish = null)
    {
        Debug.Log("하트 충전 실행");
        if (reaminTime <= 0)
        {
            m_RechargeRemainTime = HeartRechargeInterval;
        }
        else
        {
            m_RechargeRemainTime = reaminTime;
        } 
        Debug.Log("다음 하트까지 남은 시간 : " + m_RechargeRemainTime + "초");

        while (m_RechargeRemainTime > 0)
        {
            int minutes = m_RechargeRemainTime / 60;  // 분 계산
            int seconds = m_RechargeRemainTime % 60;  // 초 계산

            // "분:초" 형태로 포맷
            timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);

            //Debug.Log("하트 충전 타이머 : " + m_RechargeRemainTime + "초");
            yield return new WaitForSeconds(1f);
            m_RechargeRemainTime -= 1;
        } //1초마다 남은 시간을 감소시키며 로그 출력

        m_HeartAmount++;
        HeartManager.Instance.UpdateHeartUI(m_HeartAmount); // UI 업데이트

        if (m_HeartAmount >= MAX_HEART)
        {
            m_HeartAmount = MAX_HEART;
            m_RechargeRemainTime = 0;
            // 타이머 종료 후 텍스트 초기화 (옵션)
            timerText.text = "충전 완료";
            Debug.Log("하트가 꽉 찼습니다");
            m_RechargeTimerCoroutine = null;
        }
        else
        {
            m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(HeartRechargeInterval, onFinish));
        }
        Debug.Log("현재 하트 개수 : " + m_HeartAmount);
    } 

    //하트를 사용하는 함수
    public void UseHeart(Action onFinish = null)
    {
        if (m_HeartAmount <= 0)
        {
            return;
        } //하트가 0개 이하면 함수를 종료

        m_HeartAmount--; //그게 아니면 하트를 1개 감소
        HeartManager.Instance.UpdateHeartUI(m_HeartAmount); //UI 업데이트
        Debug.Log("현재 하트 개수 : " + m_HeartAmount);

        // 기존 충전 타이머가 실행 중이라면 중지
        if (m_RechargeTimerCoroutine != null)
        {
            StopCoroutine(m_RechargeTimerCoroutine);
            m_RechargeTimerCoroutine = null;
        }

        // 새로운 충전 타이머 시작 (180초부터 다시 시작)
        m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(HeartRechargeInterval));

        if (onFinish != null)
        {
            onFinish();
        }
    }
}
