using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Subsystems.Compass;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Tiles;
using UnityEngine;


namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class CompassPresentHandlerImp:MonoBehaviour,CompassPresentationPort
    {

        public void Rotate(HashSet<CompassTile> allCompassBoard, Direction currentDirection)
        {
            foreach (var compassTile in allCompassBoard)
            {
                compassTile.GetComponent<CompassTilePresenter>().SetRotationCompass(currentDirection);
            }
        }
    }
}