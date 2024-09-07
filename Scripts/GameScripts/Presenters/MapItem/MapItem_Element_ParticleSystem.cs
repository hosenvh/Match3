using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapItem_Element_ParticleSystem : MapItem_Element
{
    bool isShowing = false;
    ParticleSystem myParticleSystem;
    Action onFinish;

    private void Awake()
    {
        myParticleSystem = GetComponent<ParticleSystem>();

        if (!isShowing)
            SetActiveAll(false);
    }

    public override void ShowElementState(int index, bool withAnimation, Action onFinish)
    {
        this.onFinish = onFinish;
        if (index > 0)
        {
            isShowing = true;
            SetActiveAll(true);

            if (myParticleSystem == null)
                myParticleSystem = GetComponent<ParticleSystem>();
            myParticleSystem.Play(true);

            DelayCall(myParticleSystem.main.duration, () =>
            {
                if (onFinish != null)
                    onFinish();
            });
        }
        else
        {
            if (myParticleSystem == null)
                myParticleSystem = GetComponent<ParticleSystem>();
            myParticleSystem.Stop(true);

            DelayCall(myParticleSystem.main.duration, () =>
            {
                SetActiveAll(false);
                if (onFinish != null)
                    onFinish();
            });
        }
    }

    void Update()
    {
        if (!myParticleSystem.IsAlive())
        {
            onFinish?.Invoke();
        }
    }
}