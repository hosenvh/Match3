using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using PandasCanPlay.HexaWord.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using Match3.Game.Gameplay;
using Match3.Utility.GolmoradLogging;
using UnityEngine;

namespace Match3.Presentation.Gameplay
{

    [System.Serializable]
    public class SoundInfo
    {
        public AudioClip clip;
        public bool isSingleton;
        public float volume = 1;

        [HideInInspector]
        public AudioSource dedicatedAudioSource;
    }

    [System.Serializable]
    public class TileSoundInfo : SoundInfo
    {
        [TypeAttribute(typeof(Tile), false)]
        public string tileType;
    }

    [System.Serializable]
    public struct RainbowSounds
    {
        public SoundInfo rainbow;

    }


    public class SingleRainbowActivationSoundController
    {
        AudioSource audioSource1;
        AudioSource audioSource2;

        AudioClip rainbowChargeStart;
        AudioClip rainbowChargeLoop;
        AudioClip rainbowExplosion;


        GameplaySoundManager gameplaySoundManager;

        public SingleRainbowActivationSoundController(GameplaySoundManager gameplaySoundManager, AudioClip rainbowChargeStart, AudioClip rainbowChargeLoop, AudioClip rainbowExplosion)
        {
            this.gameplaySoundManager = gameplaySoundManager;

            this.rainbowChargeStart = rainbowChargeStart;
            this.rainbowChargeLoop = rainbowChargeLoop;
            this.rainbowExplosion = rainbowExplosion;
        }

        public void PlayCharging()
        {

            this.audioSource1 = gameplaySoundManager.FindAnAvailableFloatingAudioSource();
            audioSource1.volume = 1;
            audioSource1.pitch = 1;
            audioSource1.clip = rainbowChargeStart;
            audioSource1.Play();


            this.audioSource2 = gameplaySoundManager.FindAnAvailableFloatingAudioSource();
            audioSource2.volume = 1;
            audioSource2.pitch = 1;
            audioSource2.loop = true;
            audioSource2.clip = rainbowChargeLoop;
            audioSource2.PlayDelayed(rainbowChargeStart.length);
        }

        public void PlayExplosion()
        {
            audioSource1.Stop();
            audioSource2.Stop();
            audioSource2.loop = false;
            audioSource2.clip = rainbowExplosion;
            audioSource2.Play();
        }
    }

    public class DoubleRainbowActivationSoundController
    {
        AudioSource audioSource;

        AudioClip rainbowChargeStart;
        AudioClip rainbowExplosion;

        GameplaySoundManager gameplaySoundManager;

        public DoubleRainbowActivationSoundController(GameplaySoundManager gameplaySoundManager, AudioClip rainbowChargeStart, AudioClip rainbowExplosion)
        {
            this.gameplaySoundManager = gameplaySoundManager;

            this.rainbowChargeStart = rainbowChargeStart;
            this.rainbowExplosion = rainbowExplosion;
        }

        public void PlayCharging()
        {

            this.audioSource = gameplaySoundManager.FindAnAvailableFloatingAudioSource();
            audioSource.volume = 1;
            audioSource.pitch = 1;
            audioSource.clip = rainbowChargeStart;
            audioSource.Play();
        }

        public void PlayExplosion()
        {
            audioSource.Stop();
            audioSource.clip = rainbowExplosion;
            audioSource.Play();
        }
    }


    // TODO: Refactor rainbow sound handling.
    public class GameplaySoundManager : MonoBehaviour, Service
    {
        public int floatingAudioSourcePoolCount;

        public AudioClip swapSoundClip;
        public AudioClip[] matchAudioClips;
        public SoundInfo tileStoppingSoundInfo;

        public AudioClip singleRainbowChargeStartUp;
        public AudioClip singleRainbowChargeLoop;
        public AudioClip singleRainbowExplosion;


        public AudioClip doubleRainbowChargeStart;
        public AudioClip doubleRainbowExplosion;


        public TileSoundInfo[] tileSoundInfos;

        List<AudioSource> floatingAudioSourcePool;

        Dictionary<Type, TileSoundInfo> tileSoundInfoMap = new Dictionary<Type, TileSoundInfo>();

        Dictionary<Rainbow, SingleRainbowActivationSoundController> singleRainbowSoundControllers = new Dictionary<Rainbow, SingleRainbowActivationSoundController>();
        Dictionary<Rainbow, DoubleRainbowActivationSoundController> doubleRainbowSoundControllers = new Dictionary<Rainbow, DoubleRainbowActivationSoundController>();

