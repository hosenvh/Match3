using Match3.Game.Gameplay.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Match3.Presentation.Gameplay.Tiles
{
    public class BeachBallColorPresenter : MonoBehaviour
    {
        public TileColor color; 
        public UnityEvent onDisappearingStarted;

        public void PlayDisappearingEffect()
        {
            onDisappearingStarted.Invoke();
        }
    }
}