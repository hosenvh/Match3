using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Tiles
{
    public class Rock : Tile
    {
        public Rock(int level) : base(level)
        {
        }
    }

    public class RockHitProperties : HitManagement.TileHitProperties
    {
        readonly Rock rock;
        public RockHitProperties(Rock rock)
        {
            this.rock = rock;
        }

        public bool AcceptsDirectHit()
        {
            return true;
        }

        public bool AcceptsSideHit()
        {
            return rock.CurrentLevel() == 1;
        }

        public bool PropagatesHitToCell()
        {
            return false;
        }

        public bool SuppressesHitToSideHit()
        {
            return true;
        }
    }
}