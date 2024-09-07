using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/SandWithGem")]
public class SandWithGemItemConfig : BaseItemConfig
{
    public int level = 1;

    public override IItemData GetItemData()
    {
        return new SandWithGemItemData(level);
    }
}
