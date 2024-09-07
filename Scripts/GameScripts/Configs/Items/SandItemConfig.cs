using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/Sand")]
public class SandItemConfig : BaseItemConfig
{
    public int level = 1;
    

    public override IItemData GetItemData()
    {
        return new SandItemData(level);
    }
}
