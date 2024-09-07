using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using SeganX;

public class TimerActivator : Base
{
    public LocalText timerText;
    public string uniqueString;

    private float remainTime;
    private int lastRemainTimeValue;
    private int delayTime;
    public GameObject readyTextGameObject;

    public float GetRemainTime() { return remainTime; }

    public TimerActivator Setup(int delayTime)
    {
        this.delayTime = delayTime;
        return this;
    }

    void Start()
    {
        remainTime = (nextActiveTime - DateTime.Now.Ticks) / TimeSpan.TicksPerSecond;
        UpdateGui();
    }

    void Update()
    {
        if (remainTime > 0)
        {
            remainTime -= Time.deltaTime;
            UpdateGui();
        }
    }

    private long nextActiveTime
    {
        get { return Convert.ToInt64(PlayerPrefs.GetString(uniqueString, "0")); }
        set { PlayerPrefs.SetString(uniqueString, value.ToString()); }
    }

    private void UpdateGui()
    {
        if (lastRemainTimeValue == Mathf.FloorToInt(remainTime))
            return;

        lastRemainTimeValue = Mathf.FloorToInt(remainTime);
        if (remainTime < 0)
        {
            timerText.gameObject.SetActive(false);
            if (readyTextGameObject)
                readyTextGameObject.SetActive(true);
        }
        else
        {
            timerText.gameObject.SetActive(true);
            if (readyTextGameObject)
                readyTextGameObject.SetActive(false);
            int hour = (int)remainTime / 3600;
            if (hour > 0)
                timerText.SetText(string.Format("{0}:{1:00}", (int)remainTime / 3600, (int)remainTime / 60 % 60));
            else
                timerText.SetText(string.Format("{0}:{1:00}", (int)remainTime / 60 % 60, (int)remainTime % 60));

        }
    }

    public void ResetTimer()
    {
        nextActiveTime = DateTime.Now.Ticks + delayTime * TimeSpan.TicksPerSecond;
        remainTime = delayTime;
        UpdateGui();
    }
}
