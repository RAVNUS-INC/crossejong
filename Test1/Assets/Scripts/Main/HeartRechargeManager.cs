using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartRechargeManager : MonoBehaviour
{
    public int m_HeartAmount = 0; //보유 하트 개수
    private DateTime m_AppQuitTime = new DateTime(1970, 1, 1).ToLocalTime(); //앱이 종료된 시간을 저장
    private const int MAX_HEART = 5; //하트 최대값
    public int HeartRechargeInterval = 0; //하트 충전 간격(단위:초)
    private Coroutine m_RechargeTimerCoroutine = null; //하트 충전 타이머를 위한 코루틴 변수
    private int m_RechargeRemainTime = 0; //다음 하트 충전까지 남은 시간을 저장

    //Unity 오브젝트가 초기화될 때 호출(초기화 수행)
    private void Awake()
    {
        Init();
    }
    //게임 초기화, 중간 이탈, 중간 복귀 시 실행되는 함수
    public void OnApplicationFocus(bool value)
    {
        Debug.Log("OnApplicationFocus() : " + value);
        if (value) //앱에 다시 돌아오면 하트 정보와 앱 종료시간을 로드하고 충전 스케쥴러를 설정
        {
            LoadHeartInfo();
            LoadAppQuitTime();
            SetRechargeScheduler();
        }
        else //그게 아니라면 현재 하트 정보와 앱 종료 시간을 저장
        {
            SaveHeartInfo();
            SaveAppQuitTime();
        }
    }
    //게임 종료시 실행되는 함수
    public void OnApplicationQuit()
    {
        Debug.Log("GoodsRechargeTester: OnApplicationQuit()");
        SaveHeartInfo(); 
        SaveAppQuitTime();
    } //하트 정보와 종료 시간을 저장

    //하트 사용 버튼을 클릭했을 때 실행되는 함수
    public void OnClickUseHeart()
    {
        Debug.Log("OnClickUseHeart");
        UseHeart();
    } //하트를 사용

    //초기화를 위한 함수
    public void Init()
    {
        m_HeartAmount = 0;
        m_RechargeRemainTime = 0;
        m_AppQuitTime = new DateTime(1970, 1, 1).ToLocalTime();
        //Debug.Log("heartRechargeTimer : " + m_RechargeRemainTime + "s");
    }//하트 개수, 충전까지 남은 시간, 앱 종료 시간을 초기화, 디버그 출력

    //저장된 하트 정보를 로드하는 함수
    public bool LoadHeartInfo()
    {
        Debug.Log("LoadHeartInfo");
        bool result = false;
        try
        {
            if (PlayerPrefs.HasKey("HeartAmount"))
            {
                m_HeartAmount = PlayerPrefs.GetInt("HeartAmount");
                if (m_HeartAmount < 0)
                {
                    m_HeartAmount = 0;
                }
            }
            else
            {
                m_HeartAmount = MAX_HEART;
            }
            Debug.Log("Loaded HeartAmount : " + m_HeartAmount);
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadHeartInfo Failed (" + e.Message + ")");
        }
        return result;
    } //PlayerPrefs에서 HeartAmount 키로 저장된 값을 불러오고, 값이 없다면 최대 하트 개수로 설정


    //현재 하트 정보를 저장하는 함수
    public bool SaveHeartInfo()
    {
        Debug.Log("SaveHeartInfo");
        bool result = false;
        try
        {
            PlayerPrefs.SetInt("HeartAmount", m_HeartAmount);
            PlayerPrefs.Save();
            //현재 하트 개수를 PlayerPrefs에 저장
            Debug.Log("Saved HeartAmount : " + m_HeartAmount);
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveHeartInfo Failed (" + e.Message + ")");
        }
        return result;
    }
    
    //저장된 앱 종료 시간을 로드하는 함수
    public bool LoadAppQuitTime()
    {
        Debug.Log("LoadAppQuitTime");
        bool result = false;
        try
        {   
            if (PlayerPrefs.HasKey("AppQuitTime"))
            {
                Debug.Log("PlayerPrefs has key : AppQuitTime");
                var appQuitTime = string.Empty;
                appQuitTime = PlayerPrefs.GetString("AppQuitTime");
                m_AppQuitTime = DateTime.FromBinary(Convert.ToInt64(appQuitTime));
            } //PlayerPrefs에서 AppQuitTime키로 저장된 값을 불러와 DateTime 객체로 변환

            Debug.Log(string.Format("Loaded AppQuitTime : {0}", m_AppQuitTime.ToString()));
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadAppQuitTime Failed (" + e.Message + ")");
        }
        return result;
    }

    //현재 시간을 앱 종료 시간으로 저장하는 함수
    public bool SaveAppQuitTime()
    {
        Debug.Log("SaveAppQuitTime");
        bool result = false;
        try
        {
            var appQuitTime = DateTime.Now.ToLocalTime().ToBinary().ToString();
            PlayerPrefs.SetString("AppQuitTime", appQuitTime);
            PlayerPrefs.Save();
            Debug.Log("Saved AppQuitTime : " + DateTime.Now.ToLocalTime().ToString());
            result = true;
        } //현재 시간을 이진 문자열로 변환해 PlayerPrefs에 저장
        catch (System.Exception e)
        {
            Debug.LogError("SaveAppQuitTime Failed (" + e.Message + ")");
        }
        return result;
    }

    //하트 충전 스케쥴러를 저장하는 함수
    public void SetRechargeScheduler(Action onFinish = null)
    {
        if (m_RechargeTimerCoroutine != null)
        {
            StopCoroutine(m_RechargeTimerCoroutine);
        } //이미 실행중인 타이머 코루틴이 있다면 중지

        var timeDifferenceInSec = (int)((DateTime.Now.ToLocalTime() - m_AppQuitTime).TotalSeconds);
        Debug.Log("TimeDifference In Sec : " + timeDifferenceInSec + "s"); //앱 종료 시간부터 현재까지 시간 차이를 초 단위로 계산

        var heartToAdd = timeDifferenceInSec / HeartRechargeInterval;
        Debug.Log("Heart to add : " + heartToAdd); // 추가해야 할 하트 개수를 계산

        var remainTime = timeDifferenceInSec % HeartRechargeInterval;
        Debug.Log("RemainTime : " + remainTime); //다음 하트 충전까지 남은 시간을 계산

        m_HeartAmount += heartToAdd;
        if (m_HeartAmount >= MAX_HEART)
        {
            m_HeartAmount = MAX_HEART;
        }
        else
        {
            m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(remainTime, onFinish));
        } //계산된 하트를 추가하고, 최대치를 넘지 않도록 조정. 최대치에 도달하지 않았다면 충전 타이머 코루틴 시작
        Debug.Log("HeartAmount : " + m_HeartAmount);
    }

    //하트를 사용하는 함수
    public void UseHeart(Action onFinish = null)
    {
        if (m_HeartAmount <= 0)
        {
            return;
        } //하트가 0개 이하면 함수를 종료

        m_HeartAmount--; //그게 아니면 하트를 1개 감소
        
        if (m_RechargeTimerCoroutine == null)
        {
            m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(HeartRechargeInterval));
        } //충전 타이머가 실행 중이 아니라면 새로운 충전 타이머를 시작
        if (onFinish != null)
        {
            onFinish();
        }
    }

    //하트 충전 타이머를 실행하는 코루틴 함수
    private IEnumerator DoRechargeTimer(int reaminTime, Action onFinish = null)
    {
        Debug.Log("DoRechargeTimer");
        if (reaminTime <= 0)
        {
            m_RechargeRemainTime = HeartRechargeInterval;
        }
        else
        {
            m_RechargeRemainTime = reaminTime;
        } //남은 시간을 설정
        //Debug.Log("heartRechargeTimer : " + m_RechargeRemainTime + "s");

        while (m_RechargeRemainTime > 0)
        {
            //Debug.Log("heartRechargeTimer : " + m_RechargeRemainTime + "s");
            yield return new WaitForSeconds(1f);
            m_RechargeRemainTime -= 1;
        } //1초마다 남은 시간을 감소시키며 로그 출력

        m_HeartAmount++;

        if (m_HeartAmount >= MAX_HEART)
        {
            m_HeartAmount = MAX_HEART;
            m_RechargeRemainTime = 0;
            Debug.Log("HeartAmount reached max amount");
            m_RechargeTimerCoroutine = null;
        }
        else
        {
            m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(HeartRechargeInterval, onFinish));
        }
        Debug.Log("HeartAmount : " + m_HeartAmount);
    } //하트를 1개 증가시키고, 최대치에 도달했는지 확인. 최대치에 도달하지 않았다면 다음 충전 타이머 시작


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
