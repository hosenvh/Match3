using Match3.Game.Gameplay;
using Match3.Game.Gameplay.ExplosionManagement;
using Match3.Game.Gameplay.RainbowMechanic;
using Match3.Game.Gameplay.Tiles;
using Match3.Game.Gameplay.Tiles.Explosives;


namespace Match3.Game.Effects.GamePlayEffects
{
    public interface GameplayCameraShakePresentationPort
    {
        void ShakeCamera(float duration);
    }

    public class GameplayCameraShakeController
    {
        private readonly GameplayCameraShakePresentationPort presentationPort;
        private const float duration = 1;

        public GameplayCameraShakeController(GameplayCameraShakePresentationPort presentationPort, IGameplayController gameplayController)
        {
            this.presentationPort = presentationPort;
            
            gameplayController.GetSystem<ExplosionActivationSystem>().OnTileExplosion += ShakeCameraOnTNTBarrelExplosion;
            gameplayController.GetSystem<RainbowGenerationSystem>().OnTileExplosion += ShakeCameraOnExplosionsCombo;
        }

        private void ShakeCameraOnTNTBarrelExplosion(ExplosiveTile explodedTile)
        {
            if (IsExplodedTileTNTBarrel())
                presentationPort.ShakeCamera(duration: duration);

            bool IsExplodedTileTNTBarrel()
            {
                return explodedTile is TNTBarrel;
            }
        }

        private void ShakeCameraOnExplosionsCombo(int comboAmount)
        {
            if (comboAmount > 3)
                presentationPort.ShakeCamera(duration: duration);
        }
    }
}