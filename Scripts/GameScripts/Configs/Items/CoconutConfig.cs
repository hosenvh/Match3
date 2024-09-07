using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/Coconut")]
    public class CoconutConfig : BaseItemConfig
    {
        [SerializeField] private int level;

        public override IItemData GetItemData()
        {
            return new CoconutItemData(level);
        }
    }
}