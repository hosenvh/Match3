using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using SeganX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Match3
{
    // TODO: Remove this
    //Refactor this fucking type of mocking :<
//    public class ServerConfigRequestMock
//    {
//        public void CheckForUpdate(Action<string> callback)
//        {
//#if UNITY_ANDROID && !UNITY_EDITOR
//            throw new Exception("ServerConfigRequestMock has been used !!!");
//#elif UNITY_EDITOR
//            CheckUpdateState(callback);
//#endif
//        }

//        private void CheckUpdateState(Action<string> callback)
//        {

//            var jsonResponse = NiceJson.JsonNode.ParseJsonString(GetMockResponse());
//            if (jsonResponse.ContainsKey("msg"))
//            {
//                var msg = jsonResponse["msg"].ToString();
//                if (msg.ToLower() == "ok")
//                {
//                    var data = JsonUtility.FromJson<ServerConfigData>(jsonResponse["data"].ToJsonString());
//                    var timeManager = ServiceLocator.Find<ITimeManager>();
//                    data.serverTimeUtc = Utilities.NowTimeUnix() - 100;
                                                           
//                    timeManager.RefreshSyncTime(data.serverTimeUtc);
//                    ServiceLocator.Find<ServerConfigManager>().Overwrite(data);
//                    ServiceLocator.Find<EventManager>().Propagate(new ServerConfigEvent() { serverConfigData = data }, null);
//                    Advertisement.AdvertisementManager.Instance.UpdateAdvertisementConfiguration(data.config.adNetwork);
//                    callback(data.status);
//                    return;
//                }
//            }
//            callback("OK");
//        }

//        private string GetMockResponse()
//        {
//            string val = Network.MonoBehaviorUnityServerConnection.Instance.GetMockRequestData();
//            Debug.Log(val);
//            return val;
//        }

//    }
}