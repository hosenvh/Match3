using System;
using System.Collections;
using System.Collections.Generic;
using KitchenParadise.Utiltiy.Base;
using UnityEngine;
using SeganX;

public enum CharacterAnim
{
    Idle,
    Walk,
    Clap,
    SitDown,
    Sit_Idle,
    StandUp,
    Phone,
    Surprise,
    Vanish,
    Sport,
    Talk,
    Dance,
    Sit_Remote,
    Sit_Newspaper,
    Mop_Start,
    Mop_Loop,
    Mop_End,
    Watering,
    Bird_Food,
    Appear,
    Fix,
    Head_Scratch,
    Dust_Cleaning,
    Cough,
    Thanks,
    Car_GetIn,
    Car_GetOff,
    Dig,
    Cleaning,
    Fire_Capsule,
    Disappear,
    Reveal,
    Fishing,
    PlayingGuitar,
    Flying,
    Hovering,
    Meditation,
    Ground_StandingUp,
    Ground_SittingDown,
    Ground_Sitting_Idle,
    Swimming,
    Swimming_Idle,
    Eat,
    Ax_Idle,
    Axing
}


public class CharacterAnimator : Base
{
    private const int ANIMATOR_LAYER_INDEX = 0;

    [SerializeField] float speedScale = 1;
    [SerializeField] GameObject bubbleDialogueCanvasGameObject = default;
    [SerializeField] LocalText bubbleDialogueText = default;
    [SerializeField] Transform characterTransform = default;
    [SerializeField] Animator animator = null;

    public Vector3 targetPosition;
    public Quaternion targetRotation;
    public Vector3 targetScaleFactor = Vector3.one;
    public System.Action onFinish;
    public System.Action onScalingFinish;
    public bool moving = false;
    public bool scaling = false;
    public float passedTimeSinceScaling = 0;

    public float walkSpeed = 1.5f * 2;
    readonly float rotateSpeed = 600, blendFadeLength = .2f;
    private Vector3 defaultScale;
    private Vector3 scaleFactor;
    private float scaleDuration = 1;

    private void Awake()
    {
        defaultScale = transform.localScale;
        scaleFactor = Vector3.one;
    }

    public void Setup(Vector3 startPosition, Vector3 startScale)
    {
        BlendAnimation(CharacterAnim.Idle, 0, null);
        transform.position = startPosition;
        SetScale(startScale);
        walkSpeed *= speedScale;
    }

