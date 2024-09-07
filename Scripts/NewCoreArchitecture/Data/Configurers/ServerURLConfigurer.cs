using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.CohortManagement;
using Match3.Foundation.Unity.Configuration;
using Match3.Game.NeighborhoodChallenge.RequestHandling;
using Match3.Game.ReferralMarketing;
using Match3.Game.ReferralMarketing.RequestHandling;
using PandasCanPlay.HexaWord.Utility;
using UnityEngine;

namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/Configurations/GolmoradUserCohortServerCommunicationConfigurer")]
    public class ServerURLConfigurer : ScriptableConfiguration, 
        Configurer<GolmoradCohortManagementServerCommunicator> , 
        Configurer<ServerConfigRequest>, 
        Configurer<GiftCodeRequestHandler>,
        Configurer<NCServerRequestHandler>,
        Configurer<MarketManager>,
        Configurer<ReferralCenterRequestHandler>
    {

        [Dropdown(@"http://172.16.10.225:6060", @"https://srv1.trantor.medrick.games", @"http://172.16.11.188:6060",
            "https://turkey.trantor.medrick.games", "http://81.91.151.100:6040", "https://trantor.relicbit.com")]
        public string trantorServerUrl;

        [Dropdown(@"http://172.16.10.225:5010", @"https://cbv.medrickgames.com", "https://qazanfar.cbv.medrickgames.com", @"http://172.16.10.225:8080", "N/A")]
        public string marketVerficationServerUrl;

        public void Configure(GolmoradCohortManagementServerCommunicator entity)
        {
            entity.SetServerURL(trantorServerUrl);
        }

        public void Configure(ServerConfigRequest entity)
        {
            entity.SetServerURL(trantorServerUrl);
        }

        public void Configure(GiftCodeRequestHandler entity)
        {
            entity.SetServerURL(trantorServerUrl);
        }

        public void Configure(NCServerRequestHandler entity)
        {
            entity.SetServerURL(trantorServerUrl);
        }

        public void Configure(MarketManager entity)
        {
            entity.SetServerURL(marketVerficationServerUrl);
        }

        public void Configure(ReferralCenterRequestHandler entity)
        {
            entity.SetServerUrl(trantorServerUrl);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register<GolmoradCohortManagementServerCommunicator>(this);
            manager.Register<ServerConfigRequest>(this);
            manager.Register<GiftCodeRequestHandler>(this);
            manager.Register<NCServerRequestHandler>(this);
            manager.Register<MarketManager>(this);
            manager.Register<ReferralCenterRequestHandler>(this);
        }
    }
}