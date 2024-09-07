using KitchenParadise.Utiltiy.Base;
using Match3.Game.Gameplay.Core;
using System;
using System.Collections.Generic;
//using System.Linq;

namespace Match3.Game.Gameplay.Matching
{
    // TODO: Refactor this fuck.
    public class MaximallyMergedMatchesCalculator
    {
        HashSet<Match> currentStepMatches = new HashSet<Match>();
        HashSet<Match> nextStepMatches = new HashSet<Match>();
        HashSet<Match> matchesToRemoved = new HashSet<Match>();
        HashSet<Match> lockedMatches = new HashSet<Match>();

        HashSet<TileStack> __TEMP_HASHSET = new HashSet<TileStack>();

        public MaximallyMergedMatchesCalculator()
        {
            currentStepMatches.SetCapacity(16);
            nextStepMatches.SetCapacity(16);
            matchesToRemoved.SetCapacity(16);
            lockedMatches.SetCapacity(16);
            __TEMP_HASHSET.SetCapacity(16);
        }

        public HashSet<Match> Calculate(List<Match> horizontalMatches, List<Match> verticalMatches, HashSet<Match> lockedMatches)
        {
            currentStepMatches.Clear();
            nextStepMatches.Clear();

            this.lockedMatches = lockedMatches;

            foreach (var m in horizontalMatches)
                currentStepMatches.Add(m);

            foreach (var m in verticalMatches)
                currentStepMatches.Add(m);

            bool mergeOccured = false;
            do
            {
                nextStepMatches.Clear();
                MergeOneStep(currentStepMatches, nextStepMatches, out mergeOccured);
                var temp = currentStepMatches;
                currentStepMatches = nextStepMatches;
                nextStepMatches = temp;
                

            } while (mergeOccured);

            if(lockedMatches.Count > 0)
                currentStepMatches.ExceptWith(lockedMatches);

            return currentStepMatches;

        }

        public void MergeOneStep(HashSet<Match> currentStep, HashSet<Match> nextStep, out bool globalMergeOccured)
        {
            globalMergeOccured = false;
            matchesToRemoved.Clear();

            bool interalMergeOccured = false;

            while(currentStep.Count > 0)
            {
                var selectedMatch = FirstOf(currentStep);
                currentStep.Remove(selectedMatch);
                matchesToRemoved.Clear();
                interalMergeOccured = false;
                foreach (var match in currentStep)
                {
                    if (AreMergeable(selectedMatch, match))
                    {
                        globalMergeOccured = true;
                        interalMergeOccured = true;
                        var mergedMatch = Merge(selectedMatch, match);
                        if (IsLocked(selectedMatch) || IsLocked(match))
                        {
                            lockedMatches.Add(mergedMatch);
                        }

                        nextStep.Add(mergedMatch);
                        matchesToRemoved.Add(match);
                    }
                }
                if (!interalMergeOccured)
                    nextStep.Add(selectedMatch);

                currentStep.ExceptWith(matchesToRemoved);
            }

        }


        Match FirstOf(HashSet<Match> set)
        {
            var e = set.GetEnumerator();
            e.MoveNext();
            return  e.Current;
        }

        private bool IsLocked(Match match)
        {
            return lockedMatches.Contains(match);
        }

        bool AreMergeable(Match p1, Match p2)
        {
            __TEMP_HASHSET.Clear();
            __TEMP_HASHSET.UnionWith(p1.tileStacks);

            __TEMP_HASHSET.IntersectWith(p2.tileStacks);

            return __TEMP_HASHSET.Count > 0;
        }


        private Match Merge(Match match1, Match match2)
        {
            var merge = new Match();
            merge.tileStacks.AddRange(match1.tileStacks);
            merge.tileStacks.AddRange(match2.tileStacks);
            return merge;
        }
    }
}