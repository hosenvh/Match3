using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/Googleplay Settings Setter")]
    public class GoogleplayServerSettingSetter : ScriptableBuildAction
    {
        [SerializeField] private FirebaseAndGoogleplayServiceSettingsChangerBuildAction target;
        [TextArea(10, 50)] [SerializeField] private string googleplaySettings = string.Empty;

        public override void Execute()
        {
            target.SetConfig(googleplaySettings);
        }

        public override void Revert()
        {
            
        }
    }
}