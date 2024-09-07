using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.ExplosionManagement;
using Match3.Game.Gameplay.Input;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.RainbowMechanic;
using Match3.Game.Gameplay.SubSystems.General;
using Match3.Game.Gameplay.Tiles;
using Match3.Game.Gameplay.Tiles.Explosives;
using Match3.Presentation.Gameplay;
using System;
using System.Linq;

namespace Match3.Overlay.Analytics
{
    public class GameplayItemGenerationAndActivationAnalyticsHandler : GameplayAnalyticsHandler
    {
        SuccessfulUserActivationData successfulUserActivationData;
        SuccessfulSwapsData successfulSwapsData;
        protected override void Handle(GameEvent evt)
        {
            if(evt is LevelStartedEvent)
            {
                var gpc = GameplayController();
                if(gpc != null)
                {
                    successfulUserActivationData = gpc.FrameCenteredBlackBoard().GetComponent<SuccessfulUserActivationData>();
                    successfulSwapsData = gpc.FrameCenteredBlackBoard().GetComponent<SuccessfulSwapsData>();
                }
            }

            //if (evt is ExplosiveActivatedEvent)
            //{
            //    var explosive = evt.As<ExplosiveActivatedEvent>().explosiveTile;

            //    if (explosive is Rocket)
            //        AnalyticsManager.SendEvent(new AnalyticsData_Ingame_Explosion_Rocket());
            //    else if (explosive is Bomb)
            //        AnalyticsManager.SendEvent(new AnalyticsData_Ingame_Explosion_Bomb());
            //    else if (explosive is Dynamite)
            //        AnalyticsManager.SendEvent(new AnalyticsData_Ingame_Explosion_Dynamite());
            //    else if (explosive is TNTBarrel)
            //        AnalyticsManager.SendEvent(new AnalyticsData_Ingame_Explosion_Tnt());
            //}

            //else if (evt is SingleRainbowActivatedEvent)
            //    AnalyticsManager.SendEvent(new AnalyticsData_Ingame_Use_Rainbow(ItemOf(evt.As<SingleRainbowActivatedEvent>().targets.FirstOrDefault())));
            //else if (evt is DoubleRainbowActivatedEvent)
            //    AnalyticsManager.SendEvent(new AnalyticsData_Ingame_Use_Double_Rainbow());

            //else if (evt is RainbowGeneratedEvent)
            //    AnalyticsManager.SendEvent(new AnalyticsData_Ingame_Create_Rainbow());


            //// NOTE: Some systems use TileStackGeneratedEvent like BoosterCreation.
            //else if (evt is TileGeneratedEvent tileGeneratedEvent)
            //{
            //    var tile = tileGeneratedEvent.tile;
            //    if(tile is ExplosiveTile)
            //        AnalyticsManager.SendEvent(new AnalyticsData_Ingame_Create_Booster(tile.GetType().Name, IsCreatedByUserSwap(tile)));
            //}
        }

        GameplayController GameplayController()
        {
            if (Base.gameManager.CurrentState is GameplayState gameState)
                return gameState.gameplayController;
            return null;
        }

        private bool IsCreatedByUserSwap(Tile tile)
        {
            foreach (var swapData in successfulSwapsData.data)
            {
                if (swapData.originTarget.Position().Equals(tile.Parent().Parent().Position()))
                    return true;
                if (swapData.destinationTarget.Position().Equals(tile.Parent().Parent().Position()))
                    return true;
            }
            return false;
        }

        private string ItemOf(TileStack tileStack)
        {
            if (tileStack == null)
                return "Empty";

            var colorComp = tileStack.Top().GetComponent<TileColorComponent>();
            if (colorComp != null)
                return string.Format("{0}_{1}", tileStack.Top().GetType().Name, colorComp.color.ToString());;

            return tileStack.Top().GetType().Name;
        }
    }
}