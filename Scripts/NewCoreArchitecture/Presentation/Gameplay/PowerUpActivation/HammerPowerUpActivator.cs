using System.Collections;
using System.Collections.Generic;
using Match3.Game.Gameplay.SubSystems.PowerUpManagement;
using Match3.Presentation.Gameplay.Core;
using UnityEngine;

namespace Match3.Presentation.Gameplay.PowerUpActivation
{
    public class HammerPowerUpActivator : PowerUpActivator
    {
        public override bool AcceptsSelection(CellStackPresenter cellStackPresenter)
        {
            return true;
        }

        public override void OnCellStackSelected(CellStackPresenter target)
        {
            Execute(new HammerPowerUpInputCommand(target.CellStack()));
        }

        protected override void ResetActivationState()
        {
            
        }
    }
}