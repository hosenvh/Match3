using UnityEngine;
using System;
using Match3.Game.Gameplay.SubSystems.TableClothMechanic;
using Match3.Presentation.Gameplay.Tiles;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class TableClothRemovingSystemPortImp : MonoBehaviour, TableClothRemovingSystemPort
    {
        public void Remove(TableCloth tableCloth, Action onCompleted)
        {
            tableCloth.mainTile.GetComponent<TableClothTilePresenter>().PlayRemovelEffect(onCompleted);
        }
    }
}