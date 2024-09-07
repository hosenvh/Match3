using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using System;

namespace Match3.Game.Gameplay.Tiles
{
    public class TableClothMainTile : CompositeTile
    {
        public class TargetHandler
        {
            public readonly bool isActive;
            public readonly GoalTargetType goalType;
            public readonly int targetNumber;

            public event Action<int> onFillChanged = delegate { };

            private int fillNumber;

            public TargetHandler(bool isActive, GoalTargetType goalType, int targetNumber)
            {
                this.isActive = isActive;
                this.goalType = goalType;
                this.targetNumber = targetNumber;
                fillNumber = 0;
            }

            public void IncreaseFill(int number)
            {
                fillNumber += number;
                onFillChanged(fillNumber);
            }

            public int CurrentFill()
            {
                return fillNumber;
            }

            public bool IsFilled()
            {
                return  isActive == false || fillNumber >= targetNumber;
            }
        }

        //public readonly ColoredGoalType coloredBeadGoalType;
        //public readonly int targetNumber;

        public readonly TargetHandler firstTarget;
        public readonly TargetHandler secondTarget;

        public TableClothMainTile(Size size, TargetHandler firstTarget, TargetHandler secondTarget) : base(size)
        {
            this.firstTarget = firstTarget;
            this.secondTarget = secondTarget;
        }

        protected override bool InteralDoesAcceptDirectHit(HitCause hitCause)
        {
            return false;
        }

        protected override bool InteralDoesAcceptSideHit(HitCause hitCause)
        {
            return false;
        }

        public bool IsFilled()
        {
            return firstTarget.IsFilled() && secondTarget.IsFilled();
        }
    }
}