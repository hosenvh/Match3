using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using static Match3.Game.Gameplay.QueryUtilities;
using static Match3.Game.Gameplay.ActionUtilites;

namespace Match3.Game.Gameplay.SubSystems.LouieMechanic
{
    public interface LouiePresentationHandler : PresentationHandler
    {
    }

    [Before(typeof(DestructionManagement.DestructionSystem))]
    [After(typeof(HitManagement.HitApplyingSystem))]
    public class LouieSystem : GameplaySystem
    {
        private readonly HashSet<Louie> louies = new HashSet<Louie>();

        public LouieSystem(GameplayController gameplayController) : base(gameplayController)
        {
            FetchLouieTilesInBoard();

            void FetchLouieTilesInBoard()
            {
                foreach (CellStack cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (HasTile<Louie>(cellStack))
                    louies.Add(FindTile<Louie>(cellStack));
            }
        }

        public override void Update(float dt)
        {
            ProcessReadyToRemoveLouies();
            TryToDeactivateSystemIfNeeded();
        }

        private void ProcessReadyToRemoveLouies()
        {
            List<Louie> louieTiles = new List<Louie>(louies);

            foreach (var louie in louieTiles)
            {
                if (louie.ShouldGetReadyForDestroy())
                    DestroyLouie(louie);
            }
        }

        private void DestroyLouie(Louie louie)
        {
            FullyDestroy(louie);
            GetFrameData<DestructionData>().tilesToDestroy.Add(louie);

            louie.MarkAsDestroyed();
            louies.Remove(louie);
        }

        private void TryToDeactivateSystemIfNeeded()
        {
            if (louies.Count == 0)
                gameplayController.DeactivateSystem<LouieSystem>();
        }
    }
}