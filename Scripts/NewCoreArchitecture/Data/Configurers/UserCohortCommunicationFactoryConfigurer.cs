using Match3.Foundation.Base.CohortManagement;
using PandasCanPlay.HexaWord.Utility;
using UnityEngine;

namespace Match3.Data.Configuration
{

    [CreateAssetMenu(menuName = "Match3/Configurations/UserCohortCommunicationFactoryConfigurer")]
    public class UserCohortCommunicationFactoryConfigurer : GenericServiceFactoryConfigurer<CohortManagementServerCommunicator>
    {
        [TypeAttribute(typeof(CohortManagementServerCommunicator), false)]
        public string targetType = "";
        [TypeAttribute(typeof(CohortManagementServerCommunicator), false)]
        public string fallbackType = "";


        protected override string FallbackType()
        {
            return fallbackType;
        }

        protected override string TargetType()
        {
            return targetType;
        }
    }
}