using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.DestructionManagement;
using Match3.Game.Gameplay.Physics;

namespace Match3.Game.Gameplay.SubSystems.General
{
    [After(typeof(DestructionManagement.DestructionSystem))]
    public class RopeBlockageUpdaterSystem : GameplaySystem
    {
        DestroyedObjectsData destroyedObjectsData;
        BoardBlockageController boardBlockageController;
        CellStackBoard cellStackBoard;
        PhysicsSystem physicsSystem;

        public RopeBlockageUpdaterSystem(GameplayController gameplayController) : base(gameplayController)
        {
            boardBlockageController = gameplayController.boardBlockageController;
            cellStackBoard = gameplayController.GameBoard().CellStackBoard();
            destroyedObjectsData = GetFrameData<DestroyedObjectsData>();
        }
        public override void Start()
        {
            this.physicsSystem = gameplayController.GetSystem<PhysicsSystem>();
        }

        public override void Update(float dt)
        {
            foreach (var attachment in destroyedObjectsData.attachments)
                if (attachment is Rope rope)
                    UpdateBlockageFor(rope);
        }

        private void UpdateBlockageFor(Rope rope)
        {
            var owner = rope.GetComponent<AttachmentDestructionExtraInfo>().owner;
            boardBlockageController.UpdateRopeBlockageFor(owner);

            UpdatePhysicsAroundTheRopeOwner(owner);
        }

        private void UpdatePhysicsAroundTheRopeOwner(CellStack owner)
        {
            var centerX = owner.Position().x;
            var centerY = owner.Position().y;

            for (int i = -1; i <= 1; ++i)
                for (int j = -1; j <= 1; ++j)
                {
                    var cellStack = cellStackBoard[centerX + i, centerY + j];
                    if (cellStack != null)
                        physicsSystem.UpdatePhysicBlockageFor(cellStack);
                }
        }
    }
}