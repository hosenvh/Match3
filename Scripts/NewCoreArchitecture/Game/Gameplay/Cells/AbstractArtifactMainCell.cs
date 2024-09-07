using System;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using UnityEngine;

namespace Match3.Game.Gameplay.Cells
{
    public abstract class AbstractArtifactMainCell : CompositeCell, DestructionBasedGoalObject
    {
        public static readonly Vector2Int identitySize = new Vector2Int(1, 2);

        Direction direction;
        int scale;


        public AbstractArtifactMainCell(Direction direction, int scale) : base(CalculateSize(direction, scale))
        {
            this.direction = direction;
            this.scale = scale;
        }

        private static Size CalculateSize(Direction direction, int scale)
        {
            switch (direction)
            {
                case Core.Direction.Right:
                    return new Size(identitySize.y * scale, identitySize.x * scale);
                case Core.Direction.Down:
                    return new Size(identitySize.x * scale, identitySize.y * scale);
            }

            throw new Exception($"Direction {direction} is not defined for Artifact");
        }


        public override void Hit()
        {

        }

        public override bool IsDestroyed()
        {
            return false;
        }

        // TODO: Maybe consider direction in size?
        // TODO: Remove this
        public Vector2Int ArtifactSize()
        {
            return identitySize * scale;
        }

        public float Scale()
        {
            return scale;
        }


        public Direction Direction()
        {
            return direction;
        }

        public override bool CanContainTile()
        {
            return true;
        }

        protected override bool InteralDoesAcceptDirectHit()
        {
            return false;
        }

        protected override bool InteralDoesAcceptSideHit()
        {
            return false;
        }
    }
}