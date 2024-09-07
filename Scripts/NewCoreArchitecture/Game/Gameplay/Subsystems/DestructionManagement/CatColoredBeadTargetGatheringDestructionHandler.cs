using System;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.CatMechanic;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.DestructionManagement
{
    public interface CatColoredBeadTargetGatheringPresentationHandler : PresentationHandler
    {
        void Gather(Tile tile, Cat cat, Action<Tile> onDetached, Action onGathered);
    }

    public class CatColoredBeadTargetGatheringDestructionHandler : DestructionHandler
    {
        CatManagementSystem catManagementSystem;
        CatColoredBeadTargetGatheringPresentationHandler presentationHandler;

        public void Initialize(GameplayController gpc)
        {
            catManagementSystem = gpc.GetSystem<CatManagementSystem>();
            presentationHandler = gpc.GetPresentationHandler<CatColoredBeadTargetGatheringPresentationHandler>();
        }

        public bool DoesAccept(Tile tile)
        {
            return catManagementSystem.IsActive() && tile is CatColoredBead;
        }

        public void Destroy(Tile tile, Action<Tile> onCompleted)
        {
            var cat = catManagementSystem.TargetCat();

            presentationHandler.Gather(
                tile,
                cat,
                onDetached: onCompleted,
                onGathered: () => EnqueueActionForCat());
        }

        private void EnqueueActionForCat()
        {
            catManagementSystem.EnequeAction();
        }

        public bool DoesAccept(Cell cell)
        {
            return false;
        }

        public void Destroy(Cell cell, Action<Cell> onCompleted)
        {
            throw new NotImplementedException();
        }

        public bool DoesAccept(HitableCellAttachment attachment)
        {
            return false;
        }

        public void Destroy(HitableCellAttachment attachment, Action<HitableCellAttachment> onCompleted)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {

        }
    }
}
