using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/ChickenNest")]
public class ChickenNestItemConfig : BaseItemConfig
{
    #region private fields
    #endregion

    #region public methods
    public override IItemData GetItemData()
    {
        return new ChickenNestItemData();
    }
    #endregion
}