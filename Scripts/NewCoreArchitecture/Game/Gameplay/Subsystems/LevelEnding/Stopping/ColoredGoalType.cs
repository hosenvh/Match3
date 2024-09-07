using Match3.Foundation.Base.ComponentSystem;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.Core;
using System;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.SubSystems.LevelEnding
{
    public class ColoredGoalType : GoalTargetType
    {
        public readonly Type type;
        public readonly TileColor color;

        public ColoredGoalType(Type type, TileColor color)
        {
            this.type = type;
            this.color = color;
        }

        public Type GoalObjectType()
        {
            return type;
        }

        public bool Includes(GoalObject obj)
        {
            if (obj == null || obj is Tile == false || type.Equals(obj.GetType()) == false)
                return false;

            var colorComponent = obj.As<Tile>().componentCache.colorComponent;

            if (obj is ColoredBead coloredBead)
                return colorComponent.color == color && coloredBead.IsClean();
            else
                return colorComponent != null && colorComponent.color == color;
        }

        public bool Is(GoalTargetType other)
        {
            return other is ColoredGoalType
                && other.As<ColoredGoalType>().type.Equals(this.type)
                && other.As<ColoredGoalType>().color == this.color;
        }
    }
}