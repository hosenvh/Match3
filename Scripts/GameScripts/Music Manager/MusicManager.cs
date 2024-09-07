using System.Collections;
using UnityEngine;
using System;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;


public class MusicManager : Base
{
    #region fields
    [SerializeField]
    AudioSource musicAudioSource = default;
    [SerializeField]
    MusicClipData[] musicClipDatas = default;

    float maxVolume;
    #endregion


    #region properties
    readonly string isMusicOnString = "IsMusicOn";

    public bool IsMusicOn
    {
        get { return PlayerPrefs.GetInt(isMusicOnString, 1) == 1; }
        set
        {
            musicAudioSource.mute = !value;
            PlayerPrefs.SetInt(isMusicOnString, value ? 1 : 0);
        }
    }

    readonly string isFxOnString = "IsFxOn";
    public bool IsFxOn
    {
        get { return PlayerPrefs.GetInt(isFxOnString, 1) == 1; }
        set
        {
            AudioListener.volume = value ? 1 : 0;
            PlayerPrefs.SetInt(isFxOnString, value ? 1 : 0);
        }
    }
    #endregion

    #region methods
    void Awake()
    {
        ServiceLocator.Find<ConfigurationManager>().Configure(this);

        musicAudioSource.ignoreListenerPause = true;
        musicAudioSource.ignoreListenerVolume = true;
        maxVolume = musicAudioSource.volume;
        musicAudioSource.volume = 0;

        AudioListener.volume = IsFxOn ? 1 : 0;
        musicAudioSource.mute = !IsMusicOn;
    }

    public void Configure(MusicClipData[] musicClipDatas)
    {
        this.musicClipDatas = musicClipDatas;
    }
    
    public void StartSceneMusic(int sceneIndex, float fadeDuration)
    {
        //if (!IsMusicOn)
          //  return;

        int newClipIndex = UnityEngine.Random.Range(0, musicClipDatas[sceneIndex].MusicClipsArr.Length);

        AudioClip clip = musicClipDatas[sceneIndex].MusicClipsArr[newClipIndex];

        if (musicAudioSource.isPlaying)
        {
            StartCoroutine(DoFadeOut(fadeDuration / 2, () =>
            {
                PlayMusic(clip);
                StartCoroutine(DoFadeIn(fadeDuration / 2));
            }));
        }
        else
        {
            PlayMusic(clip);
            StartCoroutine(DoFadeIn(fadeDuration / 2));
        }
    }

    void PlayMusic(AudioClip clip)
    {
        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }

    public void StopMusic(float fadeDuration, Action _onStopFinish = null)
    {
        StartCoroutine(DoFadeOut(fadeDuration, () =>
        {
            if (_onStopFinish != null)
                _onStopFinish();
        }));
    }

    IEnumerator DoFadeIn(float fadeInDuration)
    {
        while (musicAudioSource.volume < maxVolume)
        {
            musicAudioSource.volume += maxVolume * Time.deltaTime / fadeInDuration;
            musicAudioSource.volume = Mathf.Min(musicAudioSource.volume, maxVolume);
            yield return null;
        }
    }

    IEnumerator DoFadeOut(float fadeOutDuration, Action onFinished)
    {
        while (musicAudioSource.volume > 0)
        {
            musicAudioSource.volume -= maxVolume * Time.deltaTime / fadeOutDuration;
            yield return null;
        }
        if (onFinished != null)
            onFinished();
    }

    public void MuteMusicForDuration(float pauseDuration, float fadeOutDuration, float fadeInDuration)
    {
        StartCoroutine(DoFadeOut(fadeOutDuration, null));
        DelayCall(pauseDuration, () =>
        {
            StartCoroutine(DoFadeIn(fadeInDuration));
        });
    }
    #endregion
}

[Serializable]
public class MusicClipData
{
    public AudioClip[] MusicClipsArr;
}