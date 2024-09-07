using Match3.Foundation.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Game.MapItemResoration
{
    public class MapItemIdentifer
    {
        public readonly int itemId;
        public readonly string mapId;

        public MapItemIdentifer(int itemID, string mapID)
        {
            this.itemId = itemID;
            this.mapId = mapID;
        }

        public override bool Equals(object obj)
        {
            return obj is MapItemIdentifer identifer &&
                   itemId == identifer.itemId &&
                   mapId == identifer.mapId;
        }

        public override int GetHashCode()
        {
            int hashCode = -346445769;
            hashCode = hashCode * -1521134295 + itemId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(mapId);
            return hashCode;
        }
    }
}