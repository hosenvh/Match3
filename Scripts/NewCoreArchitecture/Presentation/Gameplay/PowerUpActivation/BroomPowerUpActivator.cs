
using Match3.Game.Gameplay.SubSystems.PowerUpManagement;
using Match3.Presentation.Gameplay.Core;

namespace Match3.Presentation.Gameplay.PowerUpActivation
{
    public class BroomPowerUpActivator : PowerUpActivator
    {
        public override bool AcceptsSelection(CellStackPresenter cellStackPresenter)
        {
            return true;
        }

        public override void OnCellStackSelected(CellStackPresenter target)
        {
            Execute(new BroomPowerUpInputCommand(target.CellStack()));
        }

        protected override void ResetActivationState()
        {
           
        }
    }
}