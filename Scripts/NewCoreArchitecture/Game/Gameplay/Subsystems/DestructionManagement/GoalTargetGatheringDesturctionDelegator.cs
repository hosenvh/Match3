

using System;
using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using System.Linq;

namespace Match3.Game.Gameplay.DestructionManagement
{

    public interface GoalTargetGatheringPresentationHandler : PresentationHandler
    {
        void GatherOnDestruction(Tile tile, Action<Tile> onTileDestroyed, Action onGatheringCompleted);
        void GatherOnDestruction(Cell cell, Action<Cell> onCellDestroyed, Action onGatheringCompleted);
    }

    /** TODO: Rewrite this system. It's duplicating the algorithm for goal tracking. Try to link this to goal tracker.
     *  Maybe the whole destruction handling must be moved to another place.
     */

    public class GoalTargetGatheringDestructionHandler : DestructionHandler
    {
        LevelStoppingSystem levelStoppingSystem;

        List<GoalTracker> goals = new List<GoalTracker>();
        Dictionary<GoalTracker, int> pendings = new Dictionary<GoalTracker, int>();

        GoalTargetGatheringPresentationHandler presentationHandler;

        public void Initialize(GameplayController gpc)
        {
            presentationHandler = gpc.GetPresentationHandler<GoalTargetGatheringPresentationHandler>();
            levelStoppingSystem = gpc.GetSystem<LevelStoppingSystem>();
            foreach (var goal in levelStoppingSystem.AllGoals())
            {
                goals.Add(goal);
                pendings.Add(goal, 0);
            }
        }

        public bool DoesAccept(Tile tile)
        {
            var goal = GoalFor(tile);
            return goal != null ? IsNotReached(goal) : false;
        }

        public bool DoesAccept(Cell cell)
        {
            var goal = GoalFor(cell);
            return goal != null ? IsNotReached(goal) : false;
        }

        public void Destroy(Tile tile, Action<Tile> onCompleted)
        {
            pendings[GoalFor(tile)]++;

            presentationHandler.GatherOnDestruction(tile, onCompleted, () => { });
        }

        public void Destroy(Cell cell, Action<Cell> onCompleted)
        {
            pendings[GoalFor(cell)]++;

            presentationHandler.GatherOnDestruction(cell, onCompleted, () => { });
        }

        private bool IsNotReached(GoalTracker goal)
        {
            return goal.CurrentAmount() + pendings[goal] < goal.goalAmount ;
        }

        GoalTracker GoalFor(object obj)
        {
            if (obj is GoalObject == false)
                return null;

            var goalObject = obj as GoalObject;
            for (int i = 0; i < goals.Count; i++)
            {
                GoalTracker goal = goals[i];
                if (goal.goalType.Includes(goalObject))
                    return goal;
            }

            return null;
        }

        public void Clear()
        {
            // TODO: Change this. It does create garbage.
            foreach (var key in pendings.Keys.ToList())
            {
                pendings[key] = 0;
            }
        }

        public bool DoesAccept(HitableCellAttachment attachment)
        {
            return false;
        }

        public void Destroy(HitableCellAttachment attachment, Action<HitableCellAttachment> onCompleted)
        {
            throw new NotImplementedException();
        }
    }
}
