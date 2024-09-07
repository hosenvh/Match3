

using Match3.Game.Gameplay.SubSystemsData.FrameData.LevelEnding;
using Match3.Game.Gameplay.Tiles;


namespace Match3.Game.Gameplay.SubSystems.ExtraMoveMechanic
{
    public interface ExtraMoveApplyingPresentationPort : PresentationHandler
    {
        void PlayExtraMoveEffect(ExtraMove extraMoveTile);
    }

    [After(typeof(DestructionManagement.DestructionSystem))]
    [Before(typeof(LevelEnding.LevelStoppingSystem))]
    public class ExtraMoveApplyingSystem : GameplaySystem
    {
        public ExtraMoveApplyingSystem(GameplayController gameplayController) : base(gameplayController)
        {
        }

        public override void Update(float dt)
        {
            foreach (var tile in GetFrameData<DestroyedObjectsData>().tiles)
                if (tile is ExtraMove extraMove)
                {
                    GetFrameData<MovesToAddData>().amount += extraMove.moveAmount;
                    gameplayController.GetPresentationHandler<ExtraMoveApplyingPresentationPort>().PlayExtraMoveEffect(extraMove);
                }
        }

    }
}