using UnityEngine;


[CreateAssetMenu(menuName = "Baord/Items/BalloonMainTile")]
public class BalloonMainTileItemConfig : BaseItemConfig
{
    public override IItemData GetItemData()
    {
        return new BalloonMainTileItemData();
    }
}