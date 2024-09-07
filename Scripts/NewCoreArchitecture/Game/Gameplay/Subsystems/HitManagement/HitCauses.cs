
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.HitManagement
{
    public struct ExplosionHitCause : HitCause
    { }

    public struct PowerUpHitCause : HitCause
    { }

    public struct BeachBallHitCause : HitCause
    { }

    public struct MatchHitCause : HitCause
    {
        public readonly TileColor matchColor;

        public MatchHitCause(TileColor matchColor)
        {
            this.matchColor = matchColor;
        }
    }

    public struct RainbowHitCause : HitCause
    {
        public readonly TileColor targetColor;

        public RainbowHitCause(TileColor matchColor)
        {
            this.targetColor = matchColor;
        }
    }

    public struct GeneralHitCause : HitCause
    {
    }
}