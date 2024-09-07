using Medrick.Development.Base.BuildManagement;
using System.Collections.Generic;

namespace Medrick.Development.Unity.BuildManagement
{
    [System.Serializable]
    public class SerializableBuildOption : BuildOption
    {
        public string name;
        public List<ScriptableBuildAction> buildActions;

        public void Apply()
        {
            foreach (var action in buildActions)
                action.Execute();
        }


        public void Reset()
        {
            foreach (var action in buildActions)
                action.Revert();
        }

        public string Name()
        {
            return name;
        }

    }
}