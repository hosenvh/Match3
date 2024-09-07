using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KitchenParadise.Presentation;
using Match3.Foundation.Base.Destruction;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.DestructionManagement;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Cells;
using Match3.Presentation.Gameplay.Core;
using Match3.Presentation.Gameplay.Tiles;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Presentation.Gameplay.GoalManagement
{
    // TODO: Refactor this shit.
    public class GoalTargetGatheringPresentationHandlerImp : MonoBehaviour , GoalTargetGatheringPresentationHandler
    {
        public CurvedMovementInfo targetToGoalMovementInfo;
        public float targetRotationSpeed;
        public Transform effectContainer;
        public Image fireflyPresenter;

        public GoalsPresentationController goalsPresentationController;
        public PresentationDestructionHandler presentationDestructionHandler;
        public GameplayStateRoot gameplayStateRoot;

        GameplaySoundManager gameplaySoundManager;

        LevelStoppingSystem levelStoppingSystem;

        void Start()
        {
            levelStoppingSystem = gameplayStateRoot.gameplayState.gameplayController.GetSystem<LevelStoppingSystem>();
            gameplaySoundManager = ServiceLocator.Find<GameplaySoundManager>();
        }

        private void OnDestroy()
        {
            ServiceLocator.Find<UnityTimeScheduler>().UnSchedule(this);
        }

        public void GatherOnDestruction(Tile tile, Action<Tile> onTileDestroyed, Action onGatheringCompleted)
        {
            tile.GetComponent<TilePresenter>().transform.SetParent(effectContainer);

            if (IsStaionaryTarget(tile))
            {
                if (IsHitBasedGoalTarget(tile))
                    HandleHitBasedTarget(tile, onGatheringCompleted);
                else
                {
                    PlayGoalGatheringEffectOf(tile);
                    onGatheringCompleted();
                }

                PresentationDestructionUtility.Destroy(tile, onTileDestroyed);
            }
            else
            {
                TryPlayMoveEffectFor(tile);

                GoalPresenterFor(tile as GoalObject).IncreasePendingGatherings();

                if (NeedsHiting(tile))
                    PlayHitEffect(tile, () => MoveToGoal(tile, tile.GetComponent<TilePresenter>(), () => onTileDestroyed(tile), onGatheringCompleted));
                else
                    MoveToGoal(tile, tile.GetComponent<TilePresenter>(), () => onTileDestroyed(tile), onGatheringCompleted);
            }

            // TODO: Find a better way to handle the explosin effect.
            presentationDestructionHandler.TryPlayExplostionEffect(tile);

        }

        public void GatherOnDestruction(Cell cell, Action<Cell> onCellDestroyed, Action onGatheringCompleted)
        {
            if (IsStaionaryTarget(cell))
            {
                PlayGoalGatheringEffectOf(cell);
                onGatheringCompleted();
                PresentationDestructionUtility.Destroy(cell, onCellDestroyed);
            }
            else
            {
                TryFixRotationAndScale(cell);

                GoalPresenterFor(cell as GoalObject).IncreasePendingGatherings();

                MoveToGoal(cell, cell.GetComponent<Core.CellPresenter>(), () => onCellDestroyed(cell), onGatheringCompleted);
            }
        }

        void TryPlayMoveEffectFor(Tile tile)
        {
            if (tile is Butterfly)
                tile.GetComponent<ButterflyTilePresenter>().PlayFlyEffect();
        }

        void TryFixRotationAndScale(Cell cell)
        {
            var presenter = cell.GetComponent<Gameplay.Core.CellPresenter>();
            if (presenter is ArtifactMainCellPresenter)
                FixRotationAndScaleFor(presenter as ArtifactMainCellPresenter, 0f);
        }

        void PlayHitEffect(Tile tile, Action onCompleted)
        {
            tile.GetComponent<TilePresenter>().PlayHitEffect(onCompleted);
        }

        private void FixRotationAndScaleFor(ArtifactMainCellPresenter presenter, float delay)
        {

            var targetScale = new Vector2(0.5f, 1f);

            presenter.scaleTransform.DOScale(targetScale, 1f).SetDelay(delay);
            presenter.rotationTransform.DORotate(Vector3.zero, 1f).SetDelay(delay);
        }

        // TODO: Move this to a config or a componenet inside tile.
        bool IsStaionaryTarget(Tile tile)
        {
            return tile is ColoredBox || tile is FireflyJar || tile is RocketBox || tile is HoneyJar || tile is Honey || tile is ExplosiveTile;
        }

        // TODO: Move this to a config or a componenet inside tile.
        bool IsStaionaryTarget(Cell cell)
        {
            return cell is IvyRootCell || cell is HedgeCell;
        }

        // TODO: Move this to a config or a componenet inside tile.
        bool NeedsHiting(Tile tile)
        {
            return tile is SandWithGem || tile is FishStatue || tile is Buoyant || tile is JamJar || tile is CompassTile;
        }


        void PlayGoalGatheringEffectOf(Tile tile)
        {
            GoalPresenterFor(tile as GoalObject).PlayGatheringEffect();
        }

        void PlayGoalGatheringEffectOf(Cell cell)
        {
            GoalPresenterFor(cell as GoalObject).PlayGatheringEffect();
        }

        public void HandleHitBasedTarget(Tile tile, Action onReached)
        {
            if (tile is FireflyJar fireflyJar)
            {
                GoalPresenterFor(tile as GoalObject).IncreasePendingGatherings();
                new FireflyExitHandler(fireflyJar, this).Handle(onReached);
            }
        }

        void MoveToGoal(object obj, MonoBehaviour presenter, Action onDetached, Action onReached)
        {
            TryPlayerSoundFor(obj);

            var goalPresenter = GoalPresenterFor(obj as GoalObject);

            presenter.transform.SetParent(effectContainer);

            presenter.transform.
                DoMoveWithForce(targetToGoalMovementInfo.force, targetToGoalMovementInfo.forceAngle, goalPresenter.transform.position, targetToGoalMovementInfo.speed, false).
                SetSpeedBased(true).
                SetEase(targetToGoalMovementInfo.curve).
                OnComplete(
                () => { AddTargetToGoal(presenter, goalPresenter); onReached(); });

            ServiceLocator.Find<UnityTimeScheduler>().Schedule(0.2f, onDetached, this);
        }

        public void MoveToGoal(object obj, MonoBehaviour presenter, Action onCompleted, Vector3 initialForce)
        {
            TryPlayerSoundFor(obj);

            var goalPresenter = GoalPresenterFor(obj as GoalObject);

            var forcePos = presenter.transform.position + initialForce;

            Vector3[] position = { goalPresenter.transform.position, forcePos, presenter.transform.position };

            presenter.transform.SetParent(effectContainer);
            presenter.transform.DOPath(position, targetToGoalMovementInfo.speed, PathType.CubicBezier).
                SetSpeedBased(true).
                SetEase(Ease.InSine).
                OnComplete(
                () => AddTargetToGoal(presenter, goalPresenter));

            ServiceLocator.Find<UnityTimeScheduler>().Schedule(0.2f, onCompleted, this);
        }

        private void TryPlayerSoundFor(object obj)
        {
            if (obj is Tile t)
                gameplaySoundManager.TryPlayHitSoundFor(t);
        }

        public bool IsHitBasedGoalTarget(object obj)
        {
            return IsTarget(obj, levelStoppingSystem.HitBasedGoals());
        }

        bool IsTarget(object obj, List<GoalTracker> trackers)
        {
            foreach (var tracker in trackers)
                if (tracker.goalType.Includes(obj as GoalObject) && tracker.IsGoalReached() == false)
                    return true;

            return false;
        }

        void AddTargetToGoal(MonoBehaviour target, GoalPresenter goalPresenter)
        {
            if (target is Destroyable destroyable)
                destroyable.Destroy();
            else
                Destroy(target.gameObject);
            goalPresenter.DecreasePendingGatherings();
            goalPresenter.PlayGatheringEffect();
        }


        private GoalPresenter GoalPresenterFor(GoalObject obj)
        {
            foreach(var goalTracker in levelStoppingSystem.AllGoals())
                if (goalTracker.goalType.Includes(obj))
                    return goalsPresentationController.GoalPresenterFor(goalTracker.goalType);

            return null;
        }

        private GoalPresenter GoalPresenterFor(GoalTracker target)
        {
            foreach (var goalTracker in levelStoppingSystem.AllGoals())
                if (goalTracker.goalType.Is(target.goalType))
                    return goalsPresentationController.GoalPresenterFor(goalTracker.goalType);

            return null;
        }
    }

}