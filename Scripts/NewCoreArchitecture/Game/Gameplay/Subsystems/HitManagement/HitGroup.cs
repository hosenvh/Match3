using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.HitManagement
{
    public class HitGroup
    {
        HashSet<TileStack> entityTileStacks = new HashSet<TileStack>();

        public void Clear()
        {
            entityTileStacks.Clear();
        }

        public bool IsNewEntity(TileStack tileStack)
        {
            return entityTileStacks.Contains(OwnerTileStackOf(tileStack)) == false;
        }

        public void AddEntityOf(TileStack tileStack)
        {
            entityTileStacks.Add(OwnerTileStackOf(tileStack));
        }

        private TileStack OwnerTileStackOf(TileStack tileStack)
        {
            if (tileStack.IsDepleted())
                return tileStack;
            else if (tileStack.Top() is SlaveTile slaveTile)
                return (slaveTile.Master().Parent());
            else
                return tileStack;
        }


    }
}