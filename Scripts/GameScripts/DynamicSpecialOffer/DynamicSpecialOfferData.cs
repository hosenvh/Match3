using UnityEngine;

namespace DynamicSpecialOfferSpace
{
    [System.Serializable]
    public class DynamicSpecialOfferData
    {
        public int storePackageIndex;
        
        [Space(10)] public int coinCount;
        public int[] powerUpsCount = new int[3];

//        [Space(10)] public Sprite featureArt;

        [Space(10)] public string title;
        public string realPrice;
        public string discountPrice;
        public string offPercent;
    }
}