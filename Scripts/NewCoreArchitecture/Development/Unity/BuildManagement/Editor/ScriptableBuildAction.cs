using Medrick.Development.Base.BuildManagement;
using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{

    public abstract class ScriptableBuildAction : ScriptableObject, BuildAction
    {
        public abstract void Execute();

        public abstract void Revert();
    }
}