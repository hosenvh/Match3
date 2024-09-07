using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity.Configuration;
using Match3.Network;
using PandasCanPlay.HexaWord.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Match3.Data.Configuration
{
    [CreateAssetMenu(menuName = "Match3/Configurations/MockServerConnectionConfigurer")]
    public class MockServerConnectionConfigurer : ScriptableConfiguration, Configurer<MockServerConnection>
    {
        [System.Serializable]
        public class SerializableRequestEntry
        {
            public string url;
            public bool isActive;
            public bool shouldFaild;
            [TextArea(minLines: 3, maxLines: 20)]
            public string successMessage;
            [TextArea(minLines:3, maxLines:20)]
            public string failureMessage;

            public MockServerConnection.RequestEntry CreateRequestEntry()
            {
                return new MockServerConnection.RequestEntry() 
                { 
                    failureMessage = failureMessage,
                    url = url, 
                    shouldFaild = shouldFaild,
                    successMessage = successMessage,
                    isActive = isActive
                };
            }
        }

        public List<SerializableRequestEntry> requestEntries;

        public void Configure(MockServerConnection entity)
        {
            foreach (var entry in requestEntries)
                entity.AddRequestEntry(entry.CreateRequestEntry());

            entity.SetRealServerConnect(new UnityWebRequestServerConnection());

        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
}