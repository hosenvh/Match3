using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeganX;

public class CurveMover : Base
{
    [SerializeField]
    float moveSpeed = 0, endScale = 0;//, rotateSpeed = 0;
    [SerializeField]
    ParticleSystem particleSystemPrefab = null;

    float scaleSpeed;
    Vector2 endPosition;
    Vector3 endScaleVector;
    bool moving = false;
    System.Action onFinish;

    public void Setup(Vector2 worlkEndPosition, System.Action onFinish)
    {
        ParticleSystem myParticleSystem = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity, transform);
        myParticleSystem.Play();
        transform.SetParent(transform.parent.parent);
        Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, worlkEndPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(gameManager.canvas.transform as RectTransform, screenP, null, out endPosition);

        float duration = Vector2.Distance(rectTransform.anchoredPosition, endPosition) / moveSpeed;
        scaleSpeed = (1 - endScale) / duration;
        endScaleVector = endScale * Vector3.one;

        moving = true;
        DelayCall(duration, () =>
        {
            myParticleSystem.Stop();
            moving = false;
            DelayCall(1.0f, () =>
            {
                gameObject.SetActive(false);
                onFinish();
            });
        });
    }

    void Update()
    {
        if (moving)
        {
            //rectTransform.localRotation = Quaternion.RotateTowards(rectTransform.localRotation, Quaternion.LookRotation(rectTransform.anchoredPosition - endPosition), rotateSpeed * Time.deltaTime);
            rectTransform.anchoredPosition = Vector3.MoveTowards(rectTransform.anchoredPosition, endPosition, moveSpeed * Time.deltaTime);
            rectTransform.localScale = Vector3.MoveTowards(rectTransform.localScale, endScaleVector, scaleSpeed * Time.deltaTime);
        }
    }
}
