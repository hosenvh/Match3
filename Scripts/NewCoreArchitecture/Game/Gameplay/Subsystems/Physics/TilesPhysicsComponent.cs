using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Game.Gameplay.Physics
{
    public class TilePhysic : Component
    {
        public enum PhysicsState { Falling, Resting};

        public Vector2 position;
        public PhysicsState physicsState;
        public Vector2Int targetLogicalPosition;

    }
    public class CellPhysic : Component
    {
        public bool canContainTile;


    }
}