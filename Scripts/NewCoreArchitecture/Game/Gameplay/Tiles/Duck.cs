using System;
using System.Linq;
using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.HitManagement;

namespace Match3.Game.Gameplay.Tiles
{
    public class Duck : Tile
    {
        private int waitedTurnsCount = 0;
        private bool isDestroyed = false;
        private IEnumerable<DuckItem> duckItems = new DuckItem[] {};
        public static int GENERATION_AMOUNT_PER_DUCK;

        public Duck (IEnumerable<Type> childTilesTypes)
        {
            foreach (Type childTileType in childTilesTypes)
            {
                duckItems = duckItems.Concat(new [] { new DuckItem(childTileType, TileColor.None) });
            }

            GENERATION_AMOUNT_PER_DUCK = childTilesTypes.Count();
        }

        public void SetRocketBoxColor (TileColor color)
        {
            foreach (DuckItem item in duckItems)
                if (item.Item == typeof(Tiles.RocketBox))
                    item.SetColor(color);
        }

        public void IncrementWaitingTurns ()
        {
            waitedTurnsCount++;
        }

        public int TurnsWaited ()
        {
            return waitedTurnsCount;
        }

        public void ResetWaiting ()
        {
            waitedTurnsCount = 0;
        }

        public IEnumerable<DuckItem> GetChildItems ()
        {
            return duckItems;
        }

        public DuckItem GetChildItemAtIndex (int index)
        {
            return duckItems.ElementAt(index);
        }

        public bool ShouldGetReadyForGeneration()
        {
            return CurrentLevel() <= 0;
        }

        protected override void InternalHit(HitType hitType, HitCause hitCause)
        {
            // Maybe looking for a better solution
            if (CurrentLevel() > 0)
                base.InternalHit(hitType, hitCause);
        }

        public void MarkAsDestroyed()
        {
            isDestroyed = true;
        }

        public override bool IsDestroyed()
        {
            return isDestroyed;
        }
    }

    // Maybe it's better to use struct instead of the class
    // But we need to modify color in SetColor(TileColor color) method with ref
    public class DuckItem
    {
        public Type Item { get; private set; }
        public TileColor Color { get; private set; }

        public DuckItem (Type item, TileColor color)
        {
            this.Item = item;
            this.Color = color;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is DuckItem))
                return false;

            return Item.Equals(((DuckItem)obj).Item) && Color.Equals(((DuckItem)obj).Color);
        }

        public override int GetHashCode()
        {
            return string.Format("{0}_{1}", Item, Color).GetHashCode();
        }

        public void SetColor (TileColor color)
        {
            this.Color = color;
        }
    }
}