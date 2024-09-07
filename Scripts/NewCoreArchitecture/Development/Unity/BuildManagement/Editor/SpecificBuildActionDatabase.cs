using Medrick.Development.Base.BuildManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Specific Build Action Database")]
    public class SpecificBuildActionDatabase : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public string tag;
            public List<ScriptableBuildAction> actions;
        }

        public List<Entry> taggedActions;

        public void RegisterTo(BuildOptionsManager manager)
        {
            foreach (var entry in taggedActions)
                foreach(var action in entry.actions)
                    manager.RegisterSpecificBuildAction(entry.tag, action);
        }
    }
}