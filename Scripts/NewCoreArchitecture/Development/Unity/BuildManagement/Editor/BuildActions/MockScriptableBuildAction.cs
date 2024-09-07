using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/Mock Scriptable Build Action")]
    public class MockScriptableBuildAction : ScriptableBuildAction
    {
        public string message;
        public override void Execute()
        {
            UnityEngine.Debug.Log(message);
        }

        public override void Revert()
        {
            Debug.Log("reseting: " + message);
        }
    }
}