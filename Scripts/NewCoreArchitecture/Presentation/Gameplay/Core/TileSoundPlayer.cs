
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using UnityEngine;

namespace Match3.Presentation.Gameplay.Core
{
    public class TileSoundPlayer : MonoBehaviour
    {

        GameplaySoundManager soundManager;

        private void Awake()
        {
            soundManager = ServiceLocator.Find<GameplaySoundManager>();

        }

        public void Play(TilePresenter tilePresentaer)
        {

            if (soundManager.HasSoundFor(tilePresentaer.Tile()))
                soundManager.PlayHitSoundFor(tilePresentaer.Tile());
            
        }


    }
}