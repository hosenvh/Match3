namespace Match3.Game.Gameplay.HitManagement
{
    public class DefaultTileHitProperties : TileHitProperties
    {
        // TODO: Merge these two varaibles with the hiting logic in Tile class.
        readonly bool acceptsDirectHit;
        readonly bool acceptsSideHit;
        readonly bool suppressesHitToSideHit;
        readonly bool propagatesHitToCell;

        public DefaultTileHitProperties(
            bool acceptsDirectHit, 
            bool acceptsSideHit, 
            bool suppressesHitToSideHit,
            bool propagatesHitToCell)
        {
            this.acceptsDirectHit = acceptsDirectHit;
            this.acceptsSideHit = acceptsSideHit;
            this.suppressesHitToSideHit = suppressesHitToSideHit;
            this.propagatesHitToCell = propagatesHitToCell;
        }

        public bool AcceptsDirectHit()
        {
            return acceptsDirectHit;
        }

        public bool AcceptsSideHit()
        {
            return acceptsSideHit;
        }

        public bool PropagatesHitToCell()
        {
            return propagatesHitToCell;
        }

        public bool SuppressesHitToSideHit()
        {
            return suppressesHitToSideHit;
        }
    }
}