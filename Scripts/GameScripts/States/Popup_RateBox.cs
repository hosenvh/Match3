using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeganX;
using DG.Tweening;
using UnityEngine.Events;


public class Popup_RateBox : GameState
{
    #region fields
    [SerializeField]
    private GameObject[] starGameObjects = default;
    [SerializeField]
    private UnityEvent onCanNotContinue;
    [SerializeField]
    private UnityEvent onCanContinue;

    int rateValue = 1;
    System.Action<bool, int> onConfirmClick;
    #endregion

    #region methods
    public Popup_RateBox Setup(System.Action<bool, int> onConfirmClick)
    {
        this.onConfirmClick = onConfirmClick;
        onCanNotContinue.Invoke();
        return this;
    }

    public void OnStarClick(int rateValue)
    {
        this.rateValue = rateValue;

        foreach (var item in starGameObjects)
        {
            item.GetComponent<DOTweenAnimation>().DOKill();
            item.GetComponent<Image>().color = Color.white;
            item.SetActive(false);
        }
        for (int i = 0; i < rateValue + 1; i++)
            starGameObjects[i].SetActive(true);

        onCanContinue.Invoke();
    }

    public void OnConfirmClick(bool confirm)
    {
        base.Back();
        gameManager.fxPresenter.PlayClickAudio();
        onConfirmClick(confirm, rateValue);
    }

    public override void Back()
    {
        OnConfirmClick(false);
    }
    #endregion
}
