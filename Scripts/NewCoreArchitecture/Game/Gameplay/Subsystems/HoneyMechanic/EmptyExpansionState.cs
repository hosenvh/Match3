using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.SubSystems.HoneyMechanic
{
    public class EmptyExpansionState : HoneyExpansionSystemState
    {
        GameplayController gameplayController;

        public EmptyExpansionState(HoneyExpansionSystem system, CellStackBoard cellStackBoard, GameplayController gameplayController) : base(system, cellStackBoard)
        {
            this.gameplayController = gameplayController;
        }

        public override void OnEnter()
        {
            gameplayController.DeactivateSystem<HoneyExpansionSystem>();
        }

        public override void Update()
        {
            
        }
    }
}