

using Match3.Game.Gameplay.SubSystemsData.FrameData.General;
using Match3.Game.Gameplay.Tiles;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.SubSystems.GasCylinderMechanic
{
    [Before(typeof(LevelEnding.LevelStoppingSystem))]
    public class GasCylinderCountdownSystem : GameplaySystem
    {
        public GasCylinderCountdownSystem(GameplayController gameplayController) : base(gameplayController)
        {
        }

        public override void Update(float dt)
        {
            if (GetFrameData<UserMovementData>().moves > 0)
            {
                foreach (var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                    if (HasTile<GasCylinder>(cellStack))
                        FindTile<GasCylinder>(cellStack).DecrementCountdown();
            }
        }
    }
}