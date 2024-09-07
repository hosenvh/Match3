
﻿using System;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.SubSystems.LevelEnding;

namespace Match3.Game.Gameplay.Tiles
{
    public class RocketBox : Tile, DestructionBasedGoalObject
    {
        protected override void InternalHit(HitType hitType, HitCause hitCause)
        {
            // Disables being destroyed by hit.
        }
    }
}