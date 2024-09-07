using System;

namespace Match3.Game.Gameplay.SubSystems.LevelEnding
{
    // TODO: Move this to General 
    public interface GoalTargetType
    {
        bool Includes(GoalObject goal);
        bool Is(GoalTargetType other);

        Type GoalObjectType();
    }
}