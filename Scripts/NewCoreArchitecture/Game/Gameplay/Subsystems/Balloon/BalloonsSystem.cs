using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using static Match3.Game.Gameplay.QueryUtilities;
using static Match3.Game.Gameplay.ActionUtilites;


namespace Match3.Game.Gameplay.SubSystems.BalloonMechanic
{
    public class BalloonsSystem : GameplaySystem
    {
        private readonly HashSet<BalloonMainTile> notReadyToFlyBalloons = new HashSet<BalloonMainTile>();
        private readonly HashSet<BalloonMainTile> readyToFlyBalloons = new HashSet<BalloonMainTile>();

        public BalloonsSystem(GameplayController gameplayController) : base(gameplayController)
        {
            FetchNotReadyToFlyBalloonTilesFromBoard();
            SetupNotReadyToFlyBalloons();
        }

        private void FetchNotReadyToFlyBalloonTilesFromBoard()
        {
            foreach (CellStack cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (HasTile<BalloonMainTile>(cellStack))
                    notReadyToFlyBalloons.Add(FindTile<BalloonMainTile>(cellStack));
        }

        private void SetupNotReadyToFlyBalloons()
        {
            foreach (BalloonMainTile balloon in notReadyToFlyBalloons)
                foreach (BalloonTiedTile tiedTile in balloon.GetTiedTiles())
                    tiedTile.onHit = () => { balloon.RemoveTiedTile(tiedTile); };
        }

        public override void Update(float dt)
        {
            TryToDetectReadyToFlyBalloons();
            ProcessReadyToFlyBalloons();

            TryToDeactivateSystemIfNeeded();
        }

        private void TryToDetectReadyToFlyBalloons()
        {
            foreach (BalloonMainTile balloon in notReadyToFlyBalloons)
                if (balloon.IsReadyToFly())
                    readyToFlyBalloons.Add(balloon);

            foreach (BalloonMainTile readyToFlyBalloon in readyToFlyBalloons)
                notReadyToFlyBalloons.Remove(readyToFlyBalloon);
        }

        private void ProcessReadyToFlyBalloons()
        {
            foreach (BalloonMainTile balloon in readyToFlyBalloons)
            {
                FullyDestroy(balloon);
                GetFrameData<DestructionData>().tilesToDestroy.Add(balloon);
            }
            readyToFlyBalloons.Clear();
        }

        private void TryToDeactivateSystemIfNeeded()
        {
            if (notReadyToFlyBalloons.Count == 0)
                gameplayController.DeactivateSystem<BalloonsSystem>();
        }
    }
}