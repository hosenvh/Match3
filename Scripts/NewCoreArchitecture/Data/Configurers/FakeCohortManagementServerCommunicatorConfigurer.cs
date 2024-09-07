
using Match3.Foundation.Base.CohortManagement;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenParadise.Data.Configuration
{
    [CreateAssetMenu(menuName = "Medrick/CohortManagemenent/FakeServerCommunicationManagerConfigurer")]
    public class FakeCohortManagementServerCommunicatorConfigurer : ScriptableConfiguration, Configurer<FakeCohortManagementServerCommunicator>
    {
        public bool useFakeServer;
        public float communicationDelay;
        public FakeCohortManagementServerCommunicator.RequestResult cohortRequestResult;
        public string cohortID;

        public FakeCohortManagementServerCommunicator.RequestResult balanceRequestResult;
        [Multiline]
        public string balanceConfigJson;

        public void Configure(FakeCohortManagementServerCommunicator entity)
        {
            var balanceConfigJsonObject = NiceJson.JsonObject.ParseJsonString(this.balanceConfigJson).As<NiceJson.JsonObject>();

            entity.balanceConfig = balanceConfigJsonObject;
            entity.balanceRequestResult = this.balanceRequestResult;
            entity.cohortRequestResult = this.cohortRequestResult;
            entity.cohortID = this.cohortID;
            entity.communicationDelay = communicationDelay;
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
}