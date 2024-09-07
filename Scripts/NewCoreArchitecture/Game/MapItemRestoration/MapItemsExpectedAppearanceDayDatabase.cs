using Match3.Game.Map;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Game.MapItemResoration
{
    public class MapItemsExpectedAppearanceDayDatabase : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public int itemId;
            public string mapId;
            public int expectedAppearanceDay;

            public Entry(int itemID, string mapID, int expectedAppearanceDay)
            {
                this.itemId = itemID;
                this.mapId = mapID;
                this.expectedAppearanceDay = expectedAppearanceDay;
            }
        }

        [SerializeField] List<Entry> entries;


        public List<Entry> Entries { get => entries; }

#if UNITY_EDITOR
        public void SetExpectedDay(MapItemIdentifer itemIdentifier, int day)
        {
            var entry = Find(itemIdentifier);

            if (entry != null)
                entry.expectedAppearanceDay = day;
            else
                entries.Add(new Entry(itemIdentifier.itemId, itemIdentifier.mapId, day));
        }

        private Entry Find(MapItemIdentifer itemFullIdentifer)
        {
            foreach (var entry in entries)
                if (entry.itemId == itemFullIdentifer.itemId && entry.mapId == itemFullIdentifer.mapId)
                    return entry;

            return null;
        }
#endif
    }
}