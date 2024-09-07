using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeganX;
using System;


public class TimerCountDown : MonoBehaviour
{
    #region fields
    [SerializeField]
    private LocalText timerText = default;
    [SerializeField]
    private string uniqueString = default;

    private float remainTime;
    private int lastRemainTimeValue;
    private int delayTime = 0;
    #endregion

    #region properties
    #endregion

    #region methods
    public GameObject freeCoinsTextGameObject;

    public float GetRemainTime() { return remainTime; }

    public TimerCountDown Setup()
    {
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
            freeCoinsTextGameObject.SetActive(true);
        }
        else
        {
            timerText.gameObject.SetActive(true);
            freeCoinsTextGameObject.SetActive(false);
            //timerText.SetText(string.Format("{0}:{01:00}", (int)remainTime / 60, (int)remainTime % 60));
        }
    }

    public void ResetTimer()
    {
        nextActiveTime = DateTime.Now.Ticks + delayTime * TimeSpan.TicksPerSecond;
        remainTime = delayTime;
        UpdateGui();
    }

    #endregion
}