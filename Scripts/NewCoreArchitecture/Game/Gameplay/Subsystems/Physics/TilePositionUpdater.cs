using Match3.Game.Gameplay.Core;
using UnityEngine;

namespace Match3.Game.Gameplay.Physics
{

    public partial class PhysicsSystem
    {
        public class TilePositionUpdater
        {
            PhysicsSystem system;

            public void Setup(PhysicsSystem system)
            {
                this.system = system;
            }

            public void Update(float dt)
            {
                foreach (var cellStack in system.gameBoard.leftToRightButtomUpCellStackArray)
                    UpdateMovementOf(cellStack, dt);
            }


            void UpdateMovementOf(CellStack cellStack, float dt)
            {
                var tileStack = cellStack.CurrentTileStack();

                if (tileStack == null)
                    return;

                var tileState = tileStack.componentCache.tileStackPhysicalState;
                if (tileState.PhysicsState() == PhysicsState.Resting)
                    return;

                tileState.speed += FALL_ACCELERATION * dt;

                var moveDistance = tileState.speed * dt;
                var targetPos = cellStack.Position();

                if (Vector2.Distance(tileStack.Position(), targetPos) <= moveDistance)
                    tileState.hasReachedTarget = true;
                tileStack.SetPosition(Vector2.MoveTowards(tileStack.Position(), targetPos, moveDistance));
            }


        }
    }
}