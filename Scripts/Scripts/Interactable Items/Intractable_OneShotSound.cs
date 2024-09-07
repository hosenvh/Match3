using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity;
using Match3.Presentation.Gameplay;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Match3.Game.Intractables
{

    public class Intractable_OneShotSound : Intractable
    {

        public List<ResourceAudioClipAsset> sounds;
        public float volume = 0.5f;

        private GameplaySoundManager soundManager;
        private AudioClip lastClip;
        
        private float timeAtStartPlaying;
        private bool isClipPlaying;

        protected override void Start()
        {
            base.Start();
            soundManager = ServiceLocator.Find<GameplaySoundManager>();
        }

        private void Update()
        {
            if (isClipPlaying && IsClipFinished)
                isClipPlaying = false;
        }

        private bool IsClipFinished => Time.time - timeAtStartPlaying > lastClip.length;
        
        
        protected override void InternalClickDown()
        {
            // Nothing for here
        }

        protected override void InternalClickUp()
        {
            if (isClipPlaying) return;
            PlayRandomSound();
        }

        private void PlayRandomSound()
        {
            var newClip = SelectAndLoadRandomSound();
            if (lastClip != null && newClip != lastClip)
                UnloadLastClip();
            lastClip = newClip;
            timeAtStartPlaying = Time.time;
            soundManager.TryPlay(lastClip, false, volume);
            isClipPlaying = true;
        }

        private AudioClip SelectAndLoadRandomSound()
        {
            return sounds[Random.Range(0, sounds.Count)].Load();
        }

        private void UnloadLastClip()
        {
            Resources.UnloadAsset(lastClip);
            lastClip = null;
        }

        protected override void InternalSaveState()
        {
            // Nothing To Save
        }

        protected override void InternalLoadState()
        {
            // Nothing To Load
        }
    }
    
}