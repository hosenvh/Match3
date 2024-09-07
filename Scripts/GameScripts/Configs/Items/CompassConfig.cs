using System.Collections;
using System.Collections.Generic;
using Match3.Datas.Items;
using UnityEngine;


namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/CellItems/CompassItem")]
    public class CompassConfig : BaseItemConfig
    {
        public override IItemData GetItemData()
        {
            return new CompassItemData();
        }
    }
}