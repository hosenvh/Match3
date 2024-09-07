using System;
using Match3.Foundation.Unity;
using UnityEngine.AddressableAssets;


namespace Match3.Game.Map
{
    [Serializable]
    public class AddressableMapItemElementData: IEquatable<AddressableMapItemElementData>
    {
        [MapSelector]
        public string mapId;
        public int itemId;
        public AssetReference iconReference;
        public AssetReferenceGameObject elementPrefabReference;
        

        private bool EqualityCheck(object obj)
        {
            if (ReferenceEquals(this, null)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = (AddressableMapItemElementData) obj;
            return obj != null && this.mapId == other.mapId && this.itemId == other.itemId;
        }
        
        public bool Equals(AddressableMapItemElementData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return mapId == other.mapId && itemId == other.itemId && Equals(iconReference, other.iconReference) &&
                   Equals(elementPrefabReference, other.elementPrefabReference);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AddressableMapItemElementData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (mapId != null ? mapId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ itemId;
                hashCode = (hashCode * 397) ^ (iconReference != null ? iconReference.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (elementPrefabReference != null ? elementPrefabReference.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}


