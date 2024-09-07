using Match3.Network;
using PandasCanPlay.HexaWord.Utility;
using UnityEngine;

namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/Configurations/ServerConnectionFactoryConfigurer")]
    public class ServerConnectionFactoryConfigurer : GenericServiceFactoryConfigurer<ServerConnection>
    {
        [TypeAttribute(typeof(ServerConnection), false)]
        public string targetType = "";
        [TypeAttribute(typeof(ServerConnection), false)]
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