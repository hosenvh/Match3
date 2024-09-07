using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using System;

namespace Match3.Game.Gameplay.Tiles
{
    public class VacuumCleaner : Tile
    {
        public readonly TileColor targetColor;
        public readonly int targetNumber;
        public readonly Direction direction;
        public readonly int priority;

        public event Action<int> onFillChanged = delegate { };

        int fillNumber;

        public VacuumCleaner(TileColor targetColor, int targetNumber, Direction direction, int priority)
        {
            this.targetColor = targetColor;
            this.targetNumber = targetNumber;
            this.direction = direction;
            this.priority = priority;
            this.fillNumber = 0;
        }

        protected override bool InteralDoesAcceptDirectHit(HitCause hitCause)
        {
            return false;
        }

        protected override bool InteralDoesAcceptSideHit(HitCause hitCause)
        {
            return false;
        }

        public int CurrentFill()
        {
            return fillNumber;
        }

        public void IncreaseFill(int number)
        {
            fillNumber += number;
            onFillChanged(fillNumber);
        }

        public bool IsFilled()
        {
            return fillNumber >= targetNumber;
        }
    }
}