using UnityEngine;
using System;
using Match3.Game.Gameplay.SubSystems.TableClothMechanic;
using Match3.Presentation.Gameplay.Tiles;
using Match3.Game.Gameplay.SubSystems.BuoyantMechanic;
using System.Collections.Generic;
using Match3.Presentation.Gameplay.Core;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class BuoyantMovementPortImp : MonoBehaviour, BuoyantMovementPort
    {
        public void Move(List<BuoyantMovementData> movementsData, Action<BuoyantMovementData> onCompleted)
        {

            foreach (var data in movementsData)
            {
                var presenter = data.buoyant.GetComponent<BuoyantTilePresenter>();
                presenter.PlayDisapperAnimation(() => Appear(presenter,data, onCompleted));
            }
        }

        private void Appear(BuoyantTilePresenter presenter, BuoyantMovementData data, Action<BuoyantMovementData> onCompleted)
        {
            presenter.Owner().SetLogicPositionUpdateFlag(false);

            presenter.Owner().transform.position = data.target.Parent().GetComponent<CellStackPresenter>().transform.position;

            onCompleted(data);
            presenter.PlayAppearAnimation(() => { presenter.Owner().SetLogicPositionUpdateFlag(true);});
        }
    }
}