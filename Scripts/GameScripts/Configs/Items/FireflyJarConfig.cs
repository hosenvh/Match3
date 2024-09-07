using UnityEngine;

[CreateAssetMenu(menuName = "Baord/Items/FireflyJar")]
public class FireflyJarConfig : BaseItemConfig
{
    public int fireflyAmount;


    public override IItemData GetItemData()
    {
        return new FireflyJarData(fireflyAmount);
    }

}
