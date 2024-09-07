

using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData.PowerUpManagement;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystems.PowerUpManagement
{

    public struct BroomPowerUpActivatedEvent : PowerUpActivatedEvent
    {
    }


    public class BroomPowerUpActivationController
    {

        PowerUpActivationSystem system;

        PowerUpActivationPresentationHandler powerUpPresentationHandler;
        CellStackBoard cellStackBoard;

        public BroomPowerUpActivationController(
            PowerUpActivationSystem system,
            PowerUpActivationPresentationHandler powerUpPresentationHandler,
            CellStackBoard cellStackBoard)
        {
            this.system = system;
            this.powerUpPresentationHandler = powerUpPresentationHandler;
            this.cellStackBoard = cellStackBoard;
        }

        public void Update()
        {
            foreach (var target in system.GetFrameData<PowerUpRequestData>().broomTargets)
                TryActivateBroomOn(target);
        }

        private void TryActivateBroomOn(CellStack target)
        {
            var horizontalTargets = new List<CellStack>();
            var verticalTargets = new List<CellStack>();

            var center = target.Position();

            for (int i = 0; i < cellStackBoard.Width(); ++i)
                horizontalTargets.Add(cellStackBoard[i, center.y]);

            for (int j = 0; j < cellStackBoard.Height(); ++j)
                verticalTargets.Add(cellStackBoard[center.x, j]);

            foreach (var cell in horizontalTargets)
                LockTileAndCellStack(cell);
            foreach (var cell in verticalTargets)
                LockTileAndCellStack(cell);

            powerUpPresentationHandler.HandleBroom(
                horizontalTargets,
                verticalTargets,
                system.ApplyPowerUpHitOn,
                () => ReleaseCellsOf(horizontalTargets, verticalTargets));

            ServiceLocator.Find<EventManager>().Propagate(new BroomPowerUpActivatedEvent(), this);
        }


        void LockTileAndCellStack(CellStack target)
        {
            target.GetComponent<LockState>().LockBy<PowerUpSystemKeyType>();
            system.LockTileStack(target);
        }


        void ReleaseCellsOf(List<CellStack> horizontalTargets, List<CellStack> verticalTargets)
        {
            foreach (var cell in horizontalTargets)
                ReleaseCellStack(cell);
            foreach (var cell in verticalTargets)
                ReleaseCellStack(cell);
        }

        void ReleaseCellStack(CellStack target)
        {
            target.GetComponent<LockState>().Release();

        }
    }
}