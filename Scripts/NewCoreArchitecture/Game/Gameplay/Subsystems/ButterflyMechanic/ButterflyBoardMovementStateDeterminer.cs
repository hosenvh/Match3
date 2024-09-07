using Match3.Game.Gameplay.SubSystemsData.FrameData.General;
using Match3.Game.Gameplay.SubSystemsData.SessionData;

namespace Match3.Game.Gameplay.SubSystems.ButterflyMechanic
{
    // NOTE: This is because of the conflic between generating butterflies in TileSourceSystem and the movement logic.
    // This will detect movement state before TileSourceSystem so no butterflies will be generated
    [Before(typeof(TileGeneration.TileSourceSystem))]
    public class ButterflyBoardMovementStateDeterminer : GameplaySystem
    {
        ButterflyBoardMovementSystem movementSystem;
        UserMovementData userMovementData;

        public ButterflyBoardMovementStateDeterminer(GameplayController gameplayController) : base(gameplayController)
        {
            this.userMovementData = GetFrameData<UserMovementData>();
        }

        public override void Start()
        {
            movementSystem = gameplayController.GetSystem<ButterflyBoardMovementSystem>();
        }

        public override void Update(float dt)
        {
            if (movementSystem.CurrentState() == ButterflyBoardMovementSystem.State.WaitingForUserMovement)
                CheckUserMovement();
        }

        void CheckUserMovement()
        {
            if (userMovementData.moves > 0)
            {
                GetSessionData<InputControlData>().AddLockedBy<ButterflyMovementKeyType>();
                movementSystem.GoToState(ButterflyBoardMovementSystem.State.ProcessingButterfliesMovement);
            }
        }
    }
}