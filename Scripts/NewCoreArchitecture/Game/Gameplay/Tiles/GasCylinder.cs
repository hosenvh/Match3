
using System;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Tiles
{
    public class GasCylinder : Tile
    {
        int currentCountdown;
        public event Action<int> onCountdownChanged = delegate { };

        public GasCylinder(int startCountdown)
        {
            this.currentCountdown = startCountdown;
        }

        public int CurrentCountdown()
        {
            return currentCountdown;
        }

        public void DecrementCountdown()
        {
            currentCountdown--;
            onCountdownChanged(currentCountdown);
        }

        public void AddCountdown(int moves)
        {
            currentCountdown+=moves;
            onCountdownChanged(currentCountdown);
        }
    }
}