        void Awake()
        {
            foreach (var info in tileSoundInfos)
            {
                SetupDedicatedAudioSourceFor(info);
                tileSoundInfoMap.Add(Type.GetType(info.tileType), info);
            }

            floatingAudioSourcePool = new List<AudioSource>(floatingAudioSourcePoolCount);

            for (int i = 0; i < floatingAudioSourcePoolCount; ++i)
                floatingAudioSourcePool.Add(gameObject.AddComponent<AudioSource>());
        }

        private void SetupDedicatedAudioSourceFor(TileSoundInfo info)
        {
            if (info.isSingleton)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();

                audioSource.clip = info.clip;
                info.dedicatedAudioSource = audioSource;
            }
        }


        public void PlaySingleRainbowChargeEffectFor(Rainbow rainbow)
        {
            var controller = new SingleRainbowActivationSoundController(
                this,
                singleRainbowChargeStartUp,
                singleRainbowChargeLoop,
                singleRainbowExplosion);

            controller.PlayCharging();

            if(singleRainbowSoundControllers.ContainsKey(rainbow) == false)
                singleRainbowSoundControllers.Add(rainbow, controller);
            else
                DebugPro.LogError<CoreGameplayLogTag>("Trying to add singleRainbowSoundControllers more than one.");
        }

        public void PlaySingleRainbowExplosionEffectFor(Rainbow rainbow)
        {
            singleRainbowSoundControllers[rainbow].PlayExplosion();
            singleRainbowSoundControllers.Remove(rainbow);
        }

        public void PlayDoubleRainbowChargeEffectFor(Rainbow rainbow)
        {
            var controller = new DoubleRainbowActivationSoundController(
                this,
                doubleRainbowChargeStart,
                doubleRainbowExplosion);

            controller.PlayCharging();

            doubleRainbowSoundControllers.Add(rainbow, controller);
        }

        public void PlayDoubleRainbowExplosionEffectFor(Rainbow rainbow)
        {
            doubleRainbowSoundControllers[rainbow].PlayExplosion();
            doubleRainbowSoundControllers.Remove(rainbow);
        }

        public void PlayMatchSound(int comboCount)
        {
            TryPlay(matchAudioClips[Mathf.Clamp(comboCount, 0, matchAudioClips.Length - 1)]);
        }


        public void PlaySwapSoundEffect()
        {
            TryPlay(swapSoundClip);
        }

        public void PlayTileStopSoundEffect()
        {
            TryPlay(tileStoppingSoundInfo, UnityEngine.Random.Range(.9f, 1.2f));
        }

        public void TryPlay(AudioClip clip, bool loop = false, float volume = 1)
        {
            var audioSource = FindAnAvailableFloatingAudioSource();
            if (audioSource != null)
            {
                audioSource.pitch = 1;
                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.loop = loop;
                audioSource.Play();
            }

        }

        private void TryPlay(SoundInfo soundInfo, float pitch = 1)
        {

            var audioSource = soundInfo.dedicatedAudioSource;
            if (audioSource == null)
                audioSource = FindAnAvailableFloatingAudioSource();


            if (audioSource != null)
            {
                audioSource.clip = soundInfo.clip;
                audioSource.pitch = pitch;
                audioSource.volume = soundInfo.volume;
                audioSource.Play();
            }

        }

        // WARNING: This is not a safe way for stopping. Find a better way.
        public void TryStop(AudioClip clip)
        {
            for (int i = 0; i < floatingAudioSourcePool.Count; ++i)
                if (floatingAudioSourcePool[i].clip == clip)
                {
                    floatingAudioSourcePool[i].Stop();
                    break;
                }
        }

        public void StopAll()
        {
            for (int i = 0; i < floatingAudioSourcePool.Count; ++i)
                floatingAudioSourcePool[i].Stop();
        }
        public bool HasSoundFor(Tile tile)
        {
            return tileSoundInfoMap.ContainsKey(tile.GetType());
        }

        public void TryPlayHitSoundFor(Tile tile)
        {
            if (HasSoundFor(tile))
                PlayHitSoundFor(tile);
        }

        public void PlayHitSoundFor(Tile tile)
        {
            var info = FindSoundInfoFor(tile);

            TryPlay(info, UnityEngine.Random.Range(.85f, 1.15f));
        }

        public AudioSource FindAnAvailableFloatingAudioSource()
        {
            var count =  floatingAudioSourcePool.Count;
            for (int i = 0; i < count; ++i)
            {
                var audioSource = floatingAudioSourcePool[i];
                if (audioSource.isPlaying == false)
                {
                    audioSource.loop = false;
                    return audioSource;
                }
            }
            var newSource = gameObject.AddComponent<AudioSource>();
            floatingAudioSourcePool.Add(newSource);
            return newSource;
        }


        private TileSoundInfo FindSoundInfoFor(Tile tile)
        {
            return tileSoundInfoMap[tile.GetType()];
        }

    }
}