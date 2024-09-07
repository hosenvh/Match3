using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.SubSystemsData.FrameData.RainbowMechanic;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.HitManagement
{
    public static class RainbowColorExtractor
    {
        // NOTE: Since there is no separation between simultaneous rainbow activations,
        // this will not work in that situation.
        public static TileColor ExtractFrom(RainbowActivationData activationData)
        {
            foreach (var tileStack in activationData.tileStackHits)
            {
                // TODO: Find out why this condition can happen.
                if (tileStack.IsDepleted())
                    continue;


                if (tileStack.Top() is Nut)
                    return TileColor.None;
                else
                {
                    var colorComp = tileStack.Top().GetComponent<TileColorComponent>();
                    if (colorComp != null)
                        return colorComp.color;
                }
            }
            return TileColor.None;

        }
    }
}