using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxPresenter : Base
{
    [SerializeField]
    AudioSource clickAudioSource = default;

    public void PlayClickAudio()
    {
        clickAudioSource.Play();
    }
}
