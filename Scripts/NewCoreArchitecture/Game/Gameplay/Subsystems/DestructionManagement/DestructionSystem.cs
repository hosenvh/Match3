

using System;
using System.Collections.Generic;
using Match3.Foundation.Base.ComponentSystem;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.DestructionManagement
{

    public struct TileDestructionExtraInfo : Component
    {
        public readonly CellStack parent;

        public TileDestructionExtraInfo(CellStack parent)
        {
            this.parent = parent;
        }
    }

    public struct AttachmentDestructionExtraInfo : Component
    {
        public readonly CellStack owner;

        public AttachmentDestructionExtraInfo(CellStack owner)
        {
            this.owner = owner;
        }
    }

    // TODO: Split this to multiple interfaces.
    public interface DestructionHandler
    {
        // TODO: Try to remove this method.
        void Clear();
        void Initialize(GameplayController gpc);

        bool DoesAccept(Tile tile);
        bool DoesAccept(Cell cell);
        bool DoesAccept(HitableCellAttachment attachment);

        void Destroy(Tile tile, Action<Tile> onCompleted);
        void Destroy(Cell cell, Action<Cell> onCompleted);
        void Destroy(HitableCellAttachment attachment, Action<HitableCellAttachment> onCompleted);
    }

    public struct DestructionSystemKeyType : KeyType
    { }

    public class DestructionSystem : GameplaySystem
    {

        DestructionData destructionData;

        List<DestructionHandler> destructionHandlers = new List<DestructionHandler>();

        DestroyedObjectsData destroyedObjectsData;

        public DestructionSystem(GameplayController gameplayController) : base(gameplayController)
        {
            destructionData = GetFrameData<DestructionData>();
            destroyedObjectsData = GetFrameData<DestroyedObjectsData>();

            destructionHandlers.Add(new GoalTargetGatheringDestructionHandler());
            destructionHandlers.Add(new VacuumCleanerTargetGatheringDestructionHandler());
            destructionHandlers.Add(new TableClothTargetGatheringDestructionHandler());
            destructionHandlers.Add(new CatColoredBeadTargetGatheringDestructionHandler());
            destructionHandlers.Add(new DefaultDestructionDelegator());
        }

        public override void Start()
        {
            foreach (var delegator in destructionHandlers)
                delegator.Initialize(gameplayController);
        }

        public override void Update(float dt)
        {
            ClearDestructionDelegators();

            foreach (var tile in destructionData.tilesToDestroy)
                ProcessTileDestruction(tile);

            foreach (var cell in destructionData.cellsToDestroy)
                ProcessCellDestruction(cell);

            foreach (var attachment in destructionData.cellAttachmentsToDestroy)
                ProcessAttachmentDestruction(attachment);
        }


        private void ProcessTileDestruction(Tile tile)
        {
            if (tile.Parent().Parent() == null)
            {
                return;
            }

            tile.AddComponent(new TileDestructionExtraInfo(tile.Parent().Parent()));
            destroyedObjectsData.tiles.Add(tile);

            tile.Parent().componentCache.lockState.LockBy<DestructionSystemKeyType>();
            DelegateDestructionFor(tile, OnTileDestructionCompleted);
        }

        private void ProcessCellDestruction(Cell cell)
        {
            destroyedObjectsData.cells.Add(cell);
            DelegateDestructionFor(cell, OnCellDestructionCompleted);
        }


        private void ProcessAttachmentDestruction(HitableCellAttachment attachment)
        {
            if (attachment.HasOwner())
            {
                DelegateDestructionFor(attachment, delegate { });
                attachment.AddComponent(new AttachmentDestructionExtraInfo(attachment.Owner()));
                attachment.Owner().RemoveAttachment(attachment);
                destroyedObjectsData.attachments.Add(attachment);
            }
        }



        private void ClearDestructionDelegators()
        {
            foreach (var delegator in destructionHandlers)
                delegator.Clear();
        }

        private void DelegateDestructionFor(Tile tile, Action<Tile> onTileDestructionCompleted)
        {
            foreach (var delegator in destructionHandlers)
            {
                if (delegator.DoesAccept(tile))
                {
                    delegator.Destroy(tile, onTileDestructionCompleted);
                    break;
                }
            }
            
        }

        private void DelegateDestructionFor(Cell cell, Action<Cell> onCellDestructionCompleted)
        {
            foreach (var delegator in destructionHandlers)
                if (delegator.DoesAccept(cell))
                {
                    delegator.Destroy(cell, onCellDestructionCompleted);
                    break;
                }
        }

        private void DelegateDestructionFor(HitableCellAttachment attachment, Action<CellAttachment> onAttachmentDestructionCompleted)
        {
            foreach (var delegator in destructionHandlers)
                if (delegator.DoesAccept(attachment))
                {
                    delegator.Destroy(attachment, onAttachmentDestructionCompleted);
                    break;
                }
        }

        private void OnTileDestructionCompleted(Tile tile)
        {
            tile.Parent().GetComponent<LockState>().Release();
            if (tile.Parent().IsDepleted() && tile.Parent().IsDestroyed() == false)
                tile.Parent().Destroy();
        }

        private void OnCellDestructionCompleted(Cell cell)
        {
           
        }
    }
}