    public void Update()
    {
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, walkSpeed * Time.deltaTime);
            characterTransform.rotation = Quaternion.RotateTowards(characterTransform.rotation, targetRotation,
                rotateSpeed * Time.deltaTime);
        }

        if (scaling)
        {
            passedTimeSinceScaling += Time.deltaTime;
            scaleFactor = Vector3.Lerp(scaleFactor, targetScaleFactor, passedTimeSinceScaling / scaleDuration);
            UpdateScale();
        }
    }

    public void SetupScale(List<Vector3> scales, List<float> durations, System.Action onFinish)
    {
        scaling = true;

        this.onScalingFinish = onFinish;
        Scale(scales, durations, id: 0);
    }


    void Scale(List<Vector3> scales, List<float> durations, int id)
    {
        targetScaleFactor = scales[id];
        scaleDuration = durations[id];
        passedTimeSinceScaling = 0;

        DelayCall(scaleDuration, () =>
        {
            if (id < scales.Count - 1)
                Scale(scales, durations, id + 1);
            else
            {
                scaling = false;
                onScalingFinish();
            }
        });
    }


    public void SetupMove(List<Vector3> positions, System.Action onFinish)
    {
        moving = true;
        BlendAnimation(CharacterAnim.Walk, 0, null);

        this.onFinish = onFinish;
        Move(positions, 0);
    }

    void Move(List<Vector3> positions, int id)
    {
        targetPosition = positions[id];
        float distance = Vector3.Distance(transform.position, targetPosition);
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        targetRotation = Quaternion.LookRotation(direction);
        float duration = distance / walkSpeed;

        DelayCall(duration, () =>
        {
            if (id < positions.Count - 1)
                Move(positions, id + 1);
            else
            {
                BlendAnimation(CharacterAnim.Idle, 0, null);
                moving = false;
                onFinish();
            }
        });
    }

    public void BlendAnimation(CharacterAnim characterAnim, int animIndex, System.Action onFinish)
    {
        StartCoroutine(PlayAnimation());

        IEnumerator PlayAnimation()
        {
            animator.CrossFadeInFixedTime(stateName: GetAnimatorStateNameFor(characterAnim, animIndex), fixedTransitionDuration: GetTransitionDurationFor(characterAnim));
            yield return null; // Unity Animator.CrossFade needs at least one frame delay, other wise it's nextState is referring to currentState
            float remainedTime = GetRemainedTimeTillEndOfStartedAnimation();
            DelayCall(
                seconds: remainedTime,
                callback: () => onFinish?.Invoke());


            float GetRemainedTimeTillEndOfStartedAnimation()
            {
                AnimatorClipInfo[] nextAnimations = animator.GetNextAnimatorClipInfo(ANIMATOR_LAYER_INDEX);
                if (nextAnimations.Length > 0)
                    return nextAnimations[0].clip.length / GetAnimationSpeed();
                return 0;

                float GetAnimationSpeed() => characterAnim == CharacterAnim.Walk ? 2 : 1;
            }
        }
    }

    private float GetTransitionDurationFor(CharacterAnim characterAnim)
    {
        if (characterAnim == CharacterAnim.Reveal ||
            characterAnim == CharacterAnim.Fishing)
            return 0;
        else
            return blendFadeLength;
    }

    private string GetAnimatorStateNameFor(CharacterAnim characterAnim, int animationIndex)
    {
        return $"{characterAnim.ToString() + animationIndex.ToString()}";
    }


    public void SetRotation(int rotateValue)
    {
        characterTransform.localRotation = Quaternion.Euler(0, rotateValue, 0);
    }

    public void SetPosition(Vector3 position)
    {
        transform.localPosition = position;
    }

    public void SetScale(Vector3 scale)
    {
        scaleFactor = scale;
        UpdateScale();
    }

    private void UpdateScale()
    {
        transform.localScale = new Vector3(scaleFactor.x * defaultScale.x, scaleFactor.y * defaultScale.y, scaleFactor.z * defaultScale.z);
    }

    public Vector3 GetPosition()
    {
        return transform.localPosition;
    }

    public Vector3 GetScale()
    {
        return scaleFactor;
    }

    public void ShowBubbleDialogue(string text, float duration, System.Action onFinish)
    {
        ValidateReceivedText();

        bubbleDialogueCanvasGameObject.SetActive(true);
        bubbleDialogueText.SetText(text.Replace("\\n", "\n"));
        DelayCall(duration, () =>
        {
            bubbleDialogueCanvasGameObject.SetActive(false);
            onFinish();
        });

        ValidateReceivedText();

        void ValidateReceivedText()
        {
            if (IsTextInvalid())
                LogErrorTextIsNotValid();

            bool IsTextInvalid() => text == "";
            void LogErrorTextIsNotValid() => Debug.LogError($"Trying to show a dialogue box with an empty string | CharacterName: {name}", gameObject);
        }
    }

    public void PlayAnimationFromTime(CharacterAnim characterAnim, int animIndex, float time)
    {
        string animationName = GetAnimatorStateNameFor(characterAnim, animIndex);
        animator.Play(animationName, ANIMATOR_LAYER_INDEX, time);
    }

    public float GetCurrentAnimationTime()
    {
        return animator.GetCurrentAnimatorStateInfo(ANIMATOR_LAYER_INDEX).normalizedTime;
    }
}