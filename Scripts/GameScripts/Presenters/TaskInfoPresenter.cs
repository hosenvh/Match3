using System;
using System.Collections;
using System.Collections.Generic;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;
using SeganX;
using UnityEngine.UI;

public class TaskInfoPresenter : MonoBehaviour
{
    #region fields
    [SerializeField]
    Transform starTransform = default;
    [SerializeField]
    LocalText labelText = null, requrementStarsCountText = null;
    [SerializeField]
    Image iconImage = null;
    [SerializeField]
    public Spine.Unity.SkeletonGraphic doneGraphic = null;
    [SerializeField]
    GameObject doButtonGameObject = default;
    System.Action<TaskInfoPresenter, TaskConfig, Vector2> OnDoneClick;
    TaskConfig taskConfig;

    bool shouldShowTaskId;

    #endregion

    #region public methods

    private void Awake()
    {
        GetConfigured();

        void GetConfigured()
        {
            try // Note: Remove this Try/Catch block after proofed to be safe
            {
                ServiceLocator.Find<ConfigurationManager>().Configure(this);
            }
            catch (Exception exception)
            {
                Debug.LogError($"TaskInfoPresenter Getting Configured failed reason: {exception}");
            }

        }
    }

    public void Setup(TaskConfig taskConfig, System.Action<TaskInfoPresenter, TaskConfig, Vector2> OnDoneClick)
    {
        this.taskConfig = taskConfig;
        if(shouldShowTaskId)
            labelText.SetText(taskConfig.GetLocalizedTaskName() + " : " + taskConfig.id.ToString());
        else
            labelText.SetText(taskConfig.GetLocalizedTaskName());
        requrementStarsCountText.SetText(taskConfig.requiremnetStars.ToString());
        iconImage.sprite = taskConfig.TaskIcon();

        this.OnDoneClick = OnDoneClick;
    }

    public void SetShouldShowTaskId(bool shouldShow)
    {
        this.shouldShowTaskId = shouldShow;
    }

    public void OnDoneButtonClick()
    {
        OnDoneClick(this, taskConfig, starTransform.position);
    }

    public void PlayDoneAnimation()
    {
        doButtonGameObject.SetActive(false);
        doneGraphic.AnimationState.SetAnimation(0, "tik_1", false);
    }
    #endregion
}