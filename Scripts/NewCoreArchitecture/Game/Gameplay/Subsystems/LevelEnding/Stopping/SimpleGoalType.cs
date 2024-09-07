using System;

namespace Match3.Game.Gameplay.SubSystems.LevelEnding
{
    public class SimpleGoalType : GoalTargetType
    {
        public readonly Type type;

        public SimpleGoalType(Type type)
        {
            this.type = type;
        }

        public Type GoalObjectType()
        {
            return type;
        }

        public bool Includes(GoalObject obj)
        {
            return obj != null && type.Equals(obj.GetType());
        }

        public bool Is(GoalTargetType other)
        {
            return other is SimpleGoalType && other.As<SimpleGoalType>().type.Equals(this.type);
        }
    }
}