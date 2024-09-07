using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Presentation
{
    [RequireComponent(typeof(AudioSource))]
    public class RandomAudioPlayer : MonoBehaviour
    {
        public bool playOnWake;
        public AudioClip[] clips;
        public Vector2 pitchOffsetRange;


        AudioSource audioSource;
        float originalPitch;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            originalPitch = audioSource.pitch;

            if (playOnWake)
                Play();
        }

        public void Play()
        {
            audioSource.clip = clips[UnityEngine.Random.Range(0, clips.Length - 1)];
            audioSource.pitch = originalPitch + UnityEngine.Random.Range(pitchOffsetRange.x, pitchOffsetRange.y);
            audioSource.Play();
        }


    }
}