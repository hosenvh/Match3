using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeganX;
using UnityEngine.UI;

public class ScenarioScreen : Base
{
    [SerializeField]
    Image screenImage = default;
    [SerializeField]
    Text screenText = default;

    Color screenColor = Color.black, textColor = Color.white;
    bool isScreenAlphaChanging, isScreenAlphaIncreasing;
    System.Action onFinish;
    float alpha = 0;
    readonly float fadeSpeed = 1f, delayDuraion = 2f;

    public void Setup(string text, System.Action onFinish)
    {
        screenText.GetComponent<LocalText>().SetText(text);
        this.onFinish = onFinish;
        screenImage.gameObject.SetActive(true);
        alpha = 0;
        screenText.gameObject.SetActive(true);
        isScreenAlphaChanging = isScreenAlphaIncreasing = true;
        enabled = true;
    }

    void Update()
    {
        if (isScreenAlphaChanging)
        {
            if (isScreenAlphaIncreasing)
            {
                alpha += fadeSpeed * Time.deltaTime;
                if (alpha >= 1)
                {
                    alpha = 1;
                    isScreenAlphaChanging = isScreenAlphaIncreasing = false;
                    DelayCall(delayDuraion, () =>
                    {
                        isScreenAlphaChanging = true;
                        onFinish();
                    });
                }
            }
            else
            {
                alpha -= fadeSpeed * Time.deltaTime;
                if (alpha <= 0)
                {
                    alpha = 0;
                    screenImage.gameObject.SetActive(false);
                    isScreenAlphaChanging = false;
                    enabled = false;
                }
            }
            textColor.a = screenColor.a = alpha;
            screenImage.color = screenColor;
            screenText.color = textColor;
        }
    }
}