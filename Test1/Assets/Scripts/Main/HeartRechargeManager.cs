using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartRechargeManager : MonoBehaviour
{
    public int m_HeartAmount = 0; //���� ��Ʈ ����
    private DateTime m_AppQuitTime = new DateTime(1970, 1, 1).ToLocalTime(); //���� ����� �ð��� ����
    private const int MAX_HEART = 5; //��Ʈ �ִ밪
    public int HeartRechargeInterval = 0; //��Ʈ ���� ����(����:��)
    private Coroutine m_RechargeTimerCoroutine = null; //��Ʈ ���� Ÿ�̸Ӹ� ���� �ڷ�ƾ ����
    private int m_RechargeRemainTime = 0; //���� ��Ʈ �������� ���� �ð��� ����

    //Unity ������Ʈ�� �ʱ�ȭ�� �� ȣ��(�ʱ�ȭ ����)
    private void Awake()
    {
        Init();
    }
    //���� �ʱ�ȭ, �߰� ��Ż, �߰� ���� �� ����Ǵ� �Լ�
    public void OnApplicationFocus(bool value)
    {
        Debug.Log("OnApplicationFocus() : " + value);
        if (value) //�ۿ� �ٽ� ���ƿ��� ��Ʈ ������ �� ����ð��� �ε��ϰ� ���� �����췯�� ����
        {
            LoadHeartInfo();
            LoadAppQuitTime();
            SetRechargeScheduler();
        }
        else //�װ� �ƴ϶�� ���� ��Ʈ ������ �� ���� �ð��� ����
        {
            SaveHeartInfo();
            SaveAppQuitTime();
        }
    }
    //���� ����� ����Ǵ� �Լ�
    public void OnApplicationQuit()
    {
        Debug.Log("GoodsRechargeTester: OnApplicationQuit()");
        SaveHeartInfo(); 
        SaveAppQuitTime();
    } //��Ʈ ������ ���� �ð��� ����

    //��Ʈ ��� ��ư�� Ŭ������ �� ����Ǵ� �Լ�
    public void OnClickUseHeart()
    {
        Debug.Log("OnClickUseHeart");
        UseHeart();
    } //��Ʈ�� ���

    //�ʱ�ȭ�� ���� �Լ�
    public void Init()
    {
        m_HeartAmount = 0;
        m_RechargeRemainTime = 0;
        m_AppQuitTime = new DateTime(1970, 1, 1).ToLocalTime();
        //Debug.Log("heartRechargeTimer : " + m_RechargeRemainTime + "s");
    }//��Ʈ ����, �������� ���� �ð�, �� ���� �ð��� �ʱ�ȭ, ����� ���

    //����� ��Ʈ ������ �ε��ϴ� �Լ�
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
    } //PlayerPrefs���� HeartAmount Ű�� ����� ���� �ҷ�����, ���� ���ٸ� �ִ� ��Ʈ ������ ����


    //���� ��Ʈ ������ �����ϴ� �Լ�
    public bool SaveHeartInfo()
    {
        Debug.Log("SaveHeartInfo");
        bool result = false;
        try
        {
            PlayerPrefs.SetInt("HeartAmount", m_HeartAmount);
            PlayerPrefs.Save();
            //���� ��Ʈ ������ PlayerPrefs�� ����
            Debug.Log("Saved HeartAmount : " + m_HeartAmount);
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveHeartInfo Failed (" + e.Message + ")");
        }
        return result;
    }
    
    //����� �� ���� �ð��� �ε��ϴ� �Լ�
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
            } //PlayerPrefs���� AppQuitTimeŰ�� ����� ���� �ҷ��� DateTime ��ü�� ��ȯ

            Debug.Log(string.Format("Loaded AppQuitTime : {0}", m_AppQuitTime.ToString()));
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadAppQuitTime Failed (" + e.Message + ")");
        }
        return result;
    }

    //���� �ð��� �� ���� �ð����� �����ϴ� �Լ�
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
        } //���� �ð��� ���� ���ڿ��� ��ȯ�� PlayerPrefs�� ����
        catch (System.Exception e)
        {
            Debug.LogError("SaveAppQuitTime Failed (" + e.Message + ")");
        }
        return result;
    }

    //��Ʈ ���� �����췯�� �����ϴ� �Լ�
    public void SetRechargeScheduler(Action onFinish = null)
    {
        if (m_RechargeTimerCoroutine != null)
        {
            StopCoroutine(m_RechargeTimerCoroutine);
        } //�̹� �������� Ÿ�̸� �ڷ�ƾ�� �ִٸ� ����

        var timeDifferenceInSec = (int)((DateTime.Now.ToLocalTime() - m_AppQuitTime).TotalSeconds);
        Debug.Log("TimeDifference In Sec : " + timeDifferenceInSec + "s"); //�� ���� �ð����� ������� �ð� ���̸� �� ������ ���

        var heartToAdd = timeDifferenceInSec / HeartRechargeInterval;
        Debug.Log("Heart to add : " + heartToAdd); // �߰��ؾ� �� ��Ʈ ������ ���

        var remainTime = timeDifferenceInSec % HeartRechargeInterval;
        Debug.Log("RemainTime : " + remainTime); //���� ��Ʈ �������� ���� �ð��� ���

        m_HeartAmount += heartToAdd;
        if (m_HeartAmount >= MAX_HEART)
        {
            m_HeartAmount = MAX_HEART;
        }
        else
        {
            m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(remainTime, onFinish));
        } //���� ��Ʈ�� �߰��ϰ�, �ִ�ġ�� ���� �ʵ��� ����. �ִ�ġ�� �������� �ʾҴٸ� ���� Ÿ�̸� �ڷ�ƾ ����
        Debug.Log("HeartAmount : " + m_HeartAmount);
    }

    //��Ʈ�� ����ϴ� �Լ�
    public void UseHeart(Action onFinish = null)
    {
        if (m_HeartAmount <= 0)
        {
            return;
        } //��Ʈ�� 0�� ���ϸ� �Լ��� ����

        m_HeartAmount--; //�װ� �ƴϸ� ��Ʈ�� 1�� ����
        
        if (m_RechargeTimerCoroutine == null)
        {
            m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(HeartRechargeInterval));
        } //���� Ÿ�̸Ӱ� ���� ���� �ƴ϶�� ���ο� ���� Ÿ�̸Ӹ� ����
        if (onFinish != null)
        {
            onFinish();
        }
    }

    //��Ʈ ���� Ÿ�̸Ӹ� �����ϴ� �ڷ�ƾ �Լ�
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
        } //���� �ð��� ����
        //Debug.Log("heartRechargeTimer : " + m_RechargeRemainTime + "s");

        while (m_RechargeRemainTime > 0)
        {
            //Debug.Log("heartRechargeTimer : " + m_RechargeRemainTime + "s");
            yield return new WaitForSeconds(1f);
            m_RechargeRemainTime -= 1;
        } //1�ʸ��� ���� �ð��� ���ҽ�Ű�� �α� ���

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
    } //��Ʈ�� 1�� ������Ű��, �ִ�ġ�� �����ߴ��� Ȯ��. �ִ�ġ�� �������� �ʾҴٸ� ���� ���� Ÿ�̸� ����


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
