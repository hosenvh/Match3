using System;
using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;

namespace Match3.Game.Gameplay.SubSystems.LevelEnding
{
    public enum LevelResult { Win, Lose};

    public interface LevelStoppingPresentationHandler : PresentationHandler
    {
        void HandleStopping();
    }


    public interface StopConditinon
    {
        void Update(float dt, LevelStoppingSystem system);
        bool IsSatisfied();
    }

    public class LevelStoppingSystem : GameplaySystem
    {
        List<GoalTracker> allGoalTrackers = new List<GoalTracker>();

        List<GoalTracker> destructionBasedGoalTrackers = new List<GoalTracker>();
        List<GoalTracker> hitBasedGoalTrackers = new List<GoalTracker>();

        DestroyedObjectsData destroyedObjectsData;
        GeneratedObjectsData generatedObjectsData;
        AppliedHitsData appliedHitsData;


        List<StopConditinon> stopConditions = new List<StopConditinon>();


        bool isStopped = false;
        StopConditinon stoppingCause;


        public LevelStoppingSystem(GameplayController gameplayController) : base(gameplayController)
        {
            destroyedObjectsData = GetFrameData<DestroyedObjectsData>();
            generatedObjectsData = GetFrameData<GeneratedObjectsData>();
            appliedHitsData = GetFrameData<AppliedHitsData>();
        }

        public override void Reset()
        {
            isStopped = false;
            stoppingCause = null;
        }

        public void AddGoal(GoalTargetType goalType, int goalAmount)
        {
            var tracker = new GoalTracker(goalType, goalAmount);

            if (typeof(DestructionBasedGoalObject).IsAssignableFrom(goalType.GoalObjectType()))
                destructionBasedGoalTrackers.Add(tracker);
            if (typeof(HitBasedGoalObject).IsAssignableFrom(goalType.GoalObjectType()))
                hitBasedGoalTrackers.Add(tracker);

            allGoalTrackers.Add(tracker);

        }

        public void AddStopCondition(StopConditinon stopCondition)
        {
            this.stopConditions.Add(stopCondition);
        }

        public List<StopConditinon> StopConditions()
        {
            return stopConditions;
        }

        public bool HasStoppingCondition<T>() where T : StopConditinon
        {
            foreach (var condition in stopConditions)
                if (condition is T)
                    return true;

            return false;
        }

        public T FindStoppingCondition<T>() where T : StopConditinon
        {
            foreach (var condition in stopConditions)
                if (condition is T)
                    return (T)condition;

            return default(T);
        }

        public override void Update(float dt)
        {
            UpdateGoalTrackers();
            UpdateStopConditions(dt);

            DetermineLevelStopping();
        }

        private void DetermineLevelStopping()
        {
            if (GoalsAreReached())
                StopGame();
            else if (ShouldStop())
                StopGame();
        }

        bool ShouldStop()
        {
            foreach (var condition in stopConditions)
                if (condition.IsSatisfied())
                    return true;
            return false;
        }


        void StopGame()
        {
            if (isStopped == false)
            {
                isStopped = true;
                foreach (var condition in stopConditions)
                    if (condition.IsSatisfied())
                        stoppingCause = condition;

                gameplayController.GetPresentationHandler<LevelStoppingPresentationHandler>().HandleStopping();
                gameplayController.StopGame();
            }
        }

        private void UpdateGoalTrackers()
        {
            foreach (var tile in destroyedObjectsData.tiles)
                TryTrackDestruction(tile as GoalObject, ref destructionBasedGoalTrackers);
            foreach (var cell in destroyedObjectsData.cells)
                TryTrackDestruction(cell as GoalObject, ref destructionBasedGoalTrackers);
            foreach (var tile in appliedHitsData.tilesStartedBeingHit)
                TryTrackDestruction(tile as GoalObject, ref hitBasedGoalTrackers);

            foreach (var tile in generatedObjectsData.tiles)
                TryTrackGeneration(tile as GoalObject, ref destructionBasedGoalTrackers);
            foreach (var cell in generatedObjectsData.cells)
                TryTrackGeneration(cell as GoalObject, ref destructionBasedGoalTrackers);
        }

        private void TryTrackDestruction(GoalObject goalObject, ref List<GoalTracker> trackers)
        {
            if (goalObject != null)
                foreach (var tracker in trackers)
                    tracker.TrackDestruction(goalObject);   
        }

        private void TryTrackGeneration(GoalObject goalObject, ref List<GoalTracker> trackers)
        {
            if (goalObject != null)
                foreach (var tracker in trackers)
                    tracker.TrackGeneration(goalObject);
        }

        private void UpdateStopConditions(float dt)
        {
            foreach(var stopCondition in stopConditions)
                stopCondition.Update(dt, this);
        }

        bool GoalsAreReached()
        {
            bool areReached = true;
            foreach (var tracker in allGoalTrackers)
                areReached = areReached && tracker.IsGoalReached();

            return areReached;
        }

        public LevelResult GetLevelResult()
        {
            if (GoalsAreReached())
                return LevelResult.Win;
            else
                return LevelResult.Lose;
        }

        public List<GoalTracker> DestructionBasedGoals()
        {
            return destructionBasedGoalTrackers;
        }

        public List<GoalTracker> HitBasedGoals()
        {
            return hitBasedGoalTrackers;
        }

        public List<GoalTracker> AllGoals()
        {
            return allGoalTrackers;
        }

        public StopConditinon StoppingCause()
        {
            return stoppingCause;
        }
    }
}