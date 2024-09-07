using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace Match3.CloudSave
{

    public class MapManagerDataHandler : ICloudDataHandler
    {
        
        private const string LastLoadedMapKey = "LastLoadedMap";
        private const string MapsUnlockStatusKey = "MapsUnlockStatus";
        
        public void CollectData(ICloudDataStorage cloudStorage)
        {
            var mapManager = Base.gameManager.mapManager;
            
            cloudStorage.SetString(LastLoadedMapKey, mapManager.GetLastLoadedMapId());
            
            var mapsMetaData = mapManager.MapsMetaDatabase.MapsMetadata;
            var stringBuilder = new StringBuilder();
            foreach (var mapMetaData in mapsMetaData)
            {
                if (stringBuilder.Length > 0) stringBuilder.Append(',');
                stringBuilder.Append($"{mapMetaData.mapId}:{mapManager.IsMapUnlocked(mapMetaData.mapId).ToString()}");
            }
            cloudStorage.SetString(MapsUnlockStatusKey, stringBuilder.ToString());
            stringBuilder.Clear();
        }

        public void SpreadData(ICloudDataStorage cloudStorage)
        {
            if (!IsAnyMapDataAvailable(cloudStorage)) return;

            var mapManager = Base.gameManager.mapManager;
            var lastLoadedMapId = cloudStorage.GetString(LastLoadedMapKey);
            mapManager.RestoreLastLoadedMapId(lastLoadedMapId);

            var mapsUnlockStringData = cloudStorage.GetString(MapsUnlockStatusKey).Split(',');
            foreach (var mapUnlockStringData in mapsUnlockStringData)
            {
                var mapUnlockData = mapUnlockStringData.Split(':');
                var mapId = mapUnlockData[0];
                var unlockStatus = bool.Parse(mapUnlockData[1]);
                if(unlockStatus)
                    mapManager.SetMapUnlocked(mapId);
            }
        }

        private bool IsAnyMapDataAvailable(ICloudDataStorage cloudStorage)
        {
            const string emptyDefaultMapId = "NOTHING";
            return (cloudStorage.GetString(LastLoadedMapKey, emptyDefaultMapId) != emptyDefaultMapId) ;
        }
    }

}