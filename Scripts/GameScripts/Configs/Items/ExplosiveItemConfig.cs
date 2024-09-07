using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExplosiveItemType { None, Rocket, Bomb, Dynamite, TNT, Rainbow, RocketPack, RocketBucket }
[CreateAssetMenu(menuName = "Baord/Items/Explosive Item")]
public class ExplosiveItemConfig : BaseItemConfig
{
    #region private fields
    [SerializeField]
    private ExplosiveItemType explosiveType = default;
    [SerializeField]
    private ItemColorType colorType = default;
    #endregion

    #region public methods
    public override IItemData GetItemData()
    {
        return new ExplosiveItemData(explosiveType, colorType);
    }
    #endregion
}