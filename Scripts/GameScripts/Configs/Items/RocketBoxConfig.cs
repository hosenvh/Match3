using Match3.Game.Gameplay.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/RocketBox")]
public class RocketBoxConfig : BaseItemConfig
{
    class RocketBoxData : IItemData
    {
        public ItemType GetItemType()
        {
            return ItemType.Butterfly;
        }
    }

    public TileColor color;

    public override IItemData GetItemData()
    {
        return new RocketBoxData();
    }
}