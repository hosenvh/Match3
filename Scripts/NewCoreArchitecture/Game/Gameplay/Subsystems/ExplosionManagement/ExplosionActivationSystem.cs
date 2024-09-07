using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Input;
using Match3.Game.Gameplay.SubSystemsData.FrameData.ExplosionManagement;
using Match3.Game.Gameplay.Swapping;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Game.Gameplay.ExplosionManagement
{
    public struct ExplosiveActivatedEvent : GameEvent
    {
        public readonly ExplosiveTile explosiveTile;

        public ExplosiveActivatedEvent(ExplosiveTile explosiveTile)
        {
            this.explosiveTile = explosiveTile;
        }
    }

    [Before(typeof(HitManagement.HitGenerationSystem))]
    public class ExplosionActivationSystem : GameplaySystem
    {
        public event Action<ExplosiveTile> OnTileExplosion = delegate {  };

        // TODO: Move this to config.
        const float explosionPropagationDelay = 0.06f;


        CellStackBoard cellStackBoard;

        HashSet<DelayedCellHitData> toHitCells = new HashSet<DelayedCellHitData>();

        Queue<ExplosionProcessingData> explosiveToProcess = new Queue<ExplosionProcessingData>(3);

        HashSet<ExplosiveTile> processedExplosives = new HashSet<ExplosiveTile>();

        List<DelayedActivationData> scheduledExplosiveActivations = new List<DelayedActivationData>();

        public ExplosionActivationSystem(GameplayController gameplayController) : base(gameplayController)
        {
            cellStackBoard = this.gameplayController.GameBoard().CellStackBoard();

            // NOTE: A workaround for setting HashSet initial capacity.
            for (int i = 0; i < 20; ++i)
                toHitCells.Add(new DelayedCellHitData());
            toHitCells.Clear();
        }

        public override void Update(float dt)
        {
            explosiveToProcess.Clear();
            processedExplosives.Clear();

            AdvanceScheduleExplosives(dt);

            DetermineExplosivesToProcess();
            ProcessExplosives();
        }

        private void AdvanceScheduleExplosives(float dt)
        {
            //activatedHitDelays.Clear();

            for (int i = 0; i < scheduledExplosiveActivations.Count; ++i)
            {
                var data = scheduledExplosiveActivations[i];

                var currentDelay = data.delay - dt;
                scheduledExplosiveActivations[i] = new DelayedActivationData(data.tileStack, data.cause, currentDelay);
            }

        }

        private void DetermineExplosivesToProcess()
        {
            foreach (var data in GetFrameData<InternalExplosionActivationData>().delayedTargets)
                if (IsExplosive(data.tileStack))
                    scheduledExplosiveActivations.Add(data);

            foreach (var tileStack in GetFrameData<InternalExplosionActivationData>().targets)
                if (IsExplosive(tileStack))
                    TryEnqueue(tileStack, 0);


            foreach (var tileStack in GetFrameData<UserRequestedTileStackActivationData>().targets)
                if (IsExplosive(tileStack))
                {
                    TryEnqueue(tileStack, 0);
                    GetFrameData<SuccessfulUserActivationData>().targets.Add(tileStack);
                }


            for (int i = scheduledExplosiveActivations.Count-1; i >= 0; --i)
            {
                var data = scheduledExplosiveActivations[i];
                if (data.delay <= 0)
                {
                    TryEnqueue(data.tileStack, 0, data.cause);
                    scheduledExplosiveActivations.RemoveAt(i);
                }
            }


            foreach (var swapData in GetFrameData<ExecutedSwapsData>().data)
            {
                if (HasExplosive(swapData.originTarget))
                    TryEnqueue(swapData.originTarget.CurrentTileStack(), 0);
                if (HasExplosive(swapData.destinationTarget))
                    TryEnqueue(swapData.destinationTarget.CurrentTileStack(), 0);
            }
        }


        private void ProcessExplosives()
        {
           
            while (explosiveToProcess.Count > 0)
            {
                ExplosionProcessingData explosionData = explosiveToProcess.Dequeue();
                processedExplosives.Add(explosionData.explosiveTile);
                Activate(explosionData);
                GetFrameData<ExplosionActivationData>().processedExplosives.Add(explosionData);
                OnTileExplosion.Invoke(explosionData.explosiveTile);
            }
        }


        // TODO: Refactor this.
        private void Activate(ExplosionProcessingData explosionData)
        {
            toHitCells.Clear();

            var explosionTile = explosionData.explosiveTile;
            var effectRadius = explosionTile.MaxEffectRadius();
            var effectDistance = explosionTile.EffectDistance();

            Vector2Int origin = explosionTile.Parent().Parent().Position();

            for(int i = -effectRadius; i <= effectRadius; ++i )
                for(int j = -effectRadius; j <= effectRadius; j++)
                {
                    var distance = Math.Abs(i) + Math.Abs(j);
                    Vector2Int position = origin + new Vector2Int(i, j);
                    if (cellStackBoard.IsInRange(position.x, position.y) && distance <= effectDistance)
                    {
                        var cellStack = cellStackBoard[position];
                        if (cellStack.HasTileStack() && cellStack.CurrentTileStack().GetComponent<LockState>().IsLocked())
                            continue;
                        var distanceFromFirstCause = distance + explosionData.distanceToFirstCause;
                        
                        toHitCells.Add(new DelayedCellHitData(cellStack, distanceFromFirstCause * explosionPropagationDelay));
                        if (HasExplosive(cellStack))
                            TryEnqueue(
                                cellStack.CurrentTileStack(),
                                distanceFromFirstCause,
                                explosionTile);
                    }
                }


            ServiceLocator.Find<EventManager>().Propagate(new ExplosiveActivatedEvent(explosionData.explosiveTile), this);

            GetFrameData<ExplosionActivationData>().explosionsHitData.Add(new SingleExplosionHitData(explosionTile, new List<DelayedCellHitData>(toHitCells)));
        }


        void TryEnqueue(TileStack tileStack, int distanceToFirstCause, Tile directCause = null)
        {
            if (QueryUtilities.HasTileOnTop<ExplosiveTile>(tileStack) == false)
                return;
            var explosiveTile = tileStack.Top().As<ExplosiveTile>();

            if (processedExplosives.Contains(explosiveTile) == false)
                explosiveToProcess.Enqueue(new ExplosionProcessingData(explosiveTile, directCause, distanceToFirstCause));
        }

        private bool HasExplosive(CellStack cellStack)
        {
            return cellStack.HasTileStack() && IsExplosive(cellStack.CurrentTileStack());
        }


        private bool IsExplosive(TileStack origin)
        {
            return origin.IsDepleted() == false && origin.Top() is ExplosiveTile;
        }

    }
}