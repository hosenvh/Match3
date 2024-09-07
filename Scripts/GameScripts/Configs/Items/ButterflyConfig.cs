using Match3.Game.Gameplay.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/Butterfly")]
public class ButterflyConfig : BaseItemConfig
{

    class ButterflyData : IItemData
    {
        public ItemType GetItemType()
        {
            return ItemType.Butterfly;
        }
    }

    public TileColor color;

    public override IItemData GetItemData()
    {
        return new ButterflyData();
    }


}