using Match3.Game.Gameplay.SubSystems.ExtraMoveMechanic;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.LevelConditionManagement;
using UnityEngine;


namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class ExtraMoveApplyingPresentationPortImp : MonoBehaviour, ExtraMoveApplyingPresentationPort
    {
        public MovementStopConditionPresenter movementStopConditionPresenter;


        public void PlayExtraMoveEffect(ExtraMove extraMoveTile)
        {
            movementStopConditionPresenter.HandleExtraMovesEffect(extraMoveTile);
        }
    }
}