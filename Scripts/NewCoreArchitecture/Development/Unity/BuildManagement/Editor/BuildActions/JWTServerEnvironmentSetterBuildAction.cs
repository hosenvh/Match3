using Match3.ServerCommunication.Config;
using UnityEngine;


namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/" + nameof(JWTServerEnvironmentSetterBuildAction))]
    public class JWTServerEnvironmentSetterBuildAction : ScriptableBuildAction
    {
        [SerializeField] private GolmoradServerCommunicationConfigurer config;
        [SerializeField] private GolmoradServerCommunicationConfigurer.ServerEnvironment targetEnvironment;

        public override void Execute()
        {
            config.SetServerEnvironment(targetEnvironment);
        }

        public override void Revert()
        {
        }
    }
}