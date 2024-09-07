using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData.PowerUpManagement;
using Match3.Game.Gameplay.Swapping;
using Match3.Presentation.Gameplay;
using static Base;

namespace Match3.Game.Gameplay.SubSystems.PowerUpManagement
{

    public struct HandPowerUpActivatedEvent : PowerUpActivatedEvent
    {
    }
    public class HandPowerUpActivationController
    {

        PowerUpActivationSystem system;

        PowerUpActivationPresentationHandler powerUpPresentationHandler;

        GameplayController gameplayController;

        public HandPowerUpActivationController(PowerUpActivationSystem system, PowerUpActivationPresentationHandler powerUpPresentationHandler, GameplayController gameplayController)
        {
            this.system = system;
            this.powerUpPresentationHandler = powerUpPresentationHandler;
            this.gameplayController = gameplayController;
        }

        public void Update()
        {
            foreach (var target in system.GetFrameData<PowerUpRequestData>().handPowerUpTargets)
                TryActivate(target);
        }

        private void TryActivate(HandPowerUpData data)
        {
            if(IsSwappable(data.orign) && IsSwappable(data.destination) 
                && !IsBlockedByWall(data.orign, data.destination))
            {
                data.orign.CurrentTileStack().GetComponent<LockState>().LockBy<PowerUpSystemKeyType>();
                data.destination.CurrentTileStack().GetComponent<LockState>().LockBy<PowerUpSystemKeyType>();
                powerUpPresentationHandler.HandleHand(data.orign, data.destination, () => ApplySwap(data));

                ServiceLocator.Find<EventManager>().Propagate(new HandPowerUpActivatedEvent(), this);
            }
        }

        private bool IsSwappable(CellStack cellStack)
        {
            if (cellStack.HasTileStack() == false || cellStack.CurrentTileStack().IsDepleted())
                return false;

            return cellStack.CurrentTileStack().Top().GetComponent<TileUserInteractionProperties>().isSwappable;
        }

        private bool IsBlockedByWall(CellStack origin, CellStack destination)
        {
            Direction direction = CalculateDirection(origin, destination);
            return gameplayController.boardBlockageController.IsBlocked(origin, direction);
        }

        public Direction CalculateDirection(CellStack origin, CellStack destination)
        {
            var xOffset = origin.Top().Parent().Position().x - destination.Top().Parent().Position().x;
            var yOffset = origin.Top().Parent().Position().y - destination.Top().Parent().Position().y;

            var direction = Direction.None;

            if (xOffset > 0)
                direction = Direction.Left;
            else if (xOffset < 0)
                direction = Direction.Right;
            else if (yOffset > 0)
                direction = Direction.Up;
            else if (yOffset < 0)
                direction = Direction.Down;

            return direction;
        }

        void ApplySwap(HandPowerUpData data)
        {
            CellStack cellStack1 = data.orign;
            CellStack cellStack2 = data.destination;

            var temp = cellStack1.CurrentTileStack();
            cellStack1.SetCurrnetTileStack(cellStack2.CurrentTileStack());
            cellStack2.SetCurrnetTileStack(temp);

            cellStack1.CurrentTileStack().SetPosition(cellStack1.Position());
            cellStack2.CurrentTileStack().SetPosition(cellStack2.Position());

            data.orign.CurrentTileStack().GetComponent<LockState>().Release();
            data.destination.CurrentTileStack().GetComponent<LockState>().Release();
        }
    }
}