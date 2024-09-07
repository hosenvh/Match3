using Match3.Game.Gameplay.SubSystems.LevelEnding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BoardConfig;

namespace Match3.Presentation
{
    [Serializable]
    public struct GoalTypePrefabInfo
    {
        public GoalTypeInfo goalTypeInfo;
        public GameObject prefab;
    }


    public class GoalTypePrefabDatabase : MonoBehaviour
    {

        public List<GoalTypePrefabInfo> goalTypePrefabs;


        public GameObject PrefabFor(GoalTargetType goalTargetType)
        {
            foreach (var prefabInfo in goalTypePrefabs)
                if (goalTargetType.Is(prefabInfo.goalTypeInfo.ExtractGoalType()))
                    return prefabInfo.prefab;


            return null;

        }

        public void ActivateObjectFor(GoalTargetType goalTargetType)
        {
            PrefabFor(goalTargetType).SetActive(true);
        }

    }
}
