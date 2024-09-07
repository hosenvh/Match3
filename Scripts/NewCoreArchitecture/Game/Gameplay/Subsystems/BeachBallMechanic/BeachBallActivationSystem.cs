

using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using static Match3.Game.Gameplay.QueryUtilities;
using static Match3.Game.Gameplay.ActionUtilites;
using Match3.Game.Gameplay.SubSystemsData.FrameData.BeachBallMechanic;
using KitchenParadise.Utiltiy.Base;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData.ExplosionManagement;

namespace Match3.Game.Gameplay.BeachBallMechanic
{
    public struct BeachBallActivationKeyType : KeyType
    { }

    public interface BeachBallActivationPresentationHandler : PresentationHandler
    {
        void Activate(BeachBallMainTile beachBallMainTile, Action onCompleted);
    }

    class BeachBall
    {
        public BeachBallMainTile mainTile;
        public List<SlaveTile> slaves = new List<SlaveTile>();

    }

    [Before(typeof(ExplosionManagement.ExplosionActivationSystem))]
    public class BeachBallActivationSystem : GameplaySystem
    {
        // TODO: Move this to config.
        const float EXPLOSION_PROPAGATION_DELAY = 0.06f;

        BeachBallActivationPresentationHandler presentationHandler;

        List<BeachBall> beachBalls = new List<BeachBall>();

        BeachBallActivationData beachBallActivationData;

        public BeachBallActivationSystem(GameplayController gameplayController) : base(gameplayController)
        {
            presentationHandler = gameplayController.GetPresentationHandler<BeachBallActivationPresentationHandler>();
            beachBallActivationData = GetFrameData<BeachBallActivationData>();
        }

        public override void Start()
        {
            foreach (var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (HasTileOnTop<BeachBallMainTile>(cellStack))
                    CreateBeachBallFor(TopTile(cellStack) as BeachBallMainTile);

            if (beachBalls.IsEmpty())
                gameplayController.DeactivateSystem<BeachBallActivationSystem>();
        }

        private void CreateBeachBallFor(BeachBallMainTile beachBallMainTile)
        {
            var beachBall = new BeachBall();
            beachBall.mainTile = beachBallMainTile;

            var cellStackBoard = gameplayController.GameBoard().CellStackBoard();
            var position = beachBallMainTile.Parent().Parent().Position();

            // TODO: Find another way to find the slaves. A way that does not rely how Beach Ball is shaped in the board
            beachBall.slaves.Add(TopTile(cellStackBoard[position.x + 1 , position.y    ]) as SlaveTile);
            beachBall.slaves.Add(TopTile(cellStackBoard[position.x     , position.y +1 ]) as SlaveTile);
            beachBall.slaves.Add(TopTile(cellStackBoard[position.x + 1 , position.y +1 ]) as SlaveTile);

            beachBalls.Add(beachBall);
        }

        public override void Update(float dt)
        {
            for(int i = beachBalls.Count-1; i>=0; --i)
            {
                var beachBall = beachBalls[i];
                if(MustBeActivated(beachBall))
                {
                    Activate(beachBall);
                    beachBalls.RemoveAt(i);
                }
            }
        }

        private bool MustBeActivated(BeachBall beachBall)
        {
            return beachBall.mainTile.IsReadyToBeDestoryed() &&
                beachBall.mainTile.IsDestroyed() == false &&
                IsFullyFree(beachBall.mainTile.Parent().Parent());
        }


        private void Activate(BeachBall beachBall)
        {
            FullyLock<BeachBallActivationKeyType>(beachBall.mainTile.Parent().Parent());
            presentationHandler.Activate(beachBall.mainTile, () => ApplyBeachBallActivation(beachBall));
        }

        private void ApplyBeachBallActivation(BeachBall beachBall)
        {
            DestroyBeachBall(beachBall);
            ApplyBeachBallExplosion(beachBall);
        }

        private void ApplyBeachBallExplosion(BeachBall beachBall)
        {
            var origin = beachBall.mainTile.Parent().Parent();

            // NOTE: This is hitting the beach ball too.
            foreach (var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
            {
                if (IsFullyFree(cellStack))
                {
                    var delay = DistanceOf(origin, cellStack) * EXPLOSION_PROPAGATION_DELAY;
                    beachBallActivationData.delayedCellStackHits.Add(new DelayedCellHitData(cellStack, delay));
                    TryActivateExplosiveIn(cause: beachBall.mainTile, cellStack, delay);
                }
            }
        }

        // TODO: Find a unified solution for this problem of checking for explosives in each different system which needs this flow, maybe a more unified way instead of being DRY
        private void TryActivateExplosiveIn(Tile cause, CellStack cellStack, float delay)
        {
            if (HasTileOnTop<ExplosiveTile>(cellStack))
                GetFrameData<InternalExplosionActivationData>().delayedTargets.
                    Add(new DelayedActivationData(cellStack.CurrentTileStack(), cause: cause, 0)); // Note: We are setting delay to zero because non zero values may cause missed combo counting, A better way should be find
        }

        private void DestroyBeachBall(BeachBall beachBall)
        {
            var destructionData = GetFrameData<DestructionData>();

            FullyDestroy(beachBall.mainTile);
            destructionData.tilesToDestroy.Add(beachBall.mainTile);

            foreach (var slave in beachBall.slaves)
            {
                FullyDestroy(slave);
                destructionData.tilesToDestroy.Add(slave);
            }
        }
    }
}