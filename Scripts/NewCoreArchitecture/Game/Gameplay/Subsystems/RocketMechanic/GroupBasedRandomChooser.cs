using KitchenParadise.Utiltiy.Base;
using System;
using System.Collections.Generic;


namespace Match3.Game.Gameplay.SubSystems.RocketMechanic
{
    public class GroupBasedRandomChooser<T>
    {
        List<T> chosenTargets = new List<T>();

        HashSet<T> allPossibleTargets = new HashSet<T>();
        List<List<T>> groups = new List<List<T>>();

        public void AddGroup(List<T> group)
        {
            if (group.Count > 0)
            {
                groups.Add(group);
                allPossibleTargets.AddRange(group);
            }
        }

        public List<T> Choose(int amount, List<T> excludeds)
        {
            chosenTargets.Clear();

            foreach (var group in groups)
                group.RemoveAll(i => excludeds.Contains(i));

            for (int i = 0; i < amount; ++i)
            {
                groups.RemoveAll(g => g.Count == 0);
                if (groups.Count == 0)
                    break;

                var chosenGroup = groups.RandomElement();
                var chosenElement = chosenGroup.RandomElement();

                chosenTargets.Add(chosenElement);

                groups.ForEach(g => g.Remove(chosenElement));
            }

            return chosenTargets;
        }

        public void Clear()
        {
            groups.Clear();
            chosenTargets.Clear();
            allPossibleTargets.Clear();

        }
    }
}