using Match3.Game.Gameplay;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Tiles;
using UnityEngine;


namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class BalloonRopePresentationHandler : PresentationController
    {
        [SerializeField] private Transform balloonRopesParent;

        protected override void InternalSetup(GameplayState gameState)
        {
            base.InternalSetup(gameState);

            var cellStacks = gameState.gameplayController.GameBoard().ArrbitrayCellStackArray();
            foreach (var cellStack in cellStacks)
                if (QueryUtilities.HasTile<BalloonTiedTile>(cellStack))
                    QueryUtilities.FindTile<BalloonTiedTile>(cellStack).GetComponent<BalloonTiedTilePresenter>().SetRopeParent(balloonRopesParent);
        }
    }
}