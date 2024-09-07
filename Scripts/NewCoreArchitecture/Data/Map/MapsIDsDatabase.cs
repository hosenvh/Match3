using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Match3.Data.Map
{

    [CreateAssetMenu(menuName = "Map/MapsIDsDatabase", fileName = "MapsIDsDatabase")]
    public class MapsIDsDatabase : ScriptableObject
    {
        public List<string> mapIDs = new List<string>();

        public void InsertMapId(string mapId)
        {
            if(!mapIDs.Contains(mapId))
                mapIDs.Add(mapId);
        }
    }

}