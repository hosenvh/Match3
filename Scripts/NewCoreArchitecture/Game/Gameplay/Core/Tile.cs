using System;
using Match3.Foundation.Base.ComponentSystem;
using Match3.Game.Gameplay.HitManagement;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.Physics;
using UnityEngine;


namespace Match3.Game.Gameplay.Core
{
    public struct TileComponentCache
    {
        public TileColorComponent colorComponent;
        public TilePhysicalProperties tilePhysicalProperties;
        public TileHitProperties tileHitProperties;
    }


    // TODO: Maybe extract the Hit handling of tile to a separate component?
    public abstract class Tile : BasicEntity
    {
        public TileComponentCache componentCache = new TileComponentCache();
        public Action<Tile> onParentTileStackDestroyed = delegate {  };
        TileStack parent;
        int level = 1;

        public Tile(int initialLevel)
        {
            this.level = initialLevel;
        }

        public Tile()
        {
            this.level = 1;
        }

        public void SetParent(TileStack parent)
        {
            this.parent = parent;
            parent.onDestroyed += HandleParentTileStackDestroyed;
        }

        private void HandleParentTileStackDestroyed()
        {
            onParentTileStackDestroyed.Invoke(this);
            parent.onDestroyed -= HandleParentTileStackDestroyed;
        }

        public bool DoesAcceptHit(HitType hitType, HitCause hitCause)
        {
            switch(hitType)
            {
                case HitType.Direct:
                    return InteralDoesAcceptDirectHit(hitCause);
                case HitType.Side:
                    return InteralDoesAcceptSideHit(hitCause);
            }

            return false;
        }

        protected override void OnComponentAdded(Foundation.Base.ComponentSystem.Component component)
        {
            switch (component)
            {
                case TileColorComponent c:
                    componentCache.colorComponent = c;
                    break;
                case TilePhysicalProperties c:
                    componentCache.tilePhysicalProperties = c;
                    break;
                case TileHitProperties c:
                    componentCache.tileHitProperties = c;
                    break;
            }

        }

        protected virtual bool InteralDoesAcceptDirectHit(HitCause hitCause)
        {
            return true;
        }


        protected virtual bool InteralDoesAcceptSideHit(HitCause hitCause)
        {
            return true;
        }


        public void Hit(HitType hitType, HitCause hitCause)
        {
            InternalHit(hitType, hitCause);
        }

        protected virtual void InternalHit(HitType hitType, HitCause hitCause)
        {
            DecrementLevel();
        }

        protected void DecrementLevel()
        {
            --level;
        }

        public virtual bool IsDestroyed()
        {
            return level <= 0;
        }

        // WARNING: I'm not sure this is a good Idea. Use with care.
        public void ForceDestroy()
        {
            SetLevel(0);
        }

        public int CurrentLevel()
        {
            return level;
        }

        protected void SetLevel(int level)
        {
            this.level = level;
        }

        public TileStack Parent()
        {
            return parent;
        }
    }
}