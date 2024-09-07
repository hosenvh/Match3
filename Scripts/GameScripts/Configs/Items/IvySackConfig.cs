using UnityEngine;


namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/CellItems/IvySackItem")]
    public class IvySackConfig : BaseItemConfig
    {
        [SerializeField] private int level = 2;

        public override IItemData GetItemData()
        {
            return new IvySackItemData(level);
        }
    }
}