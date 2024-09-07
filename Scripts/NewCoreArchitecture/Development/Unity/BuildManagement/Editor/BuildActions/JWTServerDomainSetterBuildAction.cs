using Match3.ServerCommunication.Config;
using UnityEngine;
using UnityEngine.Serialization;


namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/" + nameof(JWTServerDomainSetterBuildAction))]
    public class JWTServerDomainSetterBuildAction : ScriptableBuildAction
    {
        [SerializeField] private GolmoradServerCommunicationConfigurer config;
        [SerializeField] private GolmoradServerCommunicationConfigurer.ServerDomain targetServerDomain;

        public override void Execute()
        {
            config.SetServerDomain(targetServerDomain);
        }

        public override void Revert()
        {
        }
    }
}