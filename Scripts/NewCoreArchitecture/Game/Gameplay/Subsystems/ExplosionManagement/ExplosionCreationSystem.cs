


using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.Physics;
using Match3.Game.Gameplay.Tiles;
using KitchenParadise.Utiltiy.Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Match3.Game.Gameplay.SubSystems.General;

namespace Match3.Game.Gameplay.ExplosionManagement
{

    public struct ExplosiveCreatedFromMatchEvent : GameEvent
    { }

    public struct ExplosiveNotCreatedFromMatchEvent : GameEvent
    {

    }

    [After(typeof(Matching.MatchDetectionSystem))]
    [After(typeof(HitManagement.HitApplyingSystem))]
    [Before(typeof(Physics.PhysicsSystem))]
    [Before(typeof(TileGeneration.TileSourceSystem))]
    public class ExplosionCreationSystem : GameplaySystem
    {
        struct ExplosivePlacementData
        {
            public readonly CellStack cellStack;
            public readonly Tile tile;

            public ExplosivePlacementData(CellStack cellStack, Tile tile)
            {
                this.cellStack = cellStack;
                this.tile = tile;
            }
        }

        TileFactory tileFactroy;


        List<ExplosivePlacementData> pendingExplosives = new List<ExplosivePlacementData>();
        List<ExplosivePlacementData> temporarypendingExplosives = new List<ExplosivePlacementData>();

        List<TileStack> candidateTileStacks = new List<TileStack>(32);

        EventManager eventManager;

        public ExplosionCreationSystem(GameplayController gameplayController) : base(gameplayController)
        {
            this.tileFactroy = ServiceLocator.Find<TileFactory>();
            eventManager = ServiceLocator.Find<EventManager>();
        }

        public override void Update(float dt)
        {
            foreach (var match in GetFrameData<CreatedMatchesData>().data)
                TryCreateExplosiveItemFor(match);


            temporarypendingExplosives.Clear();
            temporarypendingExplosives.AddRange(pendingExplosives);
            pendingExplosives.Clear();

            foreach (var pendingData in temporarypendingExplosives)
                if(pendingData.cellStack.HasTileStack())
                    TryPlaceExplosiveAt(pendingData.cellStack, pendingData.tile);        }

        private void TryCreateExplosiveItemFor(Matching.Match match)
        {
            var matchLength = match.tileStacks.Count;

            var target = FindCellStackOfLastStabledTileIn(match);

            if (target == null)
            {
                eventManager.Propagate(new ExplosiveNotCreatedFromMatchEvent(), this);
                return;
            }

            if (matchLength == 4)
                TryPlaceExplosiveAt(target, tileFactroy.CreateRocketTile());
            else if (matchLength == 5)
                TryPlaceExplosiveAt(target, tileFactroy.CreateBombTile());
            else if (matchLength == 6)
                TryPlaceExplosiveAt(target, tileFactroy.CreateDynamiteTile());
            else if (matchLength >= 7)
                TryPlaceExplosiveAt(target, tileFactroy.CreateTNTBarrelTile());
        }

        private CellStack FindCellStackOfLastStabledTileIn(Matching.Match match)
        {
            candidateTileStacks.Clear();
            foreach (var tileStack in match.tileStacks)
                if (tileStack.IsDepleted())
                    candidateTileStacks.Add(tileStack);

            if (candidateTileStacks.Count == 0)
                return null;
            return candidateTileStacks.MaxElement(t => t.componentCache.lockState.LastReleaseLock()).Parent();
        }

        private void TryPlaceExplosiveAt(CellStack cellStack, Tile explosiveTile)
        {
            if (cellStack.CurrentTileStack().GetComponent<LockState>().IsFree())
            {
                cellStack.CurrentTileStack().Push(explosiveTile);
                ServiceLocator.Find<EventManager>().Propagate(new TileGeneratedEvent(cellStack.CurrentTileStack(), explosiveTile), this);
            }
            else
                pendingExplosives.Add(new ExplosivePlacementData(cellStack, explosiveTile));
        }

    }
}