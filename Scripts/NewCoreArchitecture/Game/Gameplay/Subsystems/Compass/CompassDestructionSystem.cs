using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using Match3.Game.Gameplay.DestructionManagement;
using Match3.Game.Gameplay.HitManagement;
using static Match3.Game.Gameplay.ActionUtilites;
using static Match3.Game.Gameplay.QueryUtilities;


namespace Match3.Game.Gameplay.Subsystems.Compass
{
    [After(typeof(HitApplyingSystem))]
    [Before(typeof(DestructionSystem))]
    public class CompassDestructionSystem : GameplaySystem
    {
        private readonly CompassBoardData compassBoardData;
        private readonly HashSet<CompassTile> pendingToDestroyCompasses = new HashSet<CompassTile>();

        public CompassDestructionSystem(GameplayController gameplayController) : base(gameplayController)
        {
            compassBoardData = GetSessionData<CompassBoardData>();
        }

        public override void Update(float dt)
        {
            RemoveDestroyedCompasses();
            ProcessPendingToDestroyCompasses();
            ProcessReadyToDestroyCompasses();
        }

        private void RemoveDestroyedCompasses()
        {
            var pendings = new List<CompassTile>(pendingToDestroyCompasses);
            foreach (CompassTile compassTile in pendings)
            {
                if (compassTile.IsDestroyed())
                    pendingToDestroyCompasses.Remove(compassTile);
            }

            foreach (Tile tile in GetFrameData<DestructionData>().tilesToDestroy)
            {
                if (tile is CompassTile compass)
                    pendingToDestroyCompasses.Remove(compass);
            }
        }

        private void ProcessPendingToDestroyCompasses()
        {
            HashSet<CompassTile> pendings = new HashSet<CompassTile>(this.pendingToDestroyCompasses);
            foreach (CompassTile compass in pendings)
            {
                if (IsFullyFree(compass.Parent()))
                {
                    DestroyCompass(compass);
                    pendingToDestroyCompasses.Remove(compass);
                }
            }
        }

        private void ProcessReadyToDestroyCompasses()
        {
            List<Tile> allHittedTiles = GetFrameData<AppliedHitsData>().tilesStartedBeingHit;

            foreach (Tile tile in allHittedTiles)
            {
                if (tile is CompassTile)
                    continue;
                int x = tile.Parent().Parent().Position().x;
                int y = tile.Parent().Parent().Position().y;

                Direction reverseDirection = gameplayController.GameBoard().GetOppositeDirectionOf(compassBoardData.currentDirection);
                TileStack tileStack = gameplayController.GameBoard().DirectionalTileStackOf(x, y, reverseDirection);

                if (tileStack != null && !tileStack.IsDepleted())
                {
                    if (tileStack.Top() is CompassTile compassTile)
                    {
                        if (IsFullyFree(compassTile.Parent()))
                            DestroyCompass(compassTile);
                        else
                            pendingToDestroyCompasses.Add(compassTile);
                    }
                }
            }
        }

        private void DestroyCompass(CompassTile compassTile)
        {
            FullyDestroy(compassTile);
            GetFrameData<DestructionData>().tilesToDestroy.Add(compassTile);
        }
    }
}