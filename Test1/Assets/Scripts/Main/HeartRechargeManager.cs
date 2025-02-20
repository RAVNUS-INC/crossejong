using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeartRechargeManager : MonoBehaviour
{
    public int m_HeartAmount = 5; //���� ��Ʈ ����
    private DateTime m_AppQuitTime = new DateTime(1970, 1, 1).ToLocalTime(); //���� ����� �ð��� ����
    private int m_TimerQuitTime = 0; //���� �������� Ÿ�̸� �ð� ����

    private const int MAX_HEART = 5; //��Ʈ �ִ밪
    public int HeartRechargeInterval = 180; //��Ʈ ���� ����(����:��)
    private Coroutine m_RechargeTimerCoroutine = null; //��Ʈ ���� Ÿ�̸Ӹ� ���� �ڷ�ƾ ����
    private int m_RechargeRemainTime = 0; //���� ��Ʈ �������� ���� �ð��� ����

    public TMP_Text timerText;  // Unity UI Text ������Ʈ ����

    //Unity ������Ʈ�� �ʱ�ȭ�� �� ȣ��(�ʱ�ȭ ����)
    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        LoadHeartInfo(); //���� ��Ʈ ���� �ҷ�����
        LoadAppQuitTime(); //���� �ð� �ҷ�����
        LoadTimerQuitTime(); //��Ʈ Ÿ�̸� �ҷ�����
        SetRechargeScheduler(); //���� �ð� ��� �� ������Ʈ
    }
    //���� �ʱ�ȭ, �߰� ��Ż, �߰� ���� �� ����Ǵ� �Լ�
    public void OnApplicationFocus(bool value)
    {
        Debug.Log("���� ���ӻ���: " + value);
        if (value) //�ۿ� �ٽ� ���ƿ��� ��Ʈ ������ �� ����ð��� �ε��ϰ� ���� �����췯�� ����
        {
            LoadHeartInfo();
            LoadAppQuitTime();
            LoadTimerQuitTime();
            SetRechargeScheduler();
        }
        else //�װ� �ƴ϶�� ���� ��Ʈ ������ �� ���� �ð�, ��Ʈ Ÿ�̸Ӹ� ����
        {
            SaveHeartInfo();
            SaveAppQuitTime();
            SaveTimerQuitTime();
        }
    }
    //���� ����� ����Ǵ� �Լ�
    public void OnApplicationQuit()
    {
        Debug.Log("������ ����Ǿ����ϴ�");
        SaveHeartInfo(); 
        SaveAppQuitTime();
        SaveTimerQuitTime();
    } //��Ʈ ������ ���� �ð��� ����

    //��Ʈ ��� ��ư�� Ŭ������ �� ����Ǵ� �Լ�
    public void OnClickUseHeart()
    {
        if (m_HeartAmount > 0) //��Ʈ ������ 1���� ������ Ŭ�� ��
        {
            Debug.Log("��Ʈ�� ����߽��ϴ�");
            UseHeart();
        }
    } 

    //�ʱ�ȭ�� ���� �Լ�
    public void Init()
    {
        //PlayerPrefs.DeleteKey("HeartAmount"); // "HeartAmount" Ű�� �ش��ϴ� ������ ����
        m_HeartAmount = 5;
        m_TimerQuitTime = 0;
        m_AppQuitTime = new DateTime(1970, 1, 1).ToLocalTime();
     
    }//��Ʈ ����, �������� ���� �ð�, �� ���� �ð��� �ʱ�ȭ, ����� ���

    //����� ��Ʈ ������ �ε��ϴ� �Լ�
    public bool LoadHeartInfo()
    {
        Debug.Log("��Ʈ ������ �ҷ��ɴϴ�");
        bool result = false;
        try
        {
            if (PlayerPrefs.HasKey("HeartAmount"))
            {
                m_HeartAmount = PlayerPrefs.GetInt("HeartAmount"); //playerprefs�� ����� ��Ʈ���� ������ �ҷ�����
                if (m_HeartAmount < 0)
                {
                    m_HeartAmount = 0;
                    //��Ʈ ��� ��ư ��Ȱ��ȭ �ʿ�
                }
            }
            else //����� ���� ������ �⺻�� 5�� ����
            {
                m_HeartAmount = MAX_HEART;
            }
            Debug.Log("�ҷ��� ���� ��Ʈ ��: " + m_HeartAmount);
            //ui������Ʈ �ʿ�
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("��Ʈ ���� �ҷ����� ���� (" + e.Message + ")");
        }
        return result;
    } //PlayerPrefs���� HeartAmount Ű�� ����� ���� �ҷ�����, ���� ���ٸ� �ִ� ��Ʈ ������ ����


    //���� ��Ʈ ������ �����ϴ� �Լ�
    public bool SaveHeartInfo()
    {
        Debug.Log("���� ��Ʈ ������ �����մϴ�");
        bool result = false;
        try
        {
            PlayerPrefs.SetInt("HeartAmount", m_HeartAmount);
            PlayerPrefs.Save();
            //���� ��Ʈ ������ PlayerPrefs�� ����
            Debug.Log("���� ����� ��Ʈ ����: " + m_HeartAmount);
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("��Ʈ ���� ���� ���� (" + e.Message + ")");
        }
        return result;
    }
    
    //����� �� ���� �ð��� �ε��ϴ� �Լ�
    public bool LoadAppQuitTime()
    {
        Debug.Log("������ ���� �ð��� �ҷ��ɴϴ�");
        bool result = false;
        try
        {   
            if (PlayerPrefs.HasKey("AppQuitTime"))
            {
                Debug.Log("����� �ð��� �����մϴ�");
                var appQuitTime = string.Empty;
                appQuitTime = PlayerPrefs.GetString("AppQuitTime");
                m_AppQuitTime = DateTime.FromBinary(Convert.ToInt64(appQuitTime));
            } //PlayerPrefs���� AppQuitTimeŰ�� ����� ���� �ҷ��� DateTime ��ü�� ��ȯ

            Debug.Log(string.Format("�ҷ��� ������ ���� �ð� : {0}", m_AppQuitTime.ToString()));
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("���� �ð� �ҷ����� ���� (" + e.Message + ")");
        }
        return result;
    }

    //���� �ð��� �� ���� �ð����� �����ϴ� �Լ�
    public bool SaveAppQuitTime()
    {
        Debug.Log("���� ���� �ð��� �����մϴ�");
        bool result = false;
        try
        {
            var appQuitTime = DateTime.Now.ToLocalTime().ToBinary().ToString();
            PlayerPrefs.SetString("AppQuitTime", appQuitTime);
            PlayerPrefs.Save();
            Debug.Log("���� ���� �ð� : " + DateTime.Now.ToLocalTime().ToString());
            result = true;
        } //���� �ð��� ���� ���ڿ��� ��ȯ�� PlayerPrefs�� ����
        catch (System.Exception e)
        {
            Debug.LogError("���� �ð� ���� ���� (" + e.Message + ")");
        }
        return result;
    }

    //����� Ÿ�̸� ���� �ð��� �ε��ϴ� �Լ�
    public void LoadTimerQuitTime()
    {
        Debug.Log("������ Ÿ�̸� �ð��� �ҷ��ɴϴ�");
        try
        {
            if (PlayerPrefs.HasKey("TimerQuitTime"))
            {
                m_TimerQuitTime = PlayerPrefs.GetInt("TimerQuitTime");
                Debug.Log("�ҷ��� ��Ʈ Ÿ�̸� �ð�: " + m_TimerQuitTime + "��");
            }
            else
            {
                m_TimerQuitTime = 0; // �⺻ ���� �ð����� ���� (��: 180��)
                Debug.Log("����� ��Ʈ Ÿ�̸Ӱ� ���� �⺻��(" + m_TimerQuitTime + "��)���� ����");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("��Ʈ Ÿ�̸� �ҷ����� ���� (" + e.Message + ")");
        }
    }

    //Ÿ�̸� �ð��� �����ϴ� �Լ�
    public void SaveTimerQuitTime()
    {
        try
        {
            PlayerPrefs.SetInt("TimerQuitTime", m_RechargeRemainTime); // ���� �ð�(�� ����) ����
            PlayerPrefs.Save();
            Debug.Log("��Ʈ ���� Ÿ�̸� ����: " + m_RechargeRemainTime + "��");
        }
        catch (System.Exception e)
        {
            Debug.LogError("��Ʈ ���� Ÿ�̸� ���� ���� (" + e.Message + ")");
        }
    }

    //��Ʈ ���� �����췯�� �����ϴ� �Լ�
    public void SetRechargeScheduler(Action onFinish = null)
    {
        // ��Ʈ ������ 5�� �̻��� ��� ������ �������� ����
        if (m_HeartAmount >= 5)
        {
            Debug.Log("���� ��Ʈ ������ 5�� �̻��̹Ƿ� ���� �����ٷ��� �������� �ʽ��ϴ�.");
            return; // 5�� �̻��̸� �Լ� ����
        }

        if (m_RechargeTimerCoroutine != null)
        {
            StopCoroutine(m_RechargeTimerCoroutine);
        } //�̹� �������� Ÿ�̸� �ڷ�ƾ�� �ִٸ� ����

        var timeDifferenceInSec = (int)((DateTime.Now.ToLocalTime() - m_AppQuitTime).TotalSeconds);
        Debug.Log("�� ���� �� ��� �ð� : " + timeDifferenceInSec + "��"); //�� ���� �ð����� ������� �ð� ���̸� �� ������ ���

        // ���� �������� ���� ���� �ð��� ��� �ð��� ���Ͽ� ����
        if (timeDifferenceInSec < m_TimerQuitTime)
        {
            m_TimerQuitTime -= timeDifferenceInSec; // ���� �ð����� ��� �ð���ŭ ����
        }
        else
        {
            // �ʰ��� ���, �߰��� ��Ʈ ���� ���
            int extraTime = timeDifferenceInSec - m_TimerQuitTime;
            var heartToAdd = 1 + (extraTime / HeartRechargeInterval); // �⺻������ 1�� �߰�, �ʰ� �ð� �ݿ�
            m_TimerQuitTime = HeartRechargeInterval - (extraTime % HeartRechargeInterval); // ���ο� Ÿ�̸ӷ� ����
            m_HeartAmount += heartToAdd;
        }

        // �а� �� ���
        int minutes = m_TimerQuitTime / 60;
        int seconds = m_TimerQuitTime % 60;
        string timeText = string.Format("{0:D2}:{1:D2}", minutes, seconds);
        Debug.Log("���� ��Ʈ ��������: " + "(" + timeText+ ")");
        
        // ��Ʈ ������ �ִ�ġ�� �ʰ����� �ʵ��� ����
        if (m_HeartAmount >= MAX_HEART)
        {
            m_HeartAmount = MAX_HEART;
            HeartManager.Instance.UpdateHeartUI(m_HeartAmount); // UI ������Ʈ
            timerText.text = "���� �Ϸ�"; // text ������Ʈ
        }
        else
        {
            timerText.text = timeText; // UI ������Ʈ
            // ���� Ÿ�̸� �ڷ�ƾ ����
            m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(m_TimerQuitTime, onFinish));
        }
        Debug.Log("���� ��Ʈ ����: " + m_HeartAmount);
    }

    //��Ʈ�� 1�� ������Ű��, �ִ�ġ�� �����ߴ��� Ȯ��. �ִ�ġ�� �������� �ʾҴٸ� ���� ���� Ÿ�̸� ����
    private IEnumerator DoRechargeTimer(int reaminTime, Action onFinish = null)
    {
        Debug.Log("��Ʈ ���� ����");
        if (reaminTime <= 0)
        {
            m_RechargeRemainTime = HeartRechargeInterval;
        }
        else
        {
            m_RechargeRemainTime = reaminTime;
        } 
        Debug.Log("���� ��Ʈ���� ���� �ð� : " + m_RechargeRemainTime + "��");

        while (m_RechargeRemainTime > 0)
        {
            int minutes = m_RechargeRemainTime / 60;  // �� ���
            int seconds = m_RechargeRemainTime % 60;  // �� ���

            // "��:��" ���·� ����
            timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);

            //Debug.Log("��Ʈ ���� Ÿ�̸� : " + m_RechargeRemainTime + "��");
            yield return new WaitForSeconds(1f);
            m_RechargeRemainTime -= 1;
        } //1�ʸ��� ���� �ð��� ���ҽ�Ű�� �α� ���

        m_HeartAmount++;
        HeartManager.Instance.UpdateHeartUI(m_HeartAmount); // UI ������Ʈ

        if (m_HeartAmount >= MAX_HEART)
        {
            m_HeartAmount = MAX_HEART;
            m_RechargeRemainTime = 0;
            // Ÿ�̸� ���� �� �ؽ�Ʈ �ʱ�ȭ (�ɼ�)
            timerText.text = "���� �Ϸ�";
            Debug.Log("��Ʈ�� �� á���ϴ�");
            m_RechargeTimerCoroutine = null;
        }
        else
        {
            m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(HeartRechargeInterval, onFinish));
        }
        Debug.Log("���� ��Ʈ ���� : " + m_HeartAmount);
    } 

    //��Ʈ�� ����ϴ� �Լ�
    public void UseHeart(Action onFinish = null)
    {
        if (m_HeartAmount <= 0)
        {
            return;
        } //��Ʈ�� 0�� ���ϸ� �Լ��� ����

        m_HeartAmount--; //�װ� �ƴϸ� ��Ʈ�� 1�� ����
        HeartManager.Instance.UpdateHeartUI(m_HeartAmount); //UI ������Ʈ
        Debug.Log("���� ��Ʈ ���� : " + m_HeartAmount);

        // ���� ���� Ÿ�̸Ӱ� ���� ���̶�� ����
        if (m_RechargeTimerCoroutine != null)
        {
            StopCoroutine(m_RechargeTimerCoroutine);
            m_RechargeTimerCoroutine = null;
        }

        // ���ο� ���� Ÿ�̸� ���� (180�ʺ��� �ٽ� ����)
        m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(HeartRechargeInterval));

        if (onFinish != null)
        {
            onFinish();
        }
    }
}
