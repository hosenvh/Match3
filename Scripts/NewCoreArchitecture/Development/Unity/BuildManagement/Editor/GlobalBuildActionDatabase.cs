using Medrick.Development.Base.BuildManagement;
using System.Collections.Generic;
using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Global Build Action Database")]
    public class GlobalBuildActionDatabase : ScriptableObject
    {
        public List<ScriptableBuildAction> actions;

        public void RegisterTo(BuildOptionsManager manager)
        {
            foreach (var action in actions)
                manager.RegisterGlobalBuildAction(action);
        }
    }
}