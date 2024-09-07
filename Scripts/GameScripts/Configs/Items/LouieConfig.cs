using UnityEngine;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/Items/Louie")]
    public class LouieConfig : BaseItemConfig
    {
        [SerializeField] private int level;

        public override IItemData GetItemData()
        {
            return new LouieItemData(level);
        }
    }
}