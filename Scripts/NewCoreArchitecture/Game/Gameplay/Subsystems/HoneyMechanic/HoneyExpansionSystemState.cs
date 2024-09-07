using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.SubSystems.HoneyMechanic
{
    public abstract class HoneyExpansionSystemState
    {
        protected HoneyExpansionSystem system;
        protected CellStackBoard cellStackBoard;

        protected HoneyExpansionSystemState(HoneyExpansionSystem system, CellStackBoard cellStackBoard)
        {
            this.system = system;
            this.cellStackBoard = cellStackBoard;
        }

        public abstract void Update();
        public abstract void OnEnter();

    }
}