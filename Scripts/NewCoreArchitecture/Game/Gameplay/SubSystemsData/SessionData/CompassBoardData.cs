using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;


namespace Match3.Game.Gameplay.Subsystems.Compass
{
    public class CompassBoardData : BlackBoardData
    {
        public Direction currentDirection = Direction.Up;

        public void Clear()
        {
        }
    }

